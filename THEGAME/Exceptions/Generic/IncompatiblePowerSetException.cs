using System;

namespace THEGAME.Exceptions
{
    /// <summary>
    /// Thrown when a incompatible power set is passed as a parameter.
    /// </summary>
	[Serializable()]
	public class IncompatiblePowerSetException : System.Exception
	{
        /// <summary>
        /// Base constructor.
        /// </summary>
		public IncompatiblePowerSetException() : base() { }

        /// <summary>
        /// Base constructor.
        /// </summary>
		public IncompatiblePowerSetException(string message) : base(message) { }

        /// <summary>
        /// Base constructor.
        /// </summary>
		public IncompatiblePowerSetException(string message, System.Exception inner) : base(message, inner) { }

		// A constructor is needed for serialization when an 
		// exception propagates from a remoting server to the client.  
        /// <summary>
        /// Base constructor.
        /// </summary>
		protected IncompatiblePowerSetException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}