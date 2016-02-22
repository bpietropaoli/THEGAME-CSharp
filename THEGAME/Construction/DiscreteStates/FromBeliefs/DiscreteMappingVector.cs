using System;
using System.Text;
using System.Collections.Generic;

using THEGAME.Core.DiscreteStates;
using THEGAME.Exceptions;

namespace THEGAME.Construction.DiscreteStates.FromBeliefs
{
    /// <summary>
    /// A structure to store an evidential vector (a column of the evidential mapping matrix)
    /// used to propagate mass functions from one frame of discernment to another.
    /// </summary>
    public struct DiscreteMappingVector
    {
        /*
         * Members:
         */
        private DiscreteElement _from;
        private List<DiscreteMappingPoint> _to;

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Properties

        /*
         * Properties:
         */
        /// <summary>
        /// The focal element from which the belief will be propagated.
        /// </summary>
        public DiscreteElement From
        {
            get { return _from; }
        }

        /// <summary>
        /// The list of focal elements which will result from the propagation.
        /// </summary>
        public List<DiscreteMappingPoint> To
        {
            get { return _to; }
        }

        /// <summary>
        /// The size of the element from which the belief will be propagated
        /// (the number of possible states/worlds in the frame of discernement
        /// of origin).
        /// </summary>
        public int FromSize
        {
            get { return _from.Size; }
        }

        /// <summary>
        /// The size of the elements to which the belief will be propagated
        /// (the number of possible states worlds in the frame of discernment
        /// used as recipient).
        /// </summary>
        public int ToSize
        {
            get { return _to.Count > 0 ? _to[0].Size : -1; }
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        /*
         * Constructor:
         */

        /// <summary>
        /// Builds an empty evidential mapping vector with just the element from which it will depend.
        /// </summary>
        /// <param name="e">The element from which the belief will be propagated.</param>
        public DiscreteMappingVector(DiscreteElement e)
        {
            _from = e;
            _to = new List<DiscreteMappingPoint>();
        }

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Methods

        /*
         * Methods:
         */

        /// <summary>
        /// Gives a (potentially non-valid) mass function corresponding of the propagation of the belief
        /// associated to the "from" element given the mass associated to it.
        /// </summary>
        /// <param name="mass">The mass associated to the element from which we propagate belief.</param>
        /// <returns>Returns a (potentially non-valid) mass function corresponding to the
        /// propagation of the belief associated to the "from" element given the mass associated to it.</returns>
        public DiscreteMassFunction GetEvidence(double mass)
        {
            DiscreteMassFunction toReturn = new DiscreteMassFunction();
            for (int i = 0; i < _to.Count; i++)
            {
                toReturn.AddMass(_to[i].Element, _to[i].GetMass(mass));
            }
            return toReturn;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Adds a new mapping point (a new focal element, an element of the matrix) to which belief will be propagated.
        /// </summary>
        /// <param name="bp">The point to add.</param>
        /// <exception cref="InvalidBeliefModelException">Thrown if the given point is already contained
        /// in this vector.</exception>
        public void AddPoint(DiscreteMappingPoint bp)
        {
            if (this.Contains(bp))
            {
                throw new InvalidBeliefModelException("An Element to transform belief to appears twice in the evidential mapping model!");
            }
            _to.Add(bp);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Adds a new mapping point (a new focal element, an element of the matrix) to which belief will be propagated.
        /// </summary>
        /// <param name="e">The element to add.</param>
        /// <param name="mappingFactor">The mapping factor between the element of origin and this focal element.</param>
        /// <exception cref="InvalidBeliefModelException">Thrown if the given point is already contained
        /// in this vector.</exception>
        public void AddPoint(DiscreteElement e, double mappingFactor)
        {
            this.AddPoint(new DiscreteMappingPoint(e, mappingFactor));
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Checks if the current vector contains the given point.
        /// </summary>
        /// <param name="bp">The point to test.</param>
        /// <returns>Returns true if the point is contained, false otherwise.</returns>
        public bool Contains(DiscreteMappingPoint bp)
        {
            return _to.Contains(bp);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// The factors of the mapping point of a vector should sum to 1 (it is some kind of mass function).
        /// The method thus checks if the mapping vector is valid.
        /// </summary>
        /// <returns>Returns true if mapping points factors sum to 1, false otherwise.</returns>
        public bool IsValid()
        {
            double sum = 0;
            foreach (DiscreteMappingPoint bp in _to)
            {
                if (0 > bp.MappingFactor || bp.MappingFactor > 1)
                {
                    return false;
                }
                sum += bp.MappingFactor;
            }
            return sum == 1;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Tests equality between the current mapping vector and the given object.
        /// </summary>
        /// <param name="obj">The object to compare the vector to.</param>
        /// <returns>Returns true if the given object is a mapping vector defined on the same element
        /// of origin.</returns>
        public override bool Equals(object obj)
        {
            //Checking:
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            //Equals:
            return ((DiscreteMappingVector)obj).From.Equals(this.From);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the hash code of the mapping vector.
        /// </summary>
        /// <returns>Returns the hash code of the mapping vector.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gives a string representation of the mapping vector.
        /// </summary>
        /// <returns>Returns a string representation of the mapping vector.</returns>
        public override string ToString()
        {
            StringBuilder toReturn = new StringBuilder(String.Format("DiscreteMappingVector: From = {0} to:\n", From));
            for (int i = 0; i < _to.Count; i++)
            {
                if (i == _to.Count)
                {
                    toReturn.Append(_to[i]);
                }
                else
                {
                    toReturn.Append(String.Format("{0}\n", _to[i]));
                }
            }
            return toReturn.ToString();
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gives a string representation of the mapping vector given a reference list for the element
        /// of origin and another reference list for the elements to which the belief will be propagated.
        /// </summary>
        /// <param name="refListFrom">The origin reference list.</param>
        /// <param name="refListTo">The recipient reference list.</param>
        /// <returns>Returns a string representation of the mapping vector.</returns>
        public string ToString(StringReferenceList refListFrom, StringReferenceList refListTo)
        {
            StringBuilder toReturn = new StringBuilder(String.Format("DiscreteMappingVector: From = {0} to:\n", From.ToString(refListFrom)));
            for (int i = 0; i < _to.Count; i++)
            {
                if (i == _to.Count)
                {
                    toReturn.Append(_to[i].ToString(refListTo));
                }
                else
                {
                    toReturn.Append(String.Format("{0}\n", _to[i].ToString(refListTo)));
                }
            }
            return toReturn.ToString();
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Operator overrides

        /// <summary>
        /// Overrides "==" operator to check equality between two mapping vectors.
        /// </summary>
        /// <param name="a">The first mapping vector to compare.</param>
        /// <param name="b">The second mapping vector to compare.</param>
        /// <returns>Returns true if both mapping vectors are equal, false otherwise.</returns>
        public static bool operator ==(DiscreteMappingVector a, DiscreteMappingVector b)
        {
            if ((object)a == null) return (object)b == null;
            if ((object)b == null) return (object)a == null;
            return !a.Equals(b);
        }

        /// <summary>
        /// Overrides "==" operator to check inequality between two mapping vectors.
        /// </summary>
        /// <param name="a">The first mapping vector to compare.</param>
        /// <param name="b">The second mapping vector to compare.</param>
        /// <returns>Returns true if both mapping vectors are not equal, false otherwise.</returns>
        public static bool operator !=(DiscreteMappingVector a, DiscreteMappingVector b)
        {
            if ((object)a == null) return (object)b != null;
            if ((object)b == null) return (object)a != null;
            return !a.Equals(b);
        }

        #endregion

    } //Struct
} //Namespace
