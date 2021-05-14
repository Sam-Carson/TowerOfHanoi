using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary
{
    [Serializable]
    public class InvalidHeightException : Exception
    {
        public static string errorMessage = "Discs must be between 1 - 9";

        public InvalidHeightException() : base(errorMessage) { }

        public InvalidHeightException(string message) : base(errorMessage) { }

        public InvalidHeightException(string messsage, Exception innerException)
            : base(errorMessage, innerException) { }
    }
}
