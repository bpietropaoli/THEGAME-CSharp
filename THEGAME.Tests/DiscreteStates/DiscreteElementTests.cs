using System;
using System.Diagnostics;

using THEGAME.Core.DiscreteStates;

namespace THEGAME.Tests.DiscreteStates
{
    public static class DiscreteElementTests
    {
        public static void TestCase()
        {
            DiscreteElement peuh = new DiscreteElement(5, 3);
            DiscreteElement peuh2 = new DiscreteElement(5, 9);
            StringReferenceList refList = new StringReferenceList("Aka", "Bea", "Coq", "Dad", "Elf");

            Console.WriteLine("\n\n" +
                "+--------------------------------------------------------------------+\n" +  
                "|                        DiscreteElement Tests                       |\n" +
                "+--------------------------------------------------------------------+"
            );
            Console.WriteLine(refList);
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Building Element from strings Aka, Coq, Elf");
            Console.WriteLine("Element: {0}", (new DiscreteElement(refList, new string[] { "Aka", "Coq", "Elf" })).ToString());
            Console.WriteLine("Element: {0}", (new DiscreteElement(refList, "Aka", "Coq", "Elf").ToString(refList)));
            Console.WriteLine("Building Element of size 68 with 4 2 0");
            Console.WriteLine("Element: {0}", (new DiscreteElement(68, 4, 2, 0)));
            Console.WriteLine("Building Element of size 36 with 0 2");
            Console.WriteLine("Element: {0}", (new DiscreteElement(36, 0, 2)));
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Element 1:   {0}", peuh);
            Console.WriteLine("Element 2:   {0}", peuh2);
            Console.WriteLine("Conjunction: {0}", peuh.Conjunction(peuh2));
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Element 1:   {0}", peuh);
            Console.WriteLine("Element 2:   {0}", peuh2);
            Console.WriteLine("Conjunction: {0}", peuh & peuh2);
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Element 1:   {0}", peuh.ToString(refList));
            Console.WriteLine("Element 2:   {0}", peuh2.ToString(refList));
            Console.WriteLine("Conjunction: {0}", peuh.Conjunction(peuh2).ToString(refList));
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Element 1:   {0}", peuh.ToString(refList));
            Console.WriteLine("Element 2:   {0}", peuh2.ToString(refList));
            Console.WriteLine("Conjunction: {0}", (peuh & peuh2).ToString(refList));
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Element 1:   {0}", peuh);
            Console.WriteLine("Element 2:   {0}", peuh2);
            Console.WriteLine("Disjunction: {0}", peuh.Disjunction(peuh2));
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Element 1:   {0}", peuh);
            Console.WriteLine("Element 2:   {0}", peuh2);
            Console.WriteLine("Disjunction: {0}", peuh | peuh2);
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Element 1:   {0}", peuh.ToString(refList));
            Console.WriteLine("Element 2:   {0}", peuh2.ToString(refList));
            Console.WriteLine("Disjunction: {0}", peuh.Disjunction(peuh2).ToString(refList));
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Element 1:   {0}", peuh.ToString(refList));
            Console.WriteLine("Element 2:   {0}", peuh2.ToString(refList));
            Console.WriteLine("Disjunction: {0}", (peuh | peuh2).ToString(refList));
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Element 1:  {0}", peuh);
            Console.WriteLine("Opposite 1: {0}", peuh.Opposite());
            Console.WriteLine("Element 2:  {0}", peuh2);
            Console.WriteLine("Opposite 2: {0}", peuh2.Opposite());
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Element 1: {0}", peuh);
            Console.WriteLine("Element 2: {0}", peuh2);
            Console.WriteLine("Equals (false) : {0}", peuh.Equals(peuh2));
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Element 1: {0}", peuh);
            Console.WriteLine("Element 2: {0}", peuh);
            Console.WriteLine("Equals (true) : {0}", peuh.Equals(peuh));
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Element 1: {0}", peuh);
            Console.WriteLine("Element 2: {0}", peuh2);
            Console.WriteLine("IsASubset (false) : {0}", peuh.IsASubset(peuh2));
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Element 1: {0}", peuh);
            Console.WriteLine("Element 2: {0}", peuh.Conjunction(peuh2));
            Console.WriteLine("IsASubset (true) : {0}", peuh.Conjunction(peuh2).IsASubset(peuh));
            Console.WriteLine("----------------------------------------------------------------------");

            peuh = new DiscreteElement(68, 68);
            peuh2 = new DiscreteElement(68, 75);
            DiscreteElement peuh3 = peuh.Conjunction(peuh2);

            Console.WriteLine("Test of really big elements:");
            Console.WriteLine("Element of size 68: {0}", peuh);
            Console.WriteLine("Opposite:           {0}", peuh.Opposite());
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Element 1:   {0}", peuh);
            Console.WriteLine("Element 2:   {0}", peuh2);
            Console.WriteLine("Conjunction: {0}", peuh.Conjunction(peuh2));
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Element 1:   {0}", peuh);
            Console.WriteLine("Element 2:   {0}", peuh2);
            Console.WriteLine("Disjunction: {0}", peuh.Disjunction(peuh2));
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Element 1:   {0}", peuh);
            Console.WriteLine("Opposite 1:  {0}", peuh.Opposite());
            Console.WriteLine("Element 2:   {0}", peuh2);
            Console.WriteLine("Opposite 2:  {0}", peuh2.Opposite());
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Element 1:   {0}", peuh);
            Console.WriteLine("Element 2:   {0}", peuh2);
            Console.WriteLine("Equals (false) : {0}", peuh.Equals(peuh2));
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Element 1:   {0}", peuh);
            Console.WriteLine("Element 2:   {0}", peuh);
            Console.WriteLine("Equals (true) : {0}", peuh.Equals(peuh));
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Element 1:   {0}", peuh);
            Console.WriteLine("Element 2:   {0}", peuh2);
            Console.WriteLine("IsASubset (false) : {0}", peuh.IsASubset(peuh2));
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Element 1:   {0}", peuh);
            Console.WriteLine("Element 2:   {0}", peuh.Conjunction(peuh2));
            Console.WriteLine("IsASubset (true) : {0}", peuh.Conjunction(peuh2).IsASubset(peuh));
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Get the complete set: {0}", DiscreteElement.GetCompleteElement(68));
            Console.WriteLine("Get the emptyset:     {0}", DiscreteElement.GetEmptyElement(68));
            Console.WriteLine("----------------------------------------------------------------------");

            //Performance tests:
            DiscreteElement small1 = new DiscreteElement(5, 27);
            DiscreteElement small2 = new DiscreteElement(5, 13);
            DiscreteElement small3 = new DiscreteElement(64, 49227);
            DiscreteElement small4 = new DiscreteElement(64, 987543);

            int nbIterations = 1000000;

            Stopwatch watch = Stopwatch.StartNew();
            //Small conjunction test:
            for (int i = 0; i < nbIterations; i++)
            {
                small1.Conjunction(small2);
            }
            Console.WriteLine("Small conjunctions ({1}): {0}ms", watch.ElapsedMilliseconds, nbIterations);

            watch = Stopwatch.StartNew();
            //Small conjunction test:
            for (int i = 0; i < nbIterations; i++)
            {
                small3.Conjunction(small4);
            }
            Console.WriteLine("Big conjunctions ({1}): {0}ms", watch.ElapsedMilliseconds, nbIterations);

            Console.WriteLine("----------------------------------------------------------------------");

            watch = Stopwatch.StartNew();
            //Small conjunction test:
            for (int i = 0; i < nbIterations; i++)
            {
                small1.Disjunction(small2);
            }
            Console.WriteLine("Small disjunctions ({1}): {0}ms", watch.ElapsedMilliseconds, nbIterations);

            watch = Stopwatch.StartNew();
            //Small conjunction test:
            for (int i = 0; i < nbIterations; i++)
            {
                small3.Disjunction(small4);
            }
            Console.WriteLine("Big disjunctions ({1}): {0}ms", watch.ElapsedMilliseconds, nbIterations);

            Console.WriteLine("----------------------------------------------------------------------");
        }


    } //Class
} //Namespace



