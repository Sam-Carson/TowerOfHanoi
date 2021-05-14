using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary
{
    [Serializable]
    public class InvalidMoveException : Exception
    {
        public static string errorMessage = "\tInvalid tower number.";

        public InvalidMoveException() : base() { }

        public InvalidMoveException(string message) : base(errorMessage) { }

        public InvalidMoveException(string messsage, Exception innerException)
            : base(errorMessage, innerException) { }
        
    }
}
