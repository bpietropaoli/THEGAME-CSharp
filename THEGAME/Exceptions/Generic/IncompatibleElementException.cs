using System;

namespace THEGAME.Exceptions
{
    /// <summary>
    /// Thrown when an incompatible element is passed as a parameter.
    /// </summary>
    [Serializable()]
    public class IncompatibleElementException : System.Exception
    {
        /// <summary>
        /// Base constructor.
        /// </summary>
        public IncompatibleElementException() : base() { }

        /// <summary>
        /// Base constructor.
        /// </summary>
        public IncompatibleElementException(string message) : base(message) { }

        /// <summary>
        /// Base constructor.
        /// </summary>
        public IncompatibleElementException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an 
        // exception propagates from a remoting server to the client.  
        /// <summary>
        /// Base constructor.
        /// </summary>
        protected IncompatibleElementException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
