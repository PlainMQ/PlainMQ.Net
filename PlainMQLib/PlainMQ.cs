using PlainMQLib.Models;
using PlainMQLib.Models.Enums;
using PlainMQLib.ThreadManagement.ThreadPool;
using PlainMQLib.ThreadManagement.ThreadTypes;
using System.Net.Sockets;
using System.Text;

namespace PlainMQLib
{

    /// <summary>
    /// Main static class for all PlainMQ Server Interactions.
    /// <br/> PlainMQ is a barebones broadcast only Messaging Queue. Allowing the client to develop features on
    /// top of the queue that may or may not be applicable to them.
    /// <br/><br/> In order to create a connection to PlainMQ Server you
    /// must first run the Setup Method. After that there are four main functionalities:
    /// <br/>
    /// <br/>    1. Send bytes
    /// <br/>    2. Send string
    /// <br/>    3. Receive bytes
    /// <br/>    4. Receive string
    ///     
    /// <br/><br/> Note that any message that is sent is not received back to the sender
    /// 
    /// </summary>
    public static class PlainMQ
    {

        private static Action<byte[]> _receiveBytes;
        private static Action<string> _receiveString;

        private static Thread _thread;

        /// <summary>
        /// The Function that is invoked when a message containing a byte array is received. 
        /// <br/>Further messaging handling of a byte array is initialized through this function
        /// </summary>
        /// <param name="receiveMsg">The Action containing the received array of bytes</param>
        public static void OnReceiveBytes(Action<byte[]> receiveMsg)
        {
            _receiveBytes = receiveMsg;
        }

        /// <summary>
        /// The Function that is invoked when a message consisting of a string is received. 
        /// <br/>Further messaging handling of the string is initialized through this function
        /// </summary>
        /// <param name="receiveMsg">The Action containing the received string</param>
        public static void OnReceiveString(Action<string> receiveMsgString)
        {
            _receiveString = receiveMsgString;
        }

        /// <summary>
        /// The Function that will send a message consisting of an array of bytes to PlainMQ Server.
        /// <br/>PlainMQ Server will then broadcast this message to all connected clients
        /// </summary>
        /// <param name="bytes">The byte array to send to PlainMQ Server</param>
        public static void Send(byte[] bytes)
        {
            ManagedThreadPool.Broadcast(new ThreadEvent
            {
                Class = ThreadClass.BROADCAST,
                EventPayload = new PlainMessage(bytes)
            });
        }

        /// <summary>
        /// The Function that will send a message consisting of a string to PlainMQ Server.
        /// <br/> PlainMQ Server will then broadcast this message to all connected clients
        /// </summary>
        /// <param name="strMsg">The string message to send to PlainMQ Server</param>
        public static void Send(string strMsg)
        {
            ManagedThreadPool.Broadcast(new ThreadEvent
            {
                Class = ThreadClass.BROADCAST,
                EventPayload = new PlainMessage(strMsg)
            });
        }

        /// <summary>
        /// The Setup Function that has to be called prior to sending/receiving messages from PlainMQ Server.       
        /// </summary>
        /// <param name="ipAddr">The host IP address of the PlainMQ Server instance</param>
        /// <param name="port">The port that the PlainMQ server is listening on</param>
        public static void Setup(string ipAddr, int port)
        {
            //Connect to server
            TcpClient client = new TcpClient(ipAddr, port);

            if (client.Connected)
            {
                ManagedThreadPool.Init();

                //Setup Writer methods
                NetworkStreamManagedQueueThread nsThread = CreateNSThread(client);
                ManagedThreadPool.AddToPool(nsThread);

                //Setup read thread
                _thread = new Thread(() => ReadFromStream(nsThread.NStream));
                _thread.Start();
            }

        }

        private static NetworkStreamManagedQueueThread CreateNSThread(TcpClient client)
        {
            NetworkStreamManagedQueueThread nsThread = new NetworkStreamManagedQueueThread(client.GetStream(), true);
            nsThread.InvokeClass = ThreadClass.BROADCAST;
            nsThread.QueueAction = (ThreadEvent te) =>
            {
                if (te.EventPayload == null)
                    return;

                PlainMessage msg = (PlainMessage)te.EventPayload;

                if (nsThread.NStream.CanWrite)
                {
                    nsThread.NStream.Write(msg.ToMessageBytes());
                }
            };

            return nsThread;
        }

        private static void ReadFromStream(NetworkStream nStream)
        {
            try
            {
                int i;
                byte[] lenByte = new byte[sizeof(int) + sizeof(bool)];

                while ((i = nStream.Read(lenByte)) != 0)
                {
                    if (i != (sizeof(int) + sizeof(bool)))
                        throw new Exception("unhandled message type");

                    int length = BitConverter.ToInt32(new Span<byte>(lenByte, 0, sizeof(int)).ToArray());
                    bool isStr = BitConverter.ToBoolean(new Span<byte>(lenByte, sizeof(int), 1).ToArray());

                    PlainMessage pMsg = new PlainMessage(length, isStr);

                    if (pMsg.BODY != null)
                    {
                        nStream.Read(pMsg.BODY, 0, pMsg.LENGTH);

                        if (isStr)
                            _receiveString.Invoke(Encoding.UTF8.GetString(pMsg.BODY));
                        else
                            _receiveBytes.Invoke(pMsg.BODY);

                        nStream.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
