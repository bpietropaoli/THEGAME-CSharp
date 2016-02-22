using System;

using THEGAME.Exceptions;

namespace THEGAME.Core.IntervalStates
{
    /// <summary>
    /// A simple structure to represent 1D intervals. Used in <see cref="IntervalElement"/> but cannot be
    /// used directly into mass functions as they cannot manage unions alone for instance.
    /// </summary>
    public struct Interval : IEquatable<Interval>, IComparable<Interval>
    {
        /*
         * Properties
         */
        /// <summary>
        /// Gets the starting value of the interval.
        /// </summary>
        public double Start { get; private set; }

        /// <summary>
        /// Gets the ending value of the interval.
        /// </summary>
        public double End { get; private set; }

        /// <summary>
        /// Gets the size of the current interval (could be seen as the cardinal of a discrete set).
        /// </summary>
        public double Size
        {
            get
            {
                if (IsEmpty())
                {
                    return 0;
                }
                else if (IsComplete() || 
                         this.End == Double.PositiveInfinity || 
                         this.Start == Double.NegativeInfinity)
                {
                    return Double.PositiveInfinity;
                }
                else
                {
                    return this.End - this.Start;
                }
            }
        }

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Constructors

        /*
         * Constructors:
         */

        /// <summary>
        /// Builds an interval with the given values.
        /// </summary>
        /// <param name="start">The starting value of the interval.</param>
        /// <param name="end">The ending value of the interval.</param>
        /// <exception cref="InvalidIntervalException">Thrown if the given starting value is greater
        /// than the given ending value.</exception>
        public Interval(double start, double end) : this()
        {
            if (!Double.IsNaN(start) && !Double.IsNaN(end) && start > end)
            {
                throw new InvalidIntervalException("The starting value should be smaller than the ending value!");
            }
            this.Start = start;
            this.End = end;
        }

        //------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Builds an interval which is a deep copy of the given one.
        /// </summary>
        /// <param name="i">The interval to copy.</param>
        public Interval(Interval i) : this()
        {
            this.Start = i.Start;
            this.End = i.End;
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        /*
         * Methods:
         */

        #region Interval related

        /// <summary>
        /// Checks if the current interval represents the entire possible set.
        /// </summary>
        /// <returns>Returns true if the current interval is the complete set, false otherwise.</returns>
        public bool IsComplete()
        {
            return Start == Double.NegativeInfinity && End == Double.PositiveInfinity;
        }

        //------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Checks if the current interval is the empty set.
        /// </summary>
        /// <returns>Returns true if the current interval is empty, false otherwise.</returns>
        public bool IsEmpty()
        {
            return Double.IsNaN(Start) || Double.IsNaN(End);
        }

        //------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Checks if the current interval and the given one overlap.
        /// </summary>
        /// <param name="i">The interval to compare to.</param>
        /// <returns>Returns true if both intervals overlap, false otherwise.</returns>
        public bool Overlap(Interval i)
        {
            return (i.Start <= this.Start && this.Start <= i.End) || (i.Start <= this.End && this.End <= i.End) ||
                   (this.Start <= i.Start && i.Start <= this.End) || (this.Start <= i.End && i.End <= this.End);
        }

        //------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Checks if the given interval is contained into the current one.
        /// </summary>
        /// <param name="i">The interval to test.</param>
        /// <returns>Returns true if the given interval is contained in the current one, 
        /// false otherwise.</returns>
        public bool Contains(Interval i)
        {
            if (i.IsEmpty())
            {
                return true;
            }
            return this.Start <= i.Start && i.Start <= this.End && this.Start <= i.End && i.End <= this.End; 
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Checks if the current interval is contained into the given one.
        /// </summary>
        /// <param name="i">The interval to test.</param>
        /// <returns>Returns true if the current interval is contained in the current one,
        /// false otherwise.</returns>
        public bool IsContained(Interval i)
        {
            return i.Contains(this);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the intersection/conjunction of the current interval with the given one. Strictly
        /// identical to <see cref="Conjunction"/>.
        /// </summary>
        /// <param name="i">The interval to intersect with.</param>
        /// <returns>Returns a new interval which is the intersection/conjunction of the current
        /// interval with the given one.</returns>
        public Interval Intersection(Interval i)
        {
            if (this.Overlap(i))
            {
                double start = this.Start > i.Start ? this.Start : i.Start;
                double end = this.End < i.End ? this.End : i.End;
                return new Interval(start, end);
            }
            else
            {
                return Interval.GetEmptyInterval();
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the intersection/conjunction of the current interval with the given one. Strictly
        /// identical to <see cref="Intersection"/>.
        /// </summary>
        /// <param name="i">The interval to intersect with.</param>
        /// <returns>Returns a new interval which is the intersection/conjunction of the current
        /// interval with the given one.</returns>
        public Interval Conjunction(Interval i)
        {
            return this.Intersection(i);
        }

        #endregion

        //------------------------------------------------------------------------------------------------

        #region Utility methods

        /// <summary>
        /// Creates a deep copy of the current interval.
        /// </summary>
        /// <returns>Returns a new interval which is a deep copy of the current one.</returns>
        public Interval DeepCopy()
        {
            return new Interval(this);
        }

        /// <summary>
        /// Checks if the current interval and the given one are equal.
        /// </summary>
        /// <param name="i">The interval to compare to.</param>
        /// <returns>Returns true if both intervals are equal, false otherwise.</returns>
        public bool Equals(Interval i)
        {
            if (!this.IsEmpty() && !i.IsEmpty())
            {
                return this.Start == i.Start && this.End == i.End;
            }
            else
            {
                return true;
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Checks if the current interval and the given object are equal.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>Returns true if the given object is an equal interval, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            //Checking:
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            Interval i = (Interval)obj;
            return this.Start == i.Start && this.End == i.End;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Compares the current interval with the given one. Used to sort them.
        /// </summary>
        /// <param name="i">The interval to compare to.</param>
        /// <returns>Returns -1 if the current interval precedes the given one, 1 if it follows it, 
        /// 0 if both are equal.</returns>
        public int CompareTo(Interval i)
        {
            if (this.Start < i.Start)
            {
                return -1;
            }
            else if (this.Start == i.Start)
            {
                if (this.End < i.End)
                {
                    return -1;
                }
                else if (this.End == i.End)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                return 1;
            }
        }

        //------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Gets the hash code of the interval.
        /// </summary>
        /// <returns>Returns the hash code of the interval.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        //------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Gives a string representation of the current interval.
        /// </summary>
        /// <returns>Returns a string representation of the current interval.</returns>
        public override string ToString()
        {
            if (IsEmpty())
            {
                return "[empty]";
            }
            else if (IsComplete())
            {
                return "[-infinity, +infinity]";
            }
            else 
            {
                return "[" + this.Start + ", " + this.End + "]";
            }
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
        /// Gets the interval corresponding to the complete set.
        /// </summary>
        /// <returns>Returns a new interval which is the complete set.</returns>
        public static Interval GetCompleteInterval()
        {
            return new Interval(Double.NegativeInfinity, Double.PositiveInfinity);
        }

        //------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Gets the interval corresponding to the empty set.
        /// </summary>
        /// <returns>Returns a new interval which is the empty set.</returns>
        public static Interval GetEmptyInterval()
        {
            return new Interval(Double.NaN, Double.NaN);
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        
        #region Operator overrides

        /*						
         * Operators overriding
         */

        /// <summary>
        /// Overrides the "==" operator which simply tests equality between both intervals.
        /// </summary>
        /// <param name="a">The first interval to compare.</param>
        /// <param name="b">The second interval to compare.</param>
        /// <returns>Returns true if both intervals are equal, false otherwise.</returns>
        public static bool operator ==(Interval a, Interval b)
        {
            if ((object)a == null) return (object)b == null;
            if ((object)b == null) return (object)a == null;
            return !a.Equals(b);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Overrides the "==" operator which simply tests inequality between both intervals.
        /// </summary>
        /// <param name="a">The first interval to compare.</param>
        /// <param name="b">The second interval to compare.</param>
        /// <returns>Returns true if both intervals are not equal, false otherwise.</returns>
        public static bool operator !=(Interval a, Interval b)
        {
            if ((object)a == null) return (object)b != null;
            if ((object)b == null) return (object)a != null;
            return !a.Equals(b);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Overrides the &lt; operator which simply tests if the first interval should be placed
        /// before the second one when they are sorted.
        /// </summary>
        /// <param name="a">The first interval to compare.</param>
        /// <param name="b">The second interval to compare.</param>
        /// <returns>Returns true if the first interval precedes the second one, false otherwise.</returns>
        public static bool operator <(Interval a, Interval b)
        {
            return a.CompareTo(b) == -1;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Overrides the &lt;= operator which simply tests if the first interval should be placed
        /// before the second one when they are sorted.
        /// </summary>
        /// <param name="a">The first interval to compare.</param>
        /// <param name="b">The second interval to compare.</param>
        /// <returns>Returns true if the first interval precedes the second one, false otherwise.</returns>
        public static bool operator <=(Interval a, Interval b)
        {
            int result = a.CompareTo(b);
            return result == 0 || result == -1;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Overrides the &gt; operator which simply tests if the first interval should be placed
        /// after the second one when they are sorted.
        /// </summary>
        /// <param name="a">The first interval to compare.</param>
        /// <param name="b">The second interval to compare.</param>
        /// <returns>Returns true if the first interval follows the second one, false otherwise.</returns>
        public static bool operator >(Interval a, Interval b)
        {
            return a.CompareTo(b) == 1;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Overrides the &gt;= operator which simply tests if the first interval should be placed
        /// after the second one when they are sorted.
        /// </summary>
        /// <param name="a">The first interval to compare.</param>
        /// <param name="b">The second interval to compare.</param>
        /// <returns>Returns true if the first interval follows the second one, false otherwise.</returns>
        public static bool operator >=(Interval a, Interval b)
        {
            int result = a.CompareTo(b);
            return result == 0 || result == 1;
        }

        #endregion

    } //Structure
}//Namespace
