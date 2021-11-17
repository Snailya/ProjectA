using System;
using EDoc2.IAppService.Model;

namespace ProjectA.Infrastructure.Services.Exceptions
{
    public class EDocApiException<T> : Exception
    {
        public EDocApiException(string message, ReturnValueResult<T> returnValue) : base(
            $"{message}: [{returnValue.Result}] {returnValue.Message}")
        {
        }
    }

    public class EDocApiException : Exception
    {
        public EDocApiException(string message) : base(message)
        {
        }
    }
}