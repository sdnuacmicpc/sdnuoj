using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Mozilla.NUniversalCharDet;

namespace JudgeClient.Definition
{
    public class EncodingUtils
    {
        public static readonly Encoding Default = Encoding.GetEncoding("gbk");

        public static Encoding Detect(Stream seekable_stream)
        {
            if (!seekable_stream.CanSeek)
                throw new Exception("Detect encoding error: stream can't seek.");

            long ori_pos = seekable_stream.Position;

            int buffer_size = 4096, cur;
            byte[] buffer = new byte[buffer_size];
            UniversalDetector detector = new UniversalDetector(null);
            while ((cur = seekable_stream.Read(buffer, 0, buffer_size)) > 0 && !detector.IsDone())
                detector.HandleData(buffer, 0, cur);
            detector.DataEnd();

            seekable_stream.Seek(ori_pos, SeekOrigin.Begin);

            if (detector.IsDone())
                return Encoding.GetEncoding(detector.GetDetectedCharset());
            return null;
        }

        public static Encoding Detect(byte[] data)
        {
            return Detect(new MemoryStream(data));
        }

        public static string DetectAndReadToEnd(Stream stream, Encoding default_encoding)
        {
            var ms = new MemoryStream();

            int buffer_size = 4096, cur;
            byte[] buffer = new byte[buffer_size];
            bool detect_done = false;
            UniversalDetector detector = new UniversalDetector(null);
            while ((cur = stream.Read(buffer, 0, buffer_size)) > 0)
            {
                ms.Write(buffer, 0, cur);
                if (!detect_done)
                {
                    detector.HandleData(buffer, 0, cur);
                    detect_done = detector.IsDone();
                }
            }
            detector.DataEnd();

            Encoding encoding;
            if (detect_done)
                encoding = Encoding.GetEncoding(detector.GetDetectedCharset());
            else if (default_encoding != null)
                encoding = default_encoding;
            else
                encoding = Default;

            ms.Seek(0, SeekOrigin.Begin);

            using (var sr = new StreamReader(ms, encoding))
                return sr.ReadToEnd();
        }

        public static string DetectAndReadToEnd(Stream stream)
        {
            return DetectAndReadToEnd(stream, null);
        }

        public static string DetectAndReadToEndAndDispose(Stream stream, Encoding default_encoding)
        {
            using (stream)
                return DetectAndReadToEnd(stream, default_encoding);
        }

        public static string DetectAndReadToEndAndDispose(Stream stream)
        {
            return DetectAndReadToEndAndDispose(stream, null);
        }
    }
}
