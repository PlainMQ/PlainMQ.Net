using System.Text;

/*
 * This examples follows on from [0.Pure JSON Example] and aims to demonstrate the ease of implementing a request-reply pattern
 * The process will be as follows:
 * 
 *  1. Setup a connection to PlainMQ Server
 *  2. Construct an object with a ReplyTo field and convert it to an array of bytes
 *  3. Send the bytes across the line to PlainMQ Server
 *  4. Receive a byte message from another node in the PlainMQ network
 *  5. Parse those bytes into a known object
 *  6. Reply to the message using the same ReplyTo field
 *  
 *  !!!   NOTE   !!!!
 *  
 *  Please note that this is a demonstrative example only - 
 *  If you run two clients in this manner they will endlessly loop leading to a bad time
 *  
 *  You will have to develop a terminating condition further to this
 */

//1. Setup a connection to PlainMQ Server
PlainMQ.Setup("127.0.0.1", 13000);
PlainMQ.OnReceiveBytes(ReceiveBytes);

//Generate some kind of identifier
var generated_id = 125564;

//2. Construct an object with a ReplyTo field and convert it to an array of bytes
var p = new RequestPacket
{
    ReplyTo = generated_id, //any suitable identifier 
    Payload = Encoding.UTF8.GetBytes("This is my payload")
};

//3. Send the bytes across the line to PlainMQ Server
PlainMQ.Send(p.ToBytes());

//4. Receive a byte message from another node in the PlainMQ network
void ReceiveBytes(byte[] msgBytes)
{
    //5. Parse those bytes into a known object
    var msgObj = new RequestPacket(msgBytes);

    //6. Reply to the message using the same ReplyTo field
    ReplyToMessage(msgObj);
}

void ReplyToMessage(RequestPacket msgObj)
{
    //Console.WriteLine($"Responding to: {msgObj.ReplyTo}");

    var replyObj = new RequestPacket
    {
        ReplyTo = msgObj.ReplyTo,
        Payload = Encoding.UTF8.GetBytes("Reply")
    };

    PlainMQ.Send(replyObj.ToBytes());
}

//user defined class
public class RequestPacket
{
    public int ReplyTo { get; set; }
    public byte[] Payload { get; set; }

    public byte[] ToBytes()
    {
        var buffer = new byte[sizeof(int) + Payload.Length];
        BitConverter.GetBytes(ReplyTo).CopyTo(buffer, 0);
        Payload.CopyTo(buffer, sizeof(int));

        return buffer;
    }


    public RequestPacket() { }

    public RequestPacket(byte[] bytes)
    {
        ReplyTo = BitConverter.ToInt32(bytes.AsSpan().Slice(0, sizeof(int)));
        Payload = bytes[sizeof(int)..];
    }
};