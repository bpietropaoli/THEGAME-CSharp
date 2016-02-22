using System;

using THEGAME.Core.DiscreteStates;
using THEGAME.Exceptions;

namespace THEGAME.Construction.DiscreteStates.FromSensors
{
    /// <summary>
    /// A structure to store information about the options applied to sensor belief models.
    /// </summary>
    public struct DiscreteSensorOption
    {
        /// <summary>
        /// The options that could be applied to a sensor belief model.
        /// </summary>
        [Flags]
        public enum Option_Flags
        {
            /// <summary>
            /// No option.
            /// </summary>
            OP_NONE = 0,
            /// <summary>
            /// Infers belief from directly from the sensor measure but from the
            /// variation of the measure compared to the X previous measures (X
            /// being specified as the parameter of the option).
            /// </summary>
            OP_VARIATION = 1,
            /// <summary>
            /// Applies the temporisation based on specificity when inferring
            /// belief from sensor measures. The parameter is the time in milliseconds
            /// before a belief is totally forgotten.
            /// For more details, refer to "B. Pietropaoli, Stable context recognition
            /// in smart home, 2013" (French).
            /// </summary>
            OP_TEMPO_SPECIFICITY = 2,
            /// <summary>
            /// Applies the temporisation based on the Dubois and Prade's combination
            /// rule when inferring belief from sensor measures. The parameter is the time
            /// in milliseconds before a belief is totally forgotten.
            /// For more details, refer to "B. Pietropaoli, Stable context recognition
            /// in smart home, 2013" (French).
            /// </summary>
            OP_TEMPO_FUSION = 4
        };

        /*
         * Members:
         */
        private Option_Flags _flag;
        private long _param;
        private DiscreteSensorOptionData[] _data;

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Properties

        /// <summary>
        /// Gets the flag associated with the current option.
        /// </summary>
        public Option_Flags Flag
        {
            get { return _flag; }
        }

        /// <summary>
        /// Gets the value of the parameter associated with the current option.
        /// </summary>
        public long Param
        {
            get { return _param; }
        }

        /// <summary>
        /// Gets the data stored for this option (previous measures, previous belief).
        /// </summary>
        public DiscreteSensorOptionData[] Data
        {
            get { return _data; }
        }

        /// <summary>
        /// Gets the time associated with the sensor option data. Applicable only to the temporisation
        /// options (<see cref="Option_Flags.OP_TEMPO_FUSION"/> and <see cref="Option_Flags.OP_TEMPO_SPECIFICITY"/>).
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown if called on a non-temporisation option.</exception>
        public long PreviousTime
        {
            get
            {
                if (Flag != Option_Flags.OP_TEMPO_FUSION && Flag != Option_Flags.OP_TEMPO_SPECIFICITY)
                {
                    throw new NotSupportedException("This method only applies for the temporization options!");
                }
                return _data[0].Time;
            }
        }

        /// <summary>
        /// Gets the previous mass function associated with the sensor option data. Applicable only to the temporisation
        /// options (<see cref="Option_Flags.OP_TEMPO_FUSION"/> and <see cref="Option_Flags.OP_TEMPO_SPECIFICITY"/>).
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown if called on a non-temporisation option.</exception>
        public DiscreteMassFunction PreviousMassFunction
        {
            get
            {
                if (Flag != Option_Flags.OP_TEMPO_FUSION && Flag != Option_Flags.OP_TEMPO_SPECIFICITY)
                {
                    throw new NotSupportedException("This method only applies for the temporization options!");
                }
                return _data[1].M;
            }
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Builds an option with the specified flag and the given parameter. Look at the values of
        /// <see cref="Option_Flags"/> for the meaning of the given parameter.
        /// </summary>
        /// <param name="flag">The flag for the option.</param>
        /// <param name="param">The parameter to associate to this option.</param>
        /// <exception cref="InvalidOptionFlagException">Thrown if trying to build a <see cref="Option_Flags.OP_NONE"/>
        /// because it obviously does not make sense.</exception>
        public DiscreteSensorOption(Option_Flags flag, double param)
        {
            if (flag == Option_Flags.OP_NONE)
            {
                throw new InvalidOptionFlagException("It's useless to build an empty Option, do not use OP_NONE!");
            }
            _flag = flag;
            switch (flag)
            {
                case (Option_Flags.OP_VARIATION):
                    _param = (long)param;
                    _data = new DiscreteSensorOptionData[_param];
                    for (int i = 0; i < param; i++)
                    {
                        _data[i].Measure = null;
                    }
                    break;
                case (Option_Flags.OP_TEMPO_FUSION):
                    _param = (long)(param * 1000);
                    _data = new DiscreteSensorOptionData[2];
                    _data[0].Time = -1;
                    _data[1].M = new DiscreteMassFunction();
                    break;
                case (Option_Flags.OP_TEMPO_SPECIFICITY):
                    _param = (long)(param * 1000);
                    _data = new DiscreteSensorOptionData[2];
                    _data[0].Time = -1;
                    _data[1].M = new DiscreteMassFunction();
                    break;
                default:
                    _data = null;
                    _param = (long)param;
                    break;
            }
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Methods

        /// <summary>
        /// Adds a measure to the stored option data. Valid only for <see cref="Option_Flags.OP_VARIATION"/>.
        /// </summary>
        /// <param name="measure">The new measure to store.</param>
        /// <exception cref="NotSupportedException">Thrown if applied to another type of option than
        /// <see cref="Option_Flags.OP_VARIATION"/>.</exception>
        public void AddMeasure(double? measure)
        {
            if (Flag != Option_Flags.OP_VARIATION)
            {
                throw new NotSupportedException("This method only applies for the variation option!");
            }
            for (long i = Param - 1; i > 0; i--)
            {
                _data[i].Measure = _data[i - 1].Measure;
            }
            _data[0].Measure = measure;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Updates the time at which the last mass function was obtained. Valid only for the temporisation
        /// options (<see cref="Option_Flags.OP_TEMPO_FUSION"/> and <see cref="Option_Flags.OP_TEMPO_SPECIFICITY"/>).
        /// </summary>
        /// <param name="time">The time at which the last mass function was obtained.</param>
        /// <exception cref="NotSupportedException">Thrown if applied to another type of option that the
        /// temporisation options.</exception>
        public void UpdatePreviousTime(long time)
        {
            if (Flag != Option_Flags.OP_TEMPO_FUSION && Flag != Option_Flags.OP_TEMPO_SPECIFICITY)
            {
                throw new NotSupportedException("This method only applies for the temporization options!");
            }
            _data[0].Time = time;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Updates the previous mass function. Valid only for the temporisation
        /// options (<see cref="Option_Flags.OP_TEMPO_FUSION"/> and <see cref="Option_Flags.OP_TEMPO_SPECIFICITY"/>).
        /// </summary>
        /// <param name="m">The new mass function to store.</param>
        /// <exception cref="NotSupportedException">Thrown if applied to another type of option that the
        /// temporisation options.</exception>
        public void UpdatePreviousMassFunction(DiscreteMassFunction m)
        {
            if (Flag != Option_Flags.OP_TEMPO_FUSION && Flag != Option_Flags.OP_TEMPO_SPECIFICITY)
            {
                throw new NotSupportedException("This method only applies for the temporization options!");
            }
            _data[1].M = m;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Tests the equality between the given object and the current discrete sensor option.
        /// </summary>
        /// <param name="obj">The object to compare the option to.</param>
        /// <returns>Returns true if the given object is an identical option, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            //Checking:
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            //Equals:
            return ((DiscreteSensorOption)obj).Flag == this.Flag;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gives the hash code of the discrete sensor option.
        /// </summary>
        /// <returns>Returns the hash code of the discrete sensor option.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gives a string representation of the current discrete sensor option.
        /// </summary>
        /// <returns>Returns a string representation of the current discrete sensor option.</returns>
        public override string ToString()
        {
            switch (Flag)
            {
                case (Option_Flags.OP_TEMPO_FUSION):
                    return String.Format("[tempo-fusion : {0}ms]", Param);
                case (Option_Flags.OP_TEMPO_SPECIFICITY):
                    return String.Format("[tempo-specificity : {0}ms]", Param);
                case (Option_Flags.OP_VARIATION):
                    return String.Format("[variation : {0} measures]", Param);
                default:
                    return "";
            }
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Operator overrides

        /// <summary>
        /// Overrides "==" operator to check equality between two discrete sensor options.
        /// </summary>
        /// <param name="a">The first option to compare.</param>
        /// <param name="b">The second option to compare.</param>
        /// <returns>Returns true if both options are equal, false otherwise.</returns>
        public static bool operator ==(DiscreteSensorOption a, DiscreteSensorOption b)
        {
            if ((object)a == null) return (object)b == null;
            if ((object)b == null) return (object)a == null;
            return !a.Equals(b);
        }

        /// <summary>
        /// Overrides "==" operator to check inequality between two discrete sensor options.
        /// </summary>
        /// <param name="a">The first option to compare.</param>
        /// <param name="b">The second option to compare.</param>
        /// <returns>Returns true if both options are not equal, false otherwise.</returns>
        public static bool operator !=(DiscreteSensorOption a, DiscreteSensorOption b)
        {
            if ((object)a == null) return (object)b != null;
            if ((object)b == null) return (object)a != null;
            return !a.Equals(b);
        }

        #endregion

    } //Struct
} //Namespace
