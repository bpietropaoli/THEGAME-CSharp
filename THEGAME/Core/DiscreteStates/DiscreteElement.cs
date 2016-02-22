using System;
using System.Text;

using THEGAME.Core.Generic;
using THEGAME.Exceptions;

namespace THEGAME.Core.DiscreteStates
{
    /// <summary>
    /// A class to represent discrete elements in the classic theory of belief functions 
    /// with a finite frame of discernment. This class is optimised for the common operations
    /// on the elements within the belief functions (typically disjunction and conjunction).
    /// </summary>
    public sealed class DiscreteElement : AElement<DiscreteElement>
    {

        /// <summary>
        /// The number of bits in a uint used to encode the elements.
        /// </summary>
        public const int NB_BITS_UINT = sizeof(uint) * 8;

        /*
         * Members
         */
        private int _card;         //The cardinal of the element
        private int _size;         //The number of bits to represent the element
        private uint[] _numbers;   //The bits themselves


        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Properties

        /*		
         * Properties
         */

        /// <summary>
        /// Gets the cardinal of the discrete element. This may take time to compute the first time
        /// as it is not computed at the construction of the element if not already known in
        /// obvious case. Anyway, it should be computed only once then it is stored.
        /// </summary>
        public override double Card
        {
            get
            {
                //If stored:
                if (_card != -1)
                {
                    return _card;
                }
                //if not, compute it:
                int card = 0;
                for (int i = 0; i < _numbers.Length; i++)
                {
                    //Get the card:
                    uint n = _numbers[i];
                    while (n != 0)
                    {
                        if ((n & 1) == 1)
                        {
                            card++;
                        }
                        n >>= 1;
                    }
                }
                _card = card;
                return card;
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets an array of booleans representing the discrete element (a set of possible states/worlds).
        /// </summary>
        public bool[] Worlds
        {
            get
            {
                bool[] toReturn = new bool[_size];
                for (int i = 0; i < _numbers.Length; i++)
                {
                    uint world = _numbers[i];
                    if (i == _numbers.Length - 1)
                    {
                        for (int j = 0; j < _size % NB_BITS_UINT; j++)
                        {
                            toReturn[j + i * NB_BITS_UINT] = (world & 1) == 1 ? true : false;
                            world >>= 1;
                        }
                    }
                    else
                    {
                        for (int j = 0; j < NB_BITS_UINT; j++)
                        {
                            toReturn[j + i * NB_BITS_UINT] = (world & 1) == 1 ? true : false;
                            world >>= 1;
                        }
                    }
                }
                return toReturn;
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the number of bits on which the discrete element is encoded.
        /// </summary>
        public int Size
        {
            get { return _size; }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the number representing the discrete element (the binary number in an intelligible format).
        /// Only suitable if <see cref="Size"/> &lt; <see cref="NB_BITS_UINT"/>. If the element is
        /// encoded on more bits, you should call <see cref="Numbers"/>.
        /// </summary>
        /// <exception cref="DiscreteElementTooBigForAnIntegerException">Thrown when called on an
        /// element encoded on too many bits to be represented by a unique integer. In that case,
        /// you should call <see cref="Numbers"/></exception>
        public uint Number
        {
            get
            {
                if (_numbers.Length > 1)
                {
                    throw new DiscreteElementTooBigForAnIntegerException("The DiscreteElement is too big to be represented on a single uint. You should use the property \"Numbers\" instead!");
                }
                return _numbers[0];
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the numbers representing the discrete element (an array of numbers corresponding to the bits).
        /// </summary>
        public uint[] Numbers
        {
            get { return _numbers; }
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Constructors

        /*		
         * Constructors
         */

        /// <summary>
        /// Builds a discrete element with the given size and the given value which will be converted into a
        /// binary form to know the present states/worlds. If size &gt; <see cref="NB_BITS_UINT"/>,
        /// the given number will be used only as the first number to encode the element.
        /// </summary>
        /// <param name="size">The number of bits to encode the element.</param>
        /// <param name="number">The value to use for the first <see cref="NB_BITS_UINT"/> bits.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the given size if lower or equal 
        /// to 1 OR if the given number is out of the range of the given size.</exception>
        public DiscreteElement(int size, uint number = 0)
        {
            //Checking arguments:
            if (size <= 1)
            {
                throw new ArgumentOutOfRangeException("size. The size of an DiscreteElement cannot be null, negative or too small!");
            }
            if (size < NB_BITS_UINT && number > (1U << size) - 1)
            {
                throw new ArgumentOutOfRangeException("number. The specified DiscreteElement does not exist within the specified size!");
            }

            //Constructing:
            this._card = -1;
            if (number == 0)
            {
                this._card = 0;
            }
            this._size = size;
            this._numbers = new uint[size / NB_BITS_UINT + 1];
            this._numbers[0] = number;
            for (int i = 1; i < _numbers.Length; i++)
            {
                _numbers[i] = 0;
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Builds a discrete element with the given size and the given values which will be converted into a
        /// binary form to know the present states/worlds. The given numbers will be used to encode the first
        /// numbers.Length * NB_BITS_UINT bits.
        /// </summary>
        /// <param name="size">The number of bits to encode the element.</param>
        /// <param name="numbers">The value to use for the first numbers.Length * <see cref="NB_BITS_UINT"/> bits.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the given size if lower or equal 
        /// to 1 OR if at least one of the given numbers is out of the range of the given size.</exception>
        public DiscreteElement(int size, params uint[] numbers)
        {
            //Checking arguments:
            if (size <= 1)
            {
                throw new ArgumentOutOfRangeException("size. The size of an DiscreteElement cannot be null, negative or too small!");
            }
            if (numbers.Length > size / NB_BITS_UINT + 1 ||
                (numbers.Length == size / NB_BITS_UINT + 1 &&
                    numbers[numbers.Length - 1] >  (1U << (size % NB_BITS_UINT)) - 1))
            {
                throw new ArgumentOutOfRangeException("numbers. The specified DiscreteElement does not exist within the specified size!");
            }

            //Constructing:
            this._card = -1;
            this._size = size;
            this._numbers = new uint[size / NB_BITS_UINT + 1];
            for (int i = 0; i < _numbers.Length; i++)
            {
                if (i < numbers.Length)
                {
                    _numbers[i] = numbers[i];
                }
                else
                {
                    _numbers[i] = 0;
                }
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Buils a discrete element as a deep copy of the given element.
        /// </summary>
        /// <param name="e">The element to copy.</param>
        public DiscreteElement(DiscreteElement e)
            : this(e.Size, e.Numbers)
        {
            this._card = e._card;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Builds a discrete element from a string reference list and an array of strings corresponding to
        /// the states/worlds to include.
        /// </summary>
        /// <param name="refList">The reference list.</param>
        /// <param name="worlds">The states/worlds of the frame of discernment to include.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown is the reference list is too short to
        /// create a valid element</exception>
        /// <exception cref="IncompatibleReferenceListException">Thrown if one of the given states/worlds
        /// could not be found in the reference list.</exception>
        public DiscreteElement(StringReferenceList refList, params string[] worlds)
        {
            //Checking arguments:
            if (refList.Count <= 1)
            {
                throw new ArgumentOutOfRangeException("refList.Length. The size of an DiscreteElement cannot be null, negative or too small!");
            }
            foreach (string world in worlds)
            {
                if (!refList.Contains(world))
                {
                    throw new IncompatibleReferenceListException(String.Format("The given ReferenceList does not contain the world \"{0}\"!", world));
                }
            }

            //Constructing:
            _size = refList.Count;
            _card = worlds.Length;
            _numbers = new uint[_size / NB_BITS_UINT + 1];
            foreach (string w in worlds)
            {
                int index = refList.IndexOf(w);
                _numbers[index / NB_BITS_UINT] += 1U << (index % NB_BITS_UINT);
            }

        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Builds a discrete element from a reference list and an array of objects corresponding to
        /// the states/worlds to include.
        /// </summary>
        /// <param name="refList">The reference list.</param>
        /// <param name="worlds">The states/worlds of the frame of discernment to include.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown is the reference list is too short to
        /// create a valid element</exception>
        /// <exception cref="IncompatibleReferenceListException">Thrown if one of the given states/worlds
        /// could not be found in the reference list.</exception>
        public static DiscreteElement ConstructDiscreteElement<T>(ReferenceList<T> refList, params T[] worlds)
        {
            //Checking arguments:
            if (refList.Count <= 1)
            {
                throw new ArgumentOutOfRangeException("refList.Length. The size of an DiscreteElement cannot be null, negative or too small!");
            }
            foreach (T world in worlds)
            {
                if (!refList.Contains(world))
                {
                    throw new IncompatibleReferenceListException(String.Format("The given ReferenceList does not contain the world \"{0}\"!", world.ToString()));
                }
            }

            //Constructing:
            int size = refList.Count;
            int card = worlds.Length;
            uint[] numbers = new uint[size / NB_BITS_UINT + 1];
            foreach (T w in worlds)
            {
                int index = refList.IndexOf(w);
                numbers[index / NB_BITS_UINT] += 1U << (index % NB_BITS_UINT);
            }
            DiscreteElement newlyBuilt = new DiscreteElement(size, numbers);
            newlyBuilt._card = card;
            return newlyBuilt;
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        /*		
         * Methods
         */

        #region Element operations

        /// <summary>
        /// Gets the opposite of the current discrete element. Creates a new element and does not modify the
        /// current one.
        /// </summary>
        /// <returns>Returns a new discrete element which is the opposite of the current one.</returns>
        public override DiscreteElement Opposite()
        {
            uint[] oppositeNumbers = new uint[Numbers.Length];
            for (int i = 0; i < Numbers.Length; i++)
            {
                if (i == Numbers.Length - 1)
                {
                    oppositeNumbers[i] = (1U << (Size % NB_BITS_UINT)) - _numbers[i] - 1;
                }
                else
                {
                    oppositeNumbers[i] = ~_numbers[i];
                }
            }

            DiscreteElement opposite = new DiscreteElement(Size, oppositeNumbers);
            if (Card != -1)
            {
                opposite._card = Size - (int)Card;
            }
            return opposite;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the conjunction/intersection of the current discrete element with the given one. Does not modify
        /// the current element, creates a new one instead. Strictly identical to <see cref="Intersection"/>.
        /// </summary>
        /// <param name="e">The discrete element to intersect with.</param>
        /// <returns>Returns a new discrete element which is the conjunction/intersection of the current element
        /// with the given one.</returns>
        /// <exception cref="IncompatibleDiscreteElementSizeException">Thrown if the given discrete element has not
        /// the same size as the the current one (they are not defined on the same frame of discernment).</exception>
        public override DiscreteElement Conjunction(DiscreteElement e)
        {
            //Checking arguments:
            if (this.Size != e.Size)
            {
                throw new IncompatibleDiscreteElementSizeException("The DiscreteElements must have the same size to process the conjunction!");
            }
            //Intersecting:
            uint[] numbers = new uint[this._numbers.Length];
            for (int i = 0; i < Numbers.Length; i++)
            {
                numbers[i] = this.Numbers[i] & e.Numbers[i];
            }
            return new DiscreteElement(Size, numbers);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the conjunction/intersection of the current discrete element with the given one. Does not modify
        /// the current element, creates a new one instead. Strictly identical to <see cref="Conjunction"/>.
        /// </summary>
        /// <param name="e">The discrete element to intersect with.</param>
        /// <returns>Returns a new element which is the conjunction/intersection of the current discrete element
        /// with the given one.</returns>
        /// <exception cref="IncompatibleDiscreteElementSizeException">Thrown if the given discrete element has not
        /// the same size as the the current one (they are not defined on the same frame of discernment).</exception>
        public override DiscreteElement Intersection(DiscreteElement e)
        {
            return this.Conjunction(e);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the disjunction/union of the current discrete element with the given one. Does not modify
        /// the current element, creates a new one instead. Strictly identical to <see cref="Union"/>.
        /// </summary>
        /// <param name="e">The discrete element to get the union with.</param>
        /// <returns>Returns a new element which is the disjunction/union of the current discrete element
        /// with the given one.</returns>
        /// <exception cref="IncompatibleDiscreteElementSizeException">Thrown if the given discrete element has not
        /// the same size as the the current one (they are not defined on the same frame of discernment).</exception>
        public override DiscreteElement Disjunction(DiscreteElement e)
        {
            //Checking arguments:
            if (this.Size != e.Size)
            {
                throw new IncompatibleDiscreteElementSizeException("The DiscreteElements must have the same size to process the disjunction!");
            }
            //Uniting:
            uint[] numbers = new uint[this._numbers.Length];
            for (int i = 0; i < Numbers.Length; i++)
            {
                numbers[i] = this.Numbers[i] | e.Numbers[i];
            }
            return new DiscreteElement(Size, numbers);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the disjunction/union of the current discrete element with the given one. Does not modify
        /// the current element, creates a new one instead. Strictly identical to <see cref="Disjunction"/>.
        /// </summary>
        /// <param name="e">The discrete element to get the union with.</param>
        /// <returns>Returns a new element which is the disjunction/union of the current discrete element
        /// with the given one.</returns>
        /// <exception cref="IncompatibleDiscreteElementSizeException">Thrown if the given discrete element has not
        /// the same size as the the current one (they are not defined on the same frame of discernment).</exception>
        public override DiscreteElement Union(DiscreteElement e)
        {
            return this.Disjunction(e);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Checks if the given discrete element is a subset of the current one.
        /// </summary>
        /// <param name="e">The element to test.</param>
        /// <returns>Returns true if the current discrete element is a subset of the given one, 
        /// false otherwise.</returns>
        public override bool IsASubset(DiscreteElement e)
        {
            //Checking arguments:
            if (this.Size != e.Size)
            {
                return false;
            }
            //Making the test:
            for (int i = 0; i < _numbers.Length; i++)
            {
                if ((this._numbers[i] & e._numbers[i]) != this._numbers[i])
                {
                    return false;
                }
            }
            return true;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Checks if the given discrete element is a superset of the current one.
        /// </summary>
        /// <param name="e">The element to test.</param>
        /// <returns>Returns true if the current discrete element is a superset of the given 
        /// one, false otherwise.</returns>
        public override bool IsASuperset(DiscreteElement e)
        {
            return e.IsASubset(this);
        }

        #endregion

        //------------------------------------------------------------------------------------------------

        #region Utility methods

        /// <summary>
        /// Gets the empty discrete element of the same size than the current one.
        /// </summary>
        /// <returns>Returns a new discrete element which represents the empty set (with the same size
        /// than the current one).</returns>
        public override DiscreteElement GetEmptyElement()
        {
            return DiscreteElement.GetEmptyElement(this.Size);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets a discrete element representing the complete set of possible states/worlds (in other words, gets
        /// the frame of discernment) with the same size than the current one.
        /// </summary>
        /// <returns>Returns a new discrete element which represents the complete set (with the same size
        /// than the current one).</returns>
        public override DiscreteElement GetCompleteElement()
        {
            return DiscreteElement.GetCompleteElement(this.Size);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Creates a deep copy of the current discrete element.
        /// </summary>
        /// <returns>Returns a new discrete element which is a deep copy of the current element.</returns>
        public override DiscreteElement DeepCopy()
        {
            DiscreteElement copy = new DiscreteElement(this.Size, this.Numbers);
            copy._card = this._card;
            return copy;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Checks if the given discrete element is compatible with the current one (typically, if they have the
        /// same size).
        /// </summary>
        /// <param name="e">The discrete element to test compatibility with.</param>
        /// <returns>Returns true if the given discrete element is compatible, false otherwise.</returns>
        public override bool IsCompatible(DiscreteElement e)
        {
            return this.Size == e.Size;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Checks if the current discrete element is the empty set.
        /// </summary>
        /// <returns>Returns true if the current discrete element is the empty set, false otherwise.</returns>
        public override bool IsEmpty()
        {
            if (_card != -1)
            {
                return _card == 0;
            }
            for (int i = 0; i < Numbers.Length; i++)
            {
                if (Numbers[i] != 0)
                {
                    return false;
                }
            }
            return true;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Checks if the current discrete element is the complete set (the frame of discernment).
        /// </summary>
        /// <returns>Returns true if the current discrete element is the complete set, false otherwise.</returns>
        public override bool IsComplete()
        {
            if (_card != -1)
            {
                return _card == _size;
            }
            if (Numbers[Numbers.Length - 1] != (1U << (Size % NB_BITS_UINT)) - 1)
            {
                return false;
            }
            for (int i = 0; i < Numbers.Length - 1; i++)
            {
                if (Numbers[i] != uint.MaxValue)
                {
                    return false;
                }
            }
            return true;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Checks if the current discrete element and the given one are equal.
        /// </summary>
        /// <param name="e">The discrete element to compare to.</param>
        /// <returns>Returns true if both discrete elements are equal, false otherwise.</returns>
        public override bool Equals(DiscreteElement e)
        {
            if (this.Size != e.Size)
            {
                return false;
            }

            for (int i = 0; i < Numbers.Length; i++)
            {
                if (this.Numbers[i] != e.Numbers[i])
                {
                    return false;
                }
            }
            return true;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Checks if the current discrete element and the given object are equal.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>Returns true if the given object is an equal DiscreteElement, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            //Checking:
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }

            //Comparing:
            DiscreteElement e = (DiscreteElement)obj;
            if (this.Size != e.Size)
            {
                return false;
            }
            for (int i = 0; i < Numbers.Length; i++)
            {
                if (this.Numbers[i] != e.Numbers[i])
                {
                    return false;
                }
            }
            return true;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the hash code of the current discrete element.
        /// </summary>
        /// <returns>Returns the hash code of the current discrete element.</returns>
        public override int GetHashCode()
        {
            return (int)Numbers[0];
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets a string representation of the current discrete element (in the form of a binary number).
        /// </summary>
        /// <returns>Returns a string representation of the current discrete element.</returns>
        public override string ToString()
        {
            StringBuilder toReturn = new StringBuilder();
            int i = 0;
            foreach (bool world in Worlds)
            {
                if (world)
                {
                    toReturn.Append(1);
                }
                else
                {
                    toReturn.Append(0);
                }
                i++;
                if (i % NB_BITS_UINT == 0)
                {
                    toReturn.Append(" ");
                }
            }
            return toReturn.ToString();
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets a string representation of the current discrete element given a reference list in the form "{w1, ..., wN}".
        /// </summary>
        /// <param name="refList">The reference list to give sense to the states/worlds.</param>
        /// <returns>Returns a string representation of the current discrete element.</returns>
        /// <exception cref="IncompatibleReferenceListException">Thrown if the given reference list
        /// could not correspond to frame of discernment on which the discrete element has been defined.</exception>
        public string ToString(StringReferenceList refList)
        {
            return ToString<string>(refList);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets a string representation of the current discrete element given a reference list in the form "{w1, ..., wN}".
        /// </summary>
        /// <param name="refList">The reference list to give sense to the states/worlds.</param>
        /// <returns>Returns a string representation of the current discrete element.</returns>
        /// <exception cref="IncompatibleReferenceListException">Thrown if the given reference list
        /// could not correspond to frame of discernment on which the discrete element has been defined.</exception>
        public string ToString<T>(ReferenceList<T> refList)
        {
            bool[] localWorlds = this.Worlds;
            //Checking the arguments:
            if (refList.Count != localWorlds.Length)
            {
                throw new IncompatibleReferenceListException("The given ReferenceList is not adapted to this DiscreteElement!");
            }

            //Building the string:
            StringBuilder toReturn = new StringBuilder();
            toReturn.Append("{");
            int added = 0;
            for (int i = 0; i < localWorlds.Length; i++)
            {
                if (localWorlds[i])
                {
                    toReturn.Append(refList[i].ToString());
                    added++;
                    if (added < Card)
                    {
                        toReturn.Append(" u ");
                    }
                }
            }
            toReturn.Append("}");
            return toReturn.ToString();
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets a string representation of the current discrete element given a reference list in an easy to parse
        /// form where elements are simply separated by a space.
        /// </summary>
        /// <param name="refList">The reference list to give sense to the states/worlds.</param>
        /// <returns>Returns a string representation of the current discrete element.</returns>
        /// <exception cref="IncompatibleReferenceListException">Thrown if the given reference list
        /// could not correspond to frame of discernment on which the discrete element has been defined.</exception>
        public string ToConvenientString(StringReferenceList refList)
        {
            return ToConvenientString<string>(refList);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets a string representation of the current discrete element given a reference list in an easy to parse
        /// form where elements are simply separated by a space.
        /// </summary>
        /// <param name="refList">The reference list to give sense to the states/worlds.</param>
        /// <returns>Returns a string representation of the current discrete element.</returns>
        /// <exception cref="IncompatibleReferenceListException">Thrown if the given reference list
        /// could not correspond to frame of discernment on which the discrete element has been defined.</exception>
        public string ToConvenientString<T>(ReferenceList<T> refList)
        {
            bool[] localWorlds = this.Worlds;
            //Checking the arguments:
            if (refList.Count != localWorlds.Length)
            {
                throw new IncompatibleReferenceListException("The given ReferenceList is not adapted to this DiscreteElement!");
            }

            //Building the string:
            StringBuilder toReturn = new StringBuilder();
            int added = 0;
            for (int i = 0; i < localWorlds.Length; i++)
            {
                if (localWorlds[i])
                {
                    toReturn.Append(refList[i].ToString());
                    added++;
                    if (added < Card)
                    {
                        toReturn.Append(" ");
                    }
                }
            }
            return toReturn.ToString();
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets a string representation of the current discrete element given a reference list with an element per line.
        /// </summary>
        /// <param name="refList">The reference list to give sense to the states/worlds.</param>
        /// <returns>Returns a string representation of the current discrete element.</returns>
        /// <exception cref="IncompatibleReferenceListException">Thrown if the given reference list
        /// could not correspond to frame of discernment on which the discrete element has been defined.</exception>
        public string ToStringLines(StringReferenceList refList)
        {
            return ToStringLines<string>(refList);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets a string representation of the current discrete element given a reference list with an element per line.
        /// </summary>
        /// <param name="refList">The reference list to give sense to the states/worlds.</param>
        /// <returns>Returns a string representation of the current discrete element.</returns>
        /// <exception cref="IncompatibleReferenceListException">Thrown if the given reference list
        /// could not correspond to frame of discernment on which the discrete element has been defined.</exception>
        public string ToStringLines<T>(ReferenceList<T> refList)
        {
            bool[] localWorlds = this.Worlds;
            //Checking the arguments:
            if (refList.Count != localWorlds.Length)
            {
                throw new IncompatibleReferenceListException("The given ReferenceList is not adapted to this DiscreteElement!");
            }

            //Building the string:
            StringBuilder toReturn = new StringBuilder();
            int added = 0;
            for (int i = 0; i < localWorlds.Length; i++)
            {
                if (localWorlds[i])
                {
                    toReturn.Append(refList[i].ToString());
                    added++;
                    if (added < Card)
                    {
                        toReturn.Append("\n");
                    }
                }
            }
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
        /// Static method creating a new discrete element representing the empty set with the given size.
        /// </summary>
        /// <param name="size">The number of possible states/worlds in the frame of discernment.</param>
        /// <returns>Returns a new discrete element representing the empty set.</returns>
        public static DiscreteElement GetEmptyElement(int size)
        {
            //Checking arguments:
            if (size <= 1)
            {
                throw new ArgumentOutOfRangeException("size. The size of an DiscreteElement cannot be null, negative or too small!");
            }
            //Constructing:
            DiscreteElement empty = new DiscreteElement(size, 0);
            empty._card = 0;
            return empty;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Static method creating a new discrete element representing the complete set (the frame of discernment)
        /// with the given size.
        /// </summary>
        /// <param name="size">The number of possible states/worlds in the frame of discernment.</param>
        /// <returns>Returns a new discrete element representing the complete set.</returns>
        public static DiscreteElement GetCompleteElement(int size)
        {
            return DiscreteElement.GetEmptyElement(size).Opposite();
        }

        #endregion

        //------------------------------------------------------------------------------------------------

        #region Operator overrides

        /*						
         * Operators overriding
         */

        /// <summary>
        /// Overrides the "==" operator which simply tests equality between both discrete elements.
        /// </summary>
        /// <param name="a">The first discrete element to compare.</param>
        /// <param name="b">The second discrete element to compare.</param>
        /// <returns>Returns true if both discrete elements are equal, false otherwise.</returns>
        public static bool operator ==(DiscreteElement a, DiscreteElement b)
        {
            if ((object)a == null) return (object)b == null;
            if ((object)b == null) return (object)a == null;
            return !a.Equals(b);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Overrides the "==" operator which simply tests inequality between both discrete elements.
        /// </summary>
        /// <param name="a">The first discrete element to compare.</param>
        /// <param name="b">The second discrete element to compare.</param>
        /// <returns>Returns true if both discrete elements are not equal, false otherwise.</returns>
        public static bool operator !=(DiscreteElement a, DiscreteElement b)
        {
            if ((object)a == null) return (object)b != null;
            if ((object)b == null) return (object)a != null;
            return !a.Equals(b);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Overrides the AND operator to do the intersection/conjunction of the two given discrete
        /// elements.
        /// </summary>
        /// <param name="a">The first element of the intersection.</param>
        /// <param name="b">The second element of the intersection.</param>
        /// <returns>Returns a new discrete element which is the intersection/conjunction of
        /// the two given ones.</returns>
        public static DiscreteElement operator &(DiscreteElement a, DiscreteElement b)
        {
            return a.Conjunction(b);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Overrides the OR operator to do union/disjunction of the two given discrete elements.
        /// </summary>
        /// <param name="a">The first element of the union.</param>
        /// <param name="b">The second element of the union.</param>
        /// <returns>Returns a new element which is the union/disjunction of the two given ones.</returns>
        public static DiscreteElement operator |(DiscreteElement a, DiscreteElement b)
        {
            return a.Disjunction(b);
        }

        #endregion

    } //Class
} //Namespace


