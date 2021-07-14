using System;
using System.Runtime.Serialization;

namespace CloudCopy
{
    [System.Serializable]
    public class TargetEntityInvalidFormatException : C4CClientException
    {
        private readonly string targetEntity;
        public string TargetEntity => targetEntity;

        public TargetEntityInvalidFormatException() : base()
        {
        }

        public TargetEntityInvalidFormatException(string targetEntity) : base(String.Format("{0} is not a valid TargetEntity format", targetEntity))
        {
            this.targetEntity = targetEntity;
        }

        public TargetEntityInvalidFormatException(string targetEntity, string message) : base(message)
        {
            this.targetEntity = targetEntity;
        }

        public TargetEntityInvalidFormatException(string targetEntity, string message, Exception inner) : base(message, inner)
        {
            this.targetEntity = targetEntity;
        }

        protected TargetEntityInvalidFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}