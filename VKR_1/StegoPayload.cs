using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NIRSteg_2
{
    public static class StegoPayload
    {
        private const byte TextPrefix = 0x01;
        private const byte FilePrefix = 0x02;

        public static byte[] PackText(string message)
        {
            byte[] textBytes = Encoding.UTF8.GetBytes(message);
            byte[] result = new byte[1 + textBytes.Length];
            result[0] = TextPrefix;
            Array.Copy(textBytes, 0, result, 1, textBytes.Length);
            return result;
        }

        public static byte[] PackFile(string filePath)
        {
            byte[] content = File.ReadAllBytes(filePath);
            string fileName = Path.GetFileName(filePath);
            byte[] nameBytes = Encoding.UTF8.GetBytes(fileName);
            byte nameLength = (byte)nameBytes.Length;

            byte[] result = new byte[1 + 1 + nameBytes.Length + content.Length];
            result[0] = FilePrefix;
            result[1] = nameLength;
            Array.Copy(nameBytes, 0, result, 2, nameBytes.Length);
            Array.Copy(content, 0, result, 2 + nameBytes.Length, content.Length);
            return result;
        }

        public static void Unpack(byte[] data, out string text, out byte[] fileBytes, out string fileName)
        {
            text = null;
            fileBytes = null;
            fileName = null;

            if (data == null || data.Length < 1)
                throw new ArgumentException("Пустой или недопустимый формат данных");

            if (data[0] == TextPrefix)
            {
                text = Encoding.UTF8.GetString(data, 1, data.Length - 1);
            }
            else if (data[0] == FilePrefix)
            {
                if (data.Length < 2)
                    throw new Exception("Некорректный формат файла");

                byte nameLength = data[1];
                if (data.Length < 2 + nameLength)
                    throw new Exception("Имя файла повреждено");

                fileName = Encoding.UTF8.GetString(data, 2, nameLength);
                int contentStart = 2 + nameLength;
                int contentLength = data.Length - contentStart;
                fileBytes = new byte[contentLength];
                Array.Copy(data, contentStart, fileBytes, 0, contentLength);
            }
            else
            {
                throw new Exception("Неизвестный тип данных в стеганограмме");
            }
        }
    }
}
