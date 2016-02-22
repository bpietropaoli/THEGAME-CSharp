using System;

using THEGAME.Core.IntervalStates;

namespace THEGAME.Tests.IntervalStates
{
    public static class IntervalTests
    {

        public static void TestCase(){

            Console.WriteLine("\n\n" +
                "+--------------------------------------------------------------------+\n" +
                "|                          Interval Tests                            |\n" +
                "+--------------------------------------------------------------------+"
            );
            Interval i1 = new Interval(0, 3);
            Interval i2 = new Interval(-5, -2);
            Interval i3 = new Interval(-5, 4);
            Interval i4 = new Interval(Double.NegativeInfinity, -3);

            Console.WriteLine("Interval: {0} of size {1}", i1, i1.Size);
            Console.WriteLine("Interval: {0} of size {1}", i2, i2.Size);
            Console.WriteLine("Interval: {0} of size {1}", i3, i3.Size);
            Console.WriteLine("Interval: {0} of size {1}", i4, i4.Size);

            Console.WriteLine("----------------------------------------------------------------------");

            Console.WriteLine("{0} <= {1} : {2}", i1, i2, i1 <= i2);
            Console.WriteLine("{0} <= {1} : {2}", i1, i3, i1 <= i3);
            Console.WriteLine("{0} <= {1} : {2}", i1, i4, i1 <= i4);
            Console.WriteLine("{0} <= {1} : {2}", i2, i3, i2 <= i3);
            Console.WriteLine("{0} <= {1} : {2}", i2, i4, i2 <= i4);
            Console.WriteLine("{0} <= {1} : {2}", i3, i4, i3 <= i4);

            Console.WriteLine("----------------------------------------------------------------------");

            Console.WriteLine("Interval {0} and {1} overlap: {2}", i1, i2, i1.Overlap(i2));
            Console.WriteLine("Interval {0} and {1} overlap: {2}", i1, i3, i1.Overlap(i3));
            Console.WriteLine("Interval {0} and {1} overlap: {2}", i1, i4, i1.Overlap(i4));
            Console.WriteLine("Interval {0} and {1} overlap: {2}", i2, i3, i2.Overlap(i3));
            Console.WriteLine("Interval {0} and {1} overlap: {2}", i2, i4, i2.Overlap(i4));
            Console.WriteLine("Interval {0} and {1} overlap: {2}", i3, i4, i3.Overlap(i4));

            Console.WriteLine("----------------------------------------------------------------------");

            Console.WriteLine("Interval {0} conjunction with {1}: {2}", i1, i2, i1.Conjunction(i2));
            Console.WriteLine("Interval {0} conjunction with {1}: {2}", i1, i3, i1.Conjunction(i3));
            Console.WriteLine("Interval {0} conjunction with {1}: {2}", i1, i4, i1.Conjunction(i4));
            Console.WriteLine("Interval {0} conjunction with {1}: {2}", i2, i3, i2.Conjunction(i3));
            Console.WriteLine("Interval {0} conjunction with {1}: {2}", i2, i4, i2.Conjunction(i4));
            Console.WriteLine("Interval {0} conjunction with {1}: {2}", i3, i4, i3.Conjunction(i4));

            Console.WriteLine("----------------------------------------------------------------------");

            Console.WriteLine("Interval {0} contains {1}: {2}", i1, i2, i1.Contains(i2));
            Console.WriteLine("Interval {0} contains {1}: {2}", i1, i3, i1.Contains(i3));
            Console.WriteLine("Interval {0} contains {1}: {2}", i1, i4, i1.Contains(i4));
            Console.WriteLine("Interval {0} contains {1}: {2}", i2, i1, i2.Contains(i1));
            Console.WriteLine("Interval {0} contains {1}: {2}", i2, i3, i2.Contains(i3));
            Console.WriteLine("Interval {0} contains {1}: {2}", i2, i4, i2.Contains(i4));
            Console.WriteLine("Interval {0} contains {1}: {2}", i3, i1, i3.Contains(i1));
            Console.WriteLine("Interval {0} contains {1}: {2}", i3, i2, i3.Contains(i2));
            Console.WriteLine("Interval {0} contains {1}: {2}", i3, i4, i3.Contains(i4));
            Console.WriteLine("Interval {0} contains {1}: {2}", i4, i1, i4.Contains(i1));
            Console.WriteLine("Interval {0} contains {1}: {2}", i4, i2, i4.Contains(i2));
            Console.WriteLine("Interval {0} contains {1}: {2}", i4, i3, i4.Contains(i3));


            Console.WriteLine("----------------------------------------------------------------------");

            i1 = new Interval(Double.NaN, 1);
            Console.WriteLine("Interval {0} is empty: {1}", i1, i1.IsEmpty());
            i1 = new Interval(Double.NegativeInfinity, Double.PositiveInfinity);
            Console.WriteLine("Interval {0} is complete: {1}", i1, i1.IsComplete());
        }

    } //Class
} //Namespace
