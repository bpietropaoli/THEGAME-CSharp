using System;

namespace THEGAME.Exceptions
{
    /// <summary>
    /// Thrown when a combination rule is not defined for a specific case (typically, the Dempster's rule when
    /// there is a total conflict).
    /// </summary>
	[Serializable()]
	public class CombinationNotDefinedException : System.Exception
	{
        /// <summary>
        /// Base constructor.
        /// </summary>
		public CombinationNotDefinedException() : base() { }

        /// <summary>
        /// Base constructor.
        /// </summary>
		public CombinationNotDefinedException(string message) : base(message) { }

        /// <summary>
        /// Base constructor.
        /// </summary>
		public CombinationNotDefinedException(string message, System.Exception inner) : base(message, inner) { }

		// A constructor is needed for serialization when an 
		// exception propagates from a remoting server to the client.  
        /// <summary>
        /// Base constructor.
        /// </summary>
		protected CombinationNotDefinedException(System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}