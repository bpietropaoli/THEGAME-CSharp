using System;
using System.Collections.Generic;
using System.Text;

using THEGAME.Core.Generic;
using THEGAME.Exceptions;

namespace THEGAME.Core.DiscreteStates
{
    /// <summary>
    /// A class to represent a mass function over discrete elements. Inherits from 
    /// <see cref="AMassFunction{DiscreteMassFunction, DiscreteElement}"/>. This is the classic
    /// mass function in the belief functions theory.
    /// </summary>
    public sealed class DiscreteMassFunction : AMassFunction<DiscreteMassFunction, DiscreteElement>
    {
        /*				
		 * Properties
		 */

        /// <summary>
        /// The number of possible states/worlds defined in the frame of discernment.
        /// </summary>
        public int NbPossibleWorlds
        {
            get { return _focals.Count == 0 ? -1 : _focals[0].Element.Size; }
        }


        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Constructors

        /*				
         * Constructors
         */

        /// <summary>
        /// Builds an empty mass function (not a valid one, mass needs to be added).
        /// </summary>
        public DiscreteMassFunction()
        {
            _focals = new List<FocalElement<DiscreteElement>>();
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Builds a vacuous mass function with the given number of possible states/worlds.
        /// </summary>
        /// <param name="numberOfWorlds">The number of possible states/worlds in the frame
        /// of discernment.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the given number of 
        /// possible states/world in null or negative.</exception>
        public DiscreteMassFunction(int numberOfWorlds)
            : this()
        {
            //Checking arguments:
            if (numberOfWorlds <= 0)
            {
                throw new ArgumentOutOfRangeException("numberOfWorlds. The number of possible worlds could not be null or negative!");
            }
            //Constructing:
            _focals.Add(new FocalElement<DiscreteElement>(DiscreteElement.GetCompleteElement(numberOfWorlds), 1));
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Builds a mass function with the given focal elements.
        /// </summary>
        /// <param name="elements">The elements to add as focal elements.</param>
        /// <param name="masses">The masses associated to the given elements.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if both arrays do not contain
        /// the same number of elements.</exception>
        /// <exception cref="IncompatibleDiscreteElementSizeException">Thrown if at least one of the
        /// given elements is not compatible with the others.</exception>
        public DiscreteMassFunction(DiscreteElement[] elements, double[] masses)
            : this()
        {
            //Checking arguments:
            if (elements.Length != masses.Length)
            {
                throw new ArgumentOutOfRangeException("elements and masses. Both arrays should be of the same size!");
            }
            foreach (DiscreteElement element in elements)
            {
                if (element.Size != elements[0].Size)
                {
                    throw new IncompatibleDiscreteElementSizeException("The DiscreteElements given in an array/list should all have the same size!");
                }
            }
            //Constructing:
            for (int i = 0; i < elements.Length; i++)
            {
                _focals.Add(new FocalElement<DiscreteElement>(elements[i], masses[i]));
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Builds a mass function from the given focal elements.
        /// </summary>
        /// <param name="focals">The focal elements to add to the mass function.</param>
        /// <exception cref="IncompatibleDiscreteElementSizeException">Thrown if at least one of the
        /// given focal elements is not compatible with the others.</exception>
        public DiscreteMassFunction(params FocalElement<DiscreteElement>[] focals)
        {
            //Checking arguments:
            foreach (FocalElement<DiscreteElement> focal in focals)
            {
                if (focal.Element.Size != focals[0].Element.Size)
                {
                    throw new IncompatibleDiscreteElementSizeException("The DiscreteElements given in an array/list should all have the same size!");
                }
            }
            //Constructing:
            _focals = new List<FocalElement<DiscreteElement>>(focals);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Builds a mass function from the given focal elements.
        /// </summary>
        /// <param name="focals">The focal elements to add to the mass function.</param>
        /// <exception cref="IncompatibleDiscreteElementSizeException">Thrown if at least one of the
        /// given focal elements is not compatible with the others.</exception>
        public DiscreteMassFunction(List<FocalElement<DiscreteElement>> focals)
            : this(focals.ToArray())
        {}

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Methods

        /*				
         * Methods
         */

        /// <summary>
        /// Conditions a mass function by the given element. For a definition, refer to "P. Smets,
        /// The transferable belief model for belief representation, 1999".
        /// </summary>
        /// <param name="e">The element to use to condition the mass function.</param>
        /// <returns>Returns a new mass function corresponding to the current mass function conditioned.</returns>
        /// <exception cref="EmptyMassFunctionException">Thrown if the method is called on an empty 
        /// mass function.</exception>
        /// <exception cref="IncompatibleDiscreteElementSizeException">Thrown if the given element seems to be defined
        /// on a different frame of discernement than the one on which the mass function is defined.</exception>
        /// <exception cref="EmptyElementException">Thrown if the method is called with the empty set as
        /// a parameter. The conditioning is not defined for the empty set.</exception>
        public new DiscreteMassFunction Conditioning(DiscreteElement e)
        {
            //Checking arguments:
            if (NbFocals == 0)
            {
                throw new EmptyMassFunctionException("A MassFunction should contain at least one focal element to be conditioned!");
            }
            if (e.Size != NbPossibleWorlds)
            {
                throw new IncompatibleDiscreteElementSizeException("The given DiscreteElement is not defined on the same frame of discernment as the MassFunction!");
            }
            if (e.IsEmpty())
            {
                throw new EmptyElementException("A MassFunction cannot be conditioned by the empty set!");
            }
            return this.CombinationSmets(new DiscreteMassFunction(new FocalElement<DiscreteElement>(e, 1)));
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// <para>Returns the focal elements that have the maximum value for the given criterion and which respect
        /// the maximum cardinal constraint given. It compromises between the criterion and the maximum cardinality.
        /// For more details, refer to "M. Dominici et al., Experiences in managing uncertainty and ignorance in a 
        /// lightly instrumented smart home, 2012".</para>
        /// <para>Calls AMassFunction.GetMax() with the power set as parameter for the set.</para>
        /// <para>WARNING: DO NOT CALL THIS METHOD WITH A MASS FUNCTIONS DEFINED ON A FRAME OF DISCERNMENT THAT
        /// IS TOO BIG OR IT WILL HARM YOUR POOR COMPUTER.</para>
        /// </summary>
        /// <param name="f">The criterion function (typically M, Bel, BetP, Pl or Q).</param>
        /// <param name="maxCard">The maximum cardinality the returned maxima may have.</param>
        /// <returns>Returns the list of found maxima as a list of FocalElements.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the maxCard argument is null or negative.</exception>
        /// <exception cref="EmptyMassFunctionException">Thrown if the mass function does not contain any focal element.</exception>
        /// <exception cref="IncompatibleSetException">Thrown if the given set is incompatible with the focal elements
        /// of the current mass function.</exception>
        public List<FocalElement<DiscreteElement>> GetMax(CriterionFunction f, int maxCard)
        {
            return this.GetMax(f, maxCard, DiscreteSet.GeneratePowerSet(NbPossibleWorlds));
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// <para>Returns the focal elements that have the minimum value for the given criterion and which respect
        /// the maximum cardinal constraint given. It compromises between the criterion and the maximum cardinality.
        /// For more details, refer to "M. Dominici et al., Experiences in managing uncertainty and ignorance in a 
        /// lightly instrumented smart home, 2012".</para>
        /// <para>DISCLAIMER: This function may be completely useless but who knows?!</para>
        /// <para>Calls AMassFunction.GetMin() with the power set as parameter for the set.</para>
        /// <para>WARNING: DO NOT CALL THIS METHOD WITH A MASS FUNCTIONS DEFINED ON A FRAME OF DISCERNMENT THAT
        /// IS TOO BIG OR IT WILL HARM YOUR POOR COMPUTER.</para>
        /// </summary>
        /// <param name="f">The criterion function (typically M, Bel, BetP, Pl or Q).</param>
        /// <param name="maxCard">The maximum cardinality the returned maxima may have.</param>
        /// <returns>Returns the list of found maxima as a list of FocalElements.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the maxCard argument is null or negative.</exception>
        /// <exception cref="EmptyMassFunctionException">Thrown if the mass function does not contain any focal element.</exception>
        /// <exception cref="IncompatibleSetException">Thrown if the given set is incompatible with the focal elements
        /// of the current mass function.</exception>
        public List<FocalElement<DiscreteElement>> GetMin(CriterionFunction f, int maxCard)
        {
            return this.GetMin(f, maxCard, DiscreteSet.GeneratePowerSet(NbPossibleWorlds));
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gives a string representation of the mass function using the given reference list.
        /// </summary>
        /// <param name="refList">The reference list to gives sense to the elements.</param>
        /// <returns>Returns a string representation of the mass function.</returns>
        /// <exception cref="IncompatibleReferenceListException">Thrown if the given reference
        /// list surely does not correspond to this discrete mass function.</exception>
        public string ToString(StringReferenceList refList)
        {
            return ToString<string>(refList);
        }

        /// <summary>
        /// Gives a string representation of the mass function using the given reference list.
        /// </summary>
        /// <param name="refList">The reference list to gives sense to the elements.</param>
        /// <returns>Returns a string representation of the mass function.</returns>
        /// <exception cref="IncompatibleReferenceListException">Thrown if the given reference
        /// list surely does not correspond to this discrete mass function.</exception>
        public string ToString<T>(ReferenceList<T> refList)
        {
            if (NbPossibleWorlds == -1)
            {
                return "Mass function: void";
            }
            if (refList.Count != NbPossibleWorlds)
            {
                throw new IncompatibleReferenceListException("The given ReferenceList is not adapted to this MassFunction!");
            }
            StringBuilder toReturn = new StringBuilder("Mass function:\n");
            int i = 0;
            foreach (FocalElement<DiscreteElement> e in Focals)
            {
                i++;
                if (i == NbFocals)
                {
                    toReturn.Append(String.Format("m({0}) = {1}", e.Element.ToString(refList), e.Value));
                }
                else
                {
                    toReturn.Append(String.Format("m({0}) = {1}\n", e.Element.ToString(refList), e.Value));
                }
            }
            return toReturn.ToString();
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Static utility methods

        /*				
         * Static methods
         */

        /// <summary>
        /// Gets the vacuous mass function with elements of the given size.
        /// </summary>
        /// <param name="nbOfAtoms">The number of possible states/worlds in the frame of discernment.</param>
        /// <returns>Returns a new mass function which is vacuous with elements of the given size.</returns>
        public static DiscreteMassFunction GetVacuousMassFunction(int nbOfAtoms)
        {
            DiscreteMassFunction toReturn = new DiscreteMassFunction();
            toReturn.AddMass(DiscreteElement.GetCompleteElement(nbOfAtoms), 1);
            return toReturn;
        }

    #endregion

    } //Class
} //Namespace


