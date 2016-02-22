using System;
using System.Collections.Generic;

using THEGAME.Core.DiscreteStates;
using THEGAME.Construction.Generic;
using THEGAME.Exceptions;

namespace THEGAME.Construction.DiscreteStates.FromRandomness
{
    /// <summary>
    /// A class to generate random mass functions.
    /// </summary>
    public sealed class GeneratorDiscreteBeliefFromRandomness : IBeliefConstructor<DiscreteMassFunction, DiscreteElement>
    {
        /*
         * Members
         */
        private Random _rand = new Random();

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        /*		
         * Properties
         */

        /// <summary>
        /// Gets the number of possible states/worlds defined in the frame of discernment.
        /// </summary>
        public int NbPossibleWorlds { get; private set; }

        /// <summary>
        /// Gets the number of focal element to create in the discrete mass functions.
        /// </summary>
        public int NbFocals { get; private set; }

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        /*		
         * Constructors
         */

        /// <summary>
        /// Basic constructor to initialise the generator.
        /// </summary>
        /// <param name="nbPossibleWorlds">The number of possible states/worlds in the frame of
        /// discernment.</param>
        /// <param name="nbFocals">The number of focal elements to generate in the mass functions.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the given number of possible states/worlds
        /// is negative or too small OR if the given number of focals is null or negative.</exception>
        public GeneratorDiscreteBeliefFromRandomness(int nbPossibleWorlds, int nbFocals)
        {
            if (nbPossibleWorlds <= 1)
            {
                throw new ArgumentOutOfRangeException("nbPossibleWorlds. The number of possible worlds cannot be null, negative or too small!");
            }
            if (nbFocals <= 0)
            {
                throw new ArgumentOutOfRangeException("nbFocals. The number of focal elements cannot be null or negative!");
            }
            this.NbPossibleWorlds = nbPossibleWorlds;
            this.NbFocals = nbFocals;
        }

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        /*				
         * Methods
         */

        /// <summary>
        /// Constructs a random mass function for every object given. The objects themselves have no 
        /// influence on anything.
        /// </summary>
        /// <param name="obj">The number of objects passed determines the number of generated
        /// mass functions.</param>
        /// <returns>Returns a list of randomly generated mass functions with the number of
        /// focal elements and the size specified in the constructor.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if no object is passed as an argument.</exception>
        public List<DiscreteMassFunction> ConstructEvidence(params Object[] obj)
        {
            if (obj.Length < 0)
            {
                throw new ArgumentOutOfRangeException("obj. The number of objects determine the number of generated MassFunctions!");
            }
            List<DiscreteMassFunction> evidence = new List<DiscreteMassFunction>();
            int power = 1 << NbPossibleWorlds;
            for (int i = 0; i < obj.Length; i++)
            {
                //Add the function:
                evidence.Add(new DiscreteMassFunction());
                //Get a number of focals:
                int nbFocals = 0;
                if (NbFocals > 0)
                {
                    nbFocals = NbFocals;
                }
                else
                {
                    nbFocals = _rand.Next(power - 1) + 1;
                }
                //Generate the focals to add the mass:
                for (int j = 0; j < nbFocals; j++)
                {
                    int number = _rand.Next(power - 1);
                    DiscreteElement e = new DiscreteElement(NbPossibleWorlds, (uint)number);
                    while (evidence[i].Contains(e))
                    {
                        number = _rand.Next(power - 1);
                        e = new DiscreteElement(NbPossibleWorlds, (uint)number);
                    }
                    evidence[i].AddMass(e, _rand.NextDouble());
                }
                evidence[i].Normalise();
            }
            return evidence;
        }

    } //Class
} //Namespace

