using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using THEGAME.Exceptions;

namespace THEGAME.Core.Generic
{
    /// <summary>
    /// A set of elements keeping the compatibility between all the elements as well as the unicity of
    /// the stored elements.
    /// </summary>
    /// <typeparam name="TElement">The type of element to store, must inherit <see cref="AElement{TElement}"/>.</typeparam>
    public class Set<TElement> : IEnumerable<TElement>, IEquatable<Set<TElement>>
                                 where TElement : AElement<TElement>
    {
        /*
		 * Members
		 */

        /// <summary>
        /// The list of elements to store in the set.
        /// </summary>
        protected List<TElement> _elements;


        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        /*		
         * Properties
         */
        /// <summary>
        /// Gets the list of elements stored in the set.
        /// </summary>
        public List<TElement> Elements
        {
            get { return _elements; }
        }

        /// <summary>
        /// Gets the i-th element of the set (cannot be used to modify
        /// elements, use add/remove if you want to do so, it will keep
        /// the consistency of the set).
        /// </summary>
        /// <param name="i">The index of the wanted element.</param>
        /// <returns>Returns the i-th element of the set.</returns>
        public TElement this[int i]
        {
            get { return _elements[i]; }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the cardinal of the set (the number of stored elements).
        /// </summary>
        public int Card
        {
            get { return _elements.Count; }
        }


        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Constructors

        /*		
         * Constructors
         */

        /// <summary>
        /// Creates a new empty set.
        /// </summary>
        public Set()
        {
            _elements = new List<TElement>();
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Creates an empty set then fills it with the given elements. Creates a deep copy
        /// of the given elements.
        /// </summary>
        /// <param name="elements">The elements to store in the set. 
        /// This creates a deep copy of the elements.</param>
        /// <exception cref="IncompatibleElementException">Thrown if the given elements are not
        /// compatible with each others.</exception>
        public Set(params TElement[] elements)
            : this()
        {
            //Checking that all elements are of the same size:
            foreach (TElement element in elements)
            {
                if (element.IsCompatible(elements[0]))
                {
                    throw new IncompatibleElementException("The Elements in a Set should all be compatible with each others!");
                }
            }
            //Constructing:
            foreach (TElement element in elements)
            {
                _elements.Add(element.DeepCopy());
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Creates a new set that is a deep copy of the given one.
        /// </summary>
        /// <param name="s">The set to copy.</param>
        public Set(Set<TElement> s)
            : this(s.Elements.ToArray())
        {}

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        /*		
         * Methods
         */

        #region Modifying methods

        /// <summary>
        /// Adds the given element to the set if it is not already contained.
        /// </summary>
        /// <param name="e">The element to add to the set.</param>
        /// <exception cref="IncompatibleElementException">Thrown if the given element is not compatible
        /// with the elements already stored.</exception>
        public void Add(TElement e)
        {
            //Checking arguments:
            if (Elements.Count != 0 && !Elements[0].IsCompatible(e))
            {
                throw new IncompatibleElementException("The Elements in a Set should all be compatible with each others!");
            }
            //Adding:
            if (!_elements.Contains(e))
            {
                _elements.Add(e);
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Removes an element from the set.
        /// </summary>
        /// <param name="e">The element to remove.</param>
        /// <returns>Returns true if the element was removed, false otherwise (not removed or not found).</returns>
        public bool Remove(TElement e)
        {
            return _elements.Remove(e);
        }

        #endregion

        //------------------------------------------------------------------------------------------------

        #region Conjunction/Disjunction

        /// <summary>
        /// Intersects the current set with the given one. Does not modify the current set, returns a new
        /// one instead. Strictly identical to <see cref="Intersection"/>.
        /// </summary>
        /// <param name="s">The set to intersect the current one with.</param>
        /// <returns>Returns a new set which is the intersection/conjunction of the current set
        /// with the given one.</returns>
        /// <exception cref="IncompatibleSetException">Thrown if the sets' elements are not compatible
        /// with each others.</exception>
        public Set<TElement> Conjunction(Set<TElement> s)
        {
            //Checking:
            if (s.Elements.Count == 0 || this.Elements.Count == 0)
            {
                return new Set<TElement>();
            }
            if (!s.Elements[0].IsCompatible(this.Elements[0]))
            {
                throw new IncompatibleSetException("The given Set should have compatible Elements to do the conjunction!");
            }
            //Conjunction:
            Set<TElement> result = new Set<TElement>();
            foreach (TElement e in this.Elements)
            {
                if (s.Contains(e))
                {
                    result.Add(e);
                }
            }
            return result;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Intersects the current set with the given one. Does not modify the current set, returns a new
        /// one instead. Strictly identical to <see cref="Conjunction"/>.
        /// </summary>
        /// <param name="s">The set to intersect the current one with.</param>
        /// <returns>Returns a new set which is the intersection/conjunction of the current set
        /// with the given one.</returns>
        /// <exception cref="IncompatibleSetException">Thrown if the sets' elements are not compatible
        /// with each others.</exception>
        public Set<TElement> Intersection(Set<TElement> s)
        {
            return this.Conjunction(s);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Creates the union of the current set with the given one. Does not modify the current set, returns
        /// a new one instead. Strictly identical to <see cref="Union"/>.
        /// </summary>
        /// <param name="s">The set to create the union with.</param>
        /// <returns>Returns a new set which is the union/disjunction of the current set with
        /// the given one.</returns>
        /// <exception cref="IncompatibleSetException">Thrown if the sets' elements are not compatible
        /// with each others.</exception>
        public Set<TElement> Disjunction(Set<TElement> s)
        {
            //Checking:
            if (this.Elements.Count == 0)
            {
                return new Set<TElement>(s);
            }
            if (s.Elements.Count == 0)
            {
                return new Set<TElement>(this);
            }
            if (!s.Elements[0].IsCompatible(this.Elements[0]))
            {
                throw new IncompatibleSetException("The given Set should have compatible Elements to do the disjunction!");
            }

            //Disjunction:
            Set<TElement> result = new Set<TElement>();
            foreach (TElement e in Elements)
            {
                result.Add(e);
            }

            foreach (TElement e in s.Elements)
            {
                result.Add(e);
            }

            return result;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Creates the union of the current set with the given one. Does not modify the current set, returns
        /// a new one instead. Strictly identical to <see cref="Disjunction"/>.
        /// </summary>
        /// <param name="s">The set to create the union with.</param>
        /// <returns>Returns a new set which is the union/disjunction of the current set with
        /// the given one.</returns>
        /// <exception cref="IncompatibleSetException">Thrown if the sets' elements are not compatible
        /// with each others.</exception>
        public Set<TElement> Union(Set<TElement> s)
        {
            return this.Disjunction(s);
        }

        #endregion

        //------------------------------------------------------------------------------------------------

        #region Boolean methods

        /// <summary>
        /// Checks if the set contains the given element.
        /// </summary>
        /// <param name="e">The element to look for.</param>
        /// <returns>Returns true if the given element is contained, false otherwise.</returns>
        public bool Contains(TElement e)
        {
            return _elements.Contains(e);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Checks if the current set is a subset of the given one.
        /// </summary>
        /// <param name="s">The set to compare to.</param>
        /// <returns>Returns true if the current set is a subset of the given one, false otherwise.</returns>
        public bool IsASubset(Set<TElement> s)
        {
            //Check:
            if (!this.IsCompatible(s))
            {
                return false;
            }

            //Compare:
            foreach (TElement e in this.Elements)
            {
                if (!s.Contains(e))
                {
                    return false;
                }
            }
            return true;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Checks if the current set is a superset of the given one.
        /// </summary>
        /// <param name="s">The set to compare to.</param>
        /// <returns>Returns true if the current set is a superset of the given one, false otherwise.</returns>
        public bool IsASuperset(Set<TElement> s)
        {
            return s.IsASubset(this);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Checks the compatibility of the elements of the current set with the elements of the given one.
        /// </summary>
        /// <param name="s">The set to compare to.</param>
        /// <returns>Returns true if the sets are compatible, false otherwise.</returns>
        public bool IsCompatible(Set<TElement> s)
        {
            if (this.Elements.Count == 0)
            {
                return true;
            }
            else if (s.Elements.Count == 0)
            {
                return true;
            }
            return s.Elements[0].IsCompatible(this.Elements[0]);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Checks if the given element is compatible with the ones already stored.
        /// </summary>
        /// <param name="e">The element to check for.</param>
        /// <returns>Returns true if the given element is compatible, false otherwise.</returns>
        public bool IsCompatible(TElement e)
        {
            if (this.Elements.Count == 0)
            {
                return true;
            }
            return this.Elements[0].IsCompatible(e);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Compares the current set with the given one.
        /// </summary>
        /// <param name="s">The set to compare to.</param>
        /// <returns>Returns true if both sets contain the exact same elements, false otherwise.</returns>
        public bool Equals(Set<TElement> s)
        {
            //Checking:
            if (s == null)
            {
                return false;
            }
            if (Card != s.Card || !this.IsCompatible(s))
            {
                return false;
            }

            //Compare:
            return this.IsASubset(s) && s.IsASubset(this);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Compares the current set with the given object.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>Returns true if the given object is an equal set, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as Set<TElement>);
        }

        #endregion

        //------------------------------------------------------------------------------------------------

        #region Utilities

        /// <summary>
        /// Gives a deep copy of the current set.
        /// </summary>
        /// <returns>Returns a new set which is a deep copy of the current one.</returns>
        public Set<TElement> DeepCopy()
        {
            return new Set<TElement>(this);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the hash code for the current set.
        /// </summary>
        /// <returns>Returns the hash code of the current set.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets a string representation of the set of the form "Set: {e1, e2, ..., eN}".
        /// </summary>
        /// <returns>Returns a string representing the set.</returns>
        public override string ToString()
        {
            StringBuilder toReturn = new StringBuilder("Set: {");
            for (int i = 0; i < Card - 1; i++)
            {
                toReturn.Append(Elements[i].ToString());
                toReturn.Append(", ");
            }
            if (Card > 0)
            {
                toReturn.Append(Elements[Card - 1].ToString());
            }
            toReturn.Append("}");
            return toReturn.ToString();
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets a string representation of the set of the form "Set:\n{e1\ne2\n...\neN}".
        /// </summary>
        /// <returns>Returns a string representing the set.</returns>
        public string ToStringOnePerLine()
        {
            StringBuilder toReturn = new StringBuilder("Set:\n{");
            for (int i = 0; i < Card - 1; i++)
            {
                toReturn.Append(Elements[i].ToString());
                toReturn.Append("\n");
            }
            if (Card > 0)
            {
                toReturn.Append(Elements[Card - 1].ToString());
            }
            toReturn.Append("}");
            return toReturn.ToString();
        }

        //------------------------------------------------------------------------------------------------

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Utility function used to iterate with foreach loops over the set.
        /// </summary>
        /// <returns>Returns the next element of the foreach loop.</returns>
        public IEnumerator<TElement> GetEnumerator()
        {
            foreach (TElement e in _elements)
            {
                yield return e;
            }
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Operator overrides

        /// <summary>
        /// Overrides the "==" operator which simply tests equality between both sets.
        /// </summary>
        /// <param name="a">The first set to compare.</param>
        /// <param name="b">The second set to compare.</param>
        /// <returns>Returns true if both sets are equal, false otherwise.</returns>
        public static bool operator ==(Set<TElement> a, Set<TElement> b)
        {
            if ((object)a == null) return (object)b == null;
            if ((object)b == null) return (object)a == null;
            return a.Equals(b);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Overrides the "!=" operator which simply test inequality between both sets.
        /// </summary>
        /// <param name="a">The first set to compare.</param>
        /// <param name="b">The second set to compare.</param>
        /// <returns>Returns true if both sets are not equal, false otherwise.</returns>
        public static bool operator !=(Set<TElement> a, Set<TElement> b)
        {
            if ((object)a == null) return (object)b != null;
            if ((object)b == null) return (object)a != null;
            return !a.Equals(b);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Overrides the "+" operator to add a new element to the given set.
        /// </summary>
        /// <param name="set">The set to add an element to.</param>
        /// <param name="e">The element to add to the set.</param>
        /// <returns>Returns the set with the element added (WARNING: It modifies the given set!)</returns>
        /// <exception cref="IncompatibleElementException">Thrown if the given element is not compatible
        /// with the elements already stored.</exception>
        public static Set<TElement> operator +(Set<TElement> set, TElement e)
        {
            set.Add(e);
            return set;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Overrides the "-" operator to remove a new element from the given set.
        /// </summary>
        /// <param name="set">The set to remove an element from.</param>
        /// <param name="e">The element to add to the set.</param>
        /// <returns>Returns the set with the element removed (WARNING: It modifies the given set!)</returns>
        public static Set<TElement> operator -(Set<TElement> set, TElement e)
        {
            set.Remove(e);
            return set;
        }

        #endregion

    } //Class
} //Namespace



