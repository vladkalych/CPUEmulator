using System.Runtime.Serialization;

namespace RAMEm.CPU.Registries.RegistryException
{
    [Serializable]
    public class WrongModeException : Exception
    {
        public WrongModeException()
        {
        }

        public WrongModeException(string? message) : base(message)
        {
        }

        public WrongModeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected WrongModeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}