# PlainMQLib

A .Net Core package to enable seamless connection to a known PlainMQ Server instance.

The heart of PlainMQ is contained in the following code snippet

```
PlainMQ.Setup("127.0.0.1", 13000);

PlainMQ.OnReceiveBytes(ReceiveMsg);
PlainMQ.OnReceiveString(ReceiveMsgString);

PlainMQ.Send(new byte[] { 23, 18, 87 });
PlainMQ.Send(@"{'json': true}");

void ReceiveMsg(byte[] msgBytes)
{
    Console.WriteLine($"Received bytes {msgBytes}");
}

void ReceiveMsgString(string msgStr)
{
    Console.WriteLine($"Received string {msgStr}");
}
```

All extra enhancements are then totally controlled by the client. You can write a pubsub/rpc/broadcaster your own way in your own time.
