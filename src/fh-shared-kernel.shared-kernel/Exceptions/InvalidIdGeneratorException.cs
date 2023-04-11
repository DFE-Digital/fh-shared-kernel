namespace FamilyHubs.SharedKernel
{
    public class InvalidIdGeneratorException : Exception
    {
        public InvalidIdGeneratorException()
        {
        }

        public InvalidIdGeneratorException(string message)
            : base(message)
        {
        }

        public InvalidIdGeneratorException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
