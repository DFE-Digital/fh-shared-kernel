using System.Runtime.Serialization;

namespace FamilyHubs.SharedKernel.Identity.Exceptions
{
    [Serializable]
    public class SessionException : Exception
    {
        public SessionException(string message) : base(message)
        {

        }

        protected SessionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}
