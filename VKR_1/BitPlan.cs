using System;
using System.Collections.Generic;
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

        public static Bitmap ApplyBitFilter(Bitmap source, bool useGrayscale, bool useRed, bool useGreen, bool useBlue, bool[] selectedBits)
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

            // Опционально: масштабируем результат, чтобы он занимал весь диапазон 0-255
            result = ScaleValueToFullRange(result, selectedBits);

            return result;
        }

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
    }
}
