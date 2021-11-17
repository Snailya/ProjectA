using System;

namespace ProjectA.Core.Services.Exceptions
{
    public class AppServiceException : Exception
    {
        public AppServiceException(string message) : base(message)
        {
        }
    }
}