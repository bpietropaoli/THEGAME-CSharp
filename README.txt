+---------------------------------------------------+
|                                                   |
| +-----------------------------------------------+ |
| |                                               | |
| |                  THE (GAME)#                  | |
| |                                               | |
| +-----------------------------------------------+ |
|                                                   |
+---------------------------------------------------+

Author : Bastien Pietropaoli
Contact: Bastien.Pietropaoli@cit.ie / Bastien.Pietropaoli@gmail.com


THeory of Evidence (in a lanGuage Adapted for Many Embedded systems):
---------------------------------------------------------------------
This library is a C# equivalent of the library called THE GAME 
(http://bpietropaoli.github.io/THEGAME/). Both libraries can use the
same models, either in XML, either as a set of directories. This library
can also save the models if they were modified.


THE (GAME)# provides the basics for the belief functions theory (also called 
theory of evidence, or Dempster-Shafer's theory). The library provided
here was made to be as generic as possible. The idea is that belief 
functions can be applied to any element that implements the basic
set-theoretic operations (conjunction, disjunction, etc). The main
belief functions methods are implemented calling abstract methods.
Thus, if you want to apply belief functions to a new type of elements,
it should be as easy as implementing the set-theoretic operations
for your type of elements. For examples on how to do so, look at
the two examples provided in THEGAME/Core/DiscreteStates (the classic
theory) and THEGAME/Core/IntervalStates (for belief functions applied
to intervals of values). If you look at it, elements are implemented
entirely to provide set-theoretic operations (but hey, that's not the
most complicated part to implement!) and mass functions are almost 
not reimplemented at all as the methods in the abstract class AMassFunction
are totally generic.


The classic theory is implemented within THEGAME/Core/DiscreteIntervals.
It uses internal structures to optimise the computation of set-theoretic
operations.
IF YOU DON'T WANT TECHNICAL DETAILS, DON'T READ THE FOLLOWING LINES:
The focal elements are represented as integers and binary operators
are used to perform conjunctions and disjunctions. The cardinal of the
focal element is thus hard to obtain, by default, it's set to -1 and
computed only once if required. Those small things save a loooot of
computation time for most of the operations. Any size of frame of 
discerment can be used. Be aware though that some operations may
take a lot of time if performed on huge frames of discernment. It is
usually indicated in the documentation of methods if it might hurt
your computer.


The library provides:
 - Abstract classes to perform the basic operations on belief functions
   (in THEGAME/Core/Generic).
 - The classic theory operations (in THEGAME/Core/DiscreteStates).
 - An application to intervals (in THEGAME/Core/IntervalStates).
 - Methods to build discrete mass functions from sensor data (in 
   THEGAME/Construction/DiscreteStates/FromSensors)
 - Methods to build discrete mass functions randomly (in
   THEGAME/Construction/DiscreteStats/FromRandomness)
 - Methods to propagate discrete mass functions from one frame of
   discernment to another (in THEGAME/Construction/DiscreteStates/FromBeliefs).
 - A set of tests that everything works correctly (in THEGAME.Tests)


The library is meant to be as constrained as possible by raising tons of exceptions
anytime something bad is attempted. THEGAME/Exceptions provides all the exceptions
that can be raised. The methods documentation should always contain indications of
the exceptions that can be raised.


Documentation is provided within the code as XML comments following the
C# documentation format. If configured correctly, the compiler automatically
generates the XML files with the DLLs which enable IntelliSense doc to pop
automatically when writing code.
The HTML documentation can be generated using external tools such as Doxygen
(it does not officially recognise XML documentation but it does work).
