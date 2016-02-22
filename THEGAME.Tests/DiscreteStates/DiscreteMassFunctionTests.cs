using System;
using System.Collections.Generic;

using THEGAME.Core.Generic;
using THEGAME.Core.DiscreteStates;

namespace THEGAME.Tests.DiscreteStates
{
    public static class DiscreteMassFunctionTests
    {
        public static void TestCase()
        {
            StringReferenceList refList = new StringReferenceList("Yes", "No");
            DiscreteMassFunction m1 = new DiscreteMassFunction();
            m1.AddMass(new DiscreteElement(2, 1), 0.1);
            m1.AddMass(new DiscreteElement(2, 2), 0.3);
            m1.AddMass(new DiscreteElement(2, 3), 0.6);
            m1.AddMass(new DiscreteElement(2, 3), 0.4);

            Console.WriteLine("\n\n" +
                "+--------------------------------------------------------------------+\n" +
                "|                      DiscreteMassFunction Tests                    |\n" +
                "+--------------------------------------------------------------------+"
            );
            Console.WriteLine(refList);
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine(m1);
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine(m1.ToString(refList));
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Removing 0.4 to the total ignorance.");
            m1.RemoveMass(DiscreteElement.GetCompleteElement(2), 0.4);
            Console.WriteLine(m1.ToString(refList));
            Console.WriteLine("----------------------------------------------------------------------");
            DiscreteElement yes = new DiscreteElement(2, 1);
            DiscreteElement no = new DiscreteElement(2, 2);
            DiscreteElement yesUNo = new DiscreteElement(2, 3);

            Console.WriteLine("Belief tests:");
            Console.WriteLine("bel({0}) = {1} (should be 0.1)", yes, m1.Bel(yes));
            Console.WriteLine("bel({0}) = {1} (should be 0.3)", no, m1.Bel(no));
            Console.WriteLine("bel({0}) = {1} (should be 1.0)", yesUNo, m1.Bel(yesUNo));
            Console.WriteLine("----------------------------------------------------------------------");

            Console.WriteLine("BetP tests:");
            Console.WriteLine("BetP({0}) = {1} (should be 0.4)", yes, m1.BetP(yes));
            Console.WriteLine("BetP({0}) = {1} (should be 0.6)", no, m1.BetP(no));
            Console.WriteLine("BetP({0}) = {1} (should be 1.0)", yesUNo, m1.BetP(yesUNo));
            Console.WriteLine("----------------------------------------------------------------------");

            Console.WriteLine("Plausibility tests:");
            Console.WriteLine("pl({0}) = {1} (should be 0.7)", yes, m1.Pl(yes));
            Console.WriteLine("pl({0}) = {1} (should be 0.9)", no, m1.Pl(no));
            Console.WriteLine("pl({0}) = {1} (should be 1.0)", yesUNo, m1.Pl(yesUNo));
            Console.WriteLine("----------------------------------------------------------------------");

            Console.WriteLine("Commonality tests:");
            Console.WriteLine("q({0}) = {1} (should be 0.7)", yes, m1.Q(yes));
            Console.WriteLine("q({0}) = {1} (should be 0.9)", no, m1.Q(no));
            Console.WriteLine("q({0}) = {1} (should be 0.6)", yesUNo, m1.Q(yesUNo));
            Console.WriteLine("----------------------------------------------------------------------");

            Console.WriteLine("Conditioning test:");
            Console.WriteLine(m1.ToString(refList));
            Console.WriteLine("Conditioning by {0}...", yes.ToString(refList));
            m1 = m1.Conditioning(yes);
            Console.WriteLine(m1.ToString(refList));
            Console.WriteLine("----------------------------------------------------------------------");

            Console.WriteLine("Conditioning test:");
            Console.WriteLine(m1.ToString(refList));
            Console.WriteLine("Conditioning by {0}...", no.ToString(refList));
            m1 = m1.Conditioning(no);
            Console.WriteLine(m1.ToString(refList));
            Console.WriteLine("----------------------------------------------------------------------");

            m1 = new DiscreteMassFunction();
            m1.AddMass(new DiscreteElement(2, 1), 0.1);
            m1.AddMass(new DiscreteElement(2, 2), 0.3);
            m1.AddMass(new DiscreteElement(2, 3), 0.6);

            Console.WriteLine("Conditioning test:");
            Console.WriteLine(m1.ToString(refList));
            Console.WriteLine("Conditioning by {0}...", no.ToString(refList));
            m1 = m1.Conditioning(no);
            Console.WriteLine(m1.ToString(refList));
            Console.WriteLine("----------------------------------------------------------------------");

            Console.WriteLine("Conditioning test:");
            Console.WriteLine(m1.ToString(refList));
            Console.WriteLine("Conditioning by {0}...", yes.ToString(refList));
            m1 = m1.Conditioning(yes);
            Console.WriteLine(m1.ToString(refList));
            Console.WriteLine("----------------------------------------------------------------------");

            m1 = new DiscreteMassFunction();
            m1.AddMass(new DiscreteElement(2, 1), 0.1);
            m1.AddMass(new DiscreteElement(2, 2), 0.3);
            m1.AddMass(new DiscreteElement(2, 3), 0.6);

            Console.WriteLine("Weakening test:");
            Console.WriteLine(m1.ToString(refList));
            m1 = m1.Weakening(0.1);
            Console.WriteLine("Weakened by with alpha = 0.1");
            Console.WriteLine(m1.ToString(refList));
            Console.WriteLine("----------------------------------------------------------------------");

            m1 = new DiscreteMassFunction();
            m1.AddMass(new DiscreteElement(2, 1), 0.1);
            m1.AddMass(new DiscreteElement(2, 2), 0.3);
            m1.AddMass(new DiscreteElement(2, 3), 0.6);

            Console.WriteLine("Discounting test:");
            Console.WriteLine(m1.ToString(refList));
            m1 = m1.Discounting(0.1);
            Console.WriteLine("Disounting by with alpha = 0.1");
            Console.WriteLine(m1.ToString(refList));
            Console.WriteLine("----------------------------------------------------------------------");

            m1 = new DiscreteMassFunction();
            m1.AddMass(new DiscreteElement(2, 1), 0.0);
            m1.AddMass(new DiscreteElement(2, 2), 0.3);
            m1.AddMass(new DiscreteElement(2, 3), 0.7);

            Console.WriteLine("Cleaning test:");
            Console.WriteLine(m1.ToString(refList));
            m1.Clean();
            Console.WriteLine("After cleaning:");
            Console.WriteLine(m1.ToString(refList));
            Console.WriteLine("----------------------------------------------------------------------");

            m1 = new DiscreteMassFunction();
            m1.AddMass(new DiscreteElement(2, 1), 0.33);
            m1.AddMass(new DiscreteElement(2, 2), 0.66);
            m1.AddMass(new DiscreteElement(2, 3), 1.01);

            Console.WriteLine("Normalization test");
            Console.WriteLine(m1.ToString(refList));
            m1.Normalise();
            Console.WriteLine("After normalization:");
            Console.WriteLine(m1.ToString(refList));
            Console.WriteLine("----------------------------------------------------------------------");

            m1 = new DiscreteMassFunction();
            m1.AddMass(new DiscreteElement(2, 1), 0.2);
            m1.AddMass(new DiscreteElement(2, 2), 0.2);
            m1.AddMass(new DiscreteElement(2, 3), 0.6);

            DiscreteMassFunction m2 = new DiscreteMassFunction();
            m2.AddMass(new DiscreteElement(2, 1), 0.2);
            m2.AddMass(new DiscreteElement(2, 2), 0.6);
            m2.AddMass(new DiscreteElement(2, 3), 0.2);

            Console.WriteLine("Smets combination test:");
            Console.WriteLine("m1: {0}\nm2: {1}", m1, m2);
            Console.WriteLine("Combination:\n{0}", m1.CombinationSmets(m2));
            Console.WriteLine("----------------------------------------------------------------------");

            DiscreteMassFunction m3 = new DiscreteMassFunction();
            m3.AddMass(new DiscreteElement(2, 1), 0.8);
            m3.AddMass(new DiscreteElement(2, 2), 0.2);

            Console.WriteLine("Smets combination test:");
            Console.WriteLine("Adding a mass function: {0}", m3);
            Console.WriteLine("Combination:\n{0}", m1.CombinationSmets(m2, m3));
            Console.WriteLine("----------------------------------------------------------------------");

            Console.WriteLine("Dempster combination test:");
            Console.WriteLine("m1: {0}\nm2: {1}", m1, m2);
            Console.WriteLine("Combination:\n{0}", m1.CombinationDempster(m2));
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Dempster combination test:");
            Console.WriteLine("Adding a mass function: {0}", m3);
            Console.WriteLine("Combination:\n{0}", m1.CombinationDempster(m2, m3));
            Console.WriteLine("----------------------------------------------------------------------");

            Console.WriteLine("Disjunctive combination test:");
            Console.WriteLine("m1: {0}\nm2: {1}", m1, m2);
            Console.WriteLine("Combination:\n{0}", m1.CombinationDisjunctive(m2));
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Disjunctive combination test:");
            Console.WriteLine("Adding a mass function: {0}", m3);
            Console.WriteLine("Combination:\n{0}", m1.CombinationDisjunctive(m2, m3));
            Console.WriteLine("----------------------------------------------------------------------");

            Console.WriteLine("Yager combination test:");
            Console.WriteLine("m1: {0}\nm2: {1}", m1, m2);
            Console.WriteLine("Combination:\n{0}", m1.CombinationYager(m2));
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Yager combination test:");
            Console.WriteLine("Adding a mass function: {0}", m3);
            Console.WriteLine("Combination:\n{0}", m1.CombinationYager(m2, m3));
            Console.WriteLine("----------------------------------------------------------------------");

            Console.WriteLine("Average combination test:");
            Console.WriteLine("m1: {0}\nm2: {1}", m1, m2);
            Console.WriteLine("Combination:\n{0}", m1.CombinationAverage(m2));
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Average combination test:");
            Console.WriteLine("Adding a mass function: {0}", m3);
            Console.WriteLine("Combination:\n{0}", m1.CombinationAverage(m2, m3));
            Console.WriteLine("----------------------------------------------------------------------");

            Console.WriteLine("Murphy combination test:");
            Console.WriteLine("m1: {0}\nm2: {1}", m1, m2);
            Console.WriteLine("Combination:\n{0}", m1.CombinationMurphy(m2));
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Murphy combination test:");
            Console.WriteLine("Adding a mass function: {0}", m3);
            Console.WriteLine("Combination:\n{0}", m1.CombinationMurphy(m2, m3));
            Console.WriteLine("----------------------------------------------------------------------");

            Console.WriteLine("Chen combination test:");
            Console.WriteLine("m1: {0}\nm2: {1}", m1, m2);
            Console.WriteLine("Combination:\n{0}", m1.CombinationChen(m2));
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Chen combination test:");
            Console.WriteLine("Adding a mass function: {0}", m3);
            Console.WriteLine("Combination:\n{0}", m1.CombinationChen(m2, m3));
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("GetTreeOfFocals test with previous mass functions:");
            List<List<DiscreteElement>> tree = DiscreteMassFunction.GetTreeOfFocals(m1, m2);
            foreach (List<DiscreteElement> list in tree)
            {
                Console.Write("< ");
                foreach (DiscreteElement e in list)
                {
                    Console.Write("{0} ", e);
                }
                Console.Write(">\n");
            }
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("DuboisPrade combination test:");
            Console.WriteLine("m1: {0}\nm2: {1}", m1, m2);
            Console.WriteLine("Combination:\n{0}", m1.CombinationDuboisPrade(m2));
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("GetTreeOfFocals test with previous mass functions:");
            tree = DiscreteMassFunction.GetTreeOfFocals(m1, m2, m3);
            foreach (List<DiscreteElement> list in tree)
            {
                Console.Write("< ");
                foreach (DiscreteElement e in list)
                {
                    Console.Write("{0} ", e);
                }
                Console.Write(">\n");
            }
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("DuboisPrade combination test:");
            Console.WriteLine("Adding a mass function: {0}", m3);
            Console.WriteLine("Combination:\n{0}", m1.CombinationDuboisPrade(m2, m3));
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Distance trivial test:\n{0}", m1);
            Console.WriteLine("Distance with itself: {0}", m1.Distance(m1));
            Console.WriteLine("----------------------------------------------------------------------");
            DiscreteMassFunction trivial1 = new DiscreteMassFunction();
            trivial1.AddMass(new DiscreteElement(3, 0), 1);
            DiscreteMassFunction trivial2 = new DiscreteMassFunction();
            trivial2.AddMass(new DiscreteElement(3, 7), 1);
            Console.WriteLine("Distance trivial test 2:\n{0}\nand {1}", trivial1, trivial2);
            Console.WriteLine("Distance between both: {0}", trivial1.Distance(trivial2));
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Distance test:\n{0}\n{1}", m1, m2);
            Console.WriteLine("Distance between both: {0}", m1.Distance(m2));
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Distance test:\n{0}\n{1}", m1, m2);
            Console.WriteLine("Distance between both + m1: {0}", m1.Distance(m1, m2));
            Console.WriteLine("----------------------------------------------------------------------");

            Console.WriteLine("Auto-conflict test:\n{0}", m1);
            Console.WriteLine("Auto-conflict:\nDegree 1: {0}", m1.AutoConflict(1));
            Console.WriteLine("Degree 2: {0}", m1.AutoConflict(2));
            Console.WriteLine("Degree 3: {0}", m1.AutoConflict(3));
            Console.WriteLine("Degree 4: {0}", m1.AutoConflict(4));
            Console.WriteLine("Degree 5: {0}", m1.AutoConflict(5));
            Console.WriteLine("----------------------------------------------------------------------");

            Console.WriteLine("GetMax test:");
            Console.WriteLine(m1.ToString(refList));
            Console.WriteLine();
            List<FocalElement<DiscreteElement>> max;

            max = m1.GetMax(m1.M, 1);
            Console.WriteLine("Max of m(), maxCard = 1:");
            foreach (FocalElement<DiscreteElement> fe in max)
            {
                Console.WriteLine("Element: {0}, Value: {1}", fe.Element.ToString(refList), fe.Value);
            }
            Console.WriteLine();

            max = m1.GetMax(m1.M, 2);
            Console.WriteLine("Max of m(), maxCard = 2:");
            foreach (FocalElement<DiscreteElement> fe in max)
            {
                Console.WriteLine("Element: {0}, Value: {1}", fe.Element.ToString(refList), fe.Value);
            }
            Console.WriteLine();

            max = m1.GetMax(m1.Bel, 1);
            Console.WriteLine("Max of bel(), maxCard = 1:");
            foreach (FocalElement<DiscreteElement> fe in max)
            {
                Console.WriteLine("Element: {0}, Value: {1}", fe.Element.ToString(refList), fe.Value);
            }
            Console.WriteLine();

            max = m1.GetMax(m1.Bel, 2);
            Console.WriteLine("Max of bel(), maxCard = 2:");
            foreach (FocalElement<DiscreteElement> fe in max)
            {
                Console.WriteLine("Element: {0}, Value: {1}", fe.Element.ToString(refList), fe.Value);
            }
            Console.WriteLine();

            max = m1.GetMax(m1.BetP, 1);
            Console.WriteLine("Max of BetP(), maxCard = 1:");
            foreach (FocalElement<DiscreteElement> fe in max)
            {
                Console.WriteLine("Element: {0}, Value: {1}", fe.Element.ToString(refList), fe.Value);
            }
            Console.WriteLine();

            max = m1.GetMax(m1.BetP, 2);
            Console.WriteLine("Max of BetP(), maxCard = 2:");
            foreach (FocalElement<DiscreteElement> fe in max)
            {
                Console.WriteLine("Element: {0}, Value: {1}", fe.Element.ToString(refList), fe.Value);
            }
            Console.WriteLine();

            max = m1.GetMax(m1.Pl, 1);
            Console.WriteLine("Max of pl(), maxCard = 1:");
            foreach (FocalElement<DiscreteElement> fe in max)
            {
                Console.WriteLine("Element: {0}, Value: {1}", fe.Element.ToString(refList), fe.Value);
            }
            Console.WriteLine();

            max = m1.GetMax(m1.Pl, 2);
            Console.WriteLine("Max of pl(), maxCard = 2:");
            foreach (FocalElement<DiscreteElement> fe in max)
            {
                Console.WriteLine("Element: {0}, Value: {1}", fe.Element.ToString(refList), fe.Value);
            }
            Console.WriteLine();

            max = m1.GetMax(m1.Q, 1);
            Console.WriteLine("Max of q(), maxCard = 1:");
            foreach (FocalElement<DiscreteElement> fe in max)
            {
                Console.WriteLine("Element: {0}, Value: {1}", fe.Element.ToString(refList), fe.Value);
            }
            Console.WriteLine();

            max = m1.GetMax(m1.Q, 2);
            Console.WriteLine("Max of q(), maxCard = 2:");
            foreach (FocalElement<DiscreteElement> fe in max)
            {
                Console.WriteLine("Element: {0}, Value: {1}", fe.Element.ToString(refList), fe.Value);
            }
            Console.WriteLine("----------------------------------------------------------------------");

            Console.WriteLine("GetMin test:");
            Console.WriteLine(m1.ToString(refList));
            Console.WriteLine();
            List<FocalElement<DiscreteElement>> min;

            min = m1.GetMin(m1.M, 1);
            Console.WriteLine("Min of m(), maxCard = 1:");
            foreach (FocalElement<DiscreteElement> fe in min)
            {
                Console.WriteLine("Element: {0}, Value: {1}", fe.Element.ToString(refList), fe.Value);
            }
            Console.WriteLine();

            min = m1.GetMin(m1.M, 2);
            Console.WriteLine("Min of m(), maxCard = 2:");
            foreach (FocalElement<DiscreteElement> fe in min)
            {
                Console.WriteLine("Element: {0}, Value: {1}", fe.Element.ToString(refList), fe.Value);
            }
            Console.WriteLine();

            min = m1.GetMin(m1.Bel, 1);
            Console.WriteLine("Min of bel(), maxCard = 1:");
            foreach (FocalElement<DiscreteElement> fe in min)
            {
                Console.WriteLine("Element: {0}, Value: {1}", fe.Element.ToString(refList), fe.Value);
            }
            Console.WriteLine();

            min = m1.GetMin(m1.Bel, 2);
            Console.WriteLine("Min of bel(), maxCard = 2:");
            foreach (FocalElement<DiscreteElement> fe in min)
            {
                Console.WriteLine("Element: {0}, Value: {1}", fe.Element.ToString(refList), fe.Value);
            }
            Console.WriteLine();

            min = m1.GetMin(m1.BetP, 1);
            Console.WriteLine("Min of BetP(), maxCard = 1:");
            foreach (FocalElement<DiscreteElement> fe in min)
            {
                Console.WriteLine("Element: {0}, Value: {1}", fe.Element.ToString(refList), fe.Value);
            }
            Console.WriteLine();

            min = m1.GetMin(m1.BetP, 2);
            Console.WriteLine("Min of BetP(), maxCard = 2:");
            foreach (FocalElement<DiscreteElement> fe in min)
            {
                Console.WriteLine("Element: {0}, Value: {1}", fe.Element.ToString(refList), fe.Value);
            }
            Console.WriteLine();

            min = m1.GetMin(m1.Pl, 1);
            Console.WriteLine("Min of pl(), maxCard = 1:");
            foreach (FocalElement<DiscreteElement> fe in min)
            {
                Console.WriteLine("Element: {0}, Value: {1}", fe.Element.ToString(refList), fe.Value);
            }
            Console.WriteLine();

            min = m1.GetMin(m1.Pl, 2);
            Console.WriteLine("Min of pl(), maxCard = 2:");
            foreach (FocalElement<DiscreteElement> fe in min)
            {
                Console.WriteLine("Element: {0}, Value: {1}", fe.Element.ToString(refList), fe.Value);
            }
            Console.WriteLine();

            min = m1.GetMin(m1.Q, 1);
            Console.WriteLine("Min of q(), maxCard = 1:");
            foreach (FocalElement<DiscreteElement> fe in min)
            {
                Console.WriteLine("Element: {0}, Value: {1}", fe.Element.ToString(refList), fe.Value);
            }
            Console.WriteLine();

            min = m1.GetMin(m1.Q, 2);
            Console.WriteLine("Min of q(), maxCard = 2:");
            foreach (FocalElement<DiscreteElement> fe in min)
            {
                Console.WriteLine("Element: {0}, Value: {1}", fe.Element.ToString(refList), fe.Value);
            }
            Console.WriteLine("----------------------------------------------------------------------");
        }

    } //Class
} //Namespace

