using System;

namespace Application.Exceptions
{
    public class ServiceException : Exception
    {
        public ServiceException() : base()
        {
        }

        public ServiceException(string message) : base(message)
        {
        }

        public ServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}