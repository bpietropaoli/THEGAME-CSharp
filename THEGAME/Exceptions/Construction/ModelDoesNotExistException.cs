using System;

namespace THEGAME.Exceptions
{
    /// <summary>
    /// Thrown when a model is missing (for instance, in DiscreteBeliefFromSensors when a sensor is declared without any model...).
    /// </summary>
	[Serializable()]
	public class ModelDoesNotExistException : System.Exception
	{
        /// <summary>
        /// Base constructor.
        /// </summary>
		public ModelDoesNotExistException() : base() { }

        /// <summary>
        /// Base constructor.
        /// </summary>
		public ModelDoesNotExistException(string message) : base(message) { }

        /// <summary>
        /// Base constructor.
        /// </summary>
		public ModelDoesNotExistException(string message, System.Exception inner) : base(message, inner) { }

		// A constructor is needed for serialization when an 
		// exception propagates from a remoting server to the client.  
        /// <summary>
        /// Base constructor.
        /// </summary>
		protected ModelDoesNotExistException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}