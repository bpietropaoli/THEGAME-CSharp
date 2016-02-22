using System;
using System.Collections.Generic;
using System.Text;

using THEGAME.Core.DiscreteStates;

namespace THEGAME.Tests.DiscreteStates
{
    public static class DiscreteSetTests
    {

        public static void TestCase()
        {
            DiscreteSet s = new DiscreteSet();
            DiscreteElement peuh = new DiscreteElement(5, 3);
            DiscreteElement peuh2 = new DiscreteElement(5, 9);
            StringReferenceList refList = new StringReferenceList("Aka", "Bea", "Coq", "Dad", "Elf");

            Console.WriteLine("\n\n" +
                "+--------------------------------------------------------------------+\n" +
                "|                         DiscreteSet Tests                          |\n" +
                "+--------------------------------------------------------------------+"
            );
            Console.WriteLine(refList);
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Empty set:           {0}", s.ToString(refList));
            Console.WriteLine("Adding the element   {0}", peuh.ToString(refList));
            s.Add(peuh);
            Console.WriteLine("Resulting set:       {0}", s.ToString(refList));
            Console.WriteLine("Adding the element   {0}", peuh.ToString(refList));
            s.Add(peuh);
            Console.WriteLine("Resulting set:       {0}", s.ToString(refList));
            Console.WriteLine("Adding the element   {0}", peuh2.ToString(refList));
            s.Add(peuh2);
            Console.WriteLine("Resulting set:       {0}", s.ToString(refList));
            Console.WriteLine("Removing the element {0}", peuh.ToString(refList));
            s.Remove(peuh);
            Console.WriteLine("Resulting set:       {0}", s.ToString(refList));
            Console.WriteLine("Removing the element {0}", peuh2.ToString(refList));
            s.Remove(peuh2);
            Console.WriteLine("Resulting set:       {0}", s.ToString(refList));
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Empty set:           {0}", s.ToString(refList));
            Console.WriteLine("Adding the element   {0}", peuh.ToString(refList));
            s += peuh;
            Console.WriteLine("Resulting set:       {0}", s.ToString(refList));
            Console.WriteLine("Adding the element   {0}", peuh.ToString(refList));
            s += peuh;
            Console.WriteLine("Resulting set:       {0}", s.ToString(refList));
            Console.WriteLine("Adding the element   {0}", peuh2.ToString(refList));
            s += peuh2;
            Console.WriteLine("Resulting set:       {0}", s.ToString(refList));
            Console.WriteLine("Removing the element {0}", peuh.ToString(refList));
            s -= peuh;
            Console.WriteLine("Resulting set:       {0}", s.ToString(refList));
            Console.WriteLine("Removing the element {0}", peuh2.ToString(refList));
            s -= peuh2;
            Console.WriteLine("Resulting set:       {0}", s.ToString(refList));
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Generating a set of 4 atoms.");
            s = DiscreteSet.GenerateSetOfAtoms(4);
            Console.WriteLine("Resulting set: {0}", s);
            Console.WriteLine("Number of elements: {0}", s.Card);
            Console.WriteLine("Size of elements: {0}", s.ElementSize);
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Generating a set of 6 atoms.");
            s = DiscreteSet.GenerateSetOfAtoms(6);
            Console.WriteLine("Resulting set: {0}", s);
            Console.WriteLine("Number of elements: {0}", s.Card);
            Console.WriteLine("Size of elements: {0}", s.ElementSize);
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Generating a set of 68 atoms.");
            s = DiscreteSet.GenerateSetOfAtoms(68);
            Console.WriteLine("Resulting set: {0}", s.ToStringOnePerLine());
            Console.WriteLine("Number of elements: {0}", s.Card);
            Console.WriteLine("Size of elements: {0}", s.ElementSize);
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Generating a powerset with 4 worlds.");
            s = DiscreteSet.GeneratePowerSet(4);
            Console.WriteLine("Resulting set: {0}", s.ToStringOnePerLine());
            Console.WriteLine("Number of elements: {0}", s.Card);

            Console.WriteLine("Size of elements: {0}", s.ElementSize);
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Generating a powerset with 6 worlds.");
            s = DiscreteSet.GeneratePowerSet(6);
            Console.WriteLine("Resulting set: {0}", s.ToStringOnePerLine());
            Console.WriteLine("Number of elements: {0}", s.Card);
            Console.WriteLine("Size of elements: {0}", s.ElementSize);
            Console.WriteLine("----------------------------------------------------------------------");

        }

    } //Class
} //Namespace



