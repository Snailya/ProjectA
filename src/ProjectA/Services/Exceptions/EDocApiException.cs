using System;

namespace ProjectA.Services.Exceptions
{
    public class EDocApiException : Exception
    {
        public EDocApiException(string message) : base(message)
        {
        }
    }
}