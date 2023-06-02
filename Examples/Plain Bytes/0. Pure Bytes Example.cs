using System.Text;

/*
 * This examples aims to demonstrate the ease of sending a json message via PlainMQ
 * Additionally we can also receive a json message with the same ease. 
 * The process will be as follows:
 * 
 *  1. Setup a connection to PlainMQ Server
 *  2. Construct an object and parse it into a byte array
 *  3. Send the bytes across the line to PlainMQ Server
 *  4. Receive a byte message from another node in the PlainMQ network
 *  5. Parse that byte array into a known object
 */

//1. Setup a connection to PlainMQ Server
PlainMQ.Setup("127.0.0.1", 13000);
PlainMQ.OnReceiveBytes(ReceiveBytes);

//Generate some kind of identifier
var generated_id = 125564;

//2. Construct an object and parse it into a byte array
var p = new RequestPacket
{
    Header = $"T|{generated_id}|Z", //any suitable identifier 
    Payload = Encoding.UTF8.GetBytes("This is my payload")
};


//3. Send the bytes across the line to PlainMQ Server
PlainMQ.Send(p.ToBytes());

//4. Receive a byte message from another node in the PlainMQ network
void ReceiveBytes(byte[] msgBytes)
{
    //5. Parse those bytes into a known object
    var msgObj = new RequestPacket(msgBytes);
    ProcessMessage(msgObj);
}

void ProcessMessage(RequestPacket msgObj)
{
    Console.WriteLine($"Message received: {msgObj.Header}");
}

//user defined class
public class RequestPacket
{
    //In this case we are forcing subject length to be 10 chars because of example reasons
    public string Header { get; set; }
    public byte[] Payload { get; set; }

    public byte[] ToBytes()
    {
        var buffer = new byte[Header.Length + Payload.Length];
        Encoding.UTF8.GetBytes(Header).CopyTo(buffer, 0);
        Payload.CopyTo(buffer, Header.Length);

        return buffer;
    }


    public RequestPacket() { }

    public RequestPacket(byte[] bytes)
    {
        Header = Encoding.UTF8.GetString(bytes.AsSpan().Slice(0, 10));
        Payload = bytes[10..];
    }
};
