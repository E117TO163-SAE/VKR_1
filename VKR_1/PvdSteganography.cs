using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

namespace VKR_1
{
    internal class PvdSteganography
    {
        private static readonly (int L, int U)[] Ranges =
       {
            (0, 7),
            (8, 15),
            (16, 31),
            (32, 63),
            (64, 127),
            (128, 255)
        };

        public static Bitmap EmbedDataPvd(Bitmap bmp, byte[] payload, bool _red, bool _green, bool _blue)
        {
            byte[] lengthPrefix = BitConverter.GetBytes(payload.Length);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(lengthPrefix);

            byte[] fullPayload = new byte[4 + payload.Length];
            Array.Copy(lengthPrefix, fullPayload, 4);
            Array.Copy(payload, 0, fullPayload, 4, payload.Length);

            Bitmap output = new Bitmap(bmp);

            int byteIndex = 0;
            int bitIndex = 0;

            for (int y = 0; y < output.Height; y++)
            {
                for (int x = 0; x < output.Width - 1; x += 2)
                {
                    if (byteIndex >= fullPayload.Length)
                        return output;

                    Color p1 = output.GetPixel(x, y);
                    Color p2 = output.GetPixel(x + 1, y);

                    int r1 = p1.R, g1 = p1.G, b1 = p1.B;
                    int r2 = p2.R, g2 = p2.G, b2 = p2.B;

                    if (_red)
                    {
                        int d = Math.Abs(r1 - r2);

                        if (!TryGetRange(d, out int l, out int u))
                            continue;

                        int t = (int)Math.Floor(Math.Log(u - l + 1, 2));
                        if (t <= 0)
                            continue;

                        int value = 0;
                        for (int i = 0; i < t; i++)
                            value = (value << 1) | GetBit(fullPayload, ref byteIndex, ref bitIndex);

                        int dNew = value + l;

                        AdjustPixelsRobust(r1, r2, dNew, out r1, out r2);
                    }
                    if (_green)
                    {
                        int d = Math.Abs(g1 - g2);

                        if (!TryGetRange(d, out int l, out int u))
                            continue;

                        int t = (int)Math.Floor(Math.Log(u - l + 1, 2));
                        if (t <= 0)
                            continue;

                        int value = 0;
                        for (int i = 0; i < t; i++)
                            value = (value << 1) | GetBit(fullPayload, ref byteIndex, ref bitIndex);

                        int dNew = value + l;

                        AdjustPixelsRobust(g1, g2, dNew, out g1, out g2);
                    }
                    if(_blue)
                    {
                        int d = Math.Abs(b1 - b2);

                        if (!TryGetRange(d, out int l, out int u))
                            continue;

                        int t = (int)Math.Floor(Math.Log(u - l + 1, 2));
                        if (t <= 0)
                            continue;

                        int value = 0;
                        for (int i = 0; i < t; i++)
                            value = (value << 1) | GetBit(fullPayload, ref byteIndex, ref bitIndex);

                        int dNew = value + l;

                        AdjustPixelsRobust(b1, b2, dNew, out b1, out b2);
                    }

                    
                    
                    p1 = Color.FromArgb(p1.A, r1, g1, b1);
                    p2 = Color.FromArgb(p2.A, r2, g2, b2);

                    output.SetPixel(x, y, p1);
                    output.SetPixel(x + 1, y, p2);
                }
            }

            throw new ArgumentException("Изображение слишком маленькое для встраивания данных.");
        }

