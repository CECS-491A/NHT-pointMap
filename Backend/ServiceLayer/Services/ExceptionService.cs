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

        public class KFCSSOAPIRequestException : Exception
        {
            public KFCSSOAPIRequestException() { }
            public KFCSSOAPIRequestException(string message) : base (message) { }
        }
    }
}
