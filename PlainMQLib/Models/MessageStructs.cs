using System.Text;

namespace PlainMQLib.Models
{
    public enum MessageDirection
    {
        Read,
        Write
    }

    /// <summary>
    /// Simple wrapper class of a byte array containing a length header
    /// </summary>
    public struct PlainMessage
    {
        public int LENGTH;
        public byte[]? BODY;
        public bool ISSTR;

        /// <summary>
        /// Used for sending the message byte array down the wire
        /// </summary>
        /// <returns>a byte array to send down the wire</returns>
        public byte[] ToMessageBytes()
        {
            byte[] ret = new byte[LENGTH + sizeof(int) + sizeof(bool)];

            BitConverter.GetBytes(LENGTH).CopyTo(ret, 0);
            BitConverter.GetBytes(ISSTR).CopyTo(ret, sizeof(int));
            BODY.CopyTo(ret, sizeof(int) + sizeof(bool));

            return ret;
        }

        /// <summary>
        /// Full creation of the Message 
        /// </summary>
        /// <param name="inBytes">byte array of the message</param>
        public PlainMessage(byte[] inBytes)
        {
            Span<byte> span = new Span<byte>(inBytes);
            LENGTH = inBytes.Length;
            BODY = inBytes;
            ISSTR = false;
        }

        /// <summary>
        /// Partial creation initializing an empty byte array of size len
        /// </summary>
        /// <param name="len">The desired size of the message byte array</param>
        public PlainMessage(int len)
        {
            LENGTH = len;
            BODY = new byte[LENGTH];
            ISSTR = false;
        }

        public PlainMessage(byte[] inBytes, int len, bool isStr)
        {
            Span<byte> span = new Span<byte>(inBytes);
            LENGTH = len;
            BODY = inBytes;
            ISSTR = isStr;
        }

        public PlainMessage(int len, bool isStr)
        {
            LENGTH = len;
            BODY = new byte[LENGTH];
            ISSTR = isStr;
        }

        public PlainMessage(string s)
        {
            BODY = Encoding.UTF8.GetBytes(s);
            LENGTH = BODY.Length;
            ISSTR = true;
        }
    }
}
