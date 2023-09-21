using System.Runtime.Serialization;

namespace FamilyHubs.SharedKernel.Identity.Exceptions
{
    [Serializable]
    public class ClaimsException : Exception
    {
        public ClaimsException(string message) : base(message)
        {

        }

        protected ClaimsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}
