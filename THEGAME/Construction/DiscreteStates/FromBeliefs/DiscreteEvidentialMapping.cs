using System;
using System.Text;
using System.Collections.Generic;

using THEGAME.Core.DiscreteStates;
using THEGAME.Core.Generic;
using THEGAME.Exceptions;

namespace THEGAME.Construction.DiscreteStates.FromBeliefs
{
    /// <summary>
    /// A structure to model an evidential mapping from one frame of discernement to another one.
    /// (This is basically a basic matrix).
    /// </summary>
    public struct DiscreteEvidentialMapping
    {
        /*
         * Members:
         */
        private string _frameName;
        private StringReferenceList _refList;
        private List<DiscreteMappingVector> _vectors;

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        /*
         * Properties:
         */

        /// <summary>
        /// The name of the frame from which belief will be propagated.
        /// </summary>
        public string FrameName
        {
            get { return _frameName; }
        }

        /// <summary>
        /// The reference list associated to the frame of discernment of origin.
        /// </summary>
        public StringReferenceList References
        {
            get { return _refList; }
        }

        /// <summary>
        /// The matrix to propagate belief.
        /// </summary>
        public List<DiscreteMappingVector> Matrix
        {
            get { return _vectors; }
        }

        /// <summary>
        /// The size of elements in the frame of discernment of origin.
        /// </summary>
        public int FromSize
        {
            get { return _refList.Count; }
        }

        /// <summary>
        /// The size of elements in the recipient frame of discernment.
        /// </summary>
        public int ToSize
        {
            get { return _vectors.Count > 0 ? _vectors[0].ToSize : -1; }
        }

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        /*
         * Constructors:
         */

        /// <summary>
        /// Builds an empty evidential mapping with just a frame name and the path to
        /// a reference list file.
        /// </summary>
        /// <param name="frameName">The name of the frame of discernment of origin.</param>
        /// <param name="refListPath">The reference list for this frame.</param>
        public DiscreteEvidentialMapping(string frameName, string refListPath)
            : this(frameName, new StringReferenceList(refListPath))
        {
        }

        /// <summary>
        /// Builds an empty evidential mapping with just a frame name and a reference list.
        /// </summary>
        /// <param name="frameName">The name of the frame of discernment of origin.</param>
        /// <param name="refList">The reference list for this frame.</param>
        public DiscreteEvidentialMapping(string frameName, StringReferenceList refList)
        {
            this._frameName = frameName;
            this._refList = refList;
            this._vectors = new List<DiscreteMappingVector>();
        }

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        /*
         * Methods:
         */

        /// <summary>
        /// Propagates the given mass function to the new frame of discernment using the evidential mapping.
        /// </summary>
        /// <param name="m">The mass function to propagate.</param>
        /// <returns>Returns a new mass function defined on the recipient frame of discernment.</returns>
        public DiscreteMassFunction GetEvidence(DiscreteMassFunction m)
        {
            DiscreteMassFunction evidence = new DiscreteMassFunction();
            foreach (FocalElement<DiscreteElement> e in m)
            {
                foreach (DiscreteMappingVector v in _vectors)
                {
                    if (e.Element.Equals(v.From))
                    {
                        DiscreteMassFunction partOfEvidence = v.GetEvidence(e.Value);
                        foreach (FocalElement<DiscreteElement> focal in partOfEvidence)
                        {
                            evidence.AddMass(focal.Element, focal.Value);
                        }
                    }
                }
            }
            return evidence;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Adds a mapping vector to the matrix.
        /// </summary>
        /// <param name="vector">The vector to add to the basic matrix.</param>
        /// <exception cref="InvalidBeliefModelException">Thrown if the given vector defines the propagation
        /// for an element already in the matrix.</exception>
        public void AddVector(DiscreteMappingVector vector)
        {
            if (this.Contains(vector))
            {
                throw new InvalidBeliefModelException("An Element to get the belief from appears twice in the model!");
            }
            _vectors.Add(vector);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Checks if the given vector is already contained in the basic matrix.
        /// </summary>
        /// <param name="vector">The vector to test.</param>
        /// <returns>Returns true if the given vector is already contained, false otherwise.</returns>
        public bool Contains(DiscreteMappingVector vector)
        {
            return _vectors.Contains(vector);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Checks if the evidential mapping will produce valid mass functions.
        /// </summary>
        /// <returns>Returns true if the evidential mapping will produce valid mass functions,
        /// false otherwise.</returns>
        public bool IsValid()
        {
            if (_vectors.Count == (1U << FromSize) - 1)
            {
                return false;
            }
            foreach (DiscreteMappingVector v in _vectors)
            {
                if (!v.IsValid())
                {
                    return false;
                }
            }
            return true;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gives a string representation of the current evidential mapping.
        /// </summary>
        /// <returns>Returns a string representation of the current evidential mapping.</returns>
        public override string ToString()
        {
            StringBuilder toReturn = new StringBuilder(String.Format("////    \\\\\\\\\nDiscreteEvidentialMapping: <SourceFrameName = {0}>\n{1}\n\n", FrameName, _refList));
            for (int i = 0; i < _vectors.Count; i++)
            {
                if (i == _vectors.Count)
                {
                    toReturn.Append(_vectors[i]);
                }
                else
                {
                    toReturn.Append(String.Format("{0}\n", _vectors[i]));
                }
            }
            toReturn.Append("\\\\\\\\    ////\n");
            return toReturn.ToString();
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gives a string representation of the current evidential mapping given a reference list for the
        /// recipient frame of discernment.
        /// </summary>
        /// <param name="refList">The reference list for the recipient frame of discernment.</param>
        /// <returns>Returns a string representation of the current evidential mapping.</returns>
        public string ToString(StringReferenceList refList)
        {
            StringBuilder toReturn = new StringBuilder(String.Format("////    \\\\\\\\\nDiscreteEvidentialMapping: <SourceFrameName = {0}>\n{1}\n\n", FrameName, _refList));
            for (int i = 0; i < _vectors.Count; i++)
            {
                if (i == _vectors.Count)
                {
                    toReturn.Append(_vectors[i].ToString(References, refList));
                }
                else
                {
                    toReturn.Append(String.Format("{0}\n", _vectors[i].ToString(References, refList)));
                }
            }
            toReturn.Append("\\\\\\\\    ////\n");
            return toReturn.ToString();
        }

    } //Struct
} //Namespace
