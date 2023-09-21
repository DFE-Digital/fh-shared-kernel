using System.Runtime.Serialization;

namespace FamilyHubs.SharedKernel.Identity.Exceptions
{
    [Serializable]
    public class OneLoginException : Exception
    {
        public OneLoginException(string message) : base(message)
        {

        }
        protected OneLoginException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}
