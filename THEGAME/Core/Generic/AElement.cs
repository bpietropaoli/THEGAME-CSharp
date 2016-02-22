using System;

namespace THEGAME.Core.Generic
{
    /// <summary>
    /// An abstract class to represent elements in the belief functions theory. Thus, they may not
    /// be atomic because they represent subsets of the possible states/worlds defined in the frame of
    /// discernment. They could also be intervals or whatever you want to define. This class serves 
    /// as an interface (it is not because there is also the overrides of basic operators that cannot
    /// be put into an interface).
    /// Those elements are used in <see cref="FocalElement{TElement}"/> that are used themselves 
    /// in <see cref="AMassFunction{TFunction, TElement}"/>.
    /// </summary>
    /// <typeparam name="TElement">The type of element which inherits from this class. By doing so,
    /// all the methods of the inheriting class takes itself as a parameter.</typeparam>
    public abstract class AElement<TElement> : IEquatable<TElement>
                                               where TElement : AElement<TElement>
    {
        /// <summary>
        /// Gets the cardinal of the element.
        /// </summary>
        public abstract double Card { get; }

        //------------------------------------------------------------------------------------------------

        #region Abstract methods

        /// <summary>
        /// Gets the opposite of the current element.
        /// </summary>
        /// <returns>Returns a new element which is the opposite of the current one.</returns>
        public abstract TElement Opposite();

        /// <summary>
        /// Gets the conjunction/intersection of the current element with the given one.
        /// Should be strictly identical to <see cref="Intersection"/>.
        /// </summary>
        /// <param name="e">The element to intersect with.</param>
        /// <returns>Returns a new element which is the conjunction/intersection of the current 
        /// element with the given one.</returns>
        public abstract TElement Conjunction(TElement e);

        /// <summary>
        /// Gets the conjunction/intersection of the current element with the given one.
        /// Should be strictly identical to <see cref="Conjunction"/>.
        /// </summary>
        /// <param name="e">The element to intersect with.</param>
        /// <returns>Returns a new element which is the conjunction/intersection of the current 
        /// element with the given one.</returns>
        public abstract TElement Intersection(TElement e);

        /// <summary>
        /// Gets the disjunction/union of the current element with the given one.
        /// Should be strictly identical to <see cref="Union"/>.
        /// </summary>
        /// <param name="e">The element to do the union with.</param>
        /// <returns>Returns a new element which is the disjunction/union of the current
        /// element with the given one.</returns>
        public abstract TElement Disjunction(TElement e);

        /// <summary>
        /// Gets the disjunction/union of the current element with the given one.
        /// Should be strictly identical to <see cref="Disjunction"/>.
        /// </summary>
        /// <param name="e">The element to do the union with.</param>
        /// <returns>Returns a new element which is the disjunction/union of the current
        /// element with the given one.</returns>
        public abstract TElement Union(TElement e);

        /// <summary>
        /// Gets the empty element compatible with the current element.
        /// </summary>
        /// <returns>Returns a new element which is empty and compatible
        /// with the current one.</returns>
        public abstract TElement GetEmptyElement();

        /// <summary>
        /// Gets an element compatible with the current one which is representing
        /// the complete set of possible states/worlds defined in the frame of
        /// discernment.
        /// </summary>
        /// <returns>Returns a new element representing the complete set which is
        /// compatible with the current element.</returns>
        public abstract TElement GetCompleteElement();

        /// <summary>
        /// Gets a deep copy of the current element.
        /// </summary>
        /// <returns>Returns a deep copy of the current element.</returns>
        public abstract TElement DeepCopy();

        /// <summary>
        /// Checks if the given element is a subset of the current one.
        /// </summary>
        /// <param name="e">The element to compare to.</param>
        /// <returns>Returns true if the current element is a subset of the given one,
        /// false otherwise.</returns>
        public abstract bool IsASubset(TElement e);

        /// <summary>
        /// Checks if the given element is a superset of the current one.
        /// </summary>
        /// <param name="e">The element to compare to.</param>
        /// <returns>Returns true if the current element is a superset of the given one,
        /// false otherwise.</returns>
        public abstract bool IsASuperset(TElement e);

        /// <summary>
        /// Checks if the given element is compatible with the current one (for
        /// the conjunction/disjunction, comparison, etc).
        /// </summary>
        /// <param name="e">The element to check the compatibility with.</param>
        /// <returns>Returns true if the given element is compatible with the current one,
        /// false otherwise.</returns>
        public abstract bool IsCompatible(TElement e);

        /// <summary>
        /// Checks equality between the current element and the given one.
        /// </summary>
        /// <param name="e">The element to compare to.</param>
        /// <returns>Returns true if the given element equals the current one, 
        /// false otherwise.</returns>
        public abstract bool Equals(TElement e);

        /// <summary>
        /// Checks if the given object equals the current element.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>Returns false if the given object is not a <see cref="AElement{TElement}"/>,
        /// returns <see cref="Equals(TElement)"/> otherwise.</returns>
        public abstract override bool Equals(object obj);

        /// <summary>
        /// Gets the hash code of the current element.
        /// </summary>
        /// <returns>Returns the has code of the current element.</returns>
        public abstract override int GetHashCode();

        /// <summary>
        /// Checks if the current element equals the empty set.
        /// </summary>
        /// <returns>Returns true if the current element equals the empty set,
        /// false otherwise.</returns>
        public abstract bool IsEmpty();

        /// <summary>
        /// Checks if the current element equals the complete set.
        /// </summary>
        /// <returns>Returns true if the current element is the complete set,
        /// false otherwise.</returns>
        public abstract bool IsComplete();

        #endregion

        //------------------------------------------------------------------------------------------------

        #region Operator overrides

        /// <summary>
        /// Overrides the operator "==" to test the equality between two elements.
        /// </summary>
        /// <param name="a">The first element to compare.</param>
        /// <param name="b">The second element to compare.</param>
        /// <returns>Returns true if both elements are equal, false otherwise.</returns>
        public static bool operator ==(AElement<TElement> a, AElement<TElement> b)
        {
            if ((object)a == null) return (object)b == null;
            if ((object)b == null) return ((object)a == null);
            return ((TElement)a).Equals((TElement)b);
        }

        /// <summary>
        /// Overrides the operator "!=" to test the inequality between two elements.
        /// </summary>
        /// <param name="a">The first element to compare.</param>
        /// <param name="b">The second element to compare.</param>
        /// <returns>Returns true if both elements are not equal, false otherwise.</returns>
        public static bool operator !=(AElement<TElement> a, AElement<TElement> b)
        {
            if ((object)a == null) return ((object)b != null);
            if ((object)b == null) return ((object)a != null);
            return !((TElement)a).Equals((TElement)b);
        }

        /// <summary>
        /// Overrides the AND operator to get the intersection/conjunction of two elements.
        /// </summary>
        /// <param name="a">The first element for the intersection.</param>
        /// <param name="b">The second element for the intersection.</param>
        /// <returns>Returns a new element which is the conjunction/intersection of the
        /// two given ones.</returns>
        public static AElement<TElement> operator &(AElement<TElement> a, AElement<TElement> b)
        {
            return (TElement)a.Conjunction((TElement)b);
        }

        /// <summary>
        /// Overrides the OR operator to get the union/disjunction of two elements.
        /// </summary>
        /// <param name="a">The first element for the union.</param>
        /// <param name="b">The second element for the union.</param>
        /// <returns>Returns a new element which is the union/disjunction of the 
        /// two given ones.</returns>
        public static AElement<TElement> operator |(AElement<TElement> a, AElement<TElement> b)
        {
            return (TElement)a.Disjunction((TElement)b);
        }

        #endregion

    } //Interface
} //Namespace
