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