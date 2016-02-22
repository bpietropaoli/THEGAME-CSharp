using System;
using THEGAME.Core.Generic;

namespace THEGAME.Construction.Generic
{
    /// <summary>
    /// A simple structure to associate a name to a mass function. Can be used as an input to the models.
    /// </summary>
    /// <typeparam name="TFunction">The type of mass function (the implementation) to use.</typeparam>
    /// <typeparam name="TElement">The type of element (the implementation) used by the mass functions.</typeparam>
    public struct NamedMassFunction<TFunction, TElement> where TFunction : AMassFunction<TFunction, TElement>, new()
                                                                 where TElement : AElement<TElement> 
    {
        /// <summary>
        /// Gets/Sets the name associated to that mass function.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets/Sets the mass function itself.
        /// </summary>
        public TFunction Mass { get; set; }

        /// <summary>
        /// Basic constructor of a named mass function.
        /// </summary>
        /// <param name="name">The name to associate to the mass function.</param>
        /// <param name="mass">The mass function itself.</param>
        public NamedMassFunction(string name, TFunction mass)
            : this()
        {
            this.Name = name;
            this.Mass = mass;
        }

        /// <summary>
        /// Gives a string representation of the named mass function.
        /// </summary>
        /// <returns>Returns a string representation of the named mass function.</returns>
        public override string ToString()
        {
            return String.Format("{0}, {1}", Name, Mass);
        }
    }
}
