using System;

namespace THEGAME.Construction.Generic
{
    /// <summary>
    /// An interface the belief models should implement.
    /// </summary>
	public interface IBeliefModel
	{
        /// <summary>
        /// Loads a model from the given path with the specified format.
        /// </summary>
        /// <param name="path">Where to find the model.</param>
        /// <param name="mt">The format in which the model is encoded.</param>
		void LoadModel(string path, ModelFormat mt);

        /// <summary>
        /// Saves the current model at the specified path in the specified format.
        /// </summary>
        /// <param name="path">Where to save the model.</param>
        /// <param name="mt">The format in which the model should be encoded.</param>
		void SaveModel(string path, ModelFormat mt);

        /// <summary>
        /// Checks if the model is valid and will produce valid mass functions.
        /// </summary>
        /// <returns>Returns true if the model is valid, false otherwise.</returns>
		bool IsValid();

	} //Interface
} //Namespace



