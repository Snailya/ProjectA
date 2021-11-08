using System;

namespace ProjectA.Core.Services.Exceptions
{
    public class RepositoryException : Exception
    {
        public RepositoryException(string message) : base(message)
        {
        }
    }
}