using System;

namespace ProjectA.Core.Services.Exceptions
{
    public class EDocApiException : Exception
    {
        public EDocApiException(string message) : base(message)
        {
        }
    }
}