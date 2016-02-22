using System.Collections.Generic;
using System.Diagnostics;

using THEGAME.Core.DiscreteStates;
using THEGAME.Core.Generic;

namespace THEGAME.Construction.DiscreteStates.FromSensors
{
    /// <summary>
    /// A structure to associate a model to a sensor and store data specific to that sensor
    /// (for the temporisation for instance).
    /// </summary>
    public struct DiscreteSensorData
    {
        /*
         * Members:
         */
        private List<DiscreteSensorOption> _options;
        private DiscreteSensorModel _model;
        private Stopwatch _watch;

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        /*
         * Properties:
         */

        /// <summary>
        /// Gets the belief model associated to the sensor.
        /// </summary>
        public DiscreteSensorModel Model
        {
            get { return _model; }
        }

        /// <summary>
        /// Gets the name of the sensor.
        /// </summary>
        public string SensorName { get; set; }

        /// <summary>
        /// The options and option data associated with this specific sensor.
        /// </summary>
        public List<DiscreteSensorOption> Options
        {
            get { return _options; }
        }

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        /*
         * Constructor:
         */

        /// <summary>
        /// Builds an association between a sensor and a model. Finally, initialises the sensor option data.
        /// </summary>
        /// <param name="sensorName">The name of the sensor.</param>
        /// <param name="model">The model to which it will be associated.</param>
        public DiscreteSensorData(string sensorName, DiscreteSensorModel model)
            : this()
        {
            this.SensorName = sensorName;
            _model = model;
            _watch = Stopwatch.StartNew();
            _options = new List<DiscreteSensorOption>();
            for (int i = 0; i < model.OptionFlags.Count; i++)
            {
                _options.Add(new DiscreteSensorOption(model.OptionFlags[i], model.OptionParams[i]));
            }
        }

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        /*
         * Methods:
         */

