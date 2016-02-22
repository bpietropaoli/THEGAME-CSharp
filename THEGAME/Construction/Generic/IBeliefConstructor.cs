using System;
using System.Collections.Generic;

using THEGAME.Core.Generic;

namespace THEGAME.Construction.Generic
{
    /// <summary>
    /// An interface that the belief contructors should implement.
    /// </summary>
    /// <typeparam name="TFunction">The type of mass function (the implementation) to build.</typeparam>
    /// <typeparam name="TElement">The type of element (the implementation) associated to the type of mass function.</typeparam>
	public interface IBeliefConstructor<TFunction, TElement> where TFunction : AMassFunction<TFunction, TElement>, new()
                                                                     where TElement : AElement<TElement>
	{
        /// <summary>
        /// Builds a set of mass functions from the given parameters.
        /// </summary>
        /// <param name="obj">Parameters to condition the belief construction.</param>
        /// <returns>A list of mass functions built from the given parameters.</returns>
		List<TFunction> ConstructEvidence(params Object[] obj);

	} //Interface
} //Namespace

