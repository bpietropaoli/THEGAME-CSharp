using System;

namespace THEGAME.Exceptions
{
    /// <summary>
    /// Thrown when a sensor has already been registered (sensors should have a unique name).
    /// </summary>
	[Serializable()]
	public class SensorAlreadyRegisteredException : System.Exception
	{
        /// <summary>
        /// Base constructor.
        /// </summary>
		public SensorAlreadyRegisteredException() : base() { }

        /// <summary>
        /// Base constructor.
        /// </summary>
		public SensorAlreadyRegisteredException(string message) : base(message) { }

        /// <summary>
        /// Base constructor.
        /// </summary>
		public SensorAlreadyRegisteredException(string message, System.Exception inner) : base(message, inner) { }

		// A constructor is needed for serialization when an 
		// exception propagates from a remoting server to the client.  
        /// <summary>
        /// Base constructor.
        /// </summary>
		protected SensorAlreadyRegisteredException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}