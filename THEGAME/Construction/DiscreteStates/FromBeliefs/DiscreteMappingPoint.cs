using System;

using THEGAME.Core.DiscreteStates;
using THEGAME.Exceptions;

namespace THEGAME.Construction.DiscreteStates.FromBeliefs
{
    /// <summary>
    /// A simple struct to represent a element of an evidential mapping matrix.
    /// </summary>
    public struct DiscreteMappingPoint
    {
        #region Properties

        /*
         * Properties:
         */

        /// <summary>
        /// The element to which belief will be propagated.
        /// </summary>
        public DiscreteElement Element { get; set; }

        /// <summary>
        /// The mapping factor to propagate belief.
        /// </summary>
        public double MappingFactor { get; set; }

        /// <summary>
        /// The size of the element (the number of possible states/worlds
        /// in the frame of discernment).
        /// </summary>
        public int Size
        {
            get { return Element.Size; }
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        /*
         * Constructor:
         */

        /// <summary>
        /// Simple complete constructor.
        /// </summary>
        /// <param name="e">The element to propagate belief to.</param>
        /// <param name="factor">The factor to propagate belief.</param>
        /// <exception cref="InvalidBeliefModelException">Thrown if the factor does not respect
        /// 0 &lt; factor &lt; 1.</exception>
        public DiscreteMappingPoint(DiscreteElement e, double factor)
            : this()
        {
            if (0 > factor || factor > 1)
            {
                throw new InvalidBeliefModelException("The factors in the evidential mapping should respect 0 < factor < 1!");
            }
            this.Element = e;
            this.MappingFactor = factor;
        }

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Methods

        /*
         * Methods:
         */

        /// <summary>
        /// Gets the propagated mass given the original mass.
        /// </summary>
        /// <param name="mass">The original mass.</param>
        /// <returns>Returns the propagated mass.</returns>
        public double GetMass(double mass)
        {
            return mass * MappingFactor;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Tests equality of the mapping point with the given object.
        /// </summary>
        /// <param name="obj">The object to compare the point to.</param>
        /// <returns>Returns true if the given object is a mapping point with the same 
        /// recipient element.</returns>
        public override bool Equals(object obj)
        {
            //Checking:
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            //Equals:
            return ((DiscreteMappingPoint)obj).Element.Equals(this.Element);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the hash code of the mapping point.
        /// </summary>
        /// <returns>Returns the hash code of the mapping point.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gives a string representation of the current mapping point.
        /// </summary>
        /// <returns>Returns a string representation of the mapping point.</returns>
        public override string ToString()
        {
            return String.Format("[Element={0}, MappingFactor={1}]", Element, MappingFactor);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gives a string representation of the current mapping point given a reference list.
        /// </summary>
        /// <param name="refList">The reference list to give sense to the states/worlds.</param>
        /// <returns>Returns a string representation of the current mapping point.</returns>
        public string ToString(StringReferenceList refList)
        {
            return String.Format("[Element={0}, MappingFactor={1}]", Element.ToString(refList), MappingFactor);
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Operator overrides

        /// <summary>
        /// Overrides "==" operator to check equality between two mapping points.
        /// </summary>
        /// <param name="a">The first mapping point to compare.</param>
        /// <param name="b">The second mapping point to compare.</param>
        /// <returns>Returns true if both mapping points are equal, false otherwise.</returns>
        public static bool operator ==(DiscreteMappingPoint a, DiscreteMappingPoint b)
        {
            if ((object)a == null) return (object)b == null;
            if ((object)b == null) return (object)a == null;
            return !a.Equals(b);
        }

        /// <summary>
        /// Overrides "==" operator to check inequality between two mapping points.
        /// </summary>
        /// <param name="a">The first mapping point to compare.</param>
        /// <param name="b">The second mapping point to compare.</param>
        /// <returns>Returns true if both mapping points are not equal, false otherwise.</returns>
        public static bool operator !=(DiscreteMappingPoint a, DiscreteMappingPoint b)
        {
            if ((object)a == null) return (object)b != null;
            if ((object)b == null) return (object)a != null;
            return !a.Equals(b);
        }

        #endregion

    } //Struct
} //Namespace
