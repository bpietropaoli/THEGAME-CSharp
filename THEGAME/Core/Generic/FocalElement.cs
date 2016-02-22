using System;

namespace THEGAME.Core.Generic
{
    /// <summary>
    /// A class for focal elements of mass functions. Simply aggregates an element with
    /// a value. Is used also to determine if FocalElements are compatible with one another.
    /// </summary>
    /// <typeparam name="TElement">The type of element to use for this focal element, it must ihnerit
    /// from AElement(TElement).</typeparam>
    public class FocalElement<TElement> : IEquatable<FocalElement<TElement>>
                                          where TElement : AElement<TElement>
    {
        /*
         * Properties
         */

        /// <summary>
        /// The element composing the focal element.
        /// </summary>
        public TElement Element { get; private set; }

        /// <summary>
        /// The mass associated to this focal element.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// The cardinal of the element.
        /// </summary>
        public double Card
        {
            get { return Element.Card; }
        }

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        /*
         * Constructors
         */

        /// <summary>
        /// Builds a FocalElement from an element and a mass value.
        /// </summary>
        /// <param name="e">The element to compose the focal element.</param>
        /// <param name="value">The mass value (by default 0).</param>
        public FocalElement(TElement e, double value = 0)
        {
            this.Element = e.DeepCopy();
            this.Value = value;
        }

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        /*
         * Methods
         */

        /// <summary>
        /// Checks if the given focal element is compatible with the current one. (This is used
        /// for combination of mass functions for instance as they may be defined on different
        /// frames of discernment.)
        /// </summary>
        /// <param name="f">The focal element to check compatibility with.</param>
        /// <returns>Returns true if the given focal element is compatible, false otherwise.</returns>
        public bool IsCompatible(FocalElement<TElement> f)
        {
            return this.Element.IsCompatible(f.Element);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Checks if the given element is compatible with the current focal element. (This is used
        /// for combination of mass functions for instance as they may be defined on different
        /// frames of discernment.)
        /// </summary>
        /// <param name="e">The element to check compatibility with.</param>
        /// <returns>Returns true if the given element is compatible, false otherwise.</returns>
        public bool IsCompatible(TElement e)
        {
            return this.Element.IsCompatible(e);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Checks equality between the current focal element and the given one.
        /// </summary>
        /// <param name="f">The focal element to compare to.</param>
        /// <returns>Returns true if both elements of the focal elements are equal, false otherwise.</returns>
        public bool Equals(FocalElement<TElement> f)
        {
            return this.Element.Equals(f.Element);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Checks equality between the current focal element and the given one.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>Returns false if obj is not a focal element, returns the result of the typed 
        /// <see cref="Equals(FocalElement{TElement})"/> otherwise.</returns>
        public override bool Equals(object obj)
        {
            //Checking:
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            //Comparing:
            return ((FocalElement<TElement>)obj).Element.Equals(this.Element);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Override of the hashcode method.
        /// </summary>
        /// <returns>Returns the hashcode of the focal element.</returns>
        public override int GetHashCode()
        {
            return this.Element.GetHashCode();
        }

    } //Class
} //Namespace


