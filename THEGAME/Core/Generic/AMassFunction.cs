using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using THEGAME.Exceptions;

namespace THEGAME.Core.Generic
{
    /// <summary>
    /// An abstract class representing mass functions. All the methods that can be implemented independently of the type
    /// of elements used are implemented here. This means that the combination rules, the decision support functions, etc
    /// are all implemented for any kind of element type. This should ease the application of the theory to new problems.
    /// </summary>
    /// <typeparam name="TFunction">The type of function which must inherit from <see cref="AMassFunction{TFunction, TElement}"/>.
    /// This way, all the methods are defined with the inheriting type as parameters.</typeparam>
    /// <typeparam name="TElement">The type of element to use in this mass function. It must inherit 
    /// from <see cref="AElement{TElement}"/>.</typeparam>
    public abstract class AMassFunction<TFunction, TElement> : IEnumerable<FocalElement<TElement>>
                                                               where TElement : AElement<TElement>
                                                               where TFunction : AMassFunction<TFunction, TElement>, new()
    {
        /*
         * Constants and enums
         */
        /// <summary>
        /// The precision used for the computation of mass values.
        /// This is used especially for cleaning and to check if
        /// the sum is valid.
        /// </summary>
        public const double PRECISION = 0.000002;

        /// <summary>
        /// The currently implemented combination rules.
        /// </summary>
        public enum CombinationRules
        {
            /// <summary>
            /// The Dempster's rule of combination (normalised).
            /// </summary>
            COMBINATION_DEMPSTER,
            /// <summary>
            /// The unnormalised Dempster's rule of combination (by P. Smets).
            /// For a definition, refer to ¨P. Smets: The transferable belief model, 1994".
            /// </summary>
            COMBINATION_SMETS,
            /// <summary>
            /// The disjunctive rule of combination. For a definition, refer to "P. Smets, 
            /// Belief functions: The disjunctive rule of combination and the generalized 
            /// bayesian theorem, 1993".
            /// </summary>
            COMBINATION_DISJUNCTIVE,
            /// <summary>
            /// The Yager's rule of combination (like smets' but the mass to the empty 
            /// set is given to the total ignorance instead). For a definition, refer to
            /// "R. Yager: On the Dempster-Shafer framework and new combination rules, 1987".
            /// </summary>
            COMBINATION_YAGER,
            /// <summary>
            /// The D. Dubois and H. Prade's rule of combination (conjunctive AND disjunctive).
            /// For a definition, refer to "D. Dubois and H. Prade: Representation and Combination 
            /// of Uncertainty with Belief Functions and Possibility Measures, 1988".
            /// </summary>
            COMBINATION_DUBOISPRADE,
            /// <summary>
            /// The averaging rule of combination.
            /// </summary>
            COMBINATION_AVERAGE,
            /// <summary>
            /// The Murphy's rule of combination (based on average and Dempster's). For a definition,
            /// refer to "C. K. Murphy: Combining belief functions when evidence conflicts, 2000".
            /// </summary>
            COMBINATION_MURPHY,
            /// <summary>
            /// The Chen's rule of combination (based on distance). For a definition, please refer to
            /// "L.-Z. Chen: A new fusion approach based on distance of evidences, 2005".
            /// </summary>
            COMBINATION_CHEN
        };

        /// <summary>
        /// A delegate function taking an element as argument. Used by the decision support
        /// methods as a criterion. It should compute some value between 0 and 1 for the given
        /// element.
        /// </summary>
        /// <param name="e">The element to compute something for.</param>
        /// <returns>Returns a double between 0 and 1 for the given element.</returns>
        public delegate double CriterionFunction(TElement e);

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        /*		
         * Members
         */

        /// <summary>
        /// The focal set of the mass function.
        /// </summary>
        protected List<FocalElement<TElement>> _focals;

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Properties

        /*		
         * Properties
         */

        /// <summary>
        /// Gets the focal set of the mass function.
        /// </summary>
        public List<FocalElement<TElement>> Focals
        {
            get { return _focals; }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the number of focal elements (m(e) > 0) the mass function has.
        /// </summary>
        public int NbFocals
        {
            get { return _focals.Count; }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the specificity of the mass function (for a definition, refer to "Yager, R.:
        /// Entropy and specificity in a mathematical theory of evidence, 1983").
        /// </summary>
        public double Specificity
        {
            get
            {
                double spec = 0;
                foreach (FocalElement<TElement> focal in Focals)
                {
                    if (focal.Card > 0)
                    {
                        spec += focal.Value / focal.Card;
                    }
                }
                return spec;
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the non-specificity of the mass function (for a definition, refer to "Yager, R.:
        /// Entropy and specificity in a mathematical theory of evidence, 1983").
        /// </summary>
        public double NonSpecificity
        {
            get
            {
                double nonSpec = 0;
                foreach (FocalElement<TElement> focal in Focals)
                {
                    if (focal.Card > 0)
                    {
                        nonSpec += focal.Value * Math.Log(focal.Card) / Math.Log(2);
                    }
                }
                return nonSpec;
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the discrepancy of the mass function (for a definition, refer to "J. Abellan and S. Moral,
        /// Completing a total uncertainty measure in Dempster-Shafer theory, 1999".
        /// </summary>
        public double Discrepancy
        {
            get
            {
                double disc = 0;
                foreach (FocalElement<TElement> focal in Focals)
                {
                    disc -= focal.Value * Math.Log(BetP(focal.Element)) / Math.Log(2);
                }
                return disc;
            }
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Utility methods

        /*		
         * Utility methods
         */

        /// <summary>
        /// Adds mass to the current mass function. If the given element is already a focal element, the
        /// given mass is added to the mass already associated to this element.
        /// </summary>
        /// <param name="e">The element to add mass to.</param>
        /// <param name="mass">The quantity of mass to add.</param>
        /// <exception cref="IncompatibleElementException">Thrown if the given element is not compatible with
        /// the other focal elements.</exception>
        public void AddMass(TElement e, double mass = 0)
        {
            //Checking:
            if (this.NbFocals != 0 && !this.Focals[0].IsCompatible(e))
            {
                throw new IncompatibleElementException("The given Element is not compatible with this MassFunction!");
            }
            //Adding:
            int index = this.IndexOf(e);
            if (index != -1)
            {
                this._focals[index].Value += mass;
            }
            else
            {
                _focals.Add(new FocalElement<TElement>(e, mass));
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Adds mass to the current mass function. If the given element is already a focal element, the
        /// given mass is added to the mass already associated to this element.
        /// </summary>
        /// <param name="e">The focal element to add.</param>
        /// <exception cref="IncompatibleElementException">Thrown if the given element is not compatible with
        /// the other focal elements.</exception>
        public void AddMass(FocalElement<TElement> e)
        {
            this.AddMass(e.Element, e.Value);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Removes mass to the given element. If the element was not in the focal sets, it is added and
        /// the mass is removed.
        /// Remark: You may want to clean the resulting mass function you removed mass.
        /// </summary>
        /// <param name="e">The element for which mass must be removed.</param>
        /// <param name="mass">The quantity of mass to remove.</param>
        /// <exception cref="IncompatibleElementException">Thrown if the given element is imcompatible with
        /// the focal elements of the mass function.</exception>
        public void RemoveMass(TElement e, double mass)
        {
            //Checking:
            if (this.NbFocals != 0 && !this.Focals[0].IsCompatible(e))
            {
                throw new IncompatibleElementException("The given Element is not defined on the same frame of discernment as the MassFunction!");
            }
            //Adding:
            int index = this.IndexOf(e);
            if (index != -1)
            {
                this._focals[index].Value -= mass;
            }
            else
            {
                this._focals.Add(new FocalElement<TElement>(e, mass));
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Removes mass to the given element. If that element was not a focal element, no mass is removed.
        /// </summary>
        /// <param name="e">The focal element for which mass must be removed.</param>
        /// <exception cref="IncompatibleElementException">Thrown if the given element is imcompatible with
        /// the focal elements of the mass function.</exception>
        public void RemoveMass(FocalElement<TElement> e)
        {
            this.RemoveMass(e.Element, e.Value);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Clears the focal set of every focal element.
        /// </summary>
        public void Clear()
        {
            this._focals.Clear();
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Checks if the current mass function contains the given element as a focal element.
        /// </summary>
        /// <param name="e">The element to look for.</param>
        /// <returns>Returns true if the current mass function contains the given element, false otherwise.</returns>
        public bool Contains(TElement e)
        {
            return IndexOf(e) != -1;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Checks if the current mass function contains the given focal element. The mass in the given
        /// focal element has no impact on the result, only elements themselves are compared.
        /// </summary>
        /// <param name="e">The focal element to look for.</param>
        /// <returns>Returns true if the current mass function contains the given focal element,
        /// false otherwise.</returns>
        public bool Contains(FocalElement<TElement> e)
        {
            return IndexOf(e.Element) != -1;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Checks if the current mass function contains focal elements or not.
        /// </summary>
        /// <returns>Returns true if it has no focal element, false otherwise.</returns>
        public bool IsEmpty()
        {
            return NbFocals == 0;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gives the index of the given focal element in <see cref="Focals"/>. 
        /// The mass in the given focal element has no impact on the result.
        /// </summary>
        /// <param name="e">The focal element to look for.</param>
        /// <returns>Returns the index of the given element, -1 if not present.</returns>
        public int IndexOf(FocalElement<TElement> e)
        {
            return _focals.IndexOf(e);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gives the index of the given element in <see cref="Focals"/>.
        /// </summary>
        /// <param name="e">The element to look for.</param>
        /// <returns>Returns the index of the given element, -1 if not present.</returns>
        public int IndexOf(TElement e)
        {
            for (int i = 0; i < _focals.Count; i++)
            {
                if (_focals[i].Element.Equals(e))
                {
                    return i;
                }
            }
            return -1;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Cleans the current mass function by removing all negligeable focal elements (mass &lt; PRECISION).
        /// </summary>
        public void Clean()
        {
            //Get a list of elements to remove:
            List<FocalElement<TElement>> toRemove = new List<FocalElement<TElement>>();
            foreach (FocalElement<TElement> e in Focals)
            {
                if (e.Value < PRECISION)
                {
                    toRemove.Add(e);
                }
            }
            //Remove them:
            foreach (FocalElement<TElement> e in toRemove)
            {
                _focals.Remove(e);
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Normalises the current mass function.
        /// </summary>
        /// <exception cref="EmptyMassFunctionException">Thrown if applied to an empty mass function.</exception>
        public void Normalise()
        {
            //Checking:
            if (NbFocals == 0)
            {
                throw new EmptyMassFunctionException("A MassFunction should contain at least one focal element to be normalized!");
            }
            //Normalizing:
            double sum = Sum();
            for (int i = 0; i < NbFocals; i++)
            {
                Focals[i].Value /= sum;
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gives a string representation of the mass function of the form "Mass function:\nm(e1) = mass\n..."
        /// </summary>
        /// <returns>Returns a string representing the current mass function.</returns>
        public override string ToString()
        {
            if (NbFocals == 0)
            {
                return "Mass function: void";
            }
            StringBuilder toReturn = new StringBuilder("Mass function:\n");
            int i = 0;
            foreach (FocalElement<TElement> e in Focals)
            {
                i++;
                if (i == NbFocals)
                {
                    toReturn.Append(String.Format("m({0}) = {1}", e.Element, e.Value));
                }
                else
                {
                    toReturn.Append(String.Format("m({0}) = {1}\n", e.Element, e.Value));
                }
            }
            return toReturn.ToString();
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Creates a deep copy of the current mass function.
        /// </summary>
        /// <returns>Returns a new mass function which is a copy of the current one.</returns>
        public TFunction DeepCopy()
        {
            TFunction copy = new TFunction();
            foreach (FocalElement<TElement> e in Focals)
            {
                copy.AddMass(e);
            }
            return copy;
        }

        //------------------------------------------------------------------------------------------------

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Used to enable foreach loops to be used on mass functions.
        /// </summary>
        /// <returns>Returns the next focal element of the mass function in the foreach loop.</returns>
        public IEnumerator<FocalElement<TElement>> GetEnumerator()
        {
            foreach (FocalElement<TElement> focal in Focals)
            {
                yield return focal;
            }
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Decision support methods

        /*				
         * Decision support methods
         */

        #region Decision making criteria

        /// <summary>
        /// Gets the mass associated to the given element. Strictly equivalent to
        /// <see cref="Mass"/>.
        /// </summary>
        /// <param name="e">The element to look for.</param>
        /// <returns>Returns the mass associated to the given element.</returns>
        public double M(TElement e)
        {
            int index = IndexOf(e);
            if (index != -1)
            {
                return _focals[IndexOf(e)].Value;
            }
            return 0;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the mass associated to the given element. Strictly equivalent to
        /// <see cref="M"/>.
        /// </summary>
        /// <param name="e">The element to look for.</param>
        /// <returns>Returns the mass associated to the given element.</returns>
        public double Mass(TElement e)
        {
            return M(e);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the belief/credibility associated to the given element. Strictly equivalent to
        /// <see cref="Belief"/> and <see cref="Credibility"/>.
        /// </summary>
        /// <param name="e">The element to look for.</param>
        /// <returns>Returns the belief/credibility associated to the given element.</returns>
        public double Bel(TElement e)
        {
            if (e.IsEmpty())
            {
                return 0;
            }
            double bel = 0;
            foreach (FocalElement<TElement> focal in Focals)
            {
                if (!focal.Element.IsEmpty() && focal.Element.IsASubset(e))
                {
                    bel += focal.Value;
                }
            }
            return bel;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the belief/credibility associated to the given element. Strictly equivalent to
        /// <see cref="Bel"/> and <see cref="Credibility"/>.
        /// </summary>
        /// <param name="e">The element to look for.</param>
        /// <returns>Returns the belief/credibility associated to the given element.</returns>
        public double Belief(TElement e)
        {
            return Bel(e);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the belief/credibility associated to the given element. Strictly equivalent to
        /// <see cref="Belief"/> and <see cref="Bel"/>.
        /// </summary>
        /// <param name="e">The element to look for.</param>
        /// <returns>Returns the belief/credibility associated to the given element.</returns>
        public double Credibility(TElement e)
        {
            return Bel(e);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the value of the pignistic transformation for the given element. Strictly equivalent to
        /// <see cref="PignisticTransformation"/>.
        /// </summary>
        /// <param name="e">The element to look for.</param>
        /// <returns>Returns the value of the pignistic transformation for the given element.</returns>
        public double BetP(TElement e)
        {
            if (e.IsEmpty())
            {
                return 0;
            }
            double betP = 0;
            foreach (FocalElement<TElement> focal in Focals)
            {
                if (!focal.Element.IsEmpty())
                {
                    betP += focal.Value * e.Conjunction(focal.Element).Card / focal.Element.Card;
                }
            }
            return betP;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the value of the pignistic transformation for the given element. Strictly equivalent to
        /// <see cref="BetP"/>.
        /// </summary>
        /// <param name="e">The element to look for.</param>
        /// <returns>Returns the value of the pignistic transformation for the given element.</returns>
        public double PignisticTransformation(TElement e)
        {
            return BetP(e);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the plausibility of the given element. Strictly equivalent to <see cref="Plausibility"/>.
        /// </summary>
        /// <param name="e">The element to look for.</param>
        /// <returns>Returns the plausibility associated to the given element.</returns>
        public double Pl(TElement e)
        {
            if (e.IsEmpty())
            {
                return 1;
            }
            double pl = 0;
            foreach (FocalElement<TElement> focal in Focals)
            {
                if (!e.Conjunction(focal.Element).IsEmpty())
                {
                    pl += focal.Value;
                }
            }
            return pl;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the plausibility of the given element. Strictly equivalent to <see cref="Pl"/>.
        /// </summary>
        /// <param name="e">The element to look for.</param>
        /// <returns>Returns the plausibility associated to the given element.</returns>
        public double Plausibility(TElement e)
        {
            return Pl(e);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the commonality of the given element. Strictly equivalent to <see cref="Commonality"/>.
        /// </summary>
        /// <param name="e">The element to look for.</param>
        /// <returns>Returns the commonality for the given element.</returns>
        public double Q(TElement e)
        {
            if (e.IsEmpty())
            {
                return 1;
            }
            double q = 0;
            foreach (FocalElement<TElement> focal in Focals)
            {
                if (e.IsASubset(focal.Element))
                {
                    q += focal.Value;
                }
            }
            return q;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the commonality of the given element. Strictly equivalent to <see cref="Q"/>.
        /// </summary>
        /// <param name="e">The element to look for.</param>
        /// <returns>Returns the commonality for the given element.</returns>
        public double Commonality(TElement e)
        {
            return Q(e);
        }

        #endregion

        //------------------------------------------------------------------------------------------------

        #region GetMax/Min

        /// <summary>
        /// <para>Returns the focal elements that have the maximum value for the given criterion and which respect
        /// the maximum cardinal constraint given. It compromises between the criterion and the maximum cardinality.
        /// For more details, refer to "M. Dominici et al., Experiences in managing uncertainty and ignorance in a 
        /// lightly instrumented smart home, 2012".</para>
        /// <para>WARNING: DO NOT CALL THIS METHOD WITH TOO MANY ELEMENTS IN THE GIVEN SET, IT WILL HARM YOUR POOR
        /// COMPUTER.</para>
        /// </summary>
        /// <param name="f">The criterion function (typically M, Bel, BetP, Pl or Q).</param>
        /// <param name="maxCard">The maximum cardinality the returned maxima may have.</param>
        /// <param name="set">The set of elements to look in (typically the powerset).</param>
        /// <returns>Returns the list of found maxima as a list of FocalElements.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the maxCard argument is null or negative.</exception>
        /// <exception cref="EmptyMassFunctionException">Thrown if the mass function does not contain any focal element.</exception>
        /// <exception cref="IncompatibleSetException">Thrown if the given set is incompatible with the focal elements
        /// of the current mass function.</exception>
        public List<FocalElement<TElement>> GetMax(CriterionFunction f, int maxCard, Set<TElement> set)
        {
            if (maxCard <= 0)
            {
                throw new ArgumentOutOfRangeException("maxCard. The maximum cardinality cannot be null or negative!");
            }
            if (NbFocals <= 0)
            {
                throw new EmptyMassFunctionException("It is impossible to get any max on an empty MassFunction!");
            }
            if (!set.IsCompatible(this.Focals[0].Element))
            {
                throw new IncompatibleSetException("The given set does not correspond to the frame of discernment on which the MassFunction is defined!");
            }

            List<FocalElement<TElement>> maxima = new List<FocalElement<TElement>>();
            foreach (TElement e in set)
            {
                if (maxima.Count == 0 && f(e) != 0 && 0 < e.Card && e.Card <= maxCard)
                {
                    maxima.Add(new FocalElement<TElement>(e, f(e)));
                }
                else if (0 < e.Card && e.Card <= maxCard && maxima.Count != 0 && f(e) == maxima[0].Value)
                {
                    maxima.Add(new FocalElement<TElement>(e, f(e)));
                }
                else if (0 < e.Card && e.Card <= maxCard && maxima.Count != 0 && f(e) > maxima[0].Value)
                {
                    maxima.Clear();
                    maxima.Add(new FocalElement<TElement>(e, f(e)));
                }
            }
            return maxima;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// <para>Returns the focal elements that have the minimum value for the given criterion and which respect
        /// the maximum cardinal constraint given. It compromises between the criterion and the maximum cardinality.
        /// For more details, refer to "M. Dominici et al., Experiences in managing uncertainty and ignorance in a 
        /// lightly instrumented smart home, 2012".</para>
        /// <para>DISCLAIMER: This function may be completely useless but who knows?!</para>
        /// <para>WARNING: DO NOT CALL THIS METHOD WITH TOO MANY ELEMENTS IN THE GIVEN SET, IT WILL HARM YOUR POOR
        /// COMPUTER.</para>
        /// </summary>
        /// <param name="f">The criterion function (typically M, Bel, BetP, Pl or Q).</param>
        /// <param name="maxCard">The maximum cardinality the returned maxima may have.</param>
        /// <param name="set">The set of elements to look in (typically the powerset).</param>
        /// <returns>Returns the list of found maxima as a list of FocalElements.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the maxCard argument is null or negative.</exception>
        /// <exception cref="EmptyMassFunctionException">Thrown if the mass function does not contain any focal element.</exception>
        /// <exception cref="IncompatibleSetException">Thrown if the given set is incompatible with the focal elements
        /// of the current mass function.</exception>
        public List<FocalElement<TElement>> GetMin(CriterionFunction f, int maxCard, Set<TElement> set)
        {
            if (maxCard <= 0)
            {
                throw new ArgumentOutOfRangeException("maxCard. The maximum cardinality cannot be null or negative!");
            }
            if (NbFocals <= 0)
            {
                throw new EmptyMassFunctionException("It is impossible to get any min on an empty MassFunction!");
            }
            if (!set.IsCompatible(this.Focals[0].Element))
            {
                throw new IncompatibleSetException("The given set does not correspond to the frame of discernment on which the MassFunction is defined!");
            }
            List<FocalElement<TElement>> minima = new List<FocalElement<TElement>>();
            foreach (TElement e in set)
            {
                if (minima.Count == 0 && f(e) != 0 && 0 < e.Card && e.Card <= maxCard)
                {
                    minima.Add(new FocalElement<TElement>(e, f(e)));
                    continue;
                }
                else if (0 < e.Card && e.Card <= maxCard && minima.Count != 0 && f(e) == minima[0].Value)
                {
                    minima.Add(new FocalElement<TElement>(e, f(e)));
                }
                else if (0 < e.Card && e.Card <= maxCard && minima.Count != 0 && f(e) < minima[0].Value)
                {
                    minima.Clear();
                    minima.Add(new FocalElement<TElement>(e, f(e)));
                }
            }
            return minima;
        }

        #endregion

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Discounting/Conditioning methods

        /*				
         * Discounting/Conditioning methods
         */

        /// <summary>
        /// Equivalent to the discounting but instead of transferring mass to the total ignorance,
        /// it transfers it to the empty set. This does not modify the current mass function!
        /// </summary>
        /// <param name="alpha">The weakening factor (the percentage of mass that will be
        /// lost by all focal elements).</param>
        /// <returns>Returns a new mass function corresponding to the current one weakened.</returns>
        /// <exception cref="EmptyMassFunctionException">Thrown if applied to an empty mass function.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the weakening factor does not respect 
        /// 0 &lt; alpha &lt; 1.</exception>
        public TFunction Weakening(double alpha)
        {
            //Checking arguments:
            if (NbFocals == 0)
            {
                throw new EmptyMassFunctionException("A MassFunction should contain at least one focal element to be weakened!");
            }
            if (0 > alpha || alpha > 1)
            {
                throw new ArgumentOutOfRangeException("alpha. The weakening factor should respect 0 <= alpha <= 1!");
            }
            //Weakening:
            TFunction weakened = new TFunction();
            foreach (FocalElement<TElement> e in Focals)
            {
                weakened.AddMass(e.Element, e.Value * (1 - alpha));
            }
            weakened.AddMass(Focals[0].Element.GetEmptyElement(), alpha);
            return weakened;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the discounted version of the current mass function (this does not modify the current
        /// mass function). A part of the mass of all focal elements is transferred to the total ignorance.
        /// </summary>
        /// <param name="alpha">The discounting factor (the percentage of mass that will be lost
        /// by all focal elements).</param>
        /// <returns>Returns a new mass function corresponding to the current one discounted.</returns>
        /// <exception cref="EmptyMassFunctionException">Thrown if applied to an empty mass function.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the discounting factor does not respect 
        /// 0 &lt; alpha &lt; 1.</exception>
        public TFunction Discounting(double alpha)
        {
            //Checking arguments:
            if (NbFocals == 0)
            {
                throw new EmptyMassFunctionException("A MassFunction should contain at least one focal element to be discounted!");
            }
            if (0 > alpha || alpha > 1)
            {
                throw new ArgumentOutOfRangeException("alpha. The discounting factor should respect 0 <= alpha <= 1!");
            }
            //Discounting:
            TFunction discounted = new TFunction();
            foreach (FocalElement<TElement> e in Focals)
            {
                discounted.AddMass(e.Element, e.Value * (1 - alpha));
            }
            discounted.AddMass(Focals[0].Element.GetCompleteElement(), alpha);
            return discounted;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Conditions a mass function by the given element. For a definition, refer to "P. Smets,
        /// The transferable belief model for belief representation, 1999".
        /// </summary>
        /// <param name="e">The element to use to condition the mass function.</param>
        /// <returns>Returns a new mass function corresponding to the current mass function conditioned.</returns>
        /// <exception cref="EmptyMassFunctionException">Thrown if the method is called on an empty 
        /// mass function.</exception>
        /// <exception cref="IncompatibleElementException">Thrown if the given element seems to be defined
        /// on a different frame of discernement than the one on which the mass function is defined.</exception>
        /// <exception cref="EmptyElementException">Thrown if the method is called with the empty set as
        /// a parameter. The conditioning is not defined for the empty set.</exception>
        public TFunction Conditioning(TElement e)
        {
            //Checking arguments:
            if (NbFocals == 0)
            {
                throw new EmptyMassFunctionException("A MassFunction should contain at least one focal element to be conditioned!");
            }
            if (!_focals[0].IsCompatible(e))
            {
                throw new IncompatibleElementException("The given Element is not defined on the same frame of discernment as the MassFunction!");
            }
            if (e.IsEmpty())
            {
                throw new EmptyElementException("A MassFunction cannot be conditioned by the empty set!");
            }
            //The conditioning is equivalent to the Smets combination with a categorical mass function
            //with for unique focal element the element by which we would like to condition:
            TFunction conditioning = new TFunction();
            conditioning.AddMass(new FocalElement<TElement>(e, 1));
            return this.CombinationSmets(conditioning);
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Comparison methods

        /*				
         * Comparison methods
         */

        /// <summary>
        /// Does the difference between the current mass function and the given one. Does not give
        /// a proper mass function! Used only to compute the distance.
        /// </summary>
        /// <param name="m">The mass function to do the difference with.</param>
        /// <returns>Returns a new "mass function" which is the difference between the current
        /// one and the given one.</returns>
        /// <exception cref="EmptyMassFunctionException">Thrown if the current or the given mass function is empty.</exception>
        /// <exception cref="IncompatibleMassFunctionException">Thrown if the current mass function and the given one
        /// are not compatible.</exception>
        private TFunction Difference(TFunction m)
        {
            //Checking:
            if (NbFocals == 0 || m.NbFocals == 0)
            {
                throw new EmptyMassFunctionException("A MassFunction should contain at least one focal element to compute a difference!");
            }
            if (!m.Focals[0].IsCompatible(this.Focals[0]))
            {
                throw new IncompatibleMassFunctionException("Both MassFunctions are not defined on the same frame of discernment!");
            }
            //Difference:
            TFunction diff = new TFunction();
            foreach (FocalElement<TElement> e in this)
            {
                diff.AddMass(e);
            }
            foreach (FocalElement<TElement> e in m)
            {
                diff.RemoveMass(e);
            }
            return diff;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the classic distance between the current mass function and the given one. For a definition,
        /// refer to "A. Jousselme et al, A new distance between two bodies of evidence, 2001".
        /// </summary>
        /// <param name="m">The mass function from which the distance will be computed.</param>
        /// <returns>Returns the distance between the current mass function and the given one.</returns>
        /// <exception cref="EmptyMassFunctionException">Thrown if the current mass function or the given
        /// one is empty.</exception>
        /// <exception cref="IncompatibleMassFunctionException">Thrown if the given mass function is not
        /// compatible with the current one.</exception>
        private double Distance(TFunction m)
        {
            //Checking:
            if (NbFocals == 0 || m.NbFocals == 0)
            {
                throw new EmptyMassFunctionException("A MassFunction should contain at least one focal element to compute a distance!");
            }
            if (!m.Focals[0].IsCompatible(this.Focals[0]))
            {
                throw new IncompatibleMassFunctionException("MassFunctions should be defined on the same frame of discernment to compute a distance!");
            }
            //Compute the matrix:
            TFunction difference = this.Difference(m);
            double[,] matrix = new double[difference.NbFocals, difference.NbFocals];
            for (int i = 0; i < difference.NbFocals; i++)
            {
                for (int j = 0; j < difference.NbFocals; j++)
                {
                    if (!difference.Focals[i].Element.IsEmpty() || !difference.Focals[j].Element.IsEmpty())
                    {
                        TElement conj = difference.Focals[i].Element.Conjunction(difference.Focals[j].Element);
                        TElement disj = difference.Focals[i].Element.Disjunction(difference.Focals[j].Element);
                        matrix[i, j] = (double)(conj.Card) / disj.Card;
                    }
                    else
                    {
                        matrix[i, j] = 1;
                    }
                }
            }
            //Compute the distance:
            double distance = 0;
            double[] temp = new double[difference.NbFocals];
            for (int i = 0; i < difference.NbFocals; i++)
            {
                temp[i] = 0;
                for (int j = 0; j < difference.NbFocals; j++)
                {
                    temp[i] += difference.Focals[j].Value * matrix[i, j];
                }
            }
            for (int i = 0; i < difference.NbFocals; i++)
            {
                distance += temp[i] * difference.Focals[i].Value;
            }
            return Math.Sqrt(0.5 * distance);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the classic distance between the current mass function and a set of given ones. 
        /// For a definition, refer to "A. Jousselme et al, A new distance between two bodies of evidence, 2001".
        /// </summary>
        /// <param name="m">The mass functions from which the distance will be computed.</param>
        /// <returns>Returns the distance between the current mass function and the given ones.</returns>
        /// <exception cref="EmptyMassFunctionException">Thrown if the current mass function or one of the given
        /// ones is empty.</exception>
        /// <exception cref="IncompatibleMassFunctionException">Thrown if at least one of the given mass functions is not
        /// compatible with the current one.</exception>
        public double Distance(params TFunction[] m)
        {
            //Checking:
            if (NbFocals == 0)
            {
                throw new EmptyMassFunctionException("A MassFunction should contain at least one focal element to compute a distance!");
            }
            foreach (TFunction mass in m)
            {
                if (mass.NbFocals == 0)
                {
                    throw new EmptyMassFunctionException("A MassFunction should contain at least one focal element to compute a distance!");
                }
                if (!mass.Focals[0].IsCompatible(this.Focals[0]))
                {
                    throw new IncompatibleMassFunctionException("All MassFunctions are not defined on the same frame of discernment!");
                }
            }
            //Distance:
            double distance = 0;
            foreach (TFunction mass in m)
            {
                distance += this.Distance(mass);
            }
            return distance / m.Length;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the similarity between the current mass function and the given one. It can be 
        /// representative of the agreement between two bodies of evidence. For a definition, refer to
        /// "L.-Z. Chen, A new fusion approach based on distance of evidence, 2005".
        /// </summary>
        /// <param name="m">The mass function to compare to.</param>
        /// <returns>Returns the similarity between the current mass function and the given one.</returns>
        /// <exception cref="EmptyMassFunctionException">Thrown if the current mass function function
        /// or the given one is empty.</exception>
        /// <exception cref="IncompatibleMassFunctionException">Thrown if the current mass function and
        /// the given one are not compatible.</exception>
        public double Similarity(TFunction m)
        {
            //Checking:
            if (NbFocals == 0 || m.NbFocals == 0)
            {
                throw new EmptyMassFunctionException("A MassFunction should contain at least one focal element to compute a similarity!");
            }
            if (!m.Focals[0].IsCompatible(this.Focals[0]))
            {
                throw new IncompatibleMassFunctionException("Both MassFunctions are not defined on the same frame of discernment!");
            }
            //Similarity:
            return (0.5 * (Math.Cos(Math.PI * this.Distance(m) + 1)));
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the support for the current mass function given a set of mass functions.
        /// This value is the sum of the <see cref="Similarity"/> of the current mass function 
        /// with the functions of the given set.
        /// </summary>
        /// <param name="m">A set of mass functions to compute the support from.</param>
        /// <returns>Returns the support value of the current mass function given a set of mass functions.</returns>
        public double Support(params TFunction[] m)
        {
            //Checking:
            if (NbFocals == 0)
            {
                throw new EmptyMassFunctionException("A MassFunction should contain at least one focal element to compute a support!");
            }
            foreach (TFunction mass in m)
            {
                if (mass.NbFocals == 0)
                {
                    throw new EmptyMassFunctionException("A MassFunction should contain at least one focal element to compute a support!");
                }
                if (!mass.Focals[0].IsCompatible(this.Focals[0]))
                {
                    throw new IncompatibleMassFunctionException("All MassFunctions are not defined on the same frame of discernment!");
                }
            }
            //Support:
            double support = 0;
            foreach (TFunction mass in m)
            {
                support += this.Similarity(mass);
            }
            return support;
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Validation methods

        /*				
         * Tests methods
         */

        /// <summary>
        /// Checks is the sum of masses equals 1 knowing the precision.
        /// </summary>
        /// <returns>Returns true if the sum equals 1, false otherwise.</returns>
        public bool HasAValidSum()
        {
            double sum = Sum();
            return 1 - PRECISION < sum && sum < 1 + PRECISION;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Checks if all the masses are comprised between 0 and 1.
        /// </summary>
        /// <returns>Returns true if all masses are between 0 and 1, false otherwise.</returns>
        public bool HasValidValues()
        {
            foreach (FocalElement<TElement> e in Focals)
            {
                if (0 > e.Value || e.Value > 1)
                {
                    return false;
                }
            }
            return true;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the sum of masses.
        /// </summary>
        /// <returns>Returns the sum of masses.</returns>
        protected double Sum()
        {
            double sum = 0;
            foreach (FocalElement<TElement> e in Focals)
            {
                sum += e.Value;
            }
            return sum;
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Combination rules

        /*				
         * Combination methods
         */

        /// <summary>
        /// Private method to check if the current mass function and the given ones are valid and
        /// compatible.
        /// </summary>
        /// <param name="m">The set of mass functions to check the validity with.</param>
        /// <exception cref="ArgumentNullException">Thrown if no mass function is passed.</exception>
        /// <exception cref="EmptyMassFunctionException">Thrown if the current mass function or at least one
        /// of the given ones is empty.</exception>
        /// <exception cref="IncompatibleMassFunctionException">Thrown if at least one of the given mass
        /// functions is incompatible with the current one.</exception>
        private void CheckCombinationValidity(params TFunction[] m)
        {
            if (m.Length == 0)
            {
                throw new ArgumentNullException("You should give at least one MassFunction to combine!");
            }
            foreach (TFunction mass in m)
            {
                if (mass.NbFocals == 0)
                {
                    throw new EmptyMassFunctionException("Empty MassFunctions cannot be combined!");
                }
            }
            if (this.NbFocals == 0)
            {
                throw new EmptyMassFunctionException("Empty MassFunctions cannot be combined!");
            }
            foreach (TFunction mass in m)
            {
                if (!mass.Focals[0].IsCompatible(this.Focals[0]))
                {
                    throw new IncompatibleMassFunctionException("MassFunctions must be defined on the same frame of discernment to be combined!");
                }
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Combines the current mass function with the given ones using the unnormalised Dempster's rule
        /// of combination. For a definition, refer to ¨P. Smets: The transferable belief model, 1994".
        /// </summary>
        /// <param name="m">The mass functions to combine with.</param>
        /// <returns>Returns a new mass function which is the combination of the current one with
        /// the given ones.</returns>
        /// <exception cref="ArgumentNullException">Thrown if no mass function is passed.</exception>
        /// <exception cref="EmptyMassFunctionException">Thrown if the current mass function or at least one
        /// of the given ones is empty.</exception>
        /// <exception cref="IncompatibleMassFunctionException">Thrown if at least one of the given mass
        /// functions is incompatible with the current one.</exception>
        public TFunction CombinationSmets(params TFunction[] m)
        {
            this.CheckCombinationValidity(m);
            TFunction combined = new TFunction();
            foreach (FocalElement<TElement> e1 in this.Focals)
            {
                foreach (FocalElement<TElement> e2 in m[0].Focals)
                {
                    combined.AddMass(e1.Element.Conjunction(e2.Element), e1.Value * e2.Value);
                }
            }
            if (m.Length == 1)
            {
                return combined;
            }
            else
            {
                TFunction[] newM = new TFunction[m.Length - 1];
                for (int i = 0; i < m.Length - 1; i++)
                {
                    newM[i] = m[i + 1];
                }
                return combined.CombinationSmets(newM);
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Combines the current mass function with the given ones using the Dempster's rule
        /// of combination.
        /// </summary>
        /// <param name="m">The mass functions to combine with.</param>
        /// <returns>Returns a new mass function which is the combination of the current one with
        /// the given ones.</returns>
        /// <exception cref="ArgumentNullException">Thrown if no mass function is passed.</exception>
        /// <exception cref="EmptyMassFunctionException">Thrown if the current mass function or at least one
        /// of the given ones is empty.</exception>
        /// <exception cref="IncompatibleMassFunctionException">Thrown if at least one of the given mass
        /// functions is incompatible with the current one.</exception>
        /// <exception cref="CombinationNotDefinedException">Thrown in case of total conflict where the 
        /// Dempster's rule of combination is not defined (division by 0).</exception>
        public TFunction CombinationDempster(params TFunction[] m)
        {
            this.CheckCombinationValidity(m);
            TFunction combined = new TFunction();
            foreach (FocalElement<TElement> e1 in Focals)
            {
                foreach (FocalElement<TElement> e2 in m[0].Focals)
                {
                    combined.AddMass(e1.Element.Conjunction(e2.Element), e1.Value * e2.Value);
                }
            }
            if (this.M(Focals[0].Element.GetEmptyElement()) == 1)
            {
                throw new CombinationNotDefinedException("MassFunctions are in total conflict, the Dempster's rule is thus not defined!");
            }
            combined._focals.Remove(new FocalElement<TElement>(Focals[0].Element.GetEmptyElement()));
            combined.Normalise();
            if (m.Length == 1)
            {
                return combined;
            }
            else
            {
                TFunction[] newM = new TFunction[m.Length - 1];
                for (int i = 0; i < m.Length - 1; i++)
                {
                    newM[i] = m[i + 1];
                }
                return combined.CombinationDempster(newM);
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Combines the current mass function with the given ones using the disjunctive rule of combination.
        /// For a definition, refer to "P. Smets, Belief functions: The disjunctive rule of combination and 
        /// the generalized bayesian theorem, 1993".
        /// </summary>
        /// <param name="m">The mass functions to combine with.</param>
        /// <returns>Returns a new mass function which is the combination of the current one with
        /// the given ones.</returns>
        /// <exception cref="ArgumentNullException">Thrown if no mass function is passed.</exception>
        /// <exception cref="EmptyMassFunctionException">Thrown if the current mass function or at least one
        /// of the given ones is empty.</exception>
        /// <exception cref="IncompatibleMassFunctionException">Thrown if at least one of the given mass
        /// functions is incompatible with the current one.</exception>
        public TFunction CombinationDisjunctive(params TFunction[] m)
        {
            this.CheckCombinationValidity(m);
            TFunction combined = new TFunction();
            foreach (FocalElement<TElement> e1 in this.Focals)
            {
                foreach (FocalElement<TElement> e2 in m[0].Focals)
                {
                    combined.AddMass(e1.Element.Disjunction(e2.Element), e1.Value * e2.Value);
                }
            }
            //If there is more than 1 mass function to combined:
            if (m.Length == 1)
            {
                return combined;
            }
            else
            {
                TFunction[] newM = new TFunction[m.Length - 1];
                for (int i = 0; i < m.Length - 1; i++)
                {
                    newM[i] = m[i + 1];
                }
                return combined.CombinationDisjunctive(newM);
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Combines the current mass function with the given ones using the Murphy's rule of combination.
        /// For a definition, refer to "C. K. Murphy: Combining belief functions when evidence conflicts, 2000".
        /// </summary>
        /// <param name="m">The mass functions to combine with.</param>
        /// <returns>Returns a new mass function which is the combination of the current one with
        /// the given ones.</returns>
        /// <exception cref="ArgumentNullException">Thrown if no mass function is passed.</exception>
        /// <exception cref="EmptyMassFunctionException">Thrown if the current mass function or at least one
        /// of the given ones is empty.</exception>
        /// <exception cref="IncompatibleMassFunctionException">Thrown if at least one of the given mass
        /// functions is incompatible with the current one.</exception>
        public TFunction CombinationMurphy(params TFunction[] m)
        {
            this.CheckCombinationValidity(m);
            TFunction average = new TFunction();
            foreach (FocalElement<TElement> e in Focals)
            {
                average.AddMass(e);
            }
            average = average.CombinationAverage(m);
            TFunction combined = average.CombinationDempster(average);
            for (int i = 0; i < m.Length - 1; i++)
            {
                combined = combined.CombinationDempster(average);
            }
            return combined;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Combines the current mass function with the given ones by simply averaging them altogether.
        /// </summary>
        /// <param name="m">The mass functions to combine with.</param>
        /// <returns>Returns a new mass function which is the combination of the current one with
        /// the given ones.</returns>
        /// <exception cref="ArgumentNullException">Thrown if no mass function is passed.</exception>
        /// <exception cref="EmptyMassFunctionException">Thrown if the current mass function or at least one
        /// of the given ones is empty.</exception>
        /// <exception cref="IncompatibleMassFunctionException">Thrown if at least one of the given mass
        /// functions is incompatible with the current one.</exception>
        public TFunction CombinationAverage(params TFunction[] m)
        {
            this.CheckCombinationValidity(m);
            TFunction combined = new TFunction();
            foreach (FocalElement<TElement> e in Focals)
            {
                combined.AddMass(e);
            }
            foreach (TFunction mass in m)
            {
                foreach (FocalElement<TElement> e in mass.Focals)
                {
                    combined.AddMass(e);
                }
                Console.WriteLine();
            }
            foreach (FocalElement<TElement> e in combined)
            {
                e.Value /= m.Length + 1;
            }
            return combined;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Combines the current mass function with the given ones using the Yager's rule of combination.
        /// For a definition, refer to "R. Yager: On the Dempster-Shafer framework and new combination rules, 1987".
        /// </summary>
        /// <param name="m">The mass functions to combine with.</param>
        /// <returns>Returns a new mass function which is the combination of the current one with
        /// the given ones.</returns>
        /// <exception cref="ArgumentNullException">Thrown if no mass function is passed.</exception>
        /// <exception cref="EmptyMassFunctionException">Thrown if the current mass function or at least one
        /// of the given ones is empty.</exception>
        /// <exception cref="IncompatibleMassFunctionException">Thrown if at least one of the given mass
        /// functions is incompatible with the current one.</exception>
        public TFunction CombinationYager(params TFunction[] m)
        {
            this.CheckCombinationValidity(m);
            TFunction combined = new TFunction();
            foreach (FocalElement<TElement> e1 in Focals)
            {
                foreach (FocalElement<TElement> e2 in m[0].Focals)
                {
                    combined.AddMass(e1.Element.Conjunction(e2.Element), e1.Value * e2.Value);
                }
            }
            TElement emptySet = Focals[0].Element.GetEmptyElement();
            if (combined.Contains(emptySet))
            {
                combined.AddMass(Focals[0].Element.GetCompleteElement(), combined.M(emptySet));
                combined._focals.Remove(new FocalElement<TElement>(emptySet));
            }
            if (m.Length == 1)
            {
                return combined;
            }
            else
            {
                TFunction[] newM = new TFunction[m.Length - 1];
                for (int i = 0; i < m.Length - 1; i++)
                {
                    newM[i] = m[i + 1];
                }
                return combined.CombinationYager(newM);
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// <para>Combines the current mass function with the given ones using the Dubois and Prade's rule of combination.
        /// For a definition, refer to "D. Dubois and H. Prade: Representation and Combination 
        /// of Uncertainty with Belief Functions and Possibility Measures, 1988".</para>
        /// <para>WARNING: DO NOT COMBINE TOO MANY MASS FUNCTIONS WITH TOO MANY FOCAL ELEMENTS, IT MAY HARM
        /// YOUR POOR COMPUTER.</para>
        /// </summary>
        /// <param name="m">The mass functions to combine with.</param>
        /// <returns>Returns a new mass function which is the combination of the current one with
        /// the given ones.</returns>
        /// <exception cref="ArgumentNullException">Thrown if no mass function is passed.</exception>
        /// <exception cref="EmptyMassFunctionException">Thrown if the current mass function or at least one
        /// of the given ones is empty.</exception>
        /// <exception cref="IncompatibleMassFunctionException">Thrown if at least one of the given mass
        /// functions is incompatible with the current one.</exception>
        public TFunction CombinationDuboisPrade(params TFunction[] m)
        {
            this.CheckCombinationValidity(m);
            //Get a new list of masses:
            TFunction[] masses = new TFunction[m.Length + 1];
            masses[0] = (TFunction)this;
            for (int i = 0; i < m.Length; i++)
            {
                masses[i + 1] = m[i];
            }
            //Get the tree of possible conjunctions:
            List<List<TElement>> tree = GetTreeOfFocals(masses);
            //Combine:
            TFunction combined = new TFunction();
            foreach (List<TElement> list in tree)
            {
                //Compute mass:
                double massToAdd = 1;
                for (int i = 0; i < list.Count; i++)
                {
                    massToAdd *= masses[i].M(list[i]);
                }
                TElement conj = list[0];
                for (int i = 1; i < list.Count; i++)
                {
                    conj = conj.Conjunction(list[i]);
                }
                //If empty:
                if (conj.IsEmpty())
                {
                    TElement disj = list[0];
                    for (int i = 1; i < list.Count; i++)
                    {
                        disj = disj.Disjunction(list[i]);
                    }
                    combined.AddMass(disj, massToAdd);
                }
                else
                {
                    combined.AddMass(conj, massToAdd);
                }

            }
            return combined;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Combines the current mass function with the given ones using the Chen's rule of combination.
        /// For a definition, please refer to "L.-Z. Chen: A new fusion approach based on distance of evidences, 2005".
        /// </summary>
        /// <param name="m">The mass functions to combine with.</param>
        /// <returns>Returns a new mass function which is the combination of the current one with
        /// the given ones.</returns>
        /// <exception cref="ArgumentNullException">Thrown if no mass function is passed.</exception>
        /// <exception cref="EmptyMassFunctionException">Thrown if the current mass function or at least one
        /// of the given ones is empty.</exception>
        /// <exception cref="IncompatibleMassFunctionException">Thrown if at least one of the given mass
        /// functions is incompatible with the current one.</exception>
        public TFunction CombinationChen(params TFunction[] m)
        {
            this.CheckCombinationValidity(m);
            //Compute credibility:
            TFunction[] masses = new TFunction[m.Length + 1];
            masses[0] = (TFunction)this;
            for (int i = 0; i < m.Length; i++)
            {
                masses[i + 1] = m[i];
            }
            double[] supports = new double[m.Length + 1];
            double supportSum = 0;
            for (int i = 0; i < m.Length + 1; i++)
            {
                supports[i] = masses[i].Support(masses) - 1;
                supportSum += supports[i];
            }
            double[] credibility = new double[m.Length + 1];
            for (int i = 0; i < m.Length + 1; i++)
            {
                credibility[i] = supports[i] / supportSum;
            }
            //Add the masses:
            TFunction beforeDempster = new TFunction();
            for (int i = 0; i < m.Length + 1; i++)
            {
                foreach (FocalElement<TElement> e in masses[i].Focals)
                {
                    beforeDempster.AddMass(e.Element, e.Value * credibility[i]);
                }
            }
            //N-1 Dempster combinations
            TFunction combined = beforeDempster.CombinationDempster(beforeDempster);
            for (int i = 0; i < m.Length - 1; i++)
            {
                combined = combined.CombinationDempster(beforeDempster);
            }
            return combined;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Combines the current mass function with the given ones using the selected combination rule.
        /// </summary>
        /// <param name="rule">The combination rule to use.</param>
        /// <param name="m">The mass functions to combine the current one with.</param>
        /// <returns>Returns a new mass function which is the combination of the current one with
        /// the given ones.</returns>
        /// <exception cref="ArgumentNullException">Thrown if no mass function is passed.</exception>
        /// <exception cref="EmptyMassFunctionException">Thrown if the current mass function or at least one
        /// of the given ones is empty.</exception>
        /// <exception cref="IncompatibleMassFunctionException">Thrown if at least one of the given mass
        /// functions is incompatible with the current one.</exception>
        /// <exception cref="CombinationNotDefinedException">Thrown in case of total conflict where the 
        /// Dempster's rule of combination is not defined (division by 0).</exception>
        /// <exception cref="ArgumentOutOfRangeException">If a non-implemented combination rule is required.</exception>
        public TFunction Combination(CombinationRules rule, params TFunction[] m)
        {
            switch (rule)
            {
                case CombinationRules.COMBINATION_DEMPSTER:
                    return this.CombinationDempster(m);
                case CombinationRules.COMBINATION_SMETS:
                    return this.CombinationSmets(m);
                case CombinationRules.COMBINATION_DISJUNCTIVE:
                    return this.CombinationDisjunctive(m);
                case CombinationRules.COMBINATION_YAGER:
                    return this.CombinationYager(m);
                case CombinationRules.COMBINATION_DUBOISPRADE:
                    return this.CombinationDuboisPrade(m);
                case CombinationRules.COMBINATION_AVERAGE:
                    return this.CombinationAverage(m);
                case CombinationRules.COMBINATION_MURPHY:
                    return this.CombinationMurphy(m);
                case CombinationRules.COMBINATION_CHEN:
                    return this.CombinationChen(m);
                default:
                    throw new ArgumentOutOfRangeException("rule. The given rule is not defined!");
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the auto-conflict value of the given degree. For a definition, see "A. Martin, 
        /// Modelisation et gestion du conflit dans la theorie des fonctions de croyance (French)".
        /// </summary>
        /// <param name="degree">The degree of auto-conflict wanted.</param>
        /// <returns>Returns the auto-conflict of the given degree.</returns>
        public double AutoConflict(int degree)
        {
            if (degree <= 0)
            {
                throw new ArgumentOutOfRangeException("degree. The degree of auto-conflict cannot be null or negative!");
            }
            if (NbFocals == 0)
            {
                throw new EmptyMassFunctionException("Empty MassFunctions cannot be combined!");
            }
            TFunction m = new TFunction();
            m._focals = new List<FocalElement<TElement>>(this.Focals);
            for (int i = 0; i < degree; i++)
            {
                m = m.CombinationSmets((TFunction)this);
            }
            return m.M(Focals[0].Element.GetEmptyElement());
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        /*		
         * Static methods
         */

        #region Tree of focals

        /// <summary>
        /// <para>Returns the tree of focal elements (all possible combination of focal elements) for the given
        /// set of mass functions. Used in the Dubois and Prade's combination rule.</para>
        /// <para>WARNING: DO NOT CALL THIS METHOD WITH TOO MANY MASS FUNCTIONS WITH TOO MANY FOCAL ELEMENTS OR
        /// IT WILL HARM YOUR POOR COMPUTER.</para>
        /// </summary>
        /// <param name="m">The mass functions to look in.</param>
        /// <returns>Returns a list of lists of elements.</returns>
        public static List<List<TElement>> GetTreeOfFocals(params TFunction[] m)
        {
            return GetTreeOfFocals(new List<List<TElement>>(), m);
        }

        //------------------------------------------------------------------------------------------------

        private static List<List<TElement>> GetTreeOfFocals(List<List<TElement>> tree, params TFunction[] m)
        {
            //Adding the focals to the tree:
            if (tree.Count == 0)
            {
                //New tree:
                foreach (FocalElement<TElement> e in m[0].Focals)
                {
                    List<TElement> l = new List<TElement>();
                    l.Add(e.Element);
                    tree.Add(l);
                }
            }
            else
            {
                //Create an ordered list of focals:
                List<TElement> focals = new List<TElement>();
                foreach (FocalElement<TElement> e in m[0].Focals)
                {
                    focals.Add(e.Element);
                }
                //Duplicate lists:
                List<List<TElement>> treeBis = new List<List<TElement>>(tree);
                foreach (List<TElement> list in treeBis)
                {
                    int index = tree.IndexOf(list, 0);
                    for (int i = 0; i < focals.Count - 1; i++)
                    {
                        tree.Insert(index, new List<TElement>(list));
                    }
                }
                //Complete them:
                int k = 0;
                foreach (List<TElement> list in tree)
                {
                    list.Add(focals[k]);
                    k = (k + 1) % focals.Count;
                }
            }
            //Return:
            if (m.Length == 1)
            {
                return tree;
            }
            else
            {
                TFunction[] newM = new TFunction[m.Length - 1];
                for (int i = 0; i < m.Length - 1; i++)
                {
                    newM[i] = m[i + 1];
                }
                return GetTreeOfFocals(tree, newM);
            }
        }

        #endregion

        //------------------------------------------------------------------------------------------------

        #region Static combination rules

        /// <summary>
        /// Combines the given mass functions using the selected combination rule.
        /// </summary>
        /// <param name="rule">The combination rule to use.</param>
        /// <param name="m">The mass functions to combine.</param>
        /// <returns>Returns a new mass function which is the combination of the given mass functions.</returns>
        /// <exception cref="EmptyMassFunctionException">Thrown if at least one
        /// of the given mass functions is empty.</exception>
        /// <exception cref="IncompatibleMassFunctionException">Thrown if at least one of the given mass
        /// functions is incompatible with the others.</exception>
        /// <exception cref="CombinationNotDefinedException">Thrown in case of total conflict where the 
        /// Dempster's rule of combination is not defined (division by 0).</exception>
        /// <exception cref="ArgumentOutOfRangeException">If a non-implemented combination rule is required.</exception>
        public static TFunction StaticCombination(CombinationRules rule, params TFunction[] m)
        {
            if (m.Length < 2)
            {
                throw new NotEnoughMassFunctionsException("You should give at least two MassFunctions to combine!");
            }
            switch (rule)
            {
                case CombinationRules.COMBINATION_DEMPSTER:
                    return AMassFunction<TFunction, TElement>.StaticCombinationDempster(m);
                case CombinationRules.COMBINATION_SMETS:
                    return AMassFunction<TFunction, TElement>.StaticCombinationSmets(m);
                case CombinationRules.COMBINATION_YAGER:
                    return AMassFunction<TFunction, TElement>.StaticCombinationYager(m);
                case CombinationRules.COMBINATION_DUBOISPRADE:
                    return AMassFunction<TFunction, TElement>.StaticCombinationDuboisPrade(m);
                case CombinationRules.COMBINATION_AVERAGE:
                    return AMassFunction<TFunction, TElement>.StaticCombinationAverage(m);
                case CombinationRules.COMBINATION_MURPHY:
                    return AMassFunction<TFunction, TElement>.StaticCombinationMurphy(m);
                case CombinationRules.COMBINATION_CHEN:
                    return AMassFunction<TFunction, TElement>.StaticCombinationChen(m);
                default:
                    throw new ArgumentOutOfRangeException("rule. The given rule is not defined!");
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Combines the given mass functions using the unnormalised Dempster's rule
        /// of combination. For a definition, refer to ¨P. Smets: The transferable belief model, 1994".
        /// </summary>
        /// <param name="m">The mass functions to combine.</param>
        /// <returns>Returns a new mass function which is the combination of the given mass functions.</returns>
        /// <exception cref="EmptyMassFunctionException">Thrown if at least one
        /// of the given mass functions is empty.</exception>
        /// <exception cref="IncompatibleMassFunctionException">Thrown if at least one of the given mass
        /// functions is incompatible with the others.</exception>
        public static TFunction StaticCombinationSmets(params TFunction[] m)
        {
            if (m.Length < 2)
            {
                throw new NotEnoughMassFunctionsException("You should give at least two MassFunctions to combine!");
            }
            TFunction[] newM = new TFunction[m.Length - 1];
            for (int i = 1; i < m.Length; i++)
            {
                newM[i - 1] = m[i];
            }
            return m[0].CombinationSmets(newM);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Combines the given mass functions using the Dempster's rule
        /// of combination.
        /// </summary>
        /// <param name="m">The mass functions to combine.</param>
        /// <returns>Returns a new mass function which is the combination of the given mass functions.</returns>
        /// <exception cref="EmptyMassFunctionException">Thrown if at least one
        /// of the given mass functions is empty.</exception>
        /// <exception cref="IncompatibleMassFunctionException">Thrown if at least one of the given mass
        /// functions is incompatible with the others.</exception>
        /// <exception cref="CombinationNotDefinedException">Thrown in case of total conflict where the 
        /// Dempster's rule of combination is not defined (division by 0).</exception>
        public static TFunction StaticCombinationDempster(params TFunction[] m)
        {
            if (m.Length < 2)
            {
                throw new NotEnoughMassFunctionsException("You should give at least two MassFunctions to combine!");
            }
            TFunction[] newM = new TFunction[m.Length - 1];
            for (int i = 1; i < m.Length; i++)
            {
                newM[i - 1] = m[i];
            }
            return m[0].CombinationDempster(newM);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Combines the given mass functions using the Yager's rule of combination.
        /// For a definition, refer to "R. Yager: On the Dempster-Shafer framework and new combination rules, 1987".
        /// </summary>
        /// <param name="m">The mass functions to combine.</param>
        /// <returns>Returns a new mass function which is the combination of the given mass functions.</returns>
        /// <exception cref="EmptyMassFunctionException">Thrown if at least one
        /// of the given mass functions is empty.</exception>
        /// <exception cref="IncompatibleMassFunctionException">Thrown if at least one of the given mass
        /// functions is incompatible with the others.</exception>
        public static TFunction StaticCombinationYager(params TFunction[] m)
        {
            if (m.Length < 2)
            {
                throw new NotEnoughMassFunctionsException("You should give at least two MassFunctions to combine!");
            }
            TFunction[] newM = new TFunction[m.Length - 1];
            for (int i = 1; i < m.Length; i++)
            {
                newM[i - 1] = m[i];
            }
            return m[0].CombinationYager(newM);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// <para>Combines the given mass functions using the MDubois and Prade's rule of combination.
        /// For a definition, refer to "D. Dubois and H. Prade: Representation and Combination 
        /// of Uncertainty with Belief Functions and Possibility Measures, 1988".</para>
        /// <para>WARNING: DO NOT COMBINE TOO MANY MASS FUNCTIONS WITH TOO MANY FOCAL ELEMENTS, IT MAY HARM
        /// YOUR POOR COMPUTER.</para>
        /// </summary>
        /// <param name="m">The mass functions to combine.</param>
        /// <returns>Returns a new mass function which is the combination of the given mass functions.</returns>
        /// <exception cref="EmptyMassFunctionException">Thrown if at least one
        /// of the given mass functions is empty.</exception>
        /// <exception cref="IncompatibleMassFunctionException">Thrown if at least one of the given mass
        /// functions is incompatible with the others.</exception>
        public static TFunction StaticCombinationDuboisPrade(params TFunction[] m)
        {
            if (m.Length < 2)
            {
                throw new NotEnoughMassFunctionsException("You should give at least two MassFunctions to combine!");
            }
            TFunction[] newM = new TFunction[m.Length - 1];
            for (int i = 1; i < m.Length; i++)
            {
                newM[i - 1] = m[i];
            }
            return m[0].CombinationDuboisPrade(newM);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Combines the given mass functions by simply averaging them altogether.
        /// </summary>
        /// <param name="m">The mass functions to combine.</param>
        /// <returns>Returns a new mass function which is the combination of the given mass functions.</returns>
        /// <exception cref="EmptyMassFunctionException">Thrown if at least one
        /// of the given mass functions is empty.</exception>
        /// <exception cref="IncompatibleMassFunctionException">Thrown if at least one of the given mass
        /// functions is incompatible with the others.</exception>
        public static TFunction StaticCombinationAverage(params TFunction[] m)
        {
            if (m.Length < 2)
            {
                throw new NotEnoughMassFunctionsException("You should give at least two MassFunctions to combine!");
            }
            TFunction[] newM = new TFunction[m.Length - 1];
            for (int i = 1; i < m.Length; i++)
            {
                newM[i - 1] = m[i];
            }
            return m[0].CombinationAverage(newM);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Combines the given mass functions using the Murphy's rule of combination.
        /// For a definition, refer to "C. K. Murphy: Combining belief functions when evidence conflicts, 2000".
        /// </summary>
        /// <param name="m">The mass functions to combine.</param>
        /// <returns>Returns a new mass function which is the combination of the given mass functions.</returns>
        /// <exception cref="EmptyMassFunctionException">Thrown if at least one
        /// of the given mass functions is empty.</exception>
        /// <exception cref="IncompatibleMassFunctionException">Thrown if at least one of the given mass
        /// functions is incompatible with the others.</exception>
        public static TFunction StaticCombinationMurphy(params TFunction[] m)
        {
            if (m.Length < 2)
            {
                throw new NotEnoughMassFunctionsException("You should give at least two MassFunctions to combine!");
            }
            TFunction[] newM = new TFunction[m.Length - 1];
            for (int i = 1; i < m.Length; i++)
            {
                newM[i - 1] = m[i];
            }
            return m[0].CombinationMurphy(newM);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Combines the given mass functions using the Chen's rule of combination.
        /// For a definition, please refer to "L.-Z. Chen: A new fusion approach based on distance of evidences, 2005".
        /// </summary>
        /// <param name="m">The mass functions to combine.</param>
        /// <returns>Returns a new mass function which is the combination of the given mass functions.</returns>
        /// <exception cref="EmptyMassFunctionException">Thrown if at least one
        /// of the given mass functions is empty.</exception>
        /// <exception cref="IncompatibleMassFunctionException">Thrown if at least one of the given mass
        /// functions is incompatible with the others.</exception>
        public static TFunction StaticCombinationChen(params TFunction[] m)
        {
            if (m.Length < 2)
            {
                throw new NotEnoughMassFunctionsException("You should give at least two MassFunctions to combine!");
            }
            TFunction[] newM = new TFunction[m.Length - 1];
            for (int i = 1; i < m.Length; i++)
            {
                newM[i - 1] = m[i];
            }
            return m[0].CombinationChen(newM);
        }

        #endregion

    } //Class
} //Namespace
