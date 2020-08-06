using System.IO;
using SevenZip.Compression.LZMA;

namespace Gext
{
    public static class LzmaHelper
    {
        private static byte[] GetLzmaProperties(Stream inStream, out long outSize)
        {
            byte[] lzmaProperties = new byte[5];
            if (inStream.Read(lzmaProperties, 0, 5) != 5)
            {
                throw new DecompressionException("Bad lzma properties.");
            }
            outSize = 0;
            for (int i = 0; i < 8; i++)
            {
                int b = inStream.ReadByte();
                if (b < 0)
                {
                    throw new DecompressionException("Bad lzma properties.");
                }
                outSize |= ((long) (byte) b) << (i << 3);
            }
            return lzmaProperties;
        }

        public static byte[] ExtractLzma(byte[] data)
        {
            using (MemoryStream inStream = new MemoryStream(data))
            {
                Decoder decoder = new Decoder();
                inStream.Seek(0, 0);
                using (MemoryStream outStream = new MemoryStream())
                {
                    long outSize;
                    decoder.SetDecoderProperties(GetLzmaProperties(inStream, out outSize));
                    decoder.Code(inStream, outStream, inStream.Length - inStream.Position, outSize, null);
                    return outStream.ToArray();
                }
            }
        }
    }
}