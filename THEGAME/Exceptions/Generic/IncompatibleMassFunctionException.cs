using System;

namespace THEGAME.Exceptions
{
    /// <summary>
    /// Thrown when an incompatible mass function is passed as a paramater.
    /// </summary>
	[Serializable()]
	public class IncompatibleMassFunctionException : System.Exception
	{
        /// <summary>
        /// Base constructor.
        /// </summary>
		public IncompatibleMassFunctionException() : base() { }

        /// <summary>
        /// Base constructor.
        /// </summary>
		public IncompatibleMassFunctionException(string message) : base(message) { }

        /// <summary>
        /// Base constructor.
        /// </summary>
		public IncompatibleMassFunctionException(string message, System.Exception inner) : base(message, inner) { }

		// A constructor is needed for serialization when an 
		// exception propagates from a remoting server to the client.  
        /// <summary>
        /// Base constructor.
        /// </summary>
		protected IncompatibleMassFunctionException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}