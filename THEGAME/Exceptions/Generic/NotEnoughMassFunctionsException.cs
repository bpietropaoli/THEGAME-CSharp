using System;

namespace THEGAME.Exceptions
{
    /// <summary>
    /// Thrown when not enough mass functions were passed as parameters.
    /// </summary>
	[Serializable()]
	public class NotEnoughMassFunctionsException : System.Exception
	{
        /// <summary>
        /// Base constructor.
        /// </summary>
		public NotEnoughMassFunctionsException() : base() { }

        /// <summary>
        /// Base constructor.
        /// </summary>
		public NotEnoughMassFunctionsException(string message) : base(message) { }

        /// <summary>
        /// Base constructor.
        /// </summary>
		public NotEnoughMassFunctionsException(string message, System.Exception inner) : base(message, inner) { }

		// A constructor is needed for serialization when an 
		// exception propagates from a remoting server to the client.  
        /// <summary>
        /// Base constructor.
        /// </summary>
		protected NotEnoughMassFunctionsException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}