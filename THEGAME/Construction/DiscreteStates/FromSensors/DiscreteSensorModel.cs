using System;
using System.Collections.Generic;
using System.Text;

using THEGAME.Core.DiscreteStates;
using THEGAME.Exceptions;

namespace THEGAME.Construction.DiscreteStates.FromSensors
{
    /// <summary>
    /// A structure to store a sensor belief model used to infer belief from sensor measures.
    /// </summary>
    public struct DiscreteSensorModel
    {
        /*
         * Members:
         */
        private string _sensorType;
        private List<DiscreteSensorFocalBelief> _focalBeliefs;
        private List<DiscreteSensorOption.Option_Flags> _optionFlags;
        private List<double> _params;

        //------------------------------------------------------------------------------------------------

        #region Properties

        /*
         * Properties:
         */

        /// <summary>
        /// Gets the type of sensor the model is associated with.
        /// </summary>
        public string SensorType
        {
            get { return _sensorType; }
        }

        /// <summary>
        /// The belief for all focal elements that can be inferred from the measurements.
        /// </summary>
        public List<DiscreteSensorFocalBelief> FocalBeliefs
        {
            get { return _focalBeliefs; }
        }

        /// <summary>
        /// Gets the parameters associated with the options applied to this model.
        /// </summary>
        public List<double> OptionParams
        {
            get { return _params; }
        }

        /// <summary>
        /// Gets the list of options applied to this model.
        /// </summary>
        public List<DiscreteSensorOption.Option_Flags> OptionFlags
        {
            get { return _optionFlags; }
        }

        /// <summary>
        /// Gets the size of the elements (the number of possible states/worlds in the
        /// frame of discernment).
        /// </summary>
        public int ElementSize
        {
            get { return FocalBeliefs[0].Element.Size; }
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Initialises the sensor model with juste a sensor type.
        /// </summary>
        /// <param name="sensorType">The type of sensor corresponding to the model.</param>
        public DiscreteSensorModel(string sensorType)
        {
            this._sensorType = sensorType;
            this._focalBeliefs = new List<DiscreteSensorFocalBelief>();
            //this._options = new List<DiscreteSensorOption> ();
            this._optionFlags = new List<DiscreteSensorOption.Option_Flags>();
            this._params = new List<double>();
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        /*
         * Methods:
         */

        #region Belief model methods

        /// <summary>
        /// Gets the raw mass function from the given sensor measure. No option is applied here as
        /// it requires data associated to a specific sensor. (Temporisations and variation are thus
        /// applied in <see cref="DiscreteSensorData"/>.
        /// </summary>
        /// <param name="measure">The sensor measure which the belief is inferred from.</param>
        /// <returns>Returns a new mass function given the received sensor measure.</returns>
        public DiscreteMassFunction GetEvidence(double? measure)
        {
            // ----
            //Build the new mass function:
            // ---
            DiscreteMassFunction evidence = new DiscreteMassFunction();
            if (measure != null)
            {
                foreach (DiscreteSensorFocalBelief fb in FocalBeliefs)
                {
                    evidence.AddMass(fb.Element, fb.GetEvidence((double)measure));
                }
            }
            else
            {
                evidence = DiscreteMassFunction.GetVacuousMassFunction(ElementSize);
            }
            // ---- /Building ---
            return evidence;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Adds a focal element to the current model.
        /// </summary>
        /// <param name="focal">The belief specific to a focal element to add.</param>
        /// <exception cref="InvalidBeliefModelException">Thrown if the focal element is already
        /// present in the model.</exception>
        public void AddFocal(DiscreteSensorFocalBelief focal)
        {
            if (_focalBeliefs.Contains(focal))
            {
                throw new InvalidBeliefModelException(String.Format("A focal element ({0}) appears twice in the model for the sensor of type {1}!", focal.Element, _sensorType));
            }
            _focalBeliefs.Add(focal);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Adds an option to the current model.
        /// </summary>
        /// <param name="option">The flag of the option to add.</param>
        /// <param name="param">The parameter associated to this option.</param>
        /// <exception cref="InvalidBeliefModelException">Thrown if the given option is already present in the
        /// current model OR if two temporisations are tried to applied at once.</exception>
        public void AddOption(DiscreteSensorOption.Option_Flags option, double param)
        {
            if (_optionFlags.Contains(option))
            {
                throw new InvalidBeliefModelException(String.Format("An option has been defined twice in the model for the sensor of type {0}!", _sensorType));
            }
            if ((option == DiscreteSensorOption.Option_Flags.OP_TEMPO_FUSION && _optionFlags.Contains(DiscreteSensorOption.Option_Flags.OP_TEMPO_SPECIFICITY)) ||
               option == DiscreteSensorOption.Option_Flags.OP_TEMPO_SPECIFICITY && _optionFlags.Contains(DiscreteSensorOption.Option_Flags.OP_TEMPO_FUSION))
            {
                throw new InvalidBeliefModelException("Both temporizations cannot be used at the same time!");
            }
            _optionFlags.Add(option);
            _params.Add(param);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Checks if the current model will produce valid mass functions.
        /// </summary>
        /// <returns>Returns true if the model will produce valid mass functions, false otherwise.</returns>
        public bool IsValid()
        {
            //Build the list of points:
            List<DiscreteSensorFocalPoint> list = new List<DiscreteSensorFocalPoint>();
            foreach (DiscreteSensorFocalBelief fb in _focalBeliefs)
            {
                foreach (DiscreteSensorFocalPoint bp in fb.Belief)
                {
                    if (!list.Contains(bp))
                    {
                        list.Add(bp);
                    }
                }
            }
            list.Sort();
            //Build a mass function for each point and check it is valid:
            foreach (DiscreteSensorFocalPoint bp in list)
            {
                DiscreteMassFunction evidence = new DiscreteMassFunction();
                foreach (DiscreteSensorFocalBelief fb in FocalBeliefs)
                {
                    evidence.AddMass(fb.Element, fb.GetEvidence(bp.SensorMeasure));
                }
                if (!evidence.HasAValidSum() || !evidence.HasValidValues())
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        //------------------------------------------------------------------------------------------------

        #region Utility methods

        /// <summary>
        /// Gives a string representation of the current model.
        /// </summary>
        /// <returns>Returns a string representation of the current model.</returns>
        public override string ToString()
        {
            StringBuilder toReturn = new StringBuilder(String.Format("////    \\\\\\\\\nSensorBelief: <SensorName={0}>\n", SensorType));
            if (OptionFlags.Count == 0)
            {
                toReturn.Append("Options: none\n");
            }
            else
            {
                toReturn.Append("Options: \n");
                for (int i = 0; i < OptionFlags.Count; i++)
                {
                    switch (OptionFlags[i])
                    {
                        case DiscreteSensorOption.Option_Flags.OP_TEMPO_FUSION:
                            toReturn.Append(String.Format("Tempo-fusion: {0}s\n", OptionParams[i]));
                            break;
                        case DiscreteSensorOption.Option_Flags.OP_TEMPO_SPECIFICITY:
                            toReturn.Append(String.Format("Tempo-specificity: {0}s\n", OptionParams[i]));
                            break;
                        case DiscreteSensorOption.Option_Flags.OP_VARIATION:
                            toReturn.Append(String.Format("Variation: {0} measures\n", OptionParams[i]));
                            break;
                    }
                }
            }
            toReturn.Append("Focals: \n");
            foreach (DiscreteSensorFocalBelief fb in FocalBeliefs)
            {
                toReturn.Append(String.Format("{0}\n", fb));
            }
            toReturn.Append("\n\\\\\\\\    ////\n");
            return toReturn.ToString();
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gives a string representation of the current model given a reference list for the elements.
        /// </summary>
        /// <param name="refList">The reference list to give sense to the elements.</param>
        /// <returns>Returns a string represenation of the current model.</returns>
        public string ToString(StringReferenceList refList)
        {
            StringBuilder toReturn = new StringBuilder(String.Format("////    \\\\\\\\\nSensorBelief: <SensorName = {0}>\n", SensorType));
            if (OptionFlags.Count == 0)
            {
                toReturn.Append("Options: none\n");
            }
            else
            {
                toReturn.Append("Options: \n");
                for (int i = 0; i < OptionFlags.Count; i++)
                {
                    switch (OptionFlags[i])
                    {
                        case DiscreteSensorOption.Option_Flags.OP_TEMPO_FUSION:
                            toReturn.Append(String.Format("Tempo-fusion: {0}s\n", OptionParams[i]));
                            break;
                        case DiscreteSensorOption.Option_Flags.OP_TEMPO_SPECIFICITY:
                            toReturn.Append(String.Format("Tempo-specificity: {0}s\n", OptionParams[i]));
                            break;
                        case DiscreteSensorOption.Option_Flags.OP_VARIATION:
                            toReturn.Append(String.Format("Variation: {0} measures\n", OptionParams[i]));
                            break;
                    }
                }
            }
            toReturn.Append("Focals: \n");
            for (int i = 0; i < FocalBeliefs.Count; i++)
            {
                if (i == FocalBeliefs.Count)
                {
                    toReturn.Append(String.Format("{0}", FocalBeliefs[i].ToString(refList)));
                }
                else
                {
                    toReturn.Append(String.Format("{0}\n", FocalBeliefs[i].ToString(refList)));
                }
            }
            toReturn.Append("\\\\\\\\    ////\n");
            return toReturn.ToString();
        }

        #endregion

    } //Struct
} //Namespace
