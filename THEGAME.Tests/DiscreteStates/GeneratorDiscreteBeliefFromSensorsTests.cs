using System;
using System.IO;
using System.Threading;

using THEGAME.Core.DiscreteStates;
using THEGAME.Construction.Generic;
using THEGAME.Construction.DiscreteStates.FromSensors;

namespace THEGAME.Tests.DiscreteStates
{
    public static class GeneratorDiscreteBeliefFromSensorsTests
    {
        public static void TestCase()
        {
            Console.WriteLine("\n\n" +
                "+--------------------------------------------------------------------+\n" +
                "|                  GeneratorBeliefFromSensors Tests                  |\n" +
                "+--------------------------------------------------------------------+"
            );

            try
            {
                char d = Path.DirectorySeparatorChar;

                GeneratorDiscreteBeliefFromSensors generator = new GeneratorDiscreteBeliefFromSensors("Sleeping");
                Console.WriteLine("Loading a model with the custom directory format:");
                generator.LoadModel("Resources" + d + "BeliefsFromSensors" + d + "test", ModelFormat.MODEL_CUSTOM_DIRECTORY);
                Console.WriteLine(generator);

                Console.WriteLine("\n----------------------------------------------------------------------");
                Console.WriteLine("----------------------------------------------------------------------");
                Console.WriteLine("----------------------------------------------------------------------\n");

                Console.WriteLine("Loading another model from an XML file:");
                generator.LoadModel("Resources" + d + "BeliefsFromSensors" + d + "XML" + d + "BFS.xml", ModelFormat.MODEL_XML_FILE);
                Console.WriteLine(generator);

                Console.WriteLine("\n----------------------------------------------------------------------");

                Console.WriteLine("Saving the model:");
                Console.Write("Saving a custom directory (Resources" + d + "BeliefsFromSensors" + d + "SavingTest)... ");
                generator.SaveModel("Resources" + d + "BeliefsFromSensors" + d + "SavingTest", ModelFormat.MODEL_CUSTOM_DIRECTORY);
                Console.WriteLine("done!");

                Console.Write("Saving an XML file (Resources" + d + "BeliefsFromSensors" + d + "XML" + d + "SavingTest.xml)... ");
                generator.SaveModel("Resources" + d + "BeliefsFromSensors" + d + "XML" + d + "SavingTest.xml", ModelFormat.MODEL_XML_FILE);
                Console.WriteLine("done!");

                Console.WriteLine("\n----------------------------------------------------------------------");

                Console.WriteLine("Loading a model with options:");
                generator.LoadModel("Resources" + d + "BeliefsFromSensors" + d + "optionTest", ModelFormat.MODEL_CUSTOM_DIRECTORY);
                Console.WriteLine(generator);

                Console.WriteLine("\n----------------------------------------------------------------------");

                Console.WriteLine("Building mass function with tempo-specificity (2s).");
                generator.AddSensor("tempo", "Test");
                DiscreteMassFunction m = (generator.ConstructEvidence(new SensorMeasure("Test", 100)))[0];
                Console.WriteLine(m);
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine("Waiting for 200ms (no new measure)...");
                    Thread.Sleep(200);
                    m = (generator.ConstructEvidence(new SensorMeasure("Test", null)))[0];
                    Console.WriteLine(m);
                }

                Console.WriteLine("\n----------------------------------------------------------------------");

                Console.WriteLine("New measure!");
                m = (generator.ConstructEvidence(new SensorMeasure("Test", 100)))[0];
                Console.WriteLine(m);
                Console.WriteLine("Waiting for 200ms (no new measure)...");
                Thread.Sleep(200);
                m = (generator.ConstructEvidence(new SensorMeasure("Test", null)))[0];
                Console.WriteLine(m);
                Console.WriteLine("Waiting for 200ms (no new measure)...");
                Thread.Sleep(200);
                m = (generator.ConstructEvidence(new SensorMeasure("Test", null)))[0];
                Console.WriteLine(m);
                Console.WriteLine("New measure!");
                m = (generator.ConstructEvidence(new SensorMeasure("Test", 100)))[0];
                Console.WriteLine(m);

                Console.WriteLine("\n----------------------------------------------------------------------");

                Console.WriteLine("Building mass function with tempo-fusion (2s).");
                generator.AddSensor("tempoFusion", "Test-Fusion");
                m = (generator.ConstructEvidence(new SensorMeasure("Test-Fusion", 100)))[0];
                Console.WriteLine(m);
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine("Waiting for 200ms (same measure)...");
                    Thread.Sleep(200);
                    m = (generator.ConstructEvidence(new SensorMeasure("Test-Fusion", 100)))[0];
                    Console.WriteLine(m);
                }
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine("Waiting for 200ms (no more measure)...");
                    Thread.Sleep(200);
                    m = (generator.ConstructEvidence(new SensorMeasure("Test-Fusion", null)))[0];
                    Console.WriteLine(m);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    } //Class
} //Namespace
