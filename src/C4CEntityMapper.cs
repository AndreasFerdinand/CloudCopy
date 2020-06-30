namespace CloudCopy
{
    using System.IO;
    using System.Xml;

    public class C4CEntityMapper : IC4CEntityMapper
    {
        private string entityName;

        public C4CEntityMapper(string entityName)
        {
            this.entityName = entityName;
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
            Stream entityMappingStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("CloudCopy.EntityMapping.xml");

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(entityMappingStream);

            XmlNode entityNode = xmlDoc.SelectSingleNode("/CloudCopyEntityMapping/Entity[@Name='" + this.entityName + "']");

            string userFriendlyIDName = entityNode.Attributes["HumanReadableIdentifier"].Value;

            return userFriendlyIDName;
        }
    }
}