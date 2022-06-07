namespace CloudCopy
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;

    public class C4CEntityMapper : IC4CEntityMapper
    {
        private readonly string entityName;

        public C4CEntityMapper(string entityName)
        {
            this.entityName = entityName;
        }

        public static IEnumerable<string> GetSupportedEntities()
        {
            var xmlDoc = C4CEntityMapper.GetEntityXmlDoc();
            var entities = new List<string>();

            foreach (XmlNode node in xmlDoc.SelectNodes("/CloudCopyEntityMapping/Entity"))
            {
                entities.Add(node.Attributes["Name"].Value);
            }

            return entities;
        }

        public string GetAttachmentCollectionName()
        {
            return this.entityName + "AttachmentFolder";
        }

        public string GetCollectionName()
        {
            return this.entityName + "Collection";
        }

        public string GetNameOfUserFriendlyID()
        {
            var xmlDoc = C4CEntityMapper.GetEntityXmlDoc();

            XmlNode entityNode = xmlDoc.SelectSingleNode("/CloudCopyEntityMapping/Entity[@Name='" + this.entityName + "']");

            string userFriendlyIDName = entityNode.Attributes["HumanReadableIdentifier"].Value;

            return userFriendlyIDName;
        }

        protected static XmlDocument GetEntityXmlDoc()
        {
            Stream entityMappingStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("LibCloudCopy.EntityMapping.xml");

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(entityMappingStream);

            return xmlDoc;
        }
    }
}