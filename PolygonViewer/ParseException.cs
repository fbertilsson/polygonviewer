using System;

namespace PolygonViewer
{
    public class ParseException : Exception
    {
        public ParseException(string message, Exception rootCause) 
            : base(message, rootCause)
        {
        }

        public int Index { get; set; }
    }
}
