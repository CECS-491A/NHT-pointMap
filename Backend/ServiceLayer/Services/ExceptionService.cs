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
        }

        public class InvalidTokenSignatureException : Exception
        {
            public InvalidTokenSignatureException() { }
            public InvalidTokenSignatureException(string message) : base (message) { }
        }

        public class InvalidPointException : Exception
        {
            public InvalidPointException() { }
            public InvalidPointException(string message) : base(message) { }
            public InvalidPointException(string message, Exception innerException) : base(message, innerException) { }
        }
    }
}
