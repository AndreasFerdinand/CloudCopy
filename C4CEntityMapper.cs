
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
            //toDo;
            if ( _EntityName == "Contact" )
            {
                return "ContactID";
            }
            if ( _EntityName == "Product" )
            {
                return "ProductID";
            }

            return "ID";
        }
    }
}