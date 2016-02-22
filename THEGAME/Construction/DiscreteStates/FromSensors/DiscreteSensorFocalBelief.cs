using System;
using System.Collections.Generic;
using System.Text;

using THEGAME.Core.DiscreteStates;
using THEGAME.Exceptions;

namespace THEGAME.Construction.DiscreteStates.FromSensors
{
    /// <summary>
    /// A structure to store a membership function of a focal element
    /// in a sensor measure interval. (The models are based on the fuzzy sets.)
    /// </summary>
    public struct DiscreteSensorFocalBelief
    {
        /*
         * Members:
         */
        private DiscreteElement _element;
        private List<DiscreteSensorFocalPoint> _belief;

        //------------------------------------------------------------------------------------------------

        /*
         * Properties:
         */

        /// <summary>
        /// The focal element which this model is associated to.
        /// </summary>
        public DiscreteElement Element
        {
            get { return _element; }
        }

        /// <summary>
        /// The points representing the membership function.
        /// </summary>
        public List<DiscreteSensorFocalPoint> Belief
        {
            get { return _belief; }
        }

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        /*
         * Constructor:
         */

        /// <summary>
        /// A basic constructor from a discrete element. No point is associated first.
        /// </summary>
        /// <param name="e">The element for which a model will be created.</param>
        public DiscreteSensorFocalBelief(DiscreteElement e)
        {
            _element = e;
            _belief = new List<DiscreteSensorFocalPoint>();
        }

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        /*
         * Methods:
         */

        /// <summary>
        /// Gives the belief value given a sensor measure within the current model.
        /// </summary>
        /// <param name="measure">The sensor measure for which the belief value is queried.</param>
        /// <returns>Returns the belief value given a sensor measure within the current model.</returns>
        public double GetEvidence(double measure)
        {
            if (measure <= Belief[0].SensorMeasure)
            {
                return Belief[0].BeliefValue;
            }
            else if (measure >= Belief[Belief.Count - 1].SensorMeasure)
            {
                return Belief[Belief.Count - 1].BeliefValue;
            }
            else
            {
                for (int i = 0; i < Belief.Count - 1; i++)
                {
                    if (Belief[i].SensorMeasure < measure && measure <= Belief[i + 1].SensorMeasure)
                    {
                        return Belief[i].BeliefValue +
                            (Belief[i + 1].BeliefValue - Belief[i].BeliefValue) *
                            (measure - Belief[i].SensorMeasure) / (Belief[i + 1].SensorMeasure - Belief[i].SensorMeasure);
                    }
                }
                return 0;
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Adds a point to the current model.
        /// </summary>
        /// <param name="p">The point to add the the model.</param>
        /// <exception cref="InvalidBeliefModelException">Thrown if the model already contains the given 
        /// point.</exception>
        public void AddPoint(DiscreteSensorFocalPoint p)
        {
            if (_belief.Contains(p))
            {
                throw new InvalidBeliefModelException(String.Format("The model contains twice the same sensor measure for the Element {0}!", _element.ToString()));
            }
            _belief.Add(p);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Adds a point to the current model.
        /// </summary>
        /// <param name="measure">The sensor measure for the new point.</param>
        /// <param name="belief">The belief value associated to the sensor measure.</param>
        /// <exception cref="InvalidBeliefModelException">Thrown if the model already contains the given 
        /// point.</exception>
        public void AddPoint(double measure, double belief)
        {
            this.AddPoint(new DiscreteSensorFocalPoint(measure, belief));
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Sorts the points in the model in the right order.
        /// </summary>
        public void Sort()
        {
            this._belief.Sort();
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Tests the equality of the current focal belief model with the given object.
        /// </summary>
        /// <param name="obj">The object to compare the focal belief model to.</param>
        /// <returns>Returns true if the given object is a focal belief model defined for the
        /// same element, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            //Checking:
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            //Equals:
            return ((DiscreteSensorFocalBelief)obj)._element.Equals(_element);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the hash code of the focal belief model.
        /// </summary>
        /// <returns>Returns the hash code of the focal belief model.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gives a string representation of the current focal belief model.
        /// </summary>
        /// <returns>Returns a string representation of the current focal belief model.</returns>
        public override string ToString()
        {
            StringBuilder toReturn = new StringBuilder(String.Format("Element = {0}\n", Element));
            for (int i = 0; i < Belief.Count; i++)
            {
                if (i == Belief.Count - 1)
                {
                    toReturn.Append(Belief[i]);
                }
                else
                {
                    toReturn.Append(String.Format("{0}\n", Belief[i]));
                }
            }
            return toReturn.ToString();
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gives a string representation of the current focal belief model given a reference list.
        /// </summary>
        /// <param name="refList">The reference list to give sensor to the states/worlds.</param>
        /// <returns>Returns a string representation of the current focal belief model.</returns>
        public string ToString(StringReferenceList refList)
        {
            StringBuilder toReturn = new StringBuilder(String.Format("Element = {0}\n", Element.ToString(refList)));
            for (int i = 0; i < Belief.Count; i++)
            {
                if (i == Belief.Count - 1)
                {
                    toReturn.Append(Belief[i]);
                }
                else
                {
                    toReturn.Append(String.Format("{0}\n", Belief[i]));
                }
            }
            return toReturn.ToString();
        }

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Operator overrides

        /// <summary>
        /// Overrides "==" operator to check equality between two discrete sensor focal beliefs.
        /// </summary>
        /// <param name="a">The first discrete sensor focal belief to compare.</param>
        /// <param name="b">The second discrete sensor focal belief to compare.</param>
        /// <returns>Returns true if both discrete sensor focal beliefs are equal, false otherwise.</returns>
        public static bool operator ==(DiscreteSensorFocalBelief a, DiscreteSensorFocalBelief b)
        {
            if ((object)a == null) return (object)b == null;
            if ((object)b == null) return (object)a == null;
            return !a.Equals(b);
        }

        /// <summary>
        /// Overrides "==" operator to check inequality between two discrete sensor focal beliefs.
        /// </summary>
        /// <param name="a">The first discrete sensor focal belief to compare.</param>
        /// <param name="b">The second discrete sensor focal belief to compare.</param>
        /// <returns>Returns true if both discrete sensor focal beliefs are not equal, false otherwise.</returns>
        public static bool operator !=(DiscreteSensorFocalBelief a, DiscreteSensorFocalBelief b)
        {
            if ((object)a == null) return (object)b != null;
            if ((object)b == null) return (object)a != null;
            return !a.Equals(b);
        }

        #endregion

    } //Struct
} //Namespace
