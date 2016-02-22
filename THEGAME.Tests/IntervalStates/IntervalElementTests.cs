using System;

using THEGAME.Core.IntervalStates;

namespace THEGAME.Tests.IntervalStates
{
    public static class IntervalElementTests
    {

        public static void TestCase()
        {
            Console.WriteLine("\n\n" +
                "+--------------------------------------------------------------------+\n" +
                "|                      IntervalElement Tests                         |\n" +
                "+--------------------------------------------------------------------+"
            );

            IntervalElement e0 = new IntervalElement(new Interval(1, 4), new Interval(-1, 2));
            IntervalElement e1 = new IntervalElement(-5, 2);
            IntervalElement e2 = new IntervalElement(-4, 4);
            IntervalElement e3 = IntervalElement.StaticGetEmptyElement();
            IntervalElement e4 = IntervalElement.StaticGetCompleteElement();

            Console.WriteLine("{0}\nCard: {1}\n", e0, e0.Card);
            Console.WriteLine("{0}\nCard: {1}\n", e1, e1.Card);
            Console.WriteLine("{0}\nCard: {1}\n", e2, e2.Card);
            Console.WriteLine("{0}\nCard: {1}\n", e3, e3.Card);
            Console.WriteLine("{0}\nCard: {1}\n", e4, e4.Card);

            Console.WriteLine("----------------------------------------------------------------------");

            IntervalElement e5 = e1.Opposite();
            IntervalElement e6 = e2.Opposite();
            IntervalElement e7 = e3.Opposite();
            IntervalElement e8 = e4.Opposite();

            Console.WriteLine("{0}\nOpposite: {1}\n", e1, e5);
            Console.WriteLine("{0}\nOpposite: {1}\n", e2, e6);
            Console.WriteLine("{0}\nOpposite: {1}\n", e3, e7);
            Console.WriteLine("{0}\nOpposite: {1}\n", e4, e8);
            Console.WriteLine("{0}\nOpposite: {1}\n", e5, e5.Opposite());
            Console.WriteLine("{0}\nOpposite: {1}\n", e6, e6.Opposite());
            Console.WriteLine("{0}\nOpposite: {1}\n", e7, e7.Opposite());
            Console.WriteLine("{0}\nOpposite: {1}\n", e8, e8.Opposite());

            Console.WriteLine("----------------------------------------------------------------------");

            Console.WriteLine("{0} conjunction with {1} = {2}\n---------------\n", e1, e2, e1.Conjunction(e2));
            Console.WriteLine("{0} conjunction with {1} = {2}\n---------------\n", e2, e1, e2.Conjunction(e1));
            Console.WriteLine("{0} conjunction with {1} = {2}\n---------------\n", e1, e3, e1.Conjunction(e3));
            Console.WriteLine("{0} conjunction with {1} = {2}\n---------------\n", e3, e1, e3.Conjunction(e1));
            Console.WriteLine("{0} conjunction with {1} = {2}\n---------------\n", e1, e4, e1.Conjunction(e4));
            Console.WriteLine("{0} conjunction with {1} = {2}\n---------------\n", e4, e1, e4.Conjunction(e1));
            Console.WriteLine("{0} conjunction with {1} = {2}\n---------------\n", e1, e5, e1.Conjunction(e5));
            Console.WriteLine("{0} conjunction with {1} = {2}\n---------------\n", e5, e1, e5.Conjunction(e1));
            Console.WriteLine("{0} conjunction with {1} = {2}\n---------------\n", e1, e6, e1.Conjunction(e6));
            Console.WriteLine("{0} conjunction with {1} = {2}\n---------------\n", e6, e1, e6.Conjunction(e1));
            Console.WriteLine("{0} conjunction with {1} = {2}\n---------------\n", e1, e7, e1.Conjunction(e7));
            Console.WriteLine("{0} conjunction with {1} = {2}\n---------------\n", e7, e1, e7.Conjunction(e1));
            Console.WriteLine("{0} conjunction with {1} = {2}\n---------------\n", e1, e8, e1.Conjunction(e8));
            Console.WriteLine("{0} conjunction with {1} = {2}\n---------------\n", e8, e1, e8.Conjunction(e1));

            Console.WriteLine("{0} conjunction with {1} = {2}\n---------------\n", e2, e3, e2.Conjunction(e3));
            Console.WriteLine("{0} conjunction with {1} = {2}\n---------------\n", e2, e4, e2.Conjunction(e4));
            Console.WriteLine("{0} conjunction with {1} = {2}\n---------------\n", e2, e5, e2.Conjunction(e5));
            Console.WriteLine("{0} conjunction with {1} = {2}\n---------------\n", e2, e6, e2.Conjunction(e6));
            Console.WriteLine("{0} conjunction with {1} = {2}\n---------------\n", e2, e7, e2.Conjunction(e7));
            Console.WriteLine("{0} conjunction with {1} = {2}\n---------------\n", e2, e8, e2.Conjunction(e8));

            Console.WriteLine("{0} conjunction with {1} = {2}\n---------------\n", e3, e4, e3.Conjunction(e4));
            Console.WriteLine("{0} conjunction with {1} = {2}\n---------------\n", e3, e5, e3.Conjunction(e5));
            Console.WriteLine("{0} conjunction with {1} = {2}\n---------------\n", e3, e6, e3.Conjunction(e6));
            Console.WriteLine("{0} conjunction with {1} = {2}\n---------------\n", e3, e7, e3.Conjunction(e7));
            Console.WriteLine("{0} conjunction with {1} = {2}\n---------------\n", e3, e8, e3.Conjunction(e8));

            Console.WriteLine("----------------------------------------------------------------------");

            Console.WriteLine("{0} disjunction with {1} = {2}\n---------------\n", e1, e2, e1.Disjunction(e2));
            Console.WriteLine("{0} disjunction with {1} = {2}\n---------------\n", e1, e3, e1.Disjunction(e3));
            Console.WriteLine("{0} disjunction with {1} = {2}\n---------------\n", e1, e4, e1.Disjunction(e4));
            Console.WriteLine("{0} disjunction with {1} = {2}\n---------------\n", e1, e5, e1.Disjunction(e5));
            Console.WriteLine("{0} disjunction with {1} = {2}\n---------------\n", e1, e6, e1.Disjunction(e6));
            Console.WriteLine("{0} disjunction with {1} = {2}\n---------------\n", e1, e7, e1.Disjunction(e7));
            Console.WriteLine("{0} disjunction with {1} = {2}\n---------------\n", e1, e8, e1.Disjunction(e8));

            Console.WriteLine("{0} disjunction with {1} = {2}\n---------------\n", e2, e3, e2.Disjunction(e3));
            Console.WriteLine("{0} disjunction with {1} = {2}\n---------------\n", e2, e4, e2.Disjunction(e4));
            Console.WriteLine("{0} disjunction with {1} = {2}\n---------------\n", e2, e5, e2.Disjunction(e5));
            Console.WriteLine("{0} disjunction with {1} = {2}\n---------------\n", e2, e6, e2.Disjunction(e6));
            Console.WriteLine("{0} disjunction with {1} = {2}\n---------------\n", e2, e7, e2.Disjunction(e7));
            Console.WriteLine("{0} disjunction with {1} = {2}\n---------------\n", e2, e8, e2.Disjunction(e8));

            Console.WriteLine("{0} disjunction with {1} = {2}\n---------------\n", e3, e4, e3.Disjunction(e4));
            Console.WriteLine("{0} disjunction with {1} = {2}\n---------------\n", e3, e5, e3.Disjunction(e5));
            Console.WriteLine("{0} disjunction with {1} = {2}\n---------------\n", e3, e6, e3.Disjunction(e6));
            Console.WriteLine("{0} disjunction with {1} = {2}\n---------------\n", e3, e7, e3.Disjunction(e7));
            Console.WriteLine("{0} disjunction with {1} = {2}\n---------------\n", e3, e8, e3.Disjunction(e8));

            Console.WriteLine("----------------------------------------------------------------------");

            Console.WriteLine("{0} is a subset of {1} = {2}\n---------------\n", e0, e1, e0.IsASubset(e1));
            Console.WriteLine("{0} is a subset of {1} = {2}\n---------------\n", e0, e2, e0.IsASubset(e2));
            Console.WriteLine("{0} is a subset of {1} = {2}\n---------------\n", e0, e3, e0.IsASubset(e3));
            Console.WriteLine("{0} is a subset of {1} = {2}\n---------------\n", e0, e4, e0.IsASubset(e4));
            Console.WriteLine("{0} is a subset of {1} = {2}\n---------------\n", e0, e5, e0.IsASubset(e5));
            Console.WriteLine("{0} is a subset of {1} = {2}\n---------------\n", e0, e6, e0.IsASubset(e6));
            Console.WriteLine("{0} is a subset of {1} = {2}\n---------------\n", e0, e7, e0.IsASubset(e7));

            Console.WriteLine("{0} is a subset of {1} = {2}\n---------------\n", e1, e2, e1.IsASubset(e2));
            Console.WriteLine("{0} is a subset of {1} = {2}\n---------------\n", e1, e3, e1.IsASubset(e3));
            Console.WriteLine("{0} is a subset of {1} = {2}\n---------------\n", e1, e4, e1.IsASubset(e4));
            Console.WriteLine("{0} is a subset of {1} = {2}\n---------------\n", e1, e5, e1.IsASubset(e5));
            Console.WriteLine("{0} is a subset of {1} = {2}\n---------------\n", e1, e6, e1.IsASubset(e6));
            Console.WriteLine("{0} is a subset of {1} = {2}\n---------------\n", e1, e7, e1.IsASubset(e7));
            Console.WriteLine("{0} is a subset of {1} = {2}\n---------------\n", e1, e8, e1.IsASubset(e8));

            Console.WriteLine("{0} is a subset of {1} = {2}\n---------------\n", e2, e1, e2.IsASubset(e1));
            Console.WriteLine("{0} is a subset of {1} = {2}\n---------------\n", e2, e3, e2.IsASubset(e3));
            Console.WriteLine("{0} is a subset of {1} = {2}\n---------------\n", e2, e4, e2.IsASubset(e4));
            Console.WriteLine("{0} is a subset of {1} = {2}\n---------------\n", e2, e5, e2.IsASubset(e5));
            Console.WriteLine("{0} is a subset of {1} = {2}\n---------------\n", e2, e6, e2.IsASubset(e6));
            Console.WriteLine("{0} is a subset of {1} = {2}\n---------------\n", e2, e7, e2.IsASubset(e7));
            Console.WriteLine("{0} is a subset of {1} = {2}\n---------------\n", e2, e8, e2.IsASubset(e8));
        }

    } //Class
} //Namespace
