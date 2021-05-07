using System.Collections;
using System.Text;

namespace Lab3
{
    class TcpSegment
    {
        public int sourcePort;
        public int destinationPort;
        public int sequenceNumber;
        public int acknowledgmentNumber;
        public bool urg;
        public bool ack;
        public bool psh;
        public bool rst;
        public bool syn;
        public bool fin;
        public string data;

        public TcpSegment(int sourcePort = 0,
                   int destinationPort = 0,
                   int sequenceNumber = 0,
                   int acknowledgmentNumber = 0,
                   bool urg = false,
                   bool ack = false,
                   bool psh = false,
                   bool rst = false,
                   bool syn = false,
                   bool fin = false,
                   string data = "")
        {
            this.sourcePort = sourcePort;
            this.destinationPort = destinationPort;
            this.sequenceNumber = sequenceNumber;
            this.acknowledgmentNumber = acknowledgmentNumber;
            this.urg = urg;
            this.ack = ack;
            this.psh = psh;
            this.rst = rst;
            this.syn = syn;
            this.fin = fin;
            this.data = data;
        }
        private BitArray ToBitArray()
        {
            BitArray res = new BitArray(160 + data.Length * 8);
            CopyTo(BitArray16FromInt(sourcePort), res, 0);
            CopyTo(BitArray16FromInt(destinationPort), res, 16);
            CopyTo(BitArray32FromInt(sequenceNumber), res, 32);
            CopyTo(BitArray32FromInt(acknowledgmentNumber), res, 64);
            res[106] = urg;
            res[107] = ack;
            res[108] = psh;
            res[109] = rst;
            res[110] = syn;
            res[111] = fin;
            CopyTo(BitArrayFromStr(data), res, 160);
            return res;
        }
        public byte[] ToByteArray()
        {
            BitArray bitArray = ToBitArray();
            byte[] data = new byte[bitArray.Length / 8];
            bitArray.CopyTo(data, 0);
            return data;
        }
        public static TcpSegment FromByteArray(byte[] source)
        {
            return FromBitArray(new BitArray(source));
        }
        private static TcpSegment FromBitArray(BitArray source)
        {
            return new TcpSegment(sourcePort: IntFromBitArray(Slice(source, 0, 16)),
                           destinationPort: IntFromBitArray(Slice(source, 16, 16)),
                           sequenceNumber: IntFromBitArray(Slice(source, 32, 32)),
                           acknowledgmentNumber: IntFromBitArray(Slice(source, 64, 32)),
                           urg: source[106],
                           ack: source[107],
                           psh: source[108],
                           rst: source[109],
                           syn: source[110],
                           fin: source[111],
                           data: StrFromBitArray(Slice(source, 160, source.Length - 160)));
        }
        private static BitArray BitArray16FromInt(int value)
        {
            BitArray res = new BitArray(16);
            for (int i = 0; value > 0 && i < res.Length; i++)
            {
                res[i] = value % 2 == 0 ? false : true;
                value /= 2;
            }
            return res;
        }
        private static BitArray BitArray32FromInt(int value)
        {
            int[] i = { value };
            return new BitArray(i);
        }
        private static BitArray BitArrayFromStr(string str)
        {
            byte[] data = Encoding.UTF8.GetBytes(str);
            return new BitArray(data);
        }
        private static int IntFromBitArray(BitArray source)
        {
            int[] data = new int[1];
            source.CopyTo(data, 0);
            return data[0];
        }
        private static string StrFromBitArray(BitArray source)
        {
            byte[] data = new byte[source.Length / 8];
            source.CopyTo(data, 0);
            return Encoding.UTF8.GetString(data);
        }
        private static BitArray Slice(BitArray source, int start, int count)
        {
            BitArray slice = new BitArray(count);
            for (int i = 0; i < count; i++)
            {
                slice[i] = source[i + start];
            }
            return slice;
        }
        private void CopyTo(BitArray source, BitArray dest, int start)
        {
            for (int i = 0; i < source.Length; i++)
            {
                dest[start + i] = source[i];
            }
        }
        public static byte[] GetBytes(byte[] source, int len)
        {
            byte[] res = new byte[len];
            for (int i = 0; i < len; i++)
            {
                res[i] = source[i];
            }
            return res;
        }
    }
}
