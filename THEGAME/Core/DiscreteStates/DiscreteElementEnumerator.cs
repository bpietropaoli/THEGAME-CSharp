using System.Collections;
using System.Collections.Generic;

namespace THEGAME.Core.DiscreteStates
{
    /// <summary>
    /// A class to enumerate over all the possible <see cref="DiscreteElement"/>.  It prevents from
    /// building the power set just to iterate over it when it is not used for anything else. 
    /// Be aware that you will enumerate over 2^Size elements so it may harm your computer if 
    /// you choose a size too big.
    /// </summary>
    public class DiscreteElementEnumerator : IEnumerable<DiscreteElement>
    {
        /*
         * Members:
         */
        /// <summary>
        /// Indicates if the enumeration started or not.
        /// </summary>
        protected bool _started;

        /*
         * Properties:
         */
        /// <summary>
        /// The current element in the enumeration.
        /// </summary>
        public DiscreteElement CurrentElement { get; private set; }

        /// <summary>
        /// The size of the discrete elements.
        /// </summary>
        public int Size { get; private set; }

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        /*
         * Constructor:
         */

        /// <summary>
        /// <para>Builds an enumerator of discrete elements.</para>
        /// <para>WARNING: DO NOT BUILD AN ENUMERATOR WITH A SIZE TOO BIG, IT WILL HARM YOUR POOR
        /// COMPUTER.</para>
        /// </summary>
        /// <param name="size">The size of the elments on which to enumerate.</param>
        public DiscreteElementEnumerator(int size)
        {
            this.CurrentElement = DiscreteElement.GetEmptyElement(size);
            this.Size = size;
            this._started = false;
        }

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        /*
         * Methods:
         */

        #region Utility methods

        /// <summary>
        /// Increments the enumerator to go to the next discrete element.
        /// </summary>
        /// <returns>Returns true if it can continue to increment, false otherwise.</returns>
        public bool Increment()
        {
            //Build the first element:
            if (!_started)
            {
                _started = true;
                return true;
            }
            //End the loop:
            else if (CurrentElement.IsComplete())
            {
                CurrentElement = DiscreteElement.GetEmptyElement(Size);
                _started = false;
                return false;
            }
            //Increment:
            else
            {
                uint[] numbers = CurrentElement.Numbers;
                if (numbers[0] != uint.MaxValue)
                {
                    numbers[0]++;
                }
                else
                {
                    numbers[0] = 0;
                    int indexToIncrement = 1;
                    for (int i = 1; i < numbers.Length - 1; i++)
                    {
                        if (numbers[i] == uint.MaxValue)
                        {
                            numbers[i] = 0;
                            indexToIncrement++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    numbers[indexToIncrement]++;
                }
                CurrentElement = new DiscreteElement(Size, numbers);
                return true;
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Resets the enumerator to the state used at the beginning of the loop.
        /// </summary>
        public void Reset()
        {
            CurrentElement = null;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Resets the enumerator to the state used at the beginning of the loop and set a new element size.
        /// </summary>
        /// <param name="size">The new size for the element returned by the enumerator.</param>
        public void Reset(int size)
        {
            CurrentElement = null;
            Size = size;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gives a string representation of the current state of the enumerator.
        /// </summary>
        /// <returns>Returns a string representation of the enumerator.</returns>
        public override string ToString()
        {
            return "DiscreteElementEnumerator: " + CurrentElement.ToString();
        }

        #endregion

        //------------------------------------------------------------------------------------------------

        #region IEnumerable<DiscreteElement>

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Used to enumerate over DiscreteElements using a foreach loop.
        /// </summary>
        /// <returns>Returns the current element used in the foreach loop.</returns>
        public IEnumerator<DiscreteElement> GetEnumerator()
        {
            while (Increment())
            {
                yield return CurrentElement;
            }
        }

        #endregion

    } //Class
} //Namespace
