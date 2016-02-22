using System;
using System.Collections.Generic;
using System.Text;

using THEGAME.Tests.IntervalStates;
using THEGAME.Tests.DiscreteStates;
using THEGAME.Tests.Generic;
using THEGAME.Core.DiscreteStates;
using System.Diagnostics;

namespace THEGAME.Tests
{
    public static class TestProgram
    {
        public static void Main(string[] args)
        {
            bool running = true;
            while (running)
            {
                string command = "";
                Console.WriteLine("\n\n" +
                    "+--------------------------------------------------------------------+\n" +
                    "|                      THE(GAME)# TEST PROGRAM                       |\n" +
                    "+--------------------------------------------------------------------+\n" +
                    "| Select what you want to test:                                      |\n" +
                    "| 1  - Generic   - Set                                               |\n" +
                    "+--------------------------------------------------------------------+\n" +
                    "| 2  - Discrete  - StringReferenceList                               |\n" +
                    "| 3  - Discrete  - DiscreteSet                                       |\n" +
                    "| 4  - Discrete  - DiscreteElement                                   |\n" +
                    "| 5  - Discrete  - DiscreteElementEnumerator                         |\n" +
                    "| 6  - Discrete  - DiscreteMassFunction                              |\n" +
                    "+--------------------------------------------------------------------+\n" +
                    "| 7  - Interval  - Interval                                          |\n" +
                    "| 8  - Interval  - IntervalElement                                   |\n" +
                    "| 9  - Interval  - IntervalMassFunction                              |\n" +
                    "+--------------------------------------------------------------------+\n" +
                    "| 10 - Generator - Discrete - From randomness                        |\n" +
                    "| 11 - Generator - Discrete - From sensors                           |\n" +
                    "| 12 - Generator - Discrete - From beliefs                           |\n" +
                    "+--------------------------------------------------------------------+\n" +
                    "| 13 - Terminate this program                                        |\n" +
                    "+--------------------------------------------------------------------+"
                );
                command = Console.ReadLine();
                if (command == "")
                {
                    continue;
                }
                try
                {
                    int action = Convert.ToInt32(command);
                    switch (action)
                    {
                        case 1:
                            SetTests.TestCase();
                            break;
                        case 2:
                            StringReferenceListTests.TestCase();
                            break;
                        case 3:
                            DiscreteSetTests.TestCase();
                            break;
                        case 4:
                            DiscreteElementTests.TestCase();
                            break;
                        case 5:
                            DiscreteElementEnumeratorTests.TestCase();
                            break;
                        case 6:
                            DiscreteMassFunctionTests.TestCase();
                            break;
                        case 7:
                            IntervalTests.TestCase();
                            break;
                        case 8:
                            IntervalElementTests.TestCase();
                            break;
                        case 9:
                            IntervalMassFunctionTests.TestCase();
                            break;
                        case 10:
                            GeneratorDiscreteBeliefFromRandomnessTests.TestCase();
                            break;
                        case 11:
                            GeneratorDiscreteBeliefFromSensorsTests.TestCase();
                            break;
                        case 12:
                            GeneratorDiscreteBeliefFromBeliefsTests.TestCase();
                            break;
                        case 13:
                            Console.WriteLine("Terminating the program...");
                            running = false;
                            break;
                        default:
                            Console.WriteLine("The entered number does not correspond to anything!\nPress any key to continue...");
                            Console.ReadKey(true);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.WriteLine("You entered an invalid answer!\nPress any key to continue...");
                    Console.ReadKey(true);
                }
            }
        }

    } //Class
} //Namespace
