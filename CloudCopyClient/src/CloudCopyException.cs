namespace CloudCopy
{
    [System.Serializable]
    public class CloudCopyException : System.Exception
    {
        public CloudCopyException()
        {
        }

        public CloudCopyException(string message)
            : base(message)
        {
        }

        public CloudCopyException(string message, System.Exception inner)
            : base(message, inner)
        {
        }

        protected CloudCopyException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}