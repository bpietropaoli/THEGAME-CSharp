using System;

namespace THEGAME.Exceptions
{
    /// <summary>
    /// Thrown when not enough mass functions were passed as parameters.
    /// </summary>
    [Serializable()]
    public class InvalidIntervalException : System.Exception
    {
        /// <summary>
        /// Base constructor.
        /// </summary>
        public InvalidIntervalException() : base() { }

        /// <summary>
        /// Base constructor.
        /// </summary>
        public InvalidIntervalException(string message) : base(message) { }

        /// <summary>
        /// Base constructor.
        /// </summary>
        public InvalidIntervalException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an 
        // exception propagates from a remoting server to the client.  
        /// <summary>
        /// Base constructor.
        /// </summary>
        protected InvalidIntervalException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
