using System;
using System.IO;
using System.Collections.Generic;

using THEGAME.Core.DiscreteStates;
using THEGAME.Construction.Generic;
using THEGAME.Construction.DiscreteStates.FromBeliefs;
using THEGAME.Construction.DiscreteStates.FromRandomness;

namespace THEGAME.Tests.DiscreteStates
{
    public static class GeneratorDiscreteBeliefFromBeliefsTests
    {
        public static void TestCase()
        {
            Console.WriteLine("\n\n" +
                "+--------------------------------------------------------------------+\n" +
                "|                  GeneratorBeliefFromBeliefs Tests                  |\n" +
                "+--------------------------------------------------------------------+"
            );

            try
            {
                char d = Path.DirectorySeparatorChar;

                GeneratorDiscreteBeliefFromBeliefs generator = new GeneratorDiscreteBeliefFromBeliefs("Sleeping");
                Console.WriteLine("Loading a model with the custom directory format:");
                generator.LoadModel("Resources" + d + "BeliefsFromBeliefs" + d + "Sleeping", ModelFormat.MODEL_CUSTOM_DIRECTORY);
                Console.WriteLine(generator);

                Console.WriteLine("\n----------------------------------------------------------------------");
                Console.WriteLine("----------------------------------------------------------------------");
                Console.WriteLine("----------------------------------------------------------------------\n");

                Console.WriteLine("Loading the same model from an XML file:");
                generator.LoadModel("Resources" + d + "BeliefsFromBeliefs" + d + "XML" + d + "BFB.xml", ModelFormat.MODEL_XML_FILE);
                Console.WriteLine(generator);

                Console.WriteLine("\n----------------------------------------------------------------------");

                Console.WriteLine("Saving the model:");
                Console.Write("Saving a custom directory (Resources" + d + "BeliefsFromBeliefs" + d + "SavingTest)... ");
                generator.SaveModel("Resources" + d + "BeliefsFromBeliefs" + d + "SavingTest", ModelFormat.MODEL_CUSTOM_DIRECTORY);
                Console.WriteLine("done!");

                Console.Write("Saving an XML file (Resources" + d + "BeliefsFromBeliefs" + d + "XML" + d + "SavingTest.xml)... ");
                generator.SaveModel("Resources" + d + "BeliefsFromBeliefs" + d + "XML" + d + "SavingTest.xml", ModelFormat.MODEL_XML_FILE);
                Console.WriteLine("done!");
                
                Console.WriteLine("\n----------------------------------------------------------------------");

                GeneratorDiscreteBeliefFromRandomness randomGenerator = new GeneratorDiscreteBeliefFromRandomness(3, 3);
                List<DiscreteMassFunction> toPropagate = randomGenerator.ConstructEvidence(new object[5]);
                List<NamedMassFunction<DiscreteMassFunction, DiscreteElement>> named = new List<NamedMassFunction<DiscreteMassFunction, DiscreteElement>>();
                for (int i = 0; i < 5; i++)
                {
                    named.Add(new NamedMassFunction<DiscreteMassFunction, DiscreteElement>("Posture", toPropagate[i]));
                }
                List<DiscreteMassFunction> propagated = generator.ConstructEvidence(named.ToArray());

                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine("To propagate: {0}", toPropagate[i].ToString(generator.Mappings[0].References));
                    Console.WriteLine("Propagated : {0}", propagated[i].ToString(generator.References));
                    Console.WriteLine("----------------------------------------------------------------------");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    } //Class
} //Namespace
