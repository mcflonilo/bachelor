using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UltraBend.Common.Math;

namespace UltraBend.Common.Data
{
    public static class ByteSerialization
    {
        public static byte[] GetBytes(double[] values)
        {
            if (values.Length == 0)
                return new byte[] { };

            var result = new byte[values.Length * sizeof(double)];
            Buffer.BlockCopy(values, 0, result, 0, result.Length);
            return result;
        }
        public static double[] GetDoubles(byte[] bytes)
        {
            if (bytes.Length == 0)
                return new double[] { };

            var result = new double[bytes.Length / sizeof(double)];
            Buffer.BlockCopy(bytes, 0, result, 0, bytes.Length);
            return result;
        }

        public static byte[] GetBytesByDataContract<T>(T contract)
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractSerializer(typeof(T));
                var binaryDictionaryWriter = XmlDictionaryWriter.CreateBinaryWriter(stream);

                serializer.WriteObject(binaryDictionaryWriter, contract);
                binaryDictionaryWriter.Flush();

                stream.Position = 0;

                using (var reader = new BinaryReader(stream))
                {
                    return reader.ReadBytes((int)stream.Length);
                }
            }
        }

        public static T GetDataContractByBytes<T>(byte[] bytes)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(bytes);
                    stream.Position = 0;

                    var serializer = new DataContractSerializer(typeof(T));
                    var binaryDictionaryReader = XmlDictionaryReader.CreateBinaryReader(stream, null, XmlDictionaryReaderQuotas.Max);

                    return (T)serializer.ReadObject(binaryDictionaryReader);
                }
            }
        }
    }
}
