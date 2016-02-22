using System;

using THEGAME.Core.DiscreteStates;

namespace THEGAME.Tests.DiscreteStates
{
    public static class StringReferenceListTests
    {
        public static void TestCase()
        {
            StringReferenceList refList = new StringReferenceList("Yes", "No");
            StringReferenceList refList2 = new StringReferenceList("Resources/B-eliefsFromSensors/test/values");

            Console.WriteLine("\n\n" +
                "+--------------------------------------------------------------------+\n" +
                "|                     StringReferenceList Tests                      |\n" +
                "+--------------------------------------------------------------------+"
            );
            Console.WriteLine("\n\n----------------------------------------------------------------------");
            Console.WriteLine("Construction tests:");
            Console.WriteLine(refList);
            Console.WriteLine(refList2);
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("refList2.Contains(\"Aka\") = {0}", refList2.Contains("Aka"));
            Console.WriteLine("refList2.Contains(\"Yes\") = {0}", refList2.Contains("Yes"));
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("refList2.GetIndex(\"Aka\") = {0}", refList2.IndexOf("Aka"));
            Console.WriteLine("refList2.GetIndex(\"Coq\") = {0}", refList2.IndexOf("Coq"));
            Console.WriteLine("refList2.GetIndex(\"Yes\") = {0}", refList2.IndexOf("Yes"));
            Console.WriteLine("----------------------------------------------------------------------");
        }

    } //Class
} //Namespace

