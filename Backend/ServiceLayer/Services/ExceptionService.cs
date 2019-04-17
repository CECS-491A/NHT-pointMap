using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public class ExceptionService
    {
        public class InvalidEmailException : Exception
        {
            public InvalidEmailException() { }
            public InvalidEmailException(string message) : base(message) { }
            public InvalidEmailException(string message, Exception innerException) : base(message, innerException) { }
        }

        public class InvalidDbOperationException : Exception
        {
            public InvalidDbOperationException() { }
            public InvalidDbOperationException(string message) : base (message) { }
            public InvalidDbOperationException(string message, Exception innerException) : base(message, innerException) { }
        }

        public class InvalidTokenSignatureException : Exception
        {
            public InvalidTokenSignatureException() { }
            public InvalidTokenSignatureException(string message) : base (message) { }
            public InvalidTokenSignatureException(string message, Exception innerException) : base(message, innerException) { }
        }

        public class InvalidPointException : Exception
        {
            public InvalidPointException() { }
            public InvalidPointException(string message) : base(message) { }
            public InvalidPointException(string message, Exception innerException) : base(message, innerException) { }
        }

        public class InvalidModelPayloadException : Exception
        {
            public InvalidModelPayloadException() { }
            public InvalidModelPayloadException(string message) : base(message) { }
            public InvalidModelPayloadException(string message, Exception innerException) : base(message, innerException) { }
        }

        public class NoTokenProvidedException : Exception
        {
            public NoTokenProvidedException() { }
            public NoTokenProvidedException(string message) : base(message) { }
            public NoTokenProvidedException(string message, Exception innerException) : base(message, innerException) { }
        }

        public class InvalidGuidException : Exception
        {
            public InvalidGuidException() { }
            public InvalidGuidException(string message) : base(message) { }
            public InvalidGuidException(string message, Exception innerException) : base(message, innerException) { }
        }

        public class SessionNotFoundException : Exception
        {
            public SessionNotFoundException() { }
            public SessionNotFoundException(string message) : base(message) { }
            public SessionNotFoundException(string message, Exception innerException) : base(message, innerException) { }
        }

        public class UserIsNotAdministratorException : Exception
        {
            public UserIsNotAdministratorException() { }
            public UserIsNotAdministratorException(string message) : base(message) { }
            public UserIsNotAdministratorException(string message, Exception innerException) : base(message, innerException) { }
        }

        public class UserNotFoundException : Exception
        {
            public UserNotFoundException() { }
            public UserNotFoundException(string message) : base(message) { }
            public UserNotFoundException(string message, Exception innerException) : base(message, innerException) { }
        }

        public class UserAlreadyExistsException : Exception
        {
            public UserAlreadyExistsException() { }
            public UserAlreadyExistsException(string message) : base(message) { }
            public UserAlreadyExistsException(string message, Exception innerException) : base(message, innerException) { }
        }

        public class InvalidHeaderException : Exception
        {
            public InvalidHeaderException() { }
            public InvalidHeaderException(string message) : base(message) { }
            public InvalidHeaderException(string message, Exception innerException) : base(message, innerException) { }
        }
    }
}
