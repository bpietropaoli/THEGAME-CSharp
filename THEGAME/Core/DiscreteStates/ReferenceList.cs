using System;
using System.Text;
using System.Collections.Generic;

using THEGAME.Exceptions;

namespace THEGAME.Core.DiscreteStates
{
    /// <summary>
    /// A class to store a list of references for the element (to convert them into things which make sense).
    /// The references correspond to the real states/worlds defined in the frame of discernment.
    /// As the discrete elements are represented with binary numbers, the first element of the reference list
    /// correspond to the first bit, the second reference to the second bit, and so on.
    /// Inherits from <see cref="List{T}"/> but keeps a unicity constrait over the stored references.
    /// </summary>
    /// <typeparam name="T">The type of object to associate the elements to.</typeparam>
    public class ReferenceList<T> : List<T>, IEquatable<ReferenceList<T>>
    {

        /*					
         * Properties
         */

        /// <summary>
        /// The array of references.
        /// </summary>
        public T[] References
        {
            get { return this.ToArray(); }
        }

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Constructors

        /*					
         * Constructors
         */
        /// <summary>
        /// Builds an empty reference list (an empty list...).
        /// </summary>
        public ReferenceList() : base() {}

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Builds an empty list then fills it with the given references.
        /// </summary>
        /// <param name="references">The references to add to the list.</param>
        public ReferenceList(params T[] references) : base()
        {
            foreach (T r in references)
            {
                this.Add(r);
            }
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Utility methods

        /*
         * Methods
         */
        /// <summary>
        /// Adds a new reference to the list if not already in.
        /// </summary>
        /// <param name="reference">The new reference to add.</param>
        /// <exception cref="IncompatibleReferenceListException">Thrown if the reference list already contains
        /// the given reference.</exception>
        public new void Add(T reference)
        {
            if (Contains(reference))
            {
                throw new IncompatibleReferenceListException(String.Format("This ReferenceList already contains \"{0}\"!", reference.ToString()));
            }
            base.Add(reference);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Tests equality between the current reference list and the given one.
        /// </summary>
        /// <param name="list">The list to compare to.</param>
        /// <returns>Returns true if both reference lists are equals, false otherwise.</returns>
        public bool Equals(ReferenceList<T> list)
        {
            foreach(T element in list)
            {
                if (!this.Contains(element))
                {
                    return false;
                }
            }
            foreach (T element in this)
            {
                if (!list.Contains(element))
                {
                    return false;
                }
            }
            return true;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Compares the current reference list with the given object.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>Returns true if the given object is an equal reference list, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as ReferenceList<T>);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the hash code for the current reference list.
        /// </summary>
        /// <returns>Returns the hash code of the current reference list.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gives a string representation of the current reference list.
        /// </summary>
        /// <returns>Returns a string representation of the current reference list.</returns>
        public override string ToString()
        {
            StringBuilder toReturn = new StringBuilder();
            toReturn.Append("ReferenceList: ");
            for (int i = 0; i < Count - 1; i++)
            {
                toReturn.Append(this[i].ToString());
                toReturn.Append(", ");
            }
            toReturn.Append(this[Count - 1]);
            return toReturn.ToString();
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Operator overrides

        /*
         * Overriding operators
         */

        /// <summary>
        /// Overrides the "==" operator to test for equality of two lists.
        /// </summary>
        /// <param name="a">The first list to compare.</param>
        /// <param name="b">The second list to compare.</param>
        /// <returns>Returns true if both lists are equal, false otherwise.</returns>
        public static bool operator ==(ReferenceList<T> a, ReferenceList<T> b)
        {
            if ((object)a == null) return (object)b == null;
            if ((object)b == null) return (object)a == null;
            return !a.Equals(b);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Overrides the "!=" operator to test for inequality of two lists.
        /// </summary>
        /// <param name="a">The first list to compare.</param>
        /// <param name="b">The second list to compare.</param>
        /// <returns>Returns true if both lists are not equal, false otherwise.</returns>
        public static bool operator !=(ReferenceList<T> a, ReferenceList<T> b)
        {
            if ((object)a == null) return (object)b != null;
            if ((object)b == null) return (object)a != null;
            return !a.Equals(b);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Overrides the "+" operator to add a new reference to the reference list.
        /// </summary>
        /// <param name="list">The list to add a reference to.</param>
        /// <param name="reference">The reference to add.</param>
        /// <returns>Returns the list with the reference added (WARNING: It modifies the given list!)</returns>
        /// <exception cref="IncompatibleReferenceListException">Thrown if the given reference is
        /// already contained.</exception>
        public static ReferenceList<T> operator +(ReferenceList<T> list, T reference)
        {
            list.Add(reference);
            return list;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Overrides the "-" operator to remove a reference from the reference list.
        /// </summary>
        /// <param name="list">The list to remove a reference from.</param>
        /// <param name="reference">The reference to remove.</param>
        /// <returns>Returns the list with the reference removed (WARNING: It modifies the given list!)</returns>
        public static ReferenceList<T> operator -(ReferenceList<T> list, T reference)
        {
            list.Remove(reference);
            return list;
        }

        #endregion

    } //Class
} //Namespace