        /*public static byte[] ExtractDataPvd(Bitmap bmp)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                int bitIndex = 0;
                int byteIndex = 0;
                int totalBytes = -1;
                byte currentByte = 0;

                // грубая верхняя граница, чтобы отловить мусорный заголовок
                int maxPossibleBytes = (bmp.Width * bmp.Height * 3) / 8;

                for (int y = 0; y < bmp.Height; y++)
                {
                    for (int x = 0; x < bmp.Width - 1; x += 2)
                    {
                        Color p1 = bmp.GetPixel(x, y);
                        Color p2 = bmp.GetPixel(x + 1, y);

                        int d = Math.Abs(p1.R - p2.R);

                        if (!TryGetRange(d, out int l, out int u))
                            continue;

                        int t = (int)Math.Floor(Math.Log(u - l + 1, 2));
                        if (t <= 0)
                            continue;

                        int value = d - l;

                        for (int i = t - 1; i >= 0; i--)
                        {
                            int bit = (value >> i) & 1;
                            currentByte |= (byte)(bit << (7 - bitIndex));
                            bitIndex++;

                            if (bitIndex == 8)
                            {
                                ms.WriteByte(currentByte);
                                currentByte = 0;
                                bitIndex = 0;
                                byteIndex++;

                                if (byteIndex == 4 && totalBytes < 0)
                                {
                                    byte[] header = ms.ToArray();
                                    if (!BitConverter.IsLittleEndian)
                                        Array.Reverse(header);

                                    totalBytes = BitConverter.ToInt32(header, 0);

                                    if (totalBytes < 0 || totalBytes > maxPossibleBytes)
                                        throw new Exception("Не удалось извлечь данные: заголовок длины повреждён (изображение могло быть изменено).");

                                    ms.SetLength(0);
                                    byteIndex = 0;
                                }
                                else if (totalBytes >= 0 && byteIndex >= totalBytes)
                                {
                                    return ms.ToArray();
                                }
                            }
                        }
                    }
                }
            }

            throw new Exception("Не удалось извлечь данные. Возможно, они отсутствуют или повреждены.");
        }*/
        public static byte[] ExtractDataPvd(Bitmap bmp, bool red, bool green, bool blue)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                int bitIndex = 0;
                int byteIndex = 0;
                int totalBytes = -1;
                byte currentByte = 0;

                int channelCount = (red ? 1 : 0) + (green ? 1 : 0) + (blue ? 1 : 0);
                int maxPossibleBytes = (bmp.Width * bmp.Height * channelCount * 6) / 8;

                for (int y = 0; y < bmp.Height; y++)
                {
                    for (int x = 0; x < bmp.Width - 1; x += 2)
                    {
                        Color p1 = bmp.GetPixel(x, y);
                        Color p2 = bmp.GetPixel(x + 1, y);

                        if (red && ExtractFromPair(p1.R, p2.R, ref currentByte, ref bitIndex,
                            ref byteIndex, ref totalBytes, maxPossibleBytes, ms))
                            return ms.ToArray();
                        if (green && ExtractFromPair(p1.G, p2.G, ref currentByte, ref bitIndex,
                            ref byteIndex, ref totalBytes, maxPossibleBytes, ms))
                            return ms.ToArray();
                        if (blue && ExtractFromPair(p1.B, p2.B, ref currentByte, ref bitIndex,
                            ref byteIndex, ref totalBytes, maxPossibleBytes, ms))
                            return ms.ToArray();
                    }
                }
            }

