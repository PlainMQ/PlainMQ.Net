using PlainMQLib.Models;
using PlainMQLib.Models.Enums;
using PlainMQLib.ThreadManagement.ThreadPool;
using PlainMQLib.ThreadManagement.ThreadTypes;
using System.Net.Sockets;
using System.Text;

public static class PlainMQ
{

    private static Action<byte[]> _receiveBytes;
    private static Action<string> _receiveString;

    private static Thread _thread;

    public static void OnReceiveBytes(Action<byte[]> receiveMsg)
    {
        _receiveBytes = receiveMsg;
    }

    public static void OnReceiveString(Action<string> receiveMsgString)
    {
        _receiveString = receiveMsgString;
    }

    public static void Send(byte[] bytes)
    {
        ManagedThreadPool.Broadcast(new ThreadEvent
        {
            Class = ThreadClass.BROADCAST,
            EventPayload = new PlainMessage(bytes)
        });
    }

    public static void Send(string v)
    {
        ManagedThreadPool.Broadcast(new ThreadEvent
        {
            Class = ThreadClass.BROADCAST,
            EventPayload = new PlainMessage(v)
        });
    }

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