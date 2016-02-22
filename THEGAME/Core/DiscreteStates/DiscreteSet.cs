using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using THEGAME.Core.Generic;
using THEGAME.Exceptions;

namespace THEGAME.Core.DiscreteStates
{
    /// <summary>
    /// A set of discrete elements keeping the compatibility between all the elements as well as the unicity of
    /// the stored elements. Inherits from <see cref="Set{DiscreteElement}"/>.
    /// </summary>
    public sealed class DiscreteSet : Set<DiscreteElement>,
                                      IEquatable<DiscreteSet>
    {
        /*
         * Properties
         */

        /// <summary>
        /// The size of the stored elements (the number of possible states/worlds
        /// defined in the frame of discernment).
        /// </summary>
        public int ElementSize
        {
            get { return Card > 0 ? _elements[0].Size : -1; }
        }

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Constructors

        /*		
         * Constructors
         */

        /// <summary>
        /// Builds an empty set.
        /// </summary>
        public DiscreteSet() : base()
        {
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Builds an empty set then fills it with the given elements. It creates a deep copy of the given elements.
        /// </summary>
        /// <param name="elements">The elements to add to the set.</param>
        /// <exception cref="IncompatibleDiscreteElementSizeException">Thrown if the given elements are
        /// not all of the same size (which means they are not defined on the same frame of discernment.</exception>
        public DiscreteSet(params DiscreteElement[] elements)
            : this()
        {
            //Checking that all elements are of the same size:
            foreach (DiscreteElement element in elements)
            {
                if (!element.IsCompatible(elements[0]))
                {
                    throw new IncompatibleDiscreteElementSizeException("The DiscreteElements in a DiscreteSet should all have the same size!");
                }
            }
            //Constructing:
            foreach (DiscreteElement element in elements)
            {
                _elements.Add(element.DeepCopy());
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Builds a new discrete set which is a deep copy of the given one.
        /// </summary>
        /// <param name="s">The discrete set to copy.</param>
        public DiscreteSet(DiscreteSet s)
            : this(s.Elements.ToArray())
        {
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        /*		
         * Methods
         */

        #region Modifying methods

        /// <summary>
        /// Adds a new discrete element to the discrete set.
        /// </summary>
        /// <param name="e">The element to add to the set.</param>
        /// <exception cref="IncompatibleDiscreteElementSizeException">Thrown if the given element is incompatible
        /// with the elements already stored (here, if it seems to be defined on another frame of discernment).</exception>
        public new void Add(DiscreteElement e)
        {
            //Checking arguments:
            if (this.Elements.Count != 0 && !Elements[0].IsCompatible(e))
            {
                throw new IncompatibleDiscreteElementSizeException("The Elements in a DiscreteSet should all have the same size!");
            }
            //Adding:
            base.Add(e);
        }

        #endregion

        //------------------------------------------------------------------------------------------------

        #region Conjunction/Disjunction

        /// <summary>
        /// Intersects the current discrete set with the given one. Does not modify the current set, returns a new
        /// one instead. Strictly identical to <see cref="Intersection"/>.
        /// </summary>
        /// <param name="s">The discrete set to intersect the current one with.</param>
        /// <returns>Returns a new discrete set which is the intersection/conjunction of the current discrete set
        /// with the given one.</returns>
        /// <exception cref="IncompatibleDiscreteElementSizeException">Thrown if the two sets contain elements
        /// that are defined on different frames of discernment.</exception>
        public DiscreteSet Conjunction(DiscreteSet s)
        {
            try
            {
                return (DiscreteSet)(base.Conjunction(s));
            }
            catch (IncompatibleSetException)
            {
                throw new IncompatibleDiscreteElementSizeException("The Elements of two DiscreteSets should be of the same size to perform the conjunction!");
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Intersects the current discrete set with the given one. Does not modify the current set, returns a new
        /// one instead. Strictly identical to <see cref="Conjunction"/>.
        /// </summary>
        /// <param name="s">The discrete set to intersect the current one with.</param>
        /// <returns>Returns a new discrete set which is the intersection/conjunction of the current discrete set
        /// with the given one.</returns>
        /// <exception cref="IncompatibleDiscreteElementSizeException">Thrown if the two sets contain elements
        /// that are defined on different frames of discernment.</exception>
        public DiscreteSet Intersection(DiscreteSet s)
        {
            return this.Conjunction(s);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Creates the union of the current discrete set with the given one. Does not modify the current discrete 
        /// set, returns a new one instead. Strictly identical to <see cref="Union"/>.
        /// </summary>
        /// <param name="s">The discrete set to create the union with.</param>
        /// <returns>Returns a new discrete set which is the union/disjunction of the current discrete set with
        /// the given one.</returns>
        /// <exception cref="IncompatibleDiscreteElementSizeException">Thrown if the two sets contain elements
        /// that are defined on different frames of discernment.</exception>
        public DiscreteSet Disjunction(DiscreteSet s)
        {
            try
            {
                return (DiscreteSet)(base.Disjunction(s));
            }
            catch (IncompatibleSetException)
            {
                throw new IncompatibleDiscreteElementSizeException("The Elements of two DiscreteSets should be of the same size to perform the disjunction!");
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Creates the union of the current discrete set with the given one. Does not modify the current discrete 
        /// set, returns a new one instead. Strictly identical to <see cref="Disjunction"/>.
        /// </summary>
        /// <param name="s">The discrete set to create the union with.</param>
        /// <returns>Returns a new discrete set which is the union/disjunction of the current discrete set with
        /// the given one.</returns>
        /// <exception cref="IncompatibleDiscreteElementSizeException">Thrown if the two sets contain elements
        /// that are defined on different frames of discernment.</exception>
        public DiscreteSet Union(DiscreteSet s)
        {
            return this.Disjunction(s);
        }

        #endregion

        //------------------------------------------------------------------------------------------------

        #region Utility methods

        /// <summary>
        /// Gets the hash code for the current discrete set.
        /// </summary>
        /// <returns>Returns the hash code of the current set.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Compares the current discrete set with the given one.
        /// </summary>
        /// <param name="s">The set to compare to.</param>
        /// <returns>Returns true if both sets contain the exact same elements, false otherwise.</returns>
        public bool Equals(DiscreteSet s)
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
        /// <returns>Returns true if the given object is an equal discrete set, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as DiscreteSet);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gives a string representation of the current set using the given reference list to represent
        /// the stored elements.
        /// </summary>
        /// <param name="refList">The list of reference giving sense to the elements.</param>
        /// <returns>Returns a string representation of the current set.</returns>
        public string ToString(StringReferenceList refList)
        {
            return ToString<string>(refList);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gives a string representation of the current set using the given reference list to represent
        /// the stored elements.
        /// </summary>
        /// <param name="refList">The list of reference giving sense to the elements.</param>
        /// <returns>Returns a string representation of the current set.</returns>
        public string ToString<T>(ReferenceList<T> refList)
        {
            //Checking argument:
            if (refList.Count != ElementSize && ElementSize != -1)
            {
                throw new IncompatibleReferenceListException("The given ReferenceList does not correspond to the given DiscreteSet!");
            }
            //ToString:
            StringBuilder toReturn = new StringBuilder("{");
            for (int i = 0; i < Card - 1; i++)
            {
                toReturn.Append(this[i].ToString(refList));
                toReturn.Append(", ");
            }
            if (Card > 0)
            {
                toReturn.Append(Elements[Card - 1].ToString(refList));
            }
            toReturn.Append("}");
            return toReturn.ToString();
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        /*				
         * Static methods
         */

        #region Static utility methods

        /// <summary>
        /// Static method to generate the set of atoms given a certain number of atoms (the number of
        /// possible states/worlds defined in the frame of discernment). The generated DiscreteElements
        /// will have a size of the number of atoms required.
        /// </summary>
        /// <param name="numberOfAtoms">The number of atoms to generate.</param>
        /// <returns>Returns a new discrete set containing all the atoms for elements of the size
        /// "numberOfAtoms".</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the number of atoms required is 
        /// null of negative.</exception>
        public static DiscreteSet GenerateSetOfAtoms(int numberOfAtoms)
        {
            //Checking argument:
            if (numberOfAtoms <= 0)
            {
                throw new ArgumentOutOfRangeException("numberOfAtoms. The number of atoms cannot be null or negative!");
            }
            //Init:
            DiscreteSet s = new DiscreteSet();
            uint number = 0;
            uint[] numbers = new uint[numberOfAtoms / DiscreteElement.NB_BITS_UINT + 1];
            for (int i = 0; i < numbers.Length; i++)
            {
                numbers[i] = 0;
            }
            //Generate Elements:
            for (int i = 0; i < numberOfAtoms; i++)
            {
                number = (uint)Math.Pow(2, i % DiscreteElement.NB_BITS_UINT);
                numbers[i / DiscreteElement.NB_BITS_UINT] = number;
                s.Add(new DiscreteElement(numberOfAtoms, numbers));
                numbers[i / DiscreteElement.NB_BITS_UINT] = 0;
            }
            return s;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// <para>Static method to generate the power set (the set of all possible subsets) for given number of
        /// possible states/worlds defined in the frame of discernment.</para>
        /// <para>WARNING: DO NOT CALL THIS METHOD WITH A NUMBER OF ATOMS TOO BIG OR IT WILL HARM YOUR POOR
        /// COMPUTER.</para>
        /// </summary>
        /// <param name="numberOfAtoms">The number of possible states/worlds in the frame of
        /// discernment.</param>
        /// <returns>Returns a new discrete set containing all the possible discrete elements of the
        /// size "numberOfAtoms". This corresponds to the power set.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the number of atoms required is
        /// null or negative.</exception>
        public static DiscreteSet GeneratePowerSet(int numberOfAtoms)
        {
            //Checking argument:
            if (numberOfAtoms <= 0)
            {
                throw new ArgumentOutOfRangeException("numberOfAtoms. The number of atoms cannot be null or negative!");
            }
            //Init:
            DiscreteSet s = new DiscreteSet();
            uint[] numbers = new uint[numberOfAtoms / DiscreteElement.NB_BITS_UINT + 1];
            uint limit = (uint)Math.Pow(2, numberOfAtoms % DiscreteElement.NB_BITS_UINT) - 1;
            for (int i = 0; i < numbers.Length; i++)
            {
                numbers[0] = 0;
            }
            s.Add(new DiscreteElement(numberOfAtoms, numbers));
            //Generate:
            while (numbers[numbers.Length - 1] < limit){
                int i = 0;
                while (numbers[i] == uint.MaxValue)
                {
                    numbers[i] = 0;
                    i++;
                }
                numbers[i]++;
                s.Add(new DiscreteElement(numberOfAtoms, numbers));
            }
            return s;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// <para>Static method to generate a partial power set containing all the possible discrete elements
        /// of a given size (nbOfAtoms) with a specified maximal cardinal.</para>
        /// <para>WARNING: DO NOT CALL THIS METHOD WITH A NUMBER OF ATOMS TOO BIG OR IT WILL HARM YOUR POOR
        /// COMPUTER.</para>
        /// </summary>
        /// <param name="numberOfAtoms">The number of possible states/worlds defined in the frame
        /// of discernment.</param>
        /// <param name="maxCard">The maximal cardinal for the created elements.</param>
        /// <returns>Returns a new set </returns>
        public static DiscreteSet GeneratePartialPowerSet(int numberOfAtoms, int maxCard)
        {
            DiscreteSet s = new DiscreteSet();
            DiscreteElementEnumerator enumerator = new DiscreteElementEnumerator(numberOfAtoms);
            foreach (DiscreteElement e in enumerator)
            {
                if (e.Card <= maxCard)
                    s.Add(e);
            }
            return s;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// <para>Static method to generate the set of all subsets of the given discrete element.</para>
        /// <para>WARNING: DO NOT CALL THIS METHOD WITH AN ELEMENT TOO BIG (A HIGH NUMBER OF POSSIBLE
        /// STATES/WORLD) OR IT WILL HARM YOUR POOR COMPUTER.</para>
        /// </summary>
        /// <param name="e">The element to generate the set from.</param>
        /// <returns>Returns a new discrete set containing all the subsets of the given element.</returns>
        public static DiscreteSet GenerateSetOfSubsets(DiscreteElement e)
        {
            DiscreteSet toReturn = new DiscreteSet();
            DiscreteElementEnumerator enumerator = new DiscreteElementEnumerator(e.Size);

            foreach (DiscreteElement el in enumerator)
            {
                if (el.IsASubset(e))
                {
                    toReturn.Add(el);
                }
            }
            return toReturn;
        }

        #endregion

        //------------------------------------------------------------------------------------------------

        #region Operator overrides

        /// <summary>
        /// Overrides the "==" operator which simply tests equality between both discrete sets.
        /// </summary>
        /// <param name="a">The first discrete set to compare.</param>
        /// <param name="b">The second discrete set to compare.</param>
        /// <returns>Returns true if both discrete sets are equal, false otherwise.</returns>
        public static bool operator ==(DiscreteSet a, DiscreteSet b)
        {
            if ((object)a == null) return (object)b == null;
            if ((object)b == null) return (object)a == null;
            return !a.Equals(b);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Overrides the "!=" operator which simply test inequality between both discrete sets.
        /// </summary>
        /// <param name="a">The first discrete set to compare.</param>
        /// <param name="b">The second discrete set to compare.</param>
        /// <returns>Returns true if both discrete sets are not equal, false otherwise.</returns>
        public static bool operator !=(DiscreteSet a, DiscreteSet b)
        {
            if ((object)a == null) return (object)b != null;
            if ((object)b == null) return (object)a != null;
            return !a.Equals(b);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Overrides the "+" operator to add a new element to the given discrete set.
        /// </summary>
        /// <param name="set">The discrete set to add an element to.</param>
        /// <param name="e">The discrete element to add to the set.</param>
        /// <returns>Returns the set with the element added (WARNING: It modifies the given set!)</returns>
        /// <exception cref="IncompatibleElementException">Thrown if the given element is not compatible
        /// with the elements already stored.</exception>
        public static DiscreteSet operator +(DiscreteSet set, DiscreteElement e)
        {
            set.Add(e);
            return set;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Overrides the "-" operator to remove a new element from the given discrete set.
        /// </summary>
        /// <param name="set">The discrete set to remove an element from.</param>
        /// <param name="e">The discrete element to add to the set.</param>
        /// <returns>Returns the set with the element removed (WARNING: It modifies the given set!)</returns>
        public static DiscreteSet operator -(DiscreteSet set, DiscreteElement e)
        {
            set.Remove(e);
            return set;
        }

        #endregion

    } //Class
} //Namespace
