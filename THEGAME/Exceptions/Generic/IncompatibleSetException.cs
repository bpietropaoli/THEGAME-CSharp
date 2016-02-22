using System;

namespace THEGAME.Exceptions
{
    /// <summary>
    /// Thrown when a incompatible set is passed as a parameter.
    /// </summary>
    [Serializable()]
    public class IncompatibleSetException : System.Exception
    {
        /// <summary>
        /// Base constructor.
        /// </summary>
        public IncompatibleSetException() : base() { }

        /// <summary>
        /// Base constructor.
        /// </summary>
        public IncompatibleSetException(string message) : base(message) { }

        /// <summary>
        /// Base constructor.
        /// </summary>
        public IncompatibleSetException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an 
        // exception propagates from a remoting server to the client.  
        /// <summary>
        /// Base constructor.
        /// </summary>
        protected IncompatibleSetException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
