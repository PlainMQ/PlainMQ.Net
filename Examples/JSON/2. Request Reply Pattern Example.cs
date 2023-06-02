using Newtonsoft.Json;

/*
 * This examples follows on from [0.Pure JSON Example] and aims to demonstrate the ease of implementing a request-reply pattern
 * The process will be as follows:
 * 
 *  1. Setup a connection to PlainMQ Server
 *  2. Construct an object with a ReplyTo field and parse it into a json string
 *  3. Send the json string across the line to PlainMQ Server
 *  4. Receive a json string from another node in the PlainMQ network
 *  5. Parse that string into a known object
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
PlainMQ.OnReceiveString(ReceiveJson);

//Generate some kind of identifier
var generated_id = 125564;

//2. Construct an object and parse it into a json string
var p = new RequestPacket
{
    ReplyTo = generated_id, //any suitable identifier 
    Payload = new
    {
        Meta = "this is my metadata",
        Body = "this is my content"
    }
};

//3. Send the json string across the line to PlainMQ Server
PlainMQ.Send(JsonConvert.SerializeObject(p));

//4. Receive a json string from another node in the PlainMQ network
void ReceiveJson(string msgStr)
{
    //5. Parse that string into a known object
    var msgObj = JsonConvert.DeserializeObject<RequestPacket>(msgStr);

    //6.Reply to the message using the same ReplyTo field
    ReplyToMessage(msgObj);
}

void ReplyToMessage(RequestPacket msgObj)
{
    Console.WriteLine($"Responding to: {msgObj.ReplyTo}");

    var replyObj = new RequestPacket
    {
        ReplyTo = msgObj.ReplyTo,
        Payload = new
        {
            Body = "Replied!!"
        }
    };

    PlainMQ.Send(JsonConvert.SerializeObject(replyObj));
}

//user defined class
public class RequestPacket
{
    public int ReplyTo { get; set; }
    public object Payload { get; set; }
};