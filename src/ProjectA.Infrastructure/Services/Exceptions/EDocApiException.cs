using System;

namespace ProjectA.Infrastructure.Services.Exceptions
{
    public class EDocApiException : Exception
    {
        public EDocApiException(string message) : base(message)
        {
        }
    }
}