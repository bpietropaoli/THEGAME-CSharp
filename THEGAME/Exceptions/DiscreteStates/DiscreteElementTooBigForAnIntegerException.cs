using System;

namespace THEGAME.Exceptions
{
    /// <summary>
    /// Thrown when one requires the integer representation of an element but it is made of more
    /// possible states than the number of bits in an integer.
    /// </summary>
	[Serializable()]
	public class DiscreteElementTooBigForAnIntegerException : System.Exception
	{
        /// <summary>
        /// Base constructor.
        /// </summary>
		public DiscreteElementTooBigForAnIntegerException() : base() { }

        /// <summary>
        /// Base constructor.
        /// </summary>
		public DiscreteElementTooBigForAnIntegerException(string message) : base(message) { }

        /// <summary>
        /// Base constructor.
        /// </summary>
		public DiscreteElementTooBigForAnIntegerException(string message, System.Exception inner) : base(message, inner) { }

		// A constructor is needed for serialization when an 
		// exception propagates from a remoting server to the client.  
        /// <summary>
        /// Base constructor.
        /// </summary>
        protected DiscreteElementTooBigForAnIntegerException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}