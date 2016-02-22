using System;

namespace THEGAME.Construction.Generic
{
    /// <summary>
    /// A simple structure to represent a sensor measure.
    /// </summary>
    public struct SensorMeasure
    {
        /// <summary>
        /// Gets/Sets the name of the sensor the measure came from.
        /// </summary>
        public string SensorName { get; set; }

        /// <summary>
        /// Gets/Sets the measurement value. A null means that no measure was received.
        /// </summary>
        public double? Measure { get; set; }

        /// <summary>
        /// Basic constructor of a sensor measure.
        /// </summary>
        /// <param name="name">The name of the sensor it came from.</param>
        /// <param name="measure">The measurement value.</param>
        public SensorMeasure(string name, double? measure)
            : this()
        {
            this.SensorName = name;
            this.Measure = measure;
        }

        /// <summary>
        /// Gives a string representation of the sensor measure.
        /// </summary>
        /// <returns>Returns a string representation of the sensor measure.</returns>
        public override string ToString()
        {
            return String.Format("[SensorMeasure: SensorName={0}, Measure={1}]", SensorName, Measure);
        }
    }
}
