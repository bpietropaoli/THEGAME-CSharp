using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using THEGAME.Core.Generic;

namespace THEGAME.Core.IntervalStates
{
    /// <summary>
    /// A class to represent a mass function over interval elements. Inherits from 
    /// <see cref="AMassFunction{IntervalMassFunction, IntervalElement}"/>.
    /// </summary>
    public sealed class IntervalMassFunction : AMassFunction<IntervalMassFunction, IntervalElement>
    {

        #region Constructors

        /*
         * Constructors:
         */

        /// <summary>
        /// Builds an empty mass function (not a valid one, mass needs to be added).
        /// </summary>
        public IntervalMassFunction()
        {
            this._focals = new List<FocalElement<IntervalElement>>();
        }

        //------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Builds a mass function with the given focal elements.
        /// </summary>
        /// <param name="elements">The elements to add as focal elements.</param>
        /// <param name="masses">The masses associated to the given elements.</param>
        public IntervalMassFunction(IntervalElement[] elements, double[] masses)
            : this()
        {
            for (int i = 0; i < elements.Length; i++)
            {
                _focals.Add(new FocalElement<IntervalElement>(elements[i], masses[i]));
            }
        }

        //------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Builds a mass function from the given focal elements.
        /// </summary>
        /// <param name="focals">The focal elements to add to the mass function.</param>
        public IntervalMassFunction(params FocalElement<IntervalElement>[] focals)
        {
            _focals = new List<FocalElement<IntervalElement>>(focals);
        }

        //------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Builds a mass function from the given focal elements.
        /// </summary>
        /// <param name="focals">The focal elements to add to the mass function.</param>
        public IntervalMassFunction(List<FocalElement<IntervalElement>> focals)
        {
            _focals = new List<FocalElement<IntervalElement>>(focals);
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Static methods

        /*
         * Static methods
         */

        /// <summary>
        /// Gets the vacuous mass function with interval elements.
        /// </summary>
        /// <returns>Returns a new mass functions defined on intervals which is
        /// the vacuous mass function.</returns>
        public static IntervalMassFunction GetVacuousMassFunction()
        {
            return new IntervalMassFunction(new FocalElement<IntervalElement>(new IntervalElement(new Interval(Double.NegativeInfinity, Double.PositiveInfinity)), 1));
        }

        #endregion

    } //Class
} //Namespace
