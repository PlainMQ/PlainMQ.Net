using Newtonsoft.Json;
using System.Text;

/*
 * This examples follows on from [0.Pure Bytes Example] and aims to demonstrate the ease of implementing a pub/sub pattern
 * The process will be as follows:
 * 
 *  1. Setup a connection to PlainMQ Server
 *  2. Construct an object and parse it into a byte array
 *  3. Send the bytes across the line to PlainMQ Server
 *  4. Receive a byte message from another node in the PlainMQ network
 *  5. Parse that byte array into a known object
 *  6. Test if the message is subscribed to and process
 */

//1. Setup a connection to PlainMQ Server
PlainMQ.Setup("127.0.0.1", 13000);
PlainMQ.OnReceiveBytes(ReceiveBytes);

//Generate some kind of identifier
var generated_id = 125564;

//2. Construct an object and parse it into a byte array
var p = new RequestPacket
{
    Subject = $"T|{generated_id}|Z", //any suitable identifier 
    Payload = Encoding.UTF8.GetBytes("This is my payload")
};

var discard = new RequestPacket
{
    Subject = $"T|{111222}|Z", //any suitable identifier 
    Payload = Encoding.UTF8.GetBytes("This is my payload")
};

//3. Send the bytes across the line to PlainMQ Server
PlainMQ.Send(p.ToBytes());
PlainMQ.Send(discard.ToBytes());

//4. Receive a byte message from another node in the PlainMQ network
void ReceiveBytes(byte[] msgBytes)
{
    //5. Parse those bytes into a known object
    var msgObj = new RequestPacket(msgBytes);

    //6. Test if the message is subscribed to and process
    if (PassFilter(msgObj.Subject))
        ProcessMessage(msgObj);
}

bool PassFilter(string subject)
{
    return subject.Contains($"|{125564}|");
}

void ProcessMessage(RequestPacket msgObj)
{
    Console.WriteLine($"Message subject subscribed to successfully: {msgObj.Subject}");
}

//user defined class
public class RequestPacket
{
    //In this case we are forcing subject length to be 10 chars because of example reasons
    public string Subject { get; set; }
    public byte[] Payload { get; set; }

    public byte[] ToBytes()
    {
        var buffer = new byte[Subject.Length + Payload.Length];
        Encoding.UTF8.GetBytes(Subject).CopyTo(buffer, 0);
        Payload.CopyTo(buffer, Subject.Length);

        return buffer;
    }


    public RequestPacket() { }

    public RequestPacket(byte[] bytes)
    {
        Subject = Encoding.UTF8.GetString(bytes.AsSpan().Slice(0, 9));
        Payload = bytes[10..];
    }
};