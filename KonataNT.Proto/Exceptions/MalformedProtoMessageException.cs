namespace KonataNT.Proto.Exceptions
{
    public class MalformedProtoMessageException : Exception
    {
        private const string DefaultMessage = "给定的Tar消息不完整。";

        public MalformedProtoMessageException() : this(DefaultMessage)
        {

        }

        public MalformedProtoMessageException(string? message) : this(message, null)
        {

        }

        public MalformedProtoMessageException(string? message, Exception? innerException) : base(message ?? DefaultMessage, innerException)
        {

        }
    }
}
