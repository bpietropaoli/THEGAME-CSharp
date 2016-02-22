using System;

using THEGAME.Core.DiscreteStates;

namespace THEGAME.Tests.DiscreteStates
{
    public static class DiscreteElementEnumeratorTests
    {
        public static void TestCase()
        {
            
            Console.WriteLine("\n\n" +
                "+--------------------------------------------------------------------+\n" +
                "|                  DiscreteElementEnumerator Tests                   |\n" +
                "+--------------------------------------------------------------------+"
            );
            Console.Write("What size would you like to test? ");
            string command = Console.ReadLine();
            try
            {
                int size = Convert.ToInt32(command);
                if (size > 10)
                {
                    Console.Write("This seems to be a bad idea, continue anyway? (Y/N): ");
                    ConsoleKeyInfo key = Console.ReadKey();
                    if (key.Key != ConsoleKey.Y)
                    {
                        throw new Exception();
                    }
                }
                DiscreteElementEnumerator enumerator = new DiscreteElementEnumerator(size);
                foreach (DiscreteElement e in enumerator)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            catch (Exception)
            {
                Console.WriteLine("You don't really want to test anything, do you?");
            }
        }

    } //Class
} //Namespace
