using Newtonsoft.Json;

/*
 * This examples follows on from [0.Pure JSON Example] and aims to demonstrate the ease of implementing a pub/sub pattern
 * The process will be as follows:
 * 
 *  1. Setup a connection to PlainMQ Server
 *  2. Construct an object and parse it into a json string
 *  3. Send the json string across the line to PlainMQ Server
 *  4. Receive a json string from another node in the PlainMQ network
 *  5. Parse that string into a known object
 *  6. Test if the message is subscribed to and process
 */

//1. Setup a connection to PlainMQ Server
PlainMQ.Setup("127.0.0.1", 13000);
PlainMQ.OnReceiveString(ReceiveJson);

//Generate some kind of identifier
var id = DateTime.Now.Ticks;
var subscription = "IWANTTHIS";

//2. Construct an object and parse it into a json string
var p = new Packet
{
    Subject = $"SUB|{subscription}|{id}", //subject can be in any format you desire
    Payload = new
    {
        Meta = "this is my metadata",
        Body = "this is my content"
    }
};

//2.1 Construct an object that won't be subscribed to
var discard = new Packet
{
    Subject = $"SUB|{subscription}_NOT|{id}", //subject can be in any format you desire
    Payload = new
    {
        Meta = "this is my metadata",
        Body = "this is my content"
    }
};

//3. Send the json string across the line to PlainMQ Server
PlainMQ.Send(JsonConvert.SerializeObject(p));
PlainMQ.Send(JsonConvert.SerializeObject(discard));

//4. Receive a json string from another node in the PlainMQ network
void ReceiveJson(string msgStr)
{
    //5. Parse that string into a known object
    var msgObj = JsonConvert.DeserializeObject<Packet>(msgStr);

    //6. Test if the message is subscribed to and process
    if (PassFilter(msgObj.Subject))
        ProcessMessage(msgObj);
}

void ProcessMessage(Packet msgObj)
{
    // Do the required message processing
    Console.WriteLine($"{msgObj.Subject} made it through filtering");
}

bool PassFilter(string? header)
{
    var subscribed_to = "IWANTTHIS";
    return header.Contains($"|{subscribed_to}|"); //customise the subject filtering the best way you know how
}

//user defined class
public class Packet
{
    public string Subject { get; set; }
    public object Payload { get; set; }
};