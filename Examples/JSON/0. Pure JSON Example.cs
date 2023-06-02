using Newtonsoft.Json;

/*
 * This examples aims to demonstrate the ease of sending a json message via PlainMQ
 * Additionally we can also receive a json message with the same ease. 
 * The process will be as follows:
 * 
 *  1. Setup a connection to PlainMQ Server
 *  2. Construct an object and parse it into a json string
 *  3. Send the json string across the line to PlainMQ Server
 *  4. Receive a json string from another node in the PlainMQ network
 *  5. Parse that string into a known object
 */

//1. Setup a connection to PlainMQ Server
PlainMQ.Setup("127.0.0.1", 13000);
PlainMQ.OnReceiveString(ReceiveJson);

//Generate some kind of identifier
var id = DateTime.Now.Ticks;

//2. Construct an object and parse it into a json string
var p = new Packet
{
    Header = $"Example-JSON-Send-{id}",
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
    Console.WriteLine($"Received message from {JsonConvert.DeserializeObject<Packet>(msgStr)?.Header}");
}

//user defined class
public class Packet
{
    public string Header { get; set; }
    public object Payload { get; set; }
};