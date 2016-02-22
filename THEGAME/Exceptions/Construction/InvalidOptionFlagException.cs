using System;

namespace THEGAME.Exceptions
{
    /// <summary>
    /// Thrown when an unknown or invalid flag for the options (in DiscreteBeliefFromSensors) has been detected
    /// in one of the models.
    /// </summary>
	[Serializable()]
	public class InvalidOptionFlagException : System.Exception
	{
        /// <summary>
        /// Base constructor.
        /// </summary>
		public InvalidOptionFlagException() : base() { }

        /// <summary>
        /// Base constructor.
        /// </summary>
		public InvalidOptionFlagException(string message) : base(message) { }

        /// <summary>
        /// Base constructor.
        /// </summary>
		public InvalidOptionFlagException(string message, System.Exception inner) : base(message, inner) { }

		// A constructor is needed for serialization when an 
		// exception propagates from a remoting server to the client.  
        /// <summary>
        /// Base constructor.
        /// </summary>
		protected InvalidOptionFlagException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}