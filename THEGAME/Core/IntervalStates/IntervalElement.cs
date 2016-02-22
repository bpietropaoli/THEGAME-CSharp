using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using THEGAME.Core.Generic;
using THEGAME.Exceptions;

namespace THEGAME.Core.IntervalStates
{
    /// <summary>
    /// A class to represent interval elements within the theory of belief functions. 
    /// Used in <see cref="IntervalMassFunction"/> as focal elements.
    /// </summary>
    public sealed class IntervalElement : AElement<IntervalElement>,
                                          IEnumerable<Interval>
    {

        /*
         * Members:
         */
        private List<Interval> _intervals;     //The possibly multiple intervals composing the element.

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Properties

        /*
         * Properties:
         */

        /// <summary>
        /// Gets the cardinal of the interval element (it is the sum of the intervals composing
        /// the element).
        /// </summary>
        public override double Card 
        {
            get
            {
                double sum = 0;
                foreach (Interval i in _intervals)
                {
                    sum += i.Size;
                }
                return sum;
            } 
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the intervals composing the element.
        /// </summary>
        public List<Interval> Intervals 
        { 
            get { return _intervals; } 
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Constructors

        /*
         * Constructor:
         */

        /// <summary>
        /// Builds an empty (literaly) interval element.
        /// </summary>
        public IntervalElement()
        {
            _intervals = new List<Interval>();
        }

        //------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Builds an interval element with the given values for the start and the end of the interval.
        /// </summary>
        /// <param name="start">The starting value of the interval.</param>
        /// <param name="end">The ending value of the interval.</param>
        /// <exception cref="InvalidIntervalException">Thrown if the given starting value is greater
        /// than the given ending value.</exception>
        public IntervalElement(double start, double end) : this()
        {
            _intervals.Add(new Interval(start, end));
        }

        //------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Builds an interval element with the given intervals.
        /// </summary>
        /// <param name="intervals">The intervals composing the interval element.</param>
        public IntervalElement(params Interval[] intervals) : this()
        {
            foreach(Interval i in intervals)
            {
                _intervals.Add(new Interval(i));
            }
            this.Sort();
            this.Clean();
        }

        //------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Builds an interval element with the given intervals.
        /// </summary>
        /// <param name="intervals">The intervals composing the interval element.</param>
        public IntervalElement(List<Interval> intervals) : this(intervals.ToArray()) { }

        //------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Builds an interval element as a deep copy of the given one.
        /// </summary>
        /// <param name="e">The interval element to copy.</param>
        public IntervalElement(IntervalElement e) : this(e.Intervals) { }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        /*
         * Methods:
         */

        #region Element related methods

        /// <summary>
        /// Gets the opposite of the current interval element.
        /// </summary>
        /// <returns>Returns a new interval element which is the opposite of the current one.</returns>
        public override IntervalElement Opposite()
        {
            if(IsComplete())
            {
                return new IntervalElement();
            }
            if (IsEmpty())
            {
                return new IntervalElement(Double.NegativeInfinity, Double.PositiveInfinity);
            }

            IntervalElement result = new IntervalElement();
            this.Sort();
            for (int i = 0; i < _intervals.Count; i++)
            {
                //First interval:
                if (i == 0 && !Double.IsNegativeInfinity(_intervals[0].Start))
                {
                    result.Add(Double.NegativeInfinity, _intervals[0].Start);
                }
                //Last interval:
                if (i == _intervals.Count - 1 && !Double.IsPositiveInfinity(_intervals[_intervals.Count - 1].End))
                {
                    result.Add(_intervals[_intervals.Count - 1].End, Double.PositiveInfinity);
                }
                //Other intervals:
                else if(i < _intervals.Count - 1)
                {
                    if (!_intervals[i].Overlap(_intervals[i + 1]))
                    {
                        result.Add(_intervals[i].End, _intervals[i + 1].Start);
                    }
                }
            }
            return result;
        }
        
        //------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Gets the conjunction/intersection of the current interval element with the given one.
        /// Should be strictly identical to <see cref="Intersection"/>.
        /// </summary>
        /// <param name="e">The interval element to intersect with.</param>
        /// <returns>Returns a new interval element which is the conjunction/intersection of the current 
        /// element with the given one.</returns>
        public override IntervalElement Conjunction(IntervalElement e)
        {
            IntervalElement result = new IntervalElement();
            foreach (Interval i1 in this)
            {
                foreach (Interval i2 in e)
                {
                    Interval conj = i1.Conjunction(i2);
                    if (!conj.IsEmpty())
                    {
                        result.Add(conj);
                    }
                }
            }
            result.Sort();
            result.Clean();
            return result;
        }
        
        //------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Gets the conjunction/intersection of the current interval element with the given one.
        /// Should be strictly identical to <see cref="Conjunction"/>.
        /// </summary>
        /// <param name="e">The interval element to intersect with.</param>
        /// <returns>Returns a new interval element which is the conjunction/intersection of the current 
        /// element with the given one.</returns>
        public override IntervalElement Intersection(IntervalElement e)
        {
            return this.Conjunction(e);
        }
        
        //------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Gets the disjunction/union of the current interval element with the given one.
        /// Should be strictly identical to <see cref="Union"/>.
        /// </summary>
        /// <param name="e">The interval element to do the union with.</param>
        /// <returns>Returns a new interval element which is the disjunction/union of the current
        /// element with the given one.</returns>
        public override IntervalElement Disjunction(IntervalElement e)
        {
            IntervalElement result = new IntervalElement();
            foreach (Interval i in this)
            {
                result.Add(i);
            }
            foreach (Interval i in e)
            {
                result.Add(i);
            }
            this.Sort();
            this.Clean();
            return result;
        }
        
        //------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Gets the disjunction/union of the current interval element with the given one.
        /// Should be strictly identical to <see cref="Disjunction"/>.
        /// </summary>
        /// <param name="e">The interval element to do the union with.</param>
        /// <returns>Returns a new interval element which is the disjunction/union of the current
        /// element with the given one.</returns>
        public override IntervalElement Union(IntervalElement e)
        {
            return this.Disjunction(e);
        }
        
        //------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Gets the empty interval element.
        /// </summary>
        /// <returns>Returns a new interval element which is empty.</returns>
        public override IntervalElement GetEmptyElement()
        {
            return new IntervalElement();
        }
        
        //------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Gets a new interval element which is representing the complete set.
        /// </summary>
        /// <returns>Returns a new element representing the complete set.</returns>
        public override IntervalElement GetCompleteElement()
        {
            return new IntervalElement(Double.NegativeInfinity, Double.PositiveInfinity);
        }
        
        //------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Checks if the given element is a subset of the current one.
        /// </summary>
        /// <param name="e">The element to compare to.</param>
        /// <returns>Returns true if the current element is a subset of the given one,
        /// false otherwise.</returns>
        public override bool IsASubset(IntervalElement e)
        {
            foreach (Interval i in this)
            {
                bool contained = false;
                foreach (Interval j in e)
                {
                    if (j.Contains(i))
                    {
                        contained = true;
                        break;
                    }
                }
                if (!contained)
                {
                    return false;
                }
            }
            return true;
        }
        
        //------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Checks if the given interval element is a superset of the current one.
        /// </summary>
        /// <param name="e">The interval element to compare to.</param>
        /// <returns>Returns true if the current interval element is a superset of the given one,
        /// false otherwise.</returns>
        public override bool IsASuperset(IntervalElement e)
        {
            return e.IsASubset(this);
        }
        
        //------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Checks if the current interval element equals the empty set.
        /// </summary>
        /// <returns>Returns true if the current interval element equals the empty set,
        /// false otherwise.</returns>
        public override bool IsEmpty()
        {
            this.Sort();
            this.Clean();
            return (this._intervals.Count == 1 && this._intervals[0].IsEmpty()) || this._intervals.Count == 0;
        }
        
        //------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Checks if the current interval element equals the complete set.
        /// </summary>
        /// <returns>Returns true if the current interval element is the complete set,
        /// false otherwise.</returns>
        public override bool IsComplete()
        {
            //To make sure the code following will apply:
            this.Sort();
            this.Clean();
            //Checks if complet:
            return this._intervals.Count == 1 && this._intervals[0].IsComplete();
        }

        #endregion
        
        //------------------------------------------------------------------------------------------------
        
        #region Utility methods

        #region Private methods

        private void Sort()
        {
            this._intervals.Sort();
        }

        //------------------------------------------------------------------------------------------------
        
        private void Clean()
        {
            for (int i = 0; i < _intervals.Count; i++)
            {
                for (int j = 0; j < _intervals.Count; j++)
                {
                    //If equality:
                    if (i != j && _intervals[i] == _intervals[j])
                    {
                        Console.WriteLine("{0} == {1}", _intervals[i], _intervals[j]);
                        _intervals.RemoveAt(j);
                    }
                    //If overlapping:
                    if (i != j && _intervals[i].Overlap(_intervals[j]))
                    {
                        Interval i1 = _intervals[i];
                        Interval i2 = _intervals[j];
                        if (i1 <= i2)
                        {
                            this.Add(i1.Start, i2.End);
                        }
                        else
                        {
                            this.Add(i2.Start, i1.End);
                        }
                        _intervals.Remove(i1);
                        _intervals.Remove(i2);
                    }
                }
            }
        }

        //------------------------------------------------------------------------------------------------
        
        private void Add(Interval i)
        {
            _intervals.Add(i);
        }

        //------------------------------------------------------------------------------------------------
        
        private void Add(double s, double e)
        {
            _intervals.Add(new Interval(s, e));
        }

        #endregion

        //------------------------------------------------------------------------------------------------

        #region IEnumerable<Interval>

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Used to enable foreach loops to be used on interval elements.
        /// </summary>
        /// <returns>Returns the next interval of the interval element in the foreach loop.</returns>
        public IEnumerator<Interval> GetEnumerator()
        {
            foreach (Interval i in _intervals)
            {
                yield return i;
            }
        }

        #endregion

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets a deep copy of the current interval element.
        /// </summary>
        /// <returns>Returns a deep copy of the current interval element.</returns>
        public override IntervalElement DeepCopy()
        {
            IntervalElement result = new IntervalElement();
            foreach (Interval i in this)
            {
                result.Add(new Interval(i));
            }
            return result;
        }
        
        //------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// All interval elements are compatible with each others.
        /// </summary>
        /// <param name="e">The element to check the compatibility with.</param>
        /// <returns>Returns true (this doc is exact).</returns>
        public override bool IsCompatible(IntervalElement e)
        {
            return true;
        }
        
        //------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Checks equality between the current interval element and the given one.
        /// </summary>
        /// <param name="e">The interval element to compare to.</param>
        /// <returns>Returns true if the given interval element equals the current one, 
        /// false otherwise.</returns>
        public override bool Equals(IntervalElement e)
        {
            foreach (Interval i in this)
            {
                if (!e._intervals.Contains(i))
                {
                    return false;
                }
            }
            foreach (Interval i in e)
            {
                if (!this._intervals.Contains(i))
                {
                    return false;
                }
            }
            return true;
        }
        
        //------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Checks if the given object equals the current element.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>Returns false if the given object is not a <see cref="AElement{IntervalElement}"/>,
        /// returns <see cref="Equals(IntervalElement)"/> otherwise.</returns>
        public override bool Equals(object obj)
        {
            //Checking:
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            return this.Equals(obj as IntervalElement);
        }
        
        //------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Gets the hash code of the current element.
        /// </summary>
        /// <returns>Returns the has code of the current element.</returns>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        //------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Gives a string representation of the current interval element.
        /// </summary>
        /// <returns>Returns a string representation of the current interval element.</returns>
        public override string ToString()
        {
            string result = "{";
            for (int i = 0; i < _intervals.Count; i++)
            {
                if (i == _intervals.Count - 1)
                {
                    result += _intervals[i].ToString();
                }
                else
                {
                    result += _intervals[i].ToString() + ", "; 
                }
            }
            result += "}";
            return result;
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        /*
         * Static methods:
         */

        #region Static utility methods

        /// <summary>
        /// Static method creating a new interval element representing the empty set.
        /// </summary>
        /// <returns>Returns a new interval element representing the empty set.</returns>
        public static IntervalElement StaticGetEmptyElement()
        {
            return new IntervalElement();
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Static method creating a new interval element representing the complete set (the frame of discernment)
        /// </summary>
        /// <returns>Returns a new interval element representing the complete set.</returns>
        public static IntervalElement StaticGetCompleteElement()
        {
            return new IntervalElement(Double.NegativeInfinity, Double.PositiveInfinity);
        }

        #endregion

        //------------------------------------------------------------------------------------------------

        #region Operator overrides

        /*						
         * Operators overriding
         */

        /// <summary>
        /// Overrides the "==" operator which simply tests equality between both interval elements.
        /// </summary>
        /// <param name="a">The first interval element to compare.</param>
        /// <param name="b">The second interval element to compare.</param>
        /// <returns>Returns true if both interval elements are equal, false otherwise.</returns>
        public static bool operator ==(IntervalElement a, IntervalElement b)
        {
            if ((object)a == null) return (object)b == null;
            if ((object)b == null) return (object)a == null;
            return !a.Equals(b);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Overrides the "==" operator which simply tests inequality between both interval elements.
        /// </summary>
        /// <param name="a">The first interval element to compare.</param>
        /// <param name="b">The second interval element to compare.</param>
        /// <returns>Returns true if both interval elements are not equal, false otherwise.</returns>
        public static bool operator !=(IntervalElement a, IntervalElement b)
        {
            if ((object)a == null) return (object)b != null;
            if ((object)b == null) return (object)a != null;
            return !a.Equals(b);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Overrides the AND operator to do the intersection/conjunction of the two given interval
        /// elements.
        /// </summary>
        /// <param name="a">The first element of the intersection.</param>
        /// <param name="b">The second element of the intersection.</param>
        /// <returns>Returns a new interval element which is the intersection/conjunction of
        /// the two given ones.</returns>
        public static IntervalElement operator &(IntervalElement a, IntervalElement b)
        {
            return a.Conjunction(b);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Overrides the OR operator to do union/disjunction of the two given interval elements.
        /// </summary>
        /// <param name="a">The first element of the union.</param>
        /// <param name="b">The second element of the union.</param>
        /// <returns>Returns a new interval element which is the union/disjunction of the two given ones.</returns>
        public static IntervalElement operator |(IntervalElement a, IntervalElement b)
        {
            return a.Disjunction(b);
        }

        #endregion

    } //Class
} //Namespace