            throw new Exception("Не удалось извлечь данные. Возможно, они отсутствуют или повреждены.");
        }

        private static bool ExtractFromPair(int c1, int c2, ref byte currentByte, ref int bitIndex,
                                             ref int byteIndex, ref int totalBytes, int maxPossibleBytes,
                                             MemoryStream ms)
        {
            int d = Math.Abs(c1 - c2);
            if (!TryGetRange(d, out int l, out int u))
                return false;

            int t = (int)Math.Floor(Math.Log(u - l + 1, 2));
            if (t <= 0)
                return false;

            int value = d - l;

            for (int i = t - 1; i >= 0; i--)
            {
                int bit = (value >> i) & 1;
                currentByte |= (byte)(bit << (7 - bitIndex));
                bitIndex++;

                if (bitIndex == 8)
                {
                    ms.WriteByte(currentByte);
                    currentByte = 0;
                    bitIndex = 0;
                    byteIndex++;

                    if (byteIndex == 4 && totalBytes < 0)
                    {
                        byte[] header = ms.ToArray();
                        if (!BitConverter.IsLittleEndian)
                            Array.Reverse(header);
                        totalBytes = BitConverter.ToInt32(header, 0);
                        if (totalBytes < 0 || totalBytes > maxPossibleBytes)
                            throw new Exception("Заголовок длины повреждён.");
                        ms.SetLength(0);
                        byteIndex = 0;
                    }
                    else if (totalBytes >= 0 && byteIndex >= totalBytes)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // Вспомогательные

        private static bool TryGetRange(int d, out int l, out int u)
        {
            foreach (var r in Ranges)
            {
                if (d >= r.L && d <= r.U)
                {
                    l = r.L;
                    u = r.U;
                    return true;
                }
            }

            l = u = 0;
            return false;
        }

        private static int GetBit(byte[] data, ref int byteIndex, ref int bitIndex)
        {
            if (byteIndex >= data.Length)
                return 0;

            int bit = (data[byteIndex] >> (7 - bitIndex)) & 1;
            bitIndex++;

            if (bitIndex == 8)
            {
                bitIndex = 0;
                byteIndex++;
            }

            return bit;
        }

        /// <summary>
        /// Робастная коррекция пары: находит такие nr1,nr2 (0..255),
        /// что |nr1-nr2| = dNew, и изменение минимально.
        /// Гарантированно находит решение для dNew в [0..255].
        /// </summary>
        private static void AdjustPixelsRobust(int r1, int r2, int dNew, out int nr1, out int nr2)
        {
            if (dNew < 0 || dNew > 255)
                throw new ArgumentOutOfRangeException(nameof(dNew));

            /*
            bool r1IsHigh = r1 >= r2;
            int high = r1IsHigh ? r1 : r2;
            int low = r1IsHigh ? r2 : r1;
            */

            int high, low;

            if (r1 >= r2)
            {
                high = r1;
                low = r2;
            }
            else
            {
                high = r2;
                low = r1;
            }

            int bestLow = 0;
            int bestHigh = dNew;
            int bestCost = int.MaxValue;

            int maxLow = 255 - dNew;
            for (int candLow = 0; candLow <= maxLow; candLow++)
            {
                int candHigh = candLow + dNew;

                int cost = Math.Abs(candLow - low) + Math.Abs(candHigh - high);
                if (cost < bestCost)
                {
                    bestCost = cost;
                    bestLow = candLow;
                    bestHigh = candHigh;

                    if (bestCost == 0)
                        break;
                }
            }

            if (r1 >= r2)
            {
                nr1 = bestHigh;
                nr2 = bestLow;
            }
            else
            {
                nr1 = bestLow;
                nr2 = bestHigh;
            }
        }

        
        public static int GetCapacity(Bitmap bmp, bool red, bool green, bool blue)
        {
            int totalBits = 0;
            int width = bmp.Width;
            int height = bmp.Height;

            BitmapData bmpData = bmp.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb
            );

            byte[] pixels = new byte[bmpData.Stride * height];
            Marshal.Copy(bmpData.Scan0, pixels, 0, pixels.Length);
            bmp.UnlockBits(bmpData);

            int stride = bmpData.Stride;

            for (int y = 0; y < height; y++)
            {
                int rowOffset = y * stride;

                for (int x = 0; x < width - 1; x += 2)
                {
                    int pos1 = rowOffset + x * 4;
                    int pos2 = rowOffset + (x + 1) * 4;

                    byte b1 = pixels[pos1], g1 = pixels[pos1 + 1], r1 = pixels[pos1 + 2];
                    byte b2 = pixels[pos2], g2 = pixels[pos2 + 1], r2 = pixels[pos2 + 2];

                    if (red) totalBits += GetPairCapacity(r1, r2);
                    if (green) totalBits += GetPairCapacity(g1, g2);
                    if (blue) totalBits += GetPairCapacity(b1, b2);
                }
            }

            return Math.Max(0, totalBits / 8 - 4);
        }

        private static int GetPairCapacity(int c1, int c2)
        {
            int d = Math.Abs(c1 - c2);
            if (TryGetRange(d, out int l, out int u))
            {
                int t = (int)Math.Floor(Math.Log(u - l + 1, 2));
                return t > 0 ? t : 0;
            }
            return 0;
        }
    }
}