        /// <summary>
        /// Updates the model associated with that sensor and resets the sensor option data.
        /// </summary>
        /// <param name="model">The new model to associate to the sensor.</param>
        public void UpdateModel(DiscreteSensorModel model)
        {
            _model = model;
            _options.Clear();
            for (int i = 0; i < model.OptionFlags.Count; i++)
            {
                _options.Add(new DiscreteSensorOption(model.OptionFlags[i], model.OptionParams[i]));
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Resets the sensor option data associated to the current sensor.
        /// </summary>
        public void ResetOptions()
        {
            _options.Clear();
            for (int i = 0; i < _model.OptionFlags.Count; i++)
            {
                _options.Add(new DiscreteSensorOption(_model.OptionFlags[i], _model.OptionParams[i]));
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Builds a new discrete mass function based on the received measure and the current model. If
        /// measure == null, it means that no measure has been received. The effect of options (temporisation,
        /// variation) are applied here. 
        /// </summary>
        /// <param name="measure">The sensor measure received.</param>
        /// <returns>Returns a new discrete mass function based on the current model and the received 
        /// measurement (and if options are applied also previous measurements).</returns>
        public DiscreteMassFunction GetEvidence(double? measure)
        {
            // ----
            //If variation applies:
            // ----
            double? newMeasure = 0;
            int variationIndex = -1;
            for (int i = 0; i < Options.Count; i++)
            {
                if (Options[i].Flag == DiscreteSensorOption.Option_Flags.OP_VARIATION)
                {
                    variationIndex = i;
                }
            }
            if (variationIndex != -1)
            {
                double sum = 0;
                int nbMeasures = 0;
                for (int i = 0; i < Options[i].Param; i++)
                {
                    if (Options[i].Data[i].Measure != null)
                    {
                        sum += measure.Value - Options[i].Data[i].Measure.Value;
                        nbMeasures++;
                    }
                }
                if (nbMeasures != 0)
                {
                    newMeasure = sum / nbMeasures;
                }
                else
                {
                    newMeasure = null;
                }
                Options[variationIndex].AddMeasure(measure);
            }
            else
            {
                newMeasure = measure;
            }
            // ---- /Variations ----

            //Get evidence from the model:
            DiscreteMassFunction evidence = _model.GetEvidence(newMeasure);

            // ----
            //Apply temporization if needed:
            // ----
            int tempo = -1;
            for (int i = 0; i < Options.Count; i++)
            {
                if (Options[i].Flag == DiscreteSensorOption.Option_Flags.OP_TEMPO_FUSION)
                {
                    tempo = i;
                }
            }
            // ----
            //Apply tempo fusion:
            // ----
            if (tempo != -1)
            {
                long newTime = _watch.ElapsedMilliseconds;
                //First measure:
                if (Options[tempo].PreviousTime == -1)
                {
                    Options[tempo].UpdatePreviousTime(newTime);
                    Options[tempo].PreviousMassFunction.Clear();
                    foreach (FocalElement<DiscreteElement> e in evidence)
                    {
                        Options[tempo].PreviousMassFunction.AddMass(e);
                    }
                }
                else
                {
                    //Not first measure:
                    long elapsedTime = newTime - Options[tempo].PreviousTime;
                    if (newMeasure != null)
                    {
                        if (!(elapsedTime >= Options[tempo].Param))
                        {
                            DiscreteMassFunction discounted = Options[tempo].PreviousMassFunction.Discounting((double)elapsedTime / Options[tempo].Param);
                            evidence = discounted.CombinationDuboisPrade(evidence);
                        }
                        Options[tempo].UpdatePreviousTime(newTime);
                        Options[tempo].PreviousMassFunction.Clear();
                        foreach (FocalElement<DiscreteElement> e in evidence)
                        {
                            Options[tempo].PreviousMassFunction.AddMass(e);
                        }
                    }
                    else
                    {
                        if (elapsedTime >= Options[tempo].Param)
                        {
                            evidence = DiscreteMassFunction.GetVacuousMassFunction(_model.ElementSize);
                        }
                        else
                        {
                            evidence = Options[tempo].PreviousMassFunction.Discounting((double)elapsedTime / Options[tempo].Param);
                        }
                    }
                }
                // --- /TempoFusion ----
            }
            else
            {
                for (int i = 0; i < Options.Count; i++)
                {
                    if (Options[i].Flag == DiscreteSensorOption.Option_Flags.OP_TEMPO_SPECIFICITY)
                    {
                        tempo = i;
                    }
                }
                // ----
                //Apply tempo specificity:
                // ----
                if (tempo != -1)
                {
                    long newTime = _watch.ElapsedMilliseconds;
                    //First measure:
                    if (Options[tempo].PreviousTime == -1)
                    {
                        Options[tempo].UpdatePreviousTime(newTime);
                        Options[tempo].PreviousMassFunction.Clear();
                        foreach (FocalElement<DiscreteElement> e in evidence)
                        {
                            Options[tempo].PreviousMassFunction.AddMass(e);
                        }
                    }
                    else
                    {
                        //Not first measure:
                        long elapsedTime = newTime - Options[tempo].PreviousTime;
                        if (newMeasure == null)
                        {
                            if (elapsedTime >= Options[tempo].Param)
                            {
                                evidence = DiscreteMassFunction.GetVacuousMassFunction(_model.ElementSize);
                            }
                            else
                            {
                                evidence = Options[tempo].PreviousMassFunction.Discounting((double)elapsedTime / Options[tempo].Param);
                            }
                        }
                        else
                        {
                            if (elapsedTime >= Options[tempo].Param)
                            {
                                Options[tempo].UpdatePreviousTime(newTime);
                                // ----
                                Options[tempo].PreviousMassFunction.Clear();
                                foreach (FocalElement<DiscreteElement> e in evidence)
                                {
                                    Options[tempo].PreviousMassFunction.AddMass(e);
                                }
                            }
                            else
                            {
                                DiscreteMassFunction discounted = Options[tempo].PreviousMassFunction.Discounting((double)elapsedTime / Options[tempo].Param);
                                if (discounted.Specificity > evidence.Specificity)
                                {
                                    evidence = discounted;
                                }
                                else
                                {
                                    Options[tempo].UpdatePreviousTime(newTime);
                                    // ----
                                    Options[tempo].PreviousMassFunction.Clear();
                                    foreach (FocalElement<DiscreteElement> e in evidence)
                                    {
                                        Options[tempo].PreviousMassFunction.AddMass(e);
                                    }
                                }
                            }
                        }
                    }
                }
                // ---- /TempoSpec ----
            }
            return evidence;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Tests equality between the current sensor data and the given object.
        /// </summary>
        /// <param name="obj">The object to compare the sensor data to.</param>
        /// <returns>Returns true if the object is a sensor data associated to the same sensor.</returns>
        public override bool Equals(object obj)
        {
            //Checking:
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            //Comparing:
            return this.SensorName == ((DiscreteSensorData)obj).SensorName;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the hash code of the sensor data.
        /// </summary>
        /// <returns>Returns the hash code of the sensor data.</returns>
        public override int GetHashCode()
        {
            return SensorName.GetHashCode();
        }

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Operator overrides

        /// <summary>
        /// Overrides "==" operator to check equality between two discrete sensor data.
        /// </summary>
        /// <param name="a">The first discrete sensor data to compare.</param>
        /// <param name="b">The second discrete sensor data to compare.</param>
        /// <returns>Returns true if both sensor data are equal, false otherwise.</returns>
        public static bool operator ==(DiscreteSensorData a, DiscreteSensorData b)
        {
            if ((object)a == null) return (object)b == null;
            if ((object)b == null) return (object)a == null;
            return !a.Equals(b);
        }

        /// <summary>
        /// Overrides "==" operator to check inequality between two discrete sensor data.
        /// </summary>
        /// <param name="a">The first discrete sensor data to compare.</param>
        /// <param name="b">The second discrete sensor data to compare.</param>
        /// <returns>Returns true if both sensor data are not equal, false otherwise.</returns>
        public static bool operator !=(DiscreteSensorData a, DiscreteSensorData b)
        {
            if ((object)a == null) return (object)b != null;
            if ((object)b == null) return (object)a != null;
            return !a.Equals(b);
        }

        #endregion

    } //Struct
} //Namespace
