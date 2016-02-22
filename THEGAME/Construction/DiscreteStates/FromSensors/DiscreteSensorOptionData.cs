using System.Runtime.InteropServices;

using THEGAME.Core.DiscreteStates;


namespace THEGAME.Construction.DiscreteStates.FromSensors
{
    /*/// <summary>
    /// A structure (equivalent to an union in C) to store generic data used by
    /// the options in the discrete sensor belief models. Works only if
    /// the code can be compiled with unsafe code...
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct DiscreteSensorOptionData
    {
        /// <summary>
        /// A double to store a sensor measurement.
        /// </summary>
        [FieldOffset(0)]
        public double? measure;

        /// <summary>
        /// A long to store a time (for temporisation for instance)
        /// </summary>
        [FieldOffset(0)]
        public long time;

        /// <summary>
        /// A mass function to store the previous one for instance (used in the temporisation).
        /// </summary>
        [FieldOffset(0)]
        public DiscreteMassFunction m;
    } //Struct*/


    //The union must be done as unsafe, which is not nice! :(


    /// <summary>
    /// A structure (equivalent to an union in C) to store generic data used by
    /// the options in the discrete sensor belief models. While you could use
    /// the three of them at once, it is used so far only for one type each time.
    /// This is done like that only for type safety.
    /// </summary>
    public struct DiscreteSensorOptionData
    {
        /// <summary>
        /// Gets/Sets a double to store a sensor measurement.
        /// </summary>
        public double? Measure { get; set; }

        /// <summary>
        /// Gets/Sets a long to store a time (for temporisation for instance)
        /// </summary>
        public long Time { get; set; }

        /// <summary>
        /// Gets/Sets a mass function to store the previous one for instance (used in the temporisation).
        /// </summary>
        public DiscreteMassFunction M { get; set; }
    }


} //Namespace
