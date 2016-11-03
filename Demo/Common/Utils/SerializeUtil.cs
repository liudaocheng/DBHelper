using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace DataStruct.Utils
{
    /// <summary>
    /// 序列化
    /// </summary>
    public class SerializeUtil
    {
        /// <summary>
        /// 序列化
        /// </summary>
        public static byte[] Serialize(object obj)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            if (obj == null)
            {
                binaryFormatter.Serialize(ms, string.Empty);
            }
            else
            {
                binaryFormatter.Serialize(ms, obj);
            }
            return ms.ToArray();
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        public static object Deserialize(byte[] bArr)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(bArr);
            return binaryFormatter.Deserialize(ms);
        }
    }
}
