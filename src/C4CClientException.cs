namespace CloudCopy
{
    [System.Serializable]
    public class C4CClientException : System.Exception
    {
        public C4CClientException()
        {
        }

        public C4CClientException(string message)
            : base(message)
        {
        }

        public C4CClientException(string message, System.Exception inner)
            : base(message, inner)
        {
        }

        protected C4CClientException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}