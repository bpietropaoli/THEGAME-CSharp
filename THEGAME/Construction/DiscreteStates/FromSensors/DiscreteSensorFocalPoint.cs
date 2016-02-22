using System;


namespace THEGAME.Construction.DiscreteStates.FromSensors
{
    /// <summary>
    /// A simple structure to model points in the sensor belief models.
    /// </summary>
    public struct DiscreteSensorFocalPoint : IComparable<DiscreteSensorFocalPoint>
    {
        /*
         * Properties:
         */

        /// <summary>
        /// Gets the sensor measure associated to this point.
        /// </summary>
        public double SensorMeasure { get; private set; }

        /// <summary>
        /// Gets the belief value associated to the sensor measure.
        /// </summary>
        public double BeliefValue { get; private set; }

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        /*
         * Constructor:
         */

        /// <summary>
        /// A simple complete constructor.
        /// </summary>
        /// <param name="measure">The sensor measure associated to this point.</param>
        /// <param name="belief">The belief value associated to the sensor measure.</param>
        public DiscreteSensorFocalPoint(double measure, double belief)
            : this()
        {
            this.SensorMeasure = measure;
            this.BeliefValue = belief;
        }

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        /*
         * Methods:
         */

        /// <summary>
        /// Tests equality between the current point and the given object.
        /// </summary>
        /// <param name="obj">The object to compare the current point to.</param>
        /// <returns>Returns true if the given object is a point corresponding to the same
        /// sensor measure, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            //Checking:
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            //Equals:
            return ((DiscreteSensorFocalPoint)obj).SensorMeasure == this.SensorMeasure;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the hash code of the current point.
        /// </summary>
        /// <returns>Returns the hash code of the current point.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Compares the current point with the given one. Used to sort the points by their measures
        /// in the models (to prevent weird effect of unsorted points).
        /// </summary>
        /// <param name="bp">The point to compare the current one to.</param>
        /// <returns>Returns -1 if the current point is considered inferior, 1 if considered superior,
        /// 0 if both are equal.</returns>
        public int CompareTo(DiscreteSensorFocalPoint bp)
        {
            if (this.SensorMeasure < bp.SensorMeasure)
            {
                return -1;
            }
            else if (this.SensorMeasure > bp.SensorMeasure)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gives a string representation of the current point.
        /// </summary>
        /// <returns>Returns a string representation of the current point.</returns>
        public override string ToString()
        {
            return String.Format("[SensorMeasure = {0}, BeliefValue = {1}]", SensorMeasure, BeliefValue);
        }

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Operator overrides

        /// <summary>
        /// Overrides "==" operator to check equality between two discrete focal points.
        /// </summary>
        /// <param name="a">The first focal point to compare.</param>
        /// <param name="b">The second focal point to compare.</param>
        /// <returns>Returns true if both focal points are equal, false otherwise.</returns>
        public static bool operator ==(DiscreteSensorFocalPoint a, DiscreteSensorFocalPoint b)
        {
            if ((object)a == null) return (object)b == null;
            if ((object)b == null) return (object)a == null;
            return !a.Equals(b);
        }

        /// <summary>
        /// Overrides "==" operator to check inequality between two discrete focal points.
        /// </summary>
        /// <param name="a">The first focal point to compare.</param>
        /// <param name="b">The second focal point to compare.</param>
        /// <returns>Returns true if both focal points are not equal, false otherwise.</returns>
        public static bool operator !=(DiscreteSensorFocalPoint a, DiscreteSensorFocalPoint b)
        {
            if ((object)a == null) return (object)b != null;
            if ((object)b == null) return (object)a != null;
            return !a.Equals(b);
        }

        #endregion

    } //Struct
} //Namespace
