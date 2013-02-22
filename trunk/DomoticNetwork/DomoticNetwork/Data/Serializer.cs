using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace DomoticNetwork.Data
{
    class Serializer
    {
        public const String DIRECTORY = "//DATA";
        
        //public static object Deserialize(string nameFile)
        //{
        //    if (Directory.Exists(Directory.GetCurrentDirectory() + DIRECTORY + "//" + nameFile))
        //    {
        //        XmlSerializer serialize = new XmlSerializer(o.GetType());
        //        XmlReader xml = XmlReader.Create(Directory.GetCurrentDirectory() + DIRECTORY + "//" + nameFile);
        //        if (serialize.CanDeserialize(xml))
        //        {
        //            return serialize.Deserialize(xml);
        //        }
        //    }
        //}

        public static void Serialize(object o, String nameFile)
        {
            XmlSerializer serializer = new XmlSerializer(o.GetType());
            XmlWriter xml = XmlWriter.Create(Directory.GetCurrentDirectory() + DIRECTORY + "//" + nameFile);
            serializer.Serialize(xml, o);
        }
    }
}
