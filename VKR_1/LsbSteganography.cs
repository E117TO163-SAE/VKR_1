using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace NIRSteg_2
{
    public static class LsbSteganography
    {

        public static Bitmap EmbedData(Bitmap bmp, byte[] payload, bool _red, bool _green, bool _blue)
        {
            int payloadLength = payload.Length;
            byte[] lengthPrefix = BitConverter.GetBytes(payloadLength);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(lengthPrefix);

            byte[] fullPayload = new byte[4 + payloadLength];
            Array.Copy(lengthPrefix, fullPayload, 4);
            Array.Copy(payload, 0, fullPayload, 4, payloadLength);
            int k = 0;
            if (_red) { 
                k++;
            }
            if (_green) { 
                k++;
            }
            if (_blue) { 
                k++;
            }

            int requiredBits = fullPayload.Length * 8;
            int capacity = bmp.Width * bmp.Height * k;
            if (requiredBits > capacity)
                throw new ArgumentException("Изображение слишком маленькое для встраивания данных.");

            Bitmap output = new Bitmap(bmp);
            int byteIndex = 0;
            int bitIndex = 0;

            for (int y = 0; y < output.Height; y++)
            {
                for (int x = 0; x < output.Width; x++)
                {
                    Color pixel = output.GetPixel(x, y);
                    byte r = pixel.R, g = pixel.G, b = pixel.B;

                    if (byteIndex >= fullPayload.Length) return output;

                    if (_red)
                    {
                        r = SetLeastSignificantBit(r, GetBit(fullPayload, ref byteIndex, ref bitIndex));
                    }
                    if (_green)
                    {
                        g = SetLeastSignificantBit(g, GetBit(fullPayload, ref byteIndex, ref bitIndex));
                    }
                    if (_blue)
                    {
                        b = SetLeastSignificantBit(b, GetBit(fullPayload, ref byteIndex, ref bitIndex));
                    }
                    output.SetPixel(x, y, Color.FromArgb(pixel.A, r, g, b));
                }
            }
            return output;
        }



        public static byte[] ExtractData(Bitmap bmp, bool _red, bool _green, bool _blue)
        {
            MemoryStream ms = new MemoryStream();
            int byteVal = 0;
            int bitCount = 0;
            int dataLength = -1;
            int bytesRead = 0;

            bool ProcessBit(int lsb)
            {
                byteVal = (byteVal << 1) | lsb;
                bitCount++;
                if (bitCount == 8)
                {
                    ms.WriteByte((byte)byteVal);
                    byteVal = 0;
                    bitCount = 0;
                    bytesRead++;

                    if (dataLength == -1 && bytesRead == 4)
                    {
                        byte[] header = ms.ToArray();
                        if (!BitConverter.IsLittleEndian)
                            Array.Reverse(header);
                        dataLength = BitConverter.ToInt32(header, 0);
                        ms = new MemoryStream();
                        bytesRead = 0;
                    }

                    if (dataLength != -1 && bytesRead >= dataLength)
                        return true; 
                }
                return false;
            }

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color pixel = bmp.GetPixel(x, y);

                    if (_red && ProcessBit(pixel.R & 1))
                    {
                        return ms.ToArray();
                    }                        
                    if (_green && ProcessBit(pixel.G & 1))
                    {
                        return ms.ToArray();
                    }
                    if (_blue && ProcessBit(pixel.B & 1))
                    {
                        return ms.ToArray();
                    }
                }
            }

            throw new Exception("Не удалось извлечь данные. Возможно, вы не верно выбрали цветовой(ые) канал(ы).");
        }

        private static byte SetLeastSignificantBit(byte b, int bit)
        {
            return (byte)((b & 0xFE) | (bit & 1));
        }

        
        private static int GetBit(byte[] data, ref int byteIndex, ref int bitIndex)
        {
            if (byteIndex >= data.Length)
                return 0; 

            int bit = (data[byteIndex] >> (7 - bitIndex)) & 1;
            bitIndex++;
            if (bitIndex >= 8)
            {
                bitIndex = 0;
                byteIndex++;
            }
            return bit;
        }

    }
}
