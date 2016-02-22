using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using THEGAME.Core.IntervalStates;

namespace THEGAME.Tests.IntervalStates
{
    public static class IntervalMassFunctionTests
    {
        public static void TestCase()
        {
            IntervalMassFunction m1 = new IntervalMassFunction();
            IntervalElement e1 = new IntervalElement(new Interval(-1, 1));
            IntervalElement e2 = new IntervalElement(new Interval(-2, 2));
            IntervalElement e3 = new IntervalElement(new Interval(-3, 3));
            IntervalElement e4 = new IntervalElement(new Interval(-4, 4));
            IntervalElement e5 = new IntervalElement(new Interval(-1.5, 1.5));
            IntervalElement e6 = new IntervalElement(new Interval(-2.5, 2.5));
            IntervalElement e7 = new IntervalElement(new Interval(-3.5, 3.5));
            m1.AddMass(e1, 0.1);
            m1.AddMass(e2, 0.2);
            m1.AddMass(e3, 0.3);
            m1.AddMass(e4, 0.4);

            Console.WriteLine("\n\n" +
                "+--------------------------------------------------------------------+\n" +
                "|                      IntervalMassFunction Tests                    |\n" +
                "+--------------------------------------------------------------------+"
            );

            Console.WriteLine("Belief tests:");
            Console.WriteLine("bel({0}) = {1} (should be 0.1)", e1, m1.Bel(e1));
            Console.WriteLine("bel({0}) = {1} (should be 0.3)", e2, m1.Bel(e2));
            Console.WriteLine("bel({0}) = {1} (should be 0.6)", e3, m1.Bel(e3));
            Console.WriteLine("bel({0}) = {1} (should be 1.0)", e4, m1.Bel(e4));
            Console.WriteLine("bel({0}) = {1} (should be 0.1)", e5, m1.Bel(e5));
            Console.WriteLine("bel({0}) = {1} (should be 0.3)", e6, m1.Bel(e6));
            Console.WriteLine("bel({0}) = {1} (should be 0.6)", e7, m1.Bel(e7));
            Console.WriteLine("----------------------------------------------------------------------");

            Console.WriteLine("BetP tests:");
            Console.WriteLine("BetP({0}) = {1} (should be 0.4)", e1, m1.BetP(e1));
            Console.WriteLine("BetP({0}) = {1} (should be 0.4)", e2, m1.BetP(e2));
            Console.WriteLine("BetP({0}) = {1} (should be 0.4)", e3, m1.BetP(e3));
            Console.WriteLine("BetP({0}) = {1} (should be 0.4)", e4, m1.BetP(e4));
            Console.WriteLine("BetP({0}) = {1} (should be 0.4)", e5, m1.BetP(e5));
            Console.WriteLine("BetP({0}) = {1} (should be 0.4)", e6, m1.BetP(e6));
            Console.WriteLine("BetP({0}) = {1} (should be 0.4)", e7, m1.BetP(e7));
            Console.WriteLine("----------------------------------------------------------------------");

            Console.WriteLine("Plausibility tests:");
            Console.WriteLine("pl({0}) = {1} (should be 1.0)", e1, m1.Pl(e1));
            Console.WriteLine("pl({0}) = {1} (should be 1.0)", e2, m1.Pl(e2));
            Console.WriteLine("pl({0}) = {1} (should be 1.0)", e3, m1.Pl(e3));
            Console.WriteLine("pl({0}) = {1} (should be 1.0)", e4, m1.Pl(e4));
            Console.WriteLine("pl({0}) = {1} (should be 1.0)", e5, m1.Pl(e5));
            Console.WriteLine("pl({0}) = {1} (should be 1.0)", e6, m1.Pl(e6));
            Console.WriteLine("pl({0}) = {1} (should be 1.0)", e7, m1.Pl(e7));
            Console.WriteLine("----------------------------------------------------------------------");

            Console.WriteLine("Commonality tests:");
            Console.WriteLine("q({0}) = {1} (should be 1.0)", e1, m1.Q(e1));
            Console.WriteLine("q({0}) = {1} (should be 0.9)", e2, m1.Q(e2));
            Console.WriteLine("q({0}) = {1} (should be 0.7)", e3, m1.Q(e3));
            Console.WriteLine("q({0}) = {1} (should be 0.4)", e4, m1.Q(e4));
            Console.WriteLine("q({0}) = {1} (should be 0.9)", e5, m1.Q(e5));
            Console.WriteLine("q({0}) = {1} (should be 0.7)", e6, m1.Q(e6));
            Console.WriteLine("q({0}) = {1} (should be 0.4)", e7, m1.Q(e7));
            Console.WriteLine("----------------------------------------------------------------------");
        }

    } //Class
} //Namespace
