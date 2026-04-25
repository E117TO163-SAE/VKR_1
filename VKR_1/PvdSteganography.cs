using System;
using System.Collections.Generic;
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

        public static Bitmap EmbedDataPvd(Bitmap bmp, byte[] payload)
        {
            // payload = [4 байта длины][данные]
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

                    int r1 = p1.R;
                    int r2 = p2.R;

                    int d = Math.Abs(r1 - r2);

                    if (!TryGetRange(d, out int l, out int u))
                        continue;

                    int t = (int)Math.Floor(Math.Log(u - l + 1, 2));
                    if (t <= 0)
                        continue;

                    // читаем t бит
                    int value = 0;
                    for (int i = 0; i < t; i++)
                        value = (value << 1) | GetBit(fullPayload, ref byteIndex, ref bitIndex);

                    int dNew = value + l;

                    AdjustPixelsRobust(r1, r2, dNew, out int nr1, out int nr2);

                    Color np1 = Color.FromArgb(p1.A, nr1, p1.G, p1.B);
                    Color np2 = Color.FromArgb(p2.A, nr2, p2.G, p2.B);

                    output.SetPixel(x, y, np1);
                    output.SetPixel(x + 1, y, np2);
                }
            }

            throw new ArgumentException("Изображение слишком маленькое для встраивания данных.");
        }

        public static byte[] ExtractDataPvd(Bitmap bmp)
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
    }
}
