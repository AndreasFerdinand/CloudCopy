namespace CloudCopy
{
    using System;
    using System.Collections;
    using System.Runtime.Serialization;


    public class EntityException : System.Exception
    {
        private readonly string entity;

        public EntityException()
        {
        }

        public EntityException(string entity) : base(string.Format("Entity {0} is unknown",entity))
        {
            this.entity = entity;
        }

        public EntityException(string entity, string message) : base(message)
        {
            this.entity = entity;
        }

        public  EntityException(string entity, string message, Exception inner) : base(message, inner)
        {
            this.entity = entity;
        }
        
        public string Entity => this.entity;
    }
}