namespace CloudCopy
{
    [System.Serializable]
    public class CloudCopyParametrizationException : CloudCopyException
    {
        public CloudCopyParametrizationException()
        {
        }

        public CloudCopyParametrizationException(string message)
            : base(message)
        {
        }

        public CloudCopyParametrizationException(string message, System.Exception inner)
            : base(message, inner)
        {
        }

        protected CloudCopyParametrizationException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}