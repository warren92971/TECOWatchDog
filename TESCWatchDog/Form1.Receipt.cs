using MQTTnet;
using MQTTnet.Client;
using System.Text;

namespace TESCWatchDog;

public partial class Form1
{
    private sealed record Receipt(string Topic, byte[] Payload, TaskCompletionSource<bool> Received);
    private readonly object receiptLock = new();
    private readonly List<Receipt> receipts = new();
    private readonly CancellationTokenSource receiptLifetime = new();

    private void MatchReceipt(MqttApplicationMessage message)
    {
        // A retained snapshot from SUBSCRIBE is not evidence of this publication.
        if (message.Retain) return;
        lock (receiptLock)
        {
            var receipt = receipts.FirstOrDefault(item => item.Topic == message.Topic &&
                message.PayloadSegment.AsSpan().SequenceEqual(item.Payload));
            if (receipt == null) return;
            receipts.Remove(receipt);
            receipt.Received.TrySetResult(true);
        }
    }

    private async Task<MqttClientPublishResult> PublishWithReceiptAsync(MqttApplicationMessage message, CancellationToken token)
    {
        Receipt? receipt = null;
        try
        {
            using var subscribeTimeout = CancellationTokenSource.CreateLinkedTokenSource(token);
            subscribeTimeout.CancelAfter(TimeSpan.FromSeconds(5));
            var result = await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
                .WithTopic(message.Topic).WithAtLeastOnceQoS().Build(), subscribeTimeout.Token);
            if (result.Items.Count == 0 || result.Items.Any(item => (int)item.ResultCode >= 128))
                throw new InvalidOperationException("Broker 未允許訂閱");
            AppendLog($"接收驗證：已訂閱 [{message.Topic}]，準備發布。");
            receipt = new Receipt(message.Topic, message.PayloadSegment.ToArray(),
                new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously));
            lock (receiptLock) receipts.Add(receipt);
        }
        catch (Exception ex) when (!token.IsCancellationRequested)
        {
            AppendLog($"接收驗證不可用 [{message.Topic}]：{ex.Message}；仍嘗試發布通知。");
        }

        try
        {
            using var publishTimeout = CancellationTokenSource.CreateLinkedTokenSource(token);
            publishTimeout.CancelAfter(TimeSpan.FromSeconds(10));
            var result = await mqttClient.PublishAsync(message, publishTimeout.Token);
            if ((int)result.ReasonCode >= 128)
                throw new InvalidOperationException($"Broker 拒絕發布：{result.ReasonCode}");
            // Verification is independent of delivery retries and never blocks DB polling.
            if (receipt != null) _ = ReportReceiptAsync(receipt);
            return result;
        }
        catch
        {
            if (receipt != null) lock (receiptLock) receipts.Remove(receipt);
            throw;
        }
    }

    private async Task ReportReceiptAsync(Receipt receipt)
    {
        try
        {
            bool received = await receipt.Received.Task.WaitAsync(TimeSpan.FromSeconds(10), receiptLifetime.Token);
            var payload = Encoding.UTF8.GetString(receipt.Payload);
            PostUi(() => AppendLog(received
                ? $"訂閱接收成功 [{receipt.Topic}]；接收內容：{payload}（Broker 回傳驗證，非東元處理確認）。"
                : $"訂閱接收未確認 [{receipt.Topic}]：MQTT 已中斷；不因驗證失敗重發已確認的通知。"));
        }
        catch (TimeoutException)
        {
            PostUi(() => AppendLog($"訂閱接收逾時 [{receipt.Topic}]：發布呼叫已完成，但 10 秒內未收到相同內容；不因驗證逾時重發。"));
        }
        catch (OperationCanceledException) { }
        finally
        {
            lock (receiptLock) receipts.Remove(receipt);
        }
    }

    private void DisconnectReceipts()
    {
        lock (receiptLock)
        {
            foreach (var receipt in receipts) receipt.Received.TrySetResult(false);
            receipts.Clear();
        }
    }
}
