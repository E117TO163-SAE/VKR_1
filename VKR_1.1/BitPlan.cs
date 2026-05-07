using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

namespace VKR_1
{
    internal class BitPlan
    {
        public static bool AnyBitSelected(bool[] bits)
        {
            foreach (bool bit in bits)
                if (bit) return true;
            return false;
        }

        /*public static Bitmap ApplyBitFilter(Bitmap source, bool useGrayscale, bool useRed, bool useGreen, bool useBlue, bool[] selectedBits)
        {
            Bitmap result = new Bitmap(source.Width, source.Height);

            for (int y = 0; y < source.Height; y++)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    Color originalColor = source.GetPixel(x, y);

                    int finalR = 0, finalG = 0, finalB = 0;

                    if (useGrayscale)
                    {
                        int gray = (int)(originalColor.R * 0.299 + originalColor.G * 0.587 + originalColor.B * 0.114);
                        finalR = finalG = finalB = ApplyBitMask(gray, selectedBits);
                    }
                    else
                    {
                        if (useRed) finalR = ApplyBitMask(originalColor.R, selectedBits);
                        if (useGreen) finalG = ApplyBitMask(originalColor.G, selectedBits);
                        if (useBlue) finalB = ApplyBitMask(originalColor.B, selectedBits);
                    }

                    // Ограничиваем значения
                    finalR = Math.Max(0, Math.Min(255, finalR));
                    finalG = Math.Max(0, Math.Min(255, finalG));
                    finalB = Math.Max(0, Math.Min(255, finalB));

                    result.SetPixel(x, y, Color.FromArgb(finalR, finalG, finalB));
                }
            }

            return result;
        }

        public static int ApplyBitMask(int value, bool[] selectedBits)
        {
            int result = 0;

            for (int bit = 0; bit < 8; bit++)
            {
                if (selectedBits[bit])
                {
                    // Извлекаем выбранный бит
                    int bitValue = (value >> bit) & 1;
                    // Устанавливаем этот бит в результат на ту же позицию
                    result |= (bitValue << bit);
                }
            }

            result = ScaleValueToFullRange(result, selectedBits);

            return result;
        }*/

        public static int ScaleValueToFullRange(int value, bool[] selectedBits)
        {
            // Подсчитываем количество выбранных битов
            int selectedCount = 0;
            for (int i = 0; i < 8; i++)
            {
                if (selectedBits[i]) selectedCount++;
            }

            if (selectedCount == 0) return 0;

            // Максимально возможное значение с выбранными битами
            int maxPossible = 0;
            for (int i = 0; i < 8; i++)
            {
                if (selectedBits[i])
                {
                    maxPossible |= (1 << i);
                }
            }

            if (maxPossible == 0) return 0;

            // Масштабируем до диапазона 0-255 для лучшей визуализации
            return (value * 255) / maxPossible;
        }

        public static Bitmap ApplyBitFilter(Bitmap source, bool useGrayscale, bool useRed, bool useGreen, bool useBlue, bool[] selectedBits)
        {
            Bitmap result = new Bitmap(source.Width, source.Height);

            // Вычисляем параметры маски ОДИН раз до циклов
            int selectedCount = 0;
            int maxPossible = 0;
            for (int i = 0; i < 8; i++)
            {
                if (selectedBits[i])
                {
                    selectedCount++;
                    maxPossible |= (1 << i);
                }
            }

            if (selectedCount == 0)
                return result; // все пиксели чёрные (уже 0)

            float scale = 255f / maxPossible;

            // Блокируем биты
            BitmapData srcData = source.LockBits(
                new Rectangle(0, 0, source.Width, source.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            BitmapData resData = result.LockBits(
                new Rectangle(0, 0, result.Width, result.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);

            int totalPixels = source.Width * source.Height;

            // Копируем ВСЕ пиксели исходного изображения в управляемый массив
            byte[] srcPixels = new byte[totalPixels * 4];
            byte[] resPixels = new byte[totalPixels * 4];

            Marshal.Copy(srcData.Scan0, srcPixels, 0, srcPixels.Length);

            // Обрабатываем пиксели в управляемом массиве
            for (int i = 0; i < srcPixels.Length; i += 4)
            {
                byte B = srcPixels[i];
                byte G = srcPixels[i + 1];
                byte R = srcPixels[i + 2];

                int finalR = 0, finalG = 0, finalB = 0;

                if (useGrayscale)
                {
                    // Целочисленное вычисление серого (быстрее, чем float)
                    int gray = (R * 77 + G * 150 + B * 29) >> 8;
                    int masked = ApplyBitMask(gray, selectedBits);
                    byte scaled = (byte)(masked * scale);
                    finalR = finalG = finalB = scaled;
                }
                else
                {
                    if (useRed)
                        finalR = (byte)(ApplyBitMask(R, selectedBits) * scale);
                    if (useGreen)
                        finalG = (byte)(ApplyBitMask(G, selectedBits) * scale);
                    if (useBlue)
                        finalB = (byte)(ApplyBitMask(B, selectedBits) * scale);
                }

                resPixels[i] = (byte)finalB;
                resPixels[i + 1] = (byte)finalG;
                resPixels[i + 2] = (byte)finalR;
                resPixels[i + 3] = 255;
            }

            // Копируем обработанный массив обратно в результирующий Bitmap
            Marshal.Copy(resPixels, 0, resData.Scan0, resPixels.Length);

            source.UnlockBits(srcData);
            result.UnlockBits(resData);

            return result;
        }

        // Быстрое извлечение нужных битов (без цикла)
        private static int ApplyBitMask(int value, bool[] selectedBits)
        {
            int result = 0;
            if (selectedBits[0] && (value & 1) != 0) result |= 1;
            if (selectedBits[1] && (value & 2) != 0) result |= 2;
            if (selectedBits[2] && (value & 4) != 0) result |= 4;
            if (selectedBits[3] && (value & 8) != 0) result |= 8;
            if (selectedBits[4] && (value & 16) != 0) result |= 16;
            if (selectedBits[5] && (value & 32) != 0) result |= 32;
            if (selectedBits[6] && (value & 64) != 0) result |= 64;
            if (selectedBits[7] && (value & 128) != 0) result |= 128;
            return result;
        }
    }
}
