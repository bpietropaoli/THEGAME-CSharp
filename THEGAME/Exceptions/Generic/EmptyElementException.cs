using System;

namespace THEGAME.Exceptions
{
    /// <summary>
    /// Thrown when an empty element is passed as a parameter where it shouldn't.
    /// </summary>
	[Serializable()]
	public class EmptyElementException : System.Exception
	{
        /// <summary>
        /// Base constructor.
        /// </summary>
		public EmptyElementException() : base() { }

        /// <summary>
        /// Base constructor.
        /// </summary>
		public EmptyElementException(string message) : base(message) { }

        /// <summary>
        /// Base constructor.
        /// </summary>
		public EmptyElementException(string message, System.Exception inner) : base(message, inner) { }

		// A constructor is needed for serialization when an 
		// exception propagates from a remoting server to the client.  
        /// <summary>
        /// Base constructor.
        /// </summary>
		protected EmptyElementException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}