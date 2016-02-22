using System;

namespace THEGAME.Exceptions
{
    /// <summary>
    /// Thrown when a belief model is obviously invalid.
    /// </summary>
	[Serializable()]
	public class InvalidBeliefModelException : System.Exception
	{
        /// <summary>
        /// Base constructor.
        /// </summary>
		public InvalidBeliefModelException() : base() { }

        /// <summary>
        /// Base constructor.
        /// </summary>
		public InvalidBeliefModelException(string message) : base(message) { }

        /// <summary>
        /// Base constructor.
        /// </summary>
		public InvalidBeliefModelException(string message, System.Exception inner) : base(message, inner) { }

		// A constructor is needed for serialization when an 
		// exception propagates from a remoting server to the client.  
        /// <summary>
        /// Base constructor.
        /// </summary>
		protected InvalidBeliefModelException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}