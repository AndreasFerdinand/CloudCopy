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

        public static IEnumerable<string> getSupportedEntities()
        {
            var xmlDoc = C4CEntityMapper.getEntityXmlDoc();
            var entities = new List<string>();

            foreach( XmlNode node in xmlDoc.SelectNodes("/CloudCopyEntityMapping/Entity") )
            {
                entities.Add(node.Attributes["Name"].Value);
            }

            return entities;
        }

        protected static XmlDocument getEntityXmlDoc()
        {
            Stream entityMappingStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("LibCloudCopy.EntityMapping.xml");

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(entityMappingStream);

            return xmlDoc;
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
            var xmlDoc = C4CEntityMapper.getEntityXmlDoc();

            XmlNode entityNode = xmlDoc.SelectSingleNode("/CloudCopyEntityMapping/Entity[@Name='" + this.entityName + "']");

            string userFriendlyIDName = entityNode.Attributes["HumanReadableIdentifier"].Value;

            return userFriendlyIDName;
        }
    }
}