using System;

namespace THEGAME.Exceptions
{
    /// <summary>
    /// Thrown when an empty mass function is passed as a parameter or when a methods is called
    /// on an empty mass function.
    /// </summary>
	[Serializable()]
	public class EmptyMassFunctionException : System.Exception
	{
        /// <summary>
        /// Base constructor.
        /// </summary>
		public EmptyMassFunctionException() : base() { }

        /// <summary>
        /// Base constructor.
        /// </summary>
		public EmptyMassFunctionException(string message) : base(message) { }

        /// <summary>
        /// Base constructor.
        /// </summary>
		public EmptyMassFunctionException(string message, System.Exception inner) : base(message, inner) { }

		// A constructor is needed for serialization when an 
		// exception propagates from a remoting server to the client.  
        /// <summary>
        /// Base constructor.
        /// </summary>
		protected EmptyMassFunctionException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}