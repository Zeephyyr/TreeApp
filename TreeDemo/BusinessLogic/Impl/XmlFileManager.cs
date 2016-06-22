using System;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;
using BusinessLogic.Contracts;
using Nodes.Impl;

namespace BusinessLogic.Impl
{
    public class XmlFileManager:IFileManager
    {
        private readonly string _path;

        public XmlFileManager()
        {
            _path = ConfigurationManager.AppSettings["XmlPath"];
        }

        public void SaveToFile(Tree tree, Guid userId)
        {
            var fileName = userId + ".xml";
            XmlSerializer xmlSaver = new XmlSerializer(typeof(Tree), new Type[] { typeof(Node) });
            using (Stream fStream = new FileStream(Path.Combine(_path, fileName), FileMode.Create, FileAccess.Write))
            {
                xmlSaver.Serialize(fStream, tree);
            }
        }

        public Tree RestoreFromFile(Guid userId)
        {
            Tree root = new Tree();
            var fileName = userId + ".xml";
            XmlSerializer xmlOpener = new XmlSerializer(typeof(Tree), new Type[] { typeof(Node) });
            using (Stream fStream = new FileStream(Path.Combine(_path, fileName), FileMode.OpenOrCreate, FileAccess.Read))
            {
                root = (Tree)xmlOpener.Deserialize(fStream);
            }
            return root;
        }
    }
}
