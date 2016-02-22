using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using THEGAME.Core.DiscreteStates;
using THEGAME.Construction.Generic;
using THEGAME.Exceptions;

namespace THEGAME.Construction.DiscreteStates.FromSensors
{
    /// <summary>
    /// A class to generate discrete mass functions from sensor measures and associated belief models.
    /// </summary>
    public sealed class GeneratorDiscreteBeliefFromSensors : IBeliefModel, IBeliefConstructor<DiscreteMassFunction, DiscreteElement>
    {
        /*
         * Members
         */
        private StringReferenceList _refList;
        private List<DiscreteSensorModel> _discreteSensorModels;
        private List<DiscreteSensorData> _currentSensors;

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Properties

        /*				
         * Properties
         */

        /// <summary>
        /// The name of the frame of discernment for which the mass functions
        /// will be built.
        /// </summary>
        public string FrameName { get; set; }

        /// <summary>
        /// A string reference list representing the possible states/worlds of
        /// the frame of discernment.
        /// </summary>
        public StringReferenceList References
        {
            get { return _refList; }
        }

        /// <summary>
        /// The number of possible states/worlds in the frame of discernment.
        /// </summary>
        public int NbPossibleWorlds
        {
            get { return _refList.Count; }
        }

        /// <summary>
        /// The belief models associated to sensors.
        /// </summary>
        public List<DiscreteSensorModel> DiscreteSensorModels
        {
            get { return _discreteSensorModels; }
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Constructors

        /*				
         * Constructors
         */

        /// <summary>
        /// Initialises the generator without any model and any data.
        /// </summary>
        public GeneratorDiscreteBeliefFromSensors()
        {
            this.FrameName = "";
            _discreteSensorModels = new List<DiscreteSensorModel>();
            _currentSensors = new List<DiscreteSensorData>();
        }

        /// <summary>
        /// Initialises the generator with just a name for the frame of discernment.
        /// </summary>
        /// <param name="frameName">The name of the frame of discernment.</param>
        public GeneratorDiscreteBeliefFromSensors(string frameName)
        {
            this.FrameName = frameName;
            _discreteSensorModels = new List<DiscreteSensorModel>();
            _currentSensors = new List<DiscreteSensorData>();
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        /*		
         * Methods
         */

        #region IBeliefModel

        /// <summary>
        /// <para>Loads the model from the given path with the given format.</para>
        /// <para>If the format is of type <see cref="ModelFormat.MODEL_CUSTOM_DIRECTORY"/>, the path should
        /// indicate the path to the directory where the model is stored.</para>
        /// <para>If the format is of type <see cref="ModelFormat.MODEL_XML_FILE"/>, the path should be
        /// the name of the XML file to load.</para>
        /// </summary>
        /// <param name="path">Where to find the model.</param>
        /// <param name="mt">The format in which the model is.</param>
        /// <exception cref="InvalidBeliefModelException">Thrown if anything is wrong with the model.
        /// The exception message should be explicit enough to know what's wrong.</exception>
        public void LoadModel(string path, ModelFormat mt)
        {
            switch (mt)
            {
                case (ModelFormat.MODEL_CUSTOM_DIRECTORY):
                    if (!Directory.Exists(path))
                    {
                        throw new InvalidBeliefModelException(String.Format("The given path \"{0}\" does not exist (or you don't have permission to read it)!", path));
                    }
                    FrameName = new DirectoryInfo(path).Name;
                    //Load the reference list:
                    string refListFileName = path + Path.DirectorySeparatorChar + "values";
                    if (!File.Exists(refListFileName))
                    {
                        throw new InvalidBeliefModelException(String.Format("The custom directory model (\"{0}\") should contain a file named \"values\" with the names of the possible worlds (one per line)!", path));
                    }
                    _refList = new StringReferenceList(refListFileName);
                    //Load the models:
                    _discreteSensorModels = new List<DiscreteSensorModel>();
                    foreach (string dir in Directory.EnumerateDirectories(path))
                    {
                        LoadDiscreteSensorModel(dir);
                    }
                    break;
                case (ModelFormat.MODEL_XML_FILE):
                    if (!File.Exists(path))
                    {
                        throw new InvalidBeliefModelException(String.Format("The given xml file \"{0}\" does not exist (or you don't have permission to read it)!", path));
                    }
                    //Load the file:
                    XmlDocument doc = new XmlDocument();
                    doc.Load(path);
                    //Create the ReferenceList:
                    XmlNode frame = doc.DocumentElement.SelectSingleNode("/belief-from-sensors/frame");
                    if (frame == null)
                    {
                        throw new InvalidBeliefModelException(String.Format("The xml file \"{0}\" should contain an element called \"frame\" to define the frame of discernment!", path));
                    }
                    this.FrameName = frame.Attributes["name"].InnerText;
                    this._refList = new StringReferenceList();
                    try
                    {
                        foreach (XmlNode n in frame.ChildNodes)
                        {
                            this._refList.Add(n.InnerText);
                        }
                    }
                    catch (IncompatibleReferenceListException)
                    {
                        throw new InvalidBeliefModelException(String.Format("The xml file \"{0}\" has a state defined multiple times in the frame node!", path));
                    }
                    //Load the models:
                    _discreteSensorModels = new List<DiscreteSensorModel>();
                    XmlNode models = doc.DocumentElement.SelectSingleNode("/belief-from-sensors/sensor-beliefs");
                    if (models == null)
                    {
                        throw new InvalidBeliefModelException(String.Format("The xml file \"{0}\" does not seem to contain models!", path));
                    }
                    try
                    {
                        foreach (XmlNode n in models.ChildNodes)
                        {
                            ParseDiscreteSensorModel(n);
                        }
                    }
                    catch (FormatException)
                    {
                        throw new InvalidBeliefModelException(String.Format("The numbers in the file \"{0}\" are not in the proper format or at the wrong place!", path));
                    }
                    //Add the sensors:
                    XmlNode sensors = doc.DocumentElement.SelectSingleNode("/belief-from-sensors/sensors");
                    if (sensors != null)
                    {
                        foreach (XmlNode n in sensors.ChildNodes)
                        {
                            this.AddSensor(n.Attributes["belief"].InnerText, n.Attributes["name"].InnerText);
                        }
                    }
                    break;
            }
            //Update sensors models:
            foreach (DiscreteSensorData sd in _currentSensors)
            {
                foreach (DiscreteSensorModel sm in _discreteSensorModels)
                {
                    if (sd.Model.SensorType == sm.SensorType)
                    {
                        sd.UpdateModel(sm);
                    }
                }
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// <para>Saves the model to the given path with the given format.</para>
        /// <para>If the format is of type <see cref="ModelFormat.MODEL_CUSTOM_DIRECTORY"/>, the path should
        /// indicate the path to the directory where the model should be stored.</para>
        /// <para>If the format is of type <see cref="ModelFormat.MODEL_XML_FILE"/>, the path should be
        /// the name of the XML file to save.</para>
        /// </summary>
        /// <param name="path">Where to save the model.</param>
        /// <param name="mt">The format which the model should be saved in.</param>
        public void SaveModel(string path, ModelFormat mt)
        {
            switch (mt)
            {
                case ModelFormat.MODEL_CUSTOM_DIRECTORY:
                    //Delete existing directories:
                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path, true);
                    }
                    //Create directory if it doesn't exist:
                    Directory.CreateDirectory(path);
                    //Write "values":
                    using (StreamWriter file = new StreamWriter(path + Path.DirectorySeparatorChar + "values"))
                    {
                        foreach (string s in _refList)
                        {
                            file.WriteLine(s);
                        }
                    }
                    //Foreach model :
                    foreach (DiscreteSensorModel sm in _discreteSensorModels)
                    {
                        string modelPath = path + "/" + sm.SensorType;
                        Directory.CreateDirectory(modelPath);
                        //Write options:
                        if (sm.OptionFlags.Count > 0)
                        {
                            using (StreamWriter file = new StreamWriter(modelPath + Path.DirectorySeparatorChar + "options"))
                            {
                                for (int i = 0; i < sm.OptionFlags.Count; i++)
                                {
                                    switch (sm.OptionFlags[i])
                                    {
                                        case DiscreteSensorOption.Option_Flags.OP_TEMPO_FUSION:
                                            file.WriteLine(String.Format("tempo-fusion {0}", sm.OptionParams[i]));
                                            break;
                                        case DiscreteSensorOption.Option_Flags.OP_TEMPO_SPECIFICITY:
                                            file.WriteLine(String.Format("tempo-specificity {0}", sm.OptionParams[i]));
                                            break;
                                        case DiscreteSensorOption.Option_Flags.OP_VARIATION:
                                            file.WriteLine(String.Format("variation {0}", sm.OptionFlags[i]));
                                            break;
                                    }
                                }
                            }
                        }
                        //Write focal points:
                        int j = 0;
                        foreach (DiscreteSensorFocalBelief sfb in sm.FocalBeliefs)
                        {
                            j++;
                            using (StreamWriter file = new StreamWriter(modelPath + String.Format("{0}Belief{1}", Path.DirectorySeparatorChar, j)))
                            {
                                file.WriteLine(String.Format("{0} elements", sfb.Element.Card));
                                file.WriteLine(sfb.Element.ToStringLines(_refList));
                                file.WriteLine(String.Format("{0} points", sfb.Belief.Count));
                                foreach (DiscreteSensorFocalPoint p in sfb.Belief)
                                {
                                    file.WriteLine(String.Format("{0} {1}", p.SensorMeasure, p.BeliefValue));
                                }
                            }
                        }
                    }
                    break;
                case ModelFormat.MODEL_XML_FILE:
                    //Create the main node:
                    XmlTextWriter doc = new XmlTextWriter(path, null);
                    doc.Formatting = Formatting.Indented;
                    doc.Indentation = 4;
                    doc.WriteStartDocument();
                    doc.WriteStartElement("belief-from-sensors");
                    //Add the references:
                    doc.WriteStartElement("frame");
                    doc.WriteAttributeString("name", FrameName);
                    foreach (string s in _refList)
                    {
                        doc.WriteStartElement("state");
                        doc.WriteString(s);
                        doc.WriteEndElement();
                    }
                    doc.WriteEndElement();
                    //Add the DiscreteSensorModels:
                    doc.WriteStartElement("sensor-beliefs");
                    foreach (DiscreteSensorModel sm in _discreteSensorModels)
                    {
                        doc.WriteStartElement("sensor-belief");
                        doc.WriteAttributeString("name", sm.SensorType);
                        //Get a list of possible measure points:
                        List<DiscreteSensorFocalPoint> points = new List<DiscreteSensorFocalPoint>();
                        foreach (DiscreteSensorFocalBelief sfb in sm.FocalBeliefs)
                        {
                            foreach (DiscreteSensorFocalPoint p in sfb.Belief)
                            {
                                if (!points.Contains(p))
                                {
                                    points.Add(p);
                                }
                            }
                        }
                        points.Sort();
                        //For each point, print the value, then the list of focals:
                        foreach (DiscreteSensorFocalPoint p in points)
                        {
                            doc.WriteStartElement("point");
                            //Value:
                            doc.WriteStartElement("value");
                            doc.WriteString(p.SensorMeasure.ToString());
                            doc.WriteEndElement();
                            //Focals:
                            foreach (DiscreteSensorFocalBelief e in sm.FocalBeliefs)
                            {
                                doc.WriteStartElement("mass");
                                doc.WriteAttributeString("set", e.Element.ToConvenientString(_refList));
                                doc.WriteString(e.GetEvidence(p.SensorMeasure).ToString());
                                doc.WriteEndElement();
                            }
                            doc.WriteEndElement();
                        }

                        doc.WriteEndElement();
                    }
                    doc.WriteEndElement();
                    //Add the current sensor names:
                    doc.WriteStartElement("sensors");
                    foreach (DiscreteSensorData sd in _currentSensors)
                    {
                        doc.WriteStartElement("sensor");
                        doc.WriteAttributeString("name", sd.SensorName);
                        doc.WriteAttributeString("set", sd.Model.SensorType);
                        doc.WriteEndElement();
                    }
                    doc.WriteEndElement();
                    //Close document:
                    doc.WriteEndElement();
                    doc.WriteEndDocument();
                    doc.Close();
                    break;
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Checks if the model will produce valid mass functions.
        /// </summary>
        /// <returns>Returns true if the model will produce valid mass functions, false otherwise.</returns>
        public bool IsValid()
        {
            foreach (DiscreteSensorModel sb in _discreteSensorModels)
            {
                if (!sb.IsValid())
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        //------------------------------------------------------------------------------------------------

        #region IBeliefConstructor

        /// <summary>
        /// Constructs the mass function from the given sensor measurements. The passed objects should be
        /// of the type <see cref="SensorMeasure"/> or arrays of such structure. Be aware that the sensors
        /// should be registered with <see cref="AddSensor"/> so they are associated to a model before being
        /// able to build mass functions from their measurements.
        /// </summary>
        /// <param name="obj">The sensor measures to build mass functions from.</param>
        /// <returns>Returns a list of discrete mass functions corresponding to the given sensor 
        /// measurements. If no model was found for any sensor, an empty list is returned.</returns>
        /// <exception cref="NotSupportedException">Thrown if at least one of the given objects
        /// is not of the type <see cref="SensorMeasure"/>.</exception>
        public List<DiscreteMassFunction> ConstructEvidence(params Object[] obj)
        {
            //Checking:
            for (int i = 0; i < obj.Length; i++)
            {
                if ((obj[i] == null) ||
                    (!obj[i].GetType().Equals(typeof(SensorMeasure)) &
                        !obj[i].GetType().Equals(typeof(SensorMeasure[]))))
                {
                    throw new NotSupportedException("The given measures should be of the SensorMeasure type!");
                }
            }
            //Convert:
            List<SensorMeasure> measures = new List<SensorMeasure>();
            for (int i = 0; i < obj.Length; i++)
            {
                if (obj[i].GetType().Equals(typeof(SensorMeasure)))
                {
                    measures.Add((SensorMeasure)obj[i]);
                }
                else if (obj[i].GetType().Equals(typeof(SensorMeasure[])))
                {
                    SensorMeasure[] table = (SensorMeasure[])obj[i];
                    foreach (SensorMeasure measure in table)
                    {
                        measures.Add((SensorMeasure)measure);
                    }
                }
            }
            return ConstructEvidence(measures);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Constructs the mass function from the given sensor measurements. Be aware that the sensors
        /// should be registered with <see cref="AddSensor"/> so they are associated to a model before being
        /// able to build mass functions from their measurements.
        /// </summary>
        /// <param name="measures">The sensor measures to build mass functions from.</param>
        /// <returns>Returns a list of discrete mass functions corresponding to the given sensor 
        /// measurements. If no model was found for any sensor, an empty list is returned.</returns>
        public List<DiscreteMassFunction> ConstructEvidence(List<SensorMeasure> measures)
        {
            //Get evidence:
            List<DiscreteMassFunction> evidence = new List<DiscreteMassFunction>();
            foreach (SensorMeasure m in measures)
            {
                foreach (DiscreteSensorData sd in _currentSensors)
                {
                    if (sd.SensorName == m.SensorName)
                    {
                        evidence.Add(sd.GetEvidence(m.Measure));
                        continue;
                    }
                }
            }
            foreach (DiscreteMassFunction m in evidence)
            {
                m.Clean();
            }
            return evidence;
        }

        #endregion

        //------------------------------------------------------------------------------------------------

        #region Utility methods

        /// <summary>
        /// Adds/Registers a new sensor into the mass function generator. This is used to associate a model
        /// to the sensor and to create the data associated specifically with this sensor (especially for 
        /// temporisation algorithms).
        /// </summary>
        /// <param name="discreteSensorModelName">The name of the discrete sensor model to use (the type of
        /// sensor).</param>
        /// <param name="sensorName">The unique name of the sensor to register.</param>
        /// <exception cref="ModelDoesNotExistException">Thrown if the given model name does not exist
        /// in the loaded models.</exception>
        /// <exception cref="SensorAlreadyRegisteredException">Thrown if a sensor has already been
        /// registered with the given name.</exception>
        public void AddSensor(string discreteSensorModelName, string sensorName)
        {
            //Check that the model exists:
            int modelIndex = -1;
            for (int i = 0; i < this._discreteSensorModels.Count; i++)
            {
                if (_discreteSensorModels[i].SensorType == discreteSensorModelName)
                {
                    modelIndex = i;
                    break;
                }
            }
            if (modelIndex == -1)
            {
                throw new ModelDoesNotExistException(String.Format("There is no model for the sensor type {0}!", discreteSensorModelName));
            }
            //Check that the given sensorName is not already in:
            foreach (DiscreteSensorData sd in _currentSensors)
            {
                if (sensorName == sd.SensorName)
                {
                    throw new SensorAlreadyRegisteredException(String.Format("There is already a sensor with the name \"{0}\", sensors should have a unique name!", sensorName));
                }
            }
            //Add the new sensor:
            _currentSensors.Add(new DiscreteSensorData(sensorName, _discreteSensorModels[modelIndex]));
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Removes/Unregisters the given sensor from the mass function generator.
        /// </summary>
        /// <param name="sensorName">The name of the sensor to remove/unregister.</param>
        public void RemoveSensor(string sensorName)
        {
            //Check that the model exists:
            int index = -1;
            for (int i = 0; i < this._currentSensors.Count; i++)
            {
                if (_currentSensors[i].SensorName == sensorName)
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
            {
                _currentSensors.RemoveAt(index);
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Removes/Unregisters all the sensors already added/registered. Clears all data associated
        /// with these sensors as well.
        /// </summary>
        public void ClearSensors()
        {
            _currentSensors.Clear();
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Clears data associated with the given sensor (used to resent the temporisation for instance).
        /// </summary>
        /// <param name="sensorName">The name of the sensor to look for.</param>
        public void ResetDiscreteSensorData(string sensorName)
        {
            foreach (DiscreteSensorData sd in _currentSensors)
            {
                if (sd.SensorName.Equals(sensorName))
                {
                    sd.ResetOptions();
                    break;
                }
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Clears data associated to all the sensors (used to reset the temporisation for instance).
        /// </summary>
        public void ResetDiscreteSensorData()
        {
            foreach (DiscreteSensorData sd in _currentSensors)
            {
                sd.ResetOptions();
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gives a string representation of the current generator, more specifically the current model.
        /// Not used for any parsing purpose as it is done only for readibility.
        /// </summary>
        /// <returns>Returns a string representing the current model in the generator.</returns>
        public override string ToString()
        {
            StringBuilder toReturn = new StringBuilder(String.Format("<<< Belief from Sensors >>>\nFrame name: {0}\n{1}\n", FrameName, References));
            foreach (DiscreteSensorModel sb in _discreteSensorModels)
            {
                toReturn.Append(String.Format("{0}\n", sb.ToString(References)));
            }
            return toReturn.ToString();
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Private load methods

        /*		
         * Private methods
         */
        private void LoadDiscreteSensorModel(string path)
        {
            //Load the options if there are some:
            string optionsFileName = path + Path.DirectorySeparatorChar + "options";
            DiscreteSensorModel sensorBelief = new DiscreteSensorModel(new DirectoryInfo(path).Name);
            //Load files one by one:
            foreach (string filename in Directory.GetFiles(path))
            {
                //Avoid temp files:
                if (filename[filename.Length - 1] == '~')
                {
                    continue;
                }

                using (StreamReader file = new StreamReader(filename))
                {
                    string[] lines = file.ReadToEnd().Split('\n');
                    //Options file:
                    if (filename == optionsFileName)
                    {
                        try
                        {
                            int nbOptions = Convert.ToInt32(lines[0].Split(' ')[0]);
                            if (nbOptions <= 0)
                            {
                                throw new InvalidBeliefModelException("You're seriously declaring 0 options with an options file?!");
                            }
                            for (int i = 0; i < nbOptions; i++)
                            {
                                string[] words = lines[i + 1].Split(' ');
                                words[0] = words[0].ToLower();
                                if (words[0] == "tempo-specificity")
                                {
                                    sensorBelief.AddOption(DiscreteSensorOption.Option_Flags.OP_TEMPO_SPECIFICITY, Convert.ToDouble(words[1]));
                                }
                                else if (words[0] == "tempo-fusion")
                                {
                                    sensorBelief.AddOption(DiscreteSensorOption.Option_Flags.OP_TEMPO_FUSION, Convert.ToDouble(words[1]));
                                }
                                else if (words[0] == "variation")
                                {
                                    sensorBelief.AddOption(DiscreteSensorOption.Option_Flags.OP_VARIATION, Convert.ToDouble(words[1]));
                                }
                                else
                                {
                                    throw new InvalidBeliefModelException("The option \"{0}\" does not exist!");
                                }
                            }
                        }
                        catch (IndexOutOfRangeException)
                        {
                            throw new InvalidBeliefModelException(String.Format("The number of options indicated in \"{0}\" does not correspond to the rest of the file!", filename));
                        }
                        catch (FormatException)
                        {
                            throw new InvalidBeliefModelException(String.Format("The numbers in the file \"{0}\" are not in the proper format or at the wrong place!", filename));
                        }
                        //Any other file:
                    }
                    else
                    {
                        try
                        {
                            int nbAtoms = Convert.ToInt32(lines[0].Split(' ')[0]);
                            if (nbAtoms <= 0)
                            {
                                throw new InvalidBeliefModelException("There must be at least one atom to build an Element!");
                            }
                            string[] atoms = new string[nbAtoms];
                            for (int i = 0; i < nbAtoms; i++)
                            {
                                atoms[i] = lines[i + 1];
                            }
                            DiscreteSensorFocalBelief fb = new DiscreteSensorFocalBelief(new DiscreteElement(_refList, atoms));
                            //Add the points:
                            int nbPoints = Convert.ToInt32(lines[nbAtoms + 1].Split(' ')[0]);
                            if (nbPoints < 1)
                            {
                                throw new InvalidBeliefModelException("There must be at least one point to get mass from!");
                            }
                            for (int i = 0; i < nbPoints; i++)
                            {
                                string[] values = lines[nbAtoms + 2 + i].Split(' ');
                                fb.AddPoint(new DiscreteSensorFocalPoint(Convert.ToDouble(values[0]), Convert.ToDouble(values[1])));
                            }
                            fb.Sort();
                            sensorBelief.AddFocal(fb);
                        }
                        catch (IndexOutOfRangeException)
                        {
                            throw new InvalidBeliefModelException(String.Format("The model contained in file \"{0}\" is not formatted properly!", filename));
                        }
                        catch (FormatException)
                        {
                            throw new InvalidBeliefModelException(String.Format("The numbers in the file \"{0}\" are not in the proper format or at the wrong place!", filename));
                        }
                    }
                }
            }
            _discreteSensorModels.Add(sensorBelief);
        }

        //------------------------------------------------------------------------------------------------

        private void ParseDiscreteSensorModel(XmlNode node)
        {
            DiscreteSensorModel sensorBelief = new DiscreteSensorModel(node.Attributes["name"].InnerText);
            //Load options if there are some:
            XmlNode options = node.SelectSingleNode("options");
            if (options != null)
            {
                foreach (XmlNode n in options.ChildNodes)
                {
                    string name = n.Attributes["name"].InnerText.ToLower();
                    string param = n.InnerText;
                    if (name == "tempo-specificity")
                    {
                        sensorBelief.AddOption(DiscreteSensorOption.Option_Flags.OP_TEMPO_SPECIFICITY, Convert.ToDouble(param));
                    }
                    else if (name == "tempo-fusion")
                    {
                        sensorBelief.AddOption(DiscreteSensorOption.Option_Flags.OP_TEMPO_FUSION, Convert.ToDouble(param));
                    }
                    else if (name == "variation")
                    {
                        sensorBelief.AddOption(DiscreteSensorOption.Option_Flags.OP_VARIATION, Convert.ToDouble(param));
                    }
                    else
                    {
                        throw new InvalidBeliefModelException("The option \"{0}\" does not exist!");
                    }
                }
            }
            //Load each point:
            XmlNodeList points = node.SelectNodes("point");
            if (points == null)
            {
                throw new InvalidBeliefModelException("A sensor model should contain points!");
            }
            List<DiscreteSensorFocalBelief> focals = new List<DiscreteSensorFocalBelief>();
            foreach (XmlNode point in points)
            {
                XmlNode value = point.SelectSingleNode("value");
                if (value == null)
                {
                    throw new InvalidBeliefModelException("Every point in a model should contain a sensor measure value under \"value\"!");
                }
                double measure = Convert.ToDouble(value.InnerText);
                XmlNodeList masses = point.SelectNodes("mass");
                if (masses == null)
                {
                    throw new InvalidBeliefModelException("Every point in a model should contain masses!");
                }
                foreach (XmlNode mass in masses)
                {
                    //Create the focal point:
                    double massValue = Convert.ToDouble(mass.InnerText);
                    DiscreteSensorFocalPoint sfp = new DiscreteSensorFocalPoint(measure, massValue);
                    //Create the element and add it if necessary:
                    DiscreteElement e = new DiscreteElement(this._refList, mass.Attributes["set"].InnerText.Split(' '));
                    DiscreteSensorFocalBelief sfb = new DiscreteSensorFocalBelief(e);
                    if (focals.Contains(sfb))
                    {
                        int index = focals.IndexOf(sfb);
                        focals[index].AddPoint(sfp);
                    }
                    else
                    {
                        focals.Add(sfb);
                        focals[focals.Count - 1].AddPoint(sfp);
                    }
                }
            }
            foreach (DiscreteSensorFocalBelief sfb in focals)
            {
                sfb.Sort();
                sensorBelief.AddFocal(sfb);
            }
            //Add the model:
            _discreteSensorModels.Add(sensorBelief);
        }

        #endregion

    } //Class
} //Namespace

