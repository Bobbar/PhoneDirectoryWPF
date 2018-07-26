using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;

namespace PhoneDirectoryWPF.Data
{
    public static class Serializer
    {
        public static void SerializeObject(Type type, object obj, string filepath)
        {
            var mySerializer = new XmlSerializer(type);

            using (var sw = new StreamWriter(filepath))
            {
                mySerializer.Serialize(sw, obj);
                sw.Close();
            }
        }

        public static object DeSerializeObject(Type type, string filePath)
        {
            var mySerializer = new XmlSerializer(type);

            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                return mySerializer.Deserialize(fs);
            }
        }
    }
}
