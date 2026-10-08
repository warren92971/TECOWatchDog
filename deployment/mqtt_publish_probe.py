"""One non-retained MQTT 3.1.1 test; no credentials, no business topics."""
import socket
import struct
import json
import uuid
import datetime

def string(value):
    data = value.encode('utf-8')
    return struct.pack('!H', len(data)) + data

def packet(header, body):
    size = len(body)
    encoded = bytearray()
    while True:
        digit = size % 128
        size //= 128
        encoded.append(digit | (128 if size else 0))
        if not size:
            return bytes([header]) + encoded + body

def exact(sock, size):
    result = b''
    while len(result) < size:
        part = sock.recv(size - len(result))
        if not part:
            raise RuntimeError('Broker closed connection')
        result += part
    return result

def receive(sock):
    header = exact(sock, 1)[0]
    size, scale = 0, 1
    for _ in range(4):
        digit = exact(sock, 1)[0]
        size += (digit & 127) * scale
        if not digit & 128:
            if size > 65536:
                raise RuntimeError('Unexpected large packet')
            return header, exact(sock, size)
        scale *= 128
    raise RuntimeError('Malformed length')

def main():
    probe = uuid.uuid4().hex
    topic = 'tesc/watchdog/test/' + probe
    payload = json.dumps({'type': 'connectivity_test', 'test_only': True,
                          'message': 'WatchDog connectivity test, not an alarm',
                          'test_id': probe,
                          'timestamp': datetime.datetime.now(datetime.timezone.utc).isoformat()}).encode()
    print('Endpoint: 109.123.238.225:1883', flush=True)
    print('Topic: ' + topic, flush=True)
    with socket.create_connection(('109.123.238.225', 1883), timeout=10) as sock:
        sock.settimeout(10)
        sock.sendall(packet(0x10, string('MQTT') + bytes([4, 2, 0, 30]) + string('wd-test-' + probe[:16])))
        header, body = receive(sock)
        if header != 0x20 or body != b'\x00\x00':
            raise RuntimeError(f'CONNECT rejected: {header:02x} {body.hex()}')
        print('CONNECT accepted without credentials', flush=True)
        sock.sendall(packet(0x82, b'\x00\x01' + string(topic) + b'\x01'))
        header, body = receive(sock)
        subscribed = header == 0x90 and body[:2] == b'\x00\x01' and body[2:] in (b'\x00', b'\x01')
        print(f'SUBACK: {body.hex()}, accepted={subscribed}', flush=True)
        sock.sendall(packet(0x32, b''.join([string(topic), b'\x00\x02', payload])))
        print('Sent ONE QoS 1 message; retain=False', flush=True)
        ack, delivered = False, False
        try:
            while not (ack and delivered):
                header, body = receive(sock)
                if header == 0x40 and body == b'\x00\x02':
                    ack = True
                    print('PUBACK received', flush=True)
                elif header >> 4 == 3:
                    length = int.from_bytes(body[:2], 'big')
                    incoming = body[2:2+length].decode()
                    offset = 2 + length
                    qos = (header >> 1) & 3
                    if qos:
                        message_id = body[offset:offset+2]
                        offset += 2
                        sock.sendall(packet(0x40, message_id))
                    if incoming == topic and body[offset:] == payload:
                        delivered = True
                        print('Exact test payload received by subscription', flush=True)
                if ack and not subscribed:
                    break
        except socket.timeout:
            print('Timed out waiting for remaining confirmation', flush=True)
        sock.sendall(b'\xe0\x00')
        print(json.dumps({'puback': ack, 'subscription_received': delivered, 'retained': False}))
        if not (ack and delivered):
            raise SystemExit(2)

if __name__ == '__main__':
    main()
