using System;

namespace THEGAME.Exceptions
{
    /// <summary>
    /// Thrown when a belief construction method cannot process the given parameters (typically, a random
    /// belief function generator which is asked for belief functions with 0 or less focal elements).
    /// </summary>
	[Serializable()]
	public class InvalidBeliefConstructorException : System.Exception
	{
        /// <summary>
        /// Base constructor.
        /// </summary>
		public InvalidBeliefConstructorException() : base() { }

        /// <summary>
        /// Base constructor.
        /// </summary>
		public InvalidBeliefConstructorException(string message) : base(message) { }

        /// <summary>
        /// Base constructor.
        /// </summary>
		public InvalidBeliefConstructorException(string message, System.Exception inner) : base(message, inner) { }

		// A constructor is needed for serialization when an 
		// exception propagates from a remoting server to the client.  
        /// <summary>
        /// Base constructor.
        /// </summary>
		protected InvalidBeliefConstructorException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}