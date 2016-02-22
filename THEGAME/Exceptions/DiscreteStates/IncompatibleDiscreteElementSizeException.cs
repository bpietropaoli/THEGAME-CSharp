using System;

namespace THEGAME.Exceptions
{
    /// <summary>
    /// Thrown when an operation on elements is called with elements defined on apparently different frames
    /// of discernment.
    /// </summary>
	[Serializable()]
	public class IncompatibleDiscreteElementSizeException : System.Exception
	{
        /// <summary>
        /// Base constructor.
        /// </summary>
		public IncompatibleDiscreteElementSizeException() : base() { }

        /// <summary>
        /// Base constructor.
        /// </summary>
		public IncompatibleDiscreteElementSizeException(string message) : base(message) { }

        /// <summary>
        /// Base constructor.
        /// </summary>
		public IncompatibleDiscreteElementSizeException(string message, System.Exception inner) : base(message, inner) { }

		// A constructor is needed for serialization when an 
		// exception propagates from a remoting server to the client.  
        /// <summary>
        /// Base constructor.
        /// </summary>
        protected IncompatibleDiscreteElementSizeException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}