
using System.IO;
using System.Xml;

namespace CloudCopy
{
    class C4CEntityMapper : IC4CEntityMapper
    {
        string _EntityName;

        public C4CEntityMapper(string entityName)
        {
            _EntityName = entityName;
        }

        public string getAttachmentCollectionName()
        {
            return _EntityName + "AttachmentFolder";
        }

        public string getCollectionName()
        {
            return _EntityName + "Collection";
        }

        public string getHumanReadableIDName()
        {

            Stream EntityMappingStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("CloudCopy.EntityMapping.xml");

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(EntityMappingStream);

            XmlNode EntiryNode = xmlDoc.SelectSingleNode("/CloudCopyEntityMapping/Entity[@Name='" + _EntityName + "']");

            string HumanReadableIdentifier = EntiryNode.Attributes["HumanReadableIdentifier"].Value;

            System.Console.WriteLine(HumanReadableIdentifier);

            return HumanReadableIdentifier;

        }
    }
}