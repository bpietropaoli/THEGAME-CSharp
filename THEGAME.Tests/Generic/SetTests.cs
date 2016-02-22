using System;
using System.Collections.Generic;

using THEGAME.Core.Generic;
using THEGAME.Core.DiscreteStates;

namespace THEGAME.Tests.Generic
{
	public static class SetTests
	{
		public static void TestCase ()
		{
            Set<DiscreteElement> s = new Set<DiscreteElement>();
            DiscreteElement peuh = new DiscreteElement(5, 3);
            DiscreteElement peuh2 = new DiscreteElement(5, 9);

            Console.WriteLine("\n\n" +
                "+--------------------------------------------------------------------+\n" +
                "|                         Generic Set Tests                          |\n" +
                "+--------------------------------------------------------------------+"
            );
            Console.WriteLine("Empty set:           {0}", s);
            Console.WriteLine("Adding the element   {0}", peuh);
            s.Add(peuh);
            Console.WriteLine("Resulting set:       {0}", s);
            Console.WriteLine("Adding the element   {0}", peuh);
            s.Add(peuh);
            Console.WriteLine("Resulting set:       {0}", s);
            Console.WriteLine("Adding the element   {0}", peuh2);
            s.Add(peuh2);
            Console.WriteLine("Resulting set:       {0}", s);
            Console.WriteLine("Removing the element {0}", peuh);
            s.Remove(peuh);
            Console.WriteLine("Resulting set:       {0}", s);
            Console.WriteLine("Removing the element {0}", peuh2);
            s.Remove(peuh2);
            Console.WriteLine("Resulting set:       {0}", s);

            Console.WriteLine("----------------------------------------------------------------------");

            Console.WriteLine("Empty set:           {0}", s);
            Console.WriteLine("Adding the element   {0}", peuh);
            s += peuh;
            Console.WriteLine("Resulting set:       {0}", s);
            Console.WriteLine("Adding the element   {0}", peuh);
            s += peuh;
            Console.WriteLine("Resulting set:       {0}", s);
            Console.WriteLine("Adding the element   {0}", peuh2);
            s += peuh2;
            Console.WriteLine("Resulting set:       {0}", s);
            Console.WriteLine("Removing the element {0}", peuh);
            s -= peuh;
            Console.WriteLine("Resulting set:       {0}", s);
            Console.WriteLine("Removing the element {0}", peuh2);
            s -= peuh2;
            Console.WriteLine("Resulting set:       {0}", s);
		}

	} //Class
} //Namespace



