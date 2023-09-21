using System.Runtime.Serialization;

namespace FamilyHubs.SharedKernel.Identity.Exceptions
{
    [Serializable]
    public class AuthConfigurationException : Exception
    {
        public AuthConfigurationException(string message):base(message) 
        {

        }

        protected AuthConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}
