using System;
using System.Collections.Generic;

using THEGAME.Core.DiscreteStates;
using THEGAME.Construction.DiscreteStates.FromRandomness;

namespace THEGAME.Tests.DiscreteStates
{
    public static class GeneratorDiscreteBeliefFromRandomnessTests
    {
        public static void TestCase()
        {
            Console.WriteLine("\n\n" +
                "+--------------------------------------------------------------------+\n" +
                "|                GeneratorBeliefFromRandomness Tests                 |\n" +
                "+--------------------------------------------------------------------+"
            );
            Console.Write("How many possible worlds would you like? ");
            string command = Console.ReadLine();
            try
            {
                int size = Convert.ToInt32(command);
                int nbFocals = 0;
                int nbMass = 0;
                if (size > 1)
                {
                    Console.Write("How many focal elements would you like? ");
                    command = Console.ReadLine();
                    nbFocals = Convert.ToInt32(command);
                    if (nbFocals > 0)
                    {
                        Console.Write("How many mass functions would you like to generate? ");
                        command = Console.ReadLine();
                        nbMass = Convert.ToInt32(command);
                        if (nbMass <= 0)
                        {
                            throw new Exception();
                        }
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    throw new Exception();
                }
                GeneratorDiscreteBeliefFromRandomness generator = new GeneratorDiscreteBeliefFromRandomness(size, nbFocals);
                List<DiscreteMassFunction> masses = generator.ConstructEvidence(new object[nbMass]);
                foreach (DiscreteMassFunction mass in masses)
                {
                    Console.WriteLine("----------------------------------------------------------------------\n{0}", mass.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("You don't really want to test anything, do you?");
            }
        }

    } //Class
} //Namespace
