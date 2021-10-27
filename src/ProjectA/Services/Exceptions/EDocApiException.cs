using System;

namespace ProjectA.Models
{
    public class EDocApiException : Exception
    {
        public EDocApiException(string message) : base(message)
        {
        }
    }
}