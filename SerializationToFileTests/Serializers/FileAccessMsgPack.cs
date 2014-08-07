using System;
using System.Diagnostics;
using System.IO;
using MsgPack.Serialization;
using SerializationToFiles.Dtos;

namespace SerializationToFiles.Serializers
{
    public class FileAccessMsgPack : FileAccess
    {
        public static void WriteReadNewtonsoftFileJson(string filePath)
        {
            var stopwatchTotal = new Stopwatch();
            var stopwatchWrite = new Stopwatch();
            var stopwatchRead = new Stopwatch();
            stopwatchTotal.Start();
            stopwatchWrite.Start();
            long length = WriteMsgPack(filePath);
            stopwatchWrite.Stop();
            stopwatchRead.Start();
            ReadMsgPack(filePath);
            stopwatchRead.Stop();
            stopwatchTotal.Stop();

            ExcelResultsReadAndWrite.AppendFormat("{0},", stopwatchTotal.ElapsedMilliseconds);
            ExcelResultsRead.AppendFormat("{0},", stopwatchRead.ElapsedMilliseconds);
            ExcelResultsWrite.AppendFormat("{0},", stopwatchWrite.ElapsedMilliseconds);
            ExcelResultsSize.AppendFormat("{0},", length);

            Console.WriteLine("MsgPack \t\t R/W:{0} \t R:{2} \t W:{1} \t Size:{3}",
                stopwatchTotal.ElapsedMilliseconds, stopwatchWrite.ElapsedMilliseconds,
                stopwatchRead.ElapsedMilliseconds,
                length);

            File.Delete(filePath);
        }

        private static long WriteMsgPack(string path)
        {
            var serializer = SerializationContext.Default.GetSerializer<SimpleTransferProtobuf>();
         
            using (var sw = new StreamWriter(path))
            {
                serializer.Pack(sw.BaseStream, GetTestObjects());
            }

            return new FileInfo(path).Length;
        }

        private static void ReadMsgPack(string path)
        {
            var serializer = SerializationContext.Default.GetSerializer<SimpleTransferProtobuf>();

            SimpleTransferProtobuf simpleTransferProtobuf;
            using (StreamReader file = File.OpenText(path))
            {
                simpleTransferProtobuf = (SimpleTransferProtobuf)serializer.Unpack(file.BaseStream);
            }
        }

    }
}
