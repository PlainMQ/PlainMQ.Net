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

        /// <summary>
        /// Used for sending the message byte array down the wire
        /// </summary>
        /// <returns>a byte array to send down the wire</returns>
        public byte[] ToMessageBytes()
        {
            byte[] ret = new byte[LENGTH + sizeof(int)];

            BitConverter.GetBytes(LENGTH).CopyTo(ret, 0);
            BODY?.CopyTo(ret, sizeof(int));

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
        }

        /// <summary>
        /// Partial creation initializing an empty byte array of size len
        /// </summary>
        /// <param name="len">The desired size of the message byte array</param>
        public PlainMessage(int len)
        {
            LENGTH = len;
            BODY = new byte[LENGTH];
        }
    }
}
