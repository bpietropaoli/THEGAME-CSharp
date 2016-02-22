using System;

namespace THEGAME.Exceptions
{
    /// <summary>
    /// Thrown when the a reference list passed as parameter does not seem to correspond to the frame
    /// of discernment on which an element or a mass function has been defined.
    /// </summary>
	[Serializable()]
	public class IncompatibleReferenceListException : System.Exception
	{
        /// <summary>
        /// Base constructor.
        /// </summary>
		public IncompatibleReferenceListException() : base() { }

        /// <summary>
        /// Base constructor.
        /// </summary>
		public IncompatibleReferenceListException(string message) : base(message) { }

        /// <summary>
        /// Base constructor.
        /// </summary>
		public IncompatibleReferenceListException(string message, System.Exception inner) : base(message, inner) { }

		// A constructor is needed for serialization when an 
		// exception propagates from a remoting server to the client.  
        /// <summary>
        /// Base constructor.
        /// </summary>
		protected IncompatibleReferenceListException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
