using System;
namespace Basic.Infrastructure
{
    /// <summary>
    /// Basic exception.
    /// </summary>
    public class BasicException : Exception
    {
        public BasicException(string msg)
            : base(msg)
        {
        }
    }

    /// <summary>
    /// Runtime error encountered
    /// </summary>
    public class BasicRuntimeException : BasicException
    {
        public BasicRuntimeException(string msg)
            : base(msg)
        {
        }        
    }

    /// <summary>
    /// Syntax error found
    /// </summary>
    public class BasicSyntaxException : BasicException
    {
        public string Location { get; set; } = String.Empty;

        public BasicSyntaxException(string msg)
           : base(msg)
        {
        }       
    }
}
