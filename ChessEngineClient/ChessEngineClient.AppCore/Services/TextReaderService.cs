using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace ChessEngineClient.Services
{
    public class TextReaderService : ITextReaderService
    {
        public async Task<string> ReadText(IStorageFile file)
        {
            using (var stream = await file.OpenAsync(FileAccessMode.Read))
            using (var inputStream = stream.GetInputStreamAt(0))
            using (var dataReader = new DataReader(inputStream))
            {
                uint numBytesLoaded = await dataReader.LoadAsync((uint)stream.Size);
                byte[] fileBytes = new byte[numBytesLoaded];
                dataReader.ReadBytes(fileBytes);

                Encoding fileEncoding = DetectEncoding(fileBytes);
                int textOffset = GetTextOffset(fileBytes, fileEncoding);

                return fileEncoding.GetString(fileBytes, textOffset, fileBytes.Length - textOffset);
            }
        }

        private static Encoding DetectEncoding(byte[] fileBytes)
        {
            Encoding result = null;
            if (fileBytes == null || fileBytes.Length < 3)
                return Encoding.UTF8;

            if (HasEncodingBOM(fileBytes, Encoding.UTF8))
                result = Encoding.UTF8;
            else if (HasEncodingBOM(fileBytes, Encoding.Unicode))
                result = Encoding.Unicode;
            else if (HasEncodingBOM(fileBytes, Encoding.BigEndianUnicode))
                result = Encoding.BigEndianUnicode;
            else if (HasEncodingBOM(fileBytes, Encoding.UTF32))
                result = Encoding.UTF32;

            if (result == null)
            {
                if (fileBytes.Length > 4 &&
                    fileBytes[0] == 0x0 &&
                    fileBytes[2] == 0x0)
                {
                    result = Encoding.BigEndianUnicode;
                }
                else if (fileBytes.Length > 4 &&
                    fileBytes[1] == 0x0 &&
                    fileBytes[3] == 0x0)
                {
                    result = Encoding.Unicode;
                }
                else
                {
                    result = Encoding.UTF8;
                }
            }

            return result;
        }

        private static bool HasEncodingBOM(byte[] bytes, Encoding encoding)
        {
            return bytes.Length > encoding.GetPreamble().Length &&
                bytes.Take(encoding.GetPreamble().Length).SequenceEqual(encoding.GetPreamble());
        }

        private static int GetTextOffset(byte[] fileBytes, Encoding fileEncoding)
        {
            int result = 0;
            foreach (var item in fileEncoding.GetPreamble())
            {
                if (fileBytes[result] == item)
                    result++;
                else
                    break;
            }
            return result;
        }
    }
}
