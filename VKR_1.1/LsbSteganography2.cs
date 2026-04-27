using System;
using System.Collections.Generic;
using System.Text;

namespace VKR_1
{
    internal class LsbSteganography2
    {
        // Новый метод с параметром selectedBits
        public static Bitmap EmbedData(Bitmap bmp, byte[] payload, bool _red, bool _green, bool _blue, List<int> selectedBits)
        {
            // Валидация выбранных битов
            if (selectedBits == null || selectedBits.Count == 0)
                throw new ArgumentException("Не выбрано ни одного бита для встраивания.");

            int payloadLength = payload.Length;
            byte[] lengthPrefix = BitConverter.GetBytes(payloadLength);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(lengthPrefix);

            byte[] fullPayload = new byte[4 + payloadLength];
            Array.Copy(lengthPrefix, fullPayload, 4);
            Array.Copy(payload, 0, fullPayload, 4, payloadLength);

            // Вычисляем количество используемых каналов
            int channelsUsed = 0;
            if (_red) channelsUsed++;
            if (_green) channelsUsed++;
            if (_blue) channelsUsed++;

            // Общая ёмкость: пиксели * каналы * количество выбранных битов на канал
            int requiredBits = fullPayload.Length * 8;
            int capacity = bmp.Width * bmp.Height * channelsUsed * selectedBits.Count;
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
                    bool dataExhausted = false;

                    if (_red)
                    {
                        r = EmbedBitsInChannel(r, fullPayload, selectedBits, ref byteIndex, ref bitIndex, ref dataExhausted);
                    }
                    if (dataExhausted) return output;

                    if (_green)
                    {
                        g = EmbedBitsInChannel(g, fullPayload, selectedBits, ref byteIndex, ref bitIndex, ref dataExhausted);
                    }
                    if (dataExhausted) return output;

                    if (_blue)
                    {
                        b = EmbedBitsInChannel(b, fullPayload, selectedBits, ref byteIndex, ref bitIndex, ref dataExhausted);
                    }

                    output.SetPixel(x, y, Color.FromArgb(pixel.A, r, g, b));

                    if (dataExhausted || byteIndex >= fullPayload.Length)
                        return output;
                }
            }
            return output;
        }

        public static byte[] ExtractData(Bitmap bmp, bool _red, bool _green, bool _blue, List<int> selectedBits)
        {
            if (selectedBits == null || selectedBits.Count == 0)
                throw new ArgumentException("Не выбрано ни одного бита для извлечения.");

            MemoryStream ms = new MemoryStream();
            int byteVal = 0;
            int bitCount = 0;
            int dataLength = -1;
            int bytesRead = 0;

            bool ProcessBit(int bitValue)
            {
                byteVal = (byteVal << 1) | bitValue;
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

                    if (_red)
                    {
                        foreach (int bitPos in selectedBits.OrderBy(b => b))
                        {
                            if (ProcessBit((pixel.R >> bitPos) & 1))
                                return ms.ToArray();
                        }
                    }

                    if (_green)
                    {
                        foreach (int bitPos in selectedBits.OrderBy(b => b))
                        {
                            if (ProcessBit((pixel.G >> bitPos) & 1))
                                return ms.ToArray();
                        }
                    }

                    if (_blue)
                    {
                        foreach (int bitPos in selectedBits.OrderBy(b => b))
                        {
                            if (ProcessBit((pixel.B >> bitPos) & 1))
                                return ms.ToArray();
                        }
                    }
                }
            }

            throw new Exception("Не удалось извлечь данные. Возможно, вы не верно выбрали цветовой(ые) канал(ы) или биты.");
        }

        // Заменяет несколько битов в байте канала данными из payload
        private static byte EmbedBitsInChannel(byte channelValue, byte[] data, List<int> bitPositions, ref int byteIndex, ref int bitIndex, ref bool exhausted)
        {
            byte newValue = channelValue;

            // Сортируем биты для предсказуемости (от младшего к старшему)
            foreach (int pos in bitPositions.OrderBy(b => b))
            {
                if (byteIndex >= data.Length)
                {
                    exhausted = true;
                    return newValue;
                }

                int dataBit = GetBit(data, ref byteIndex, ref bitIndex);
                newValue = SetSpecificBit(newValue, pos, dataBit);
            }
            return newValue;
        }

        // Установка конкретного бита (0-7) в байте
        private static byte SetSpecificBit(byte b, int position, int bitValue)
        {
            if (position < 0 || position > 7)
                throw new ArgumentOutOfRangeException(nameof(position), "Позиция бита должна быть от 0 до 7.");

            if (bitValue == 1)
                return (byte)(b | (1 << position));
            else
                return (byte)(b & ~(1 << position));
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
