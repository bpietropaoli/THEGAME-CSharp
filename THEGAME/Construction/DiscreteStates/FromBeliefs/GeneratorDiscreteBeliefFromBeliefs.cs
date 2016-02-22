using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

using THEGAME.Core.DiscreteStates;
using THEGAME.Exceptions;
using THEGAME.Construction.Generic;

namespace THEGAME.Construction.DiscreteStates.FromBeliefs
{
    /// <summary>
    /// A class to generate discrete mass functions from mass functions defined on other frames of discernment.
    /// For more details, refer to "B. Pietropaoli et al., Propagation of belief functions through frames of discernment, 2013".
    /// </summary>
    public sealed class GeneratorDiscreteBeliefFromBeliefs : IBeliefModel, IBeliefConstructor<DiscreteMassFunction, DiscreteElement>
    {
        /*
         * Members
         */
        private StringReferenceList _refList;
        private List<DiscreteEvidentialMapping> _mappings;

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Properties

        /*				
         * Properties
         */

        /// <summary>
        /// The name of the frame of discernment which the mass functions
        /// will be defined on. The frame the belief will be propagated to.
        /// </summary>
        public string FrameName { get; set; }

        /// <summary>
        /// The reference list to give sense to the states/worlds in the frame
        /// of discernment.
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
        /// The evidential mapping from other frames to the current one.
        /// </summary>
        public List<DiscreteEvidentialMapping> Mappings
        {
            get { return _mappings; }
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Constructor

        /*				
         * Constructors
         */

        /// <summary>
        /// Initialises the generator without any model and any data.
        /// </summary>
        public GeneratorDiscreteBeliefFromBeliefs()
        {
            this.FrameName = "";
            _mappings = new List<DiscreteEvidentialMapping>();
        }

        /// <summary>
        /// Initialises the generator with just a name for the frame of discernment.
        /// </summary>
        /// <param name="frameName">The name of the frame of discernment.</param>
        public GeneratorDiscreteBeliefFromBeliefs(string frameName)
        {
            this.FrameName = frameName;
            _mappings = new List<DiscreteEvidentialMapping>();
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
                        throw new InvalidBeliefModelException(String.Format("The given path ({0}) does not exist (or you don't have permission to read it)!", path));
                    }
                    FrameName = new DirectoryInfo(path).Name;
                    //Load the reference list:
                    string refListFileName = path + Path.DirectorySeparatorChar + "values";
                    if (!File.Exists(refListFileName))
                    {
                        throw new InvalidBeliefModelException("The custom directory model should contain a file named \"values\" with the names of the possible worlds (one per line)!");
                    }
                    _refList = new StringReferenceList(refListFileName);
                    //Load the models:
                    _mappings = new List<DiscreteEvidentialMapping>();
                    foreach (string dir in Directory.EnumerateDirectories(path))
                    {
                        LoadDiscreteEvidentialMapping(dir);
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
                    XmlNode frame = doc.DocumentElement.SelectSingleNode("/belief-from-beliefs/frame");
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
                    _mappings = new List<DiscreteEvidentialMapping>();
                    XmlNode models = doc.DocumentElement.SelectSingleNode("/belief-from-beliefs/evidential-mappings");
                    if (models == null)
                    {
                        throw new InvalidBeliefModelException(String.Format("The xml file \"{0}\" does not seem to contain models!", path));
                    }
                    try
                    {
                        foreach (XmlNode n in models.ChildNodes)
                        {
                            ParseDiscreteEvidentialMapping(n);
                        }
                    }
                    catch (FormatException)
                    {
                        throw new InvalidBeliefModelException(String.Format("The numbers in the file \"{0}\" are not in the proper format or at the wrong place!", path));
                    }
                    break;
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
                    foreach (DiscreteEvidentialMapping em in _mappings)
                    {
                        string modelPath = path + "/" + em.FrameName;
                        Directory.CreateDirectory(modelPath);
                        //Save values:
                        using (StreamWriter file = new StreamWriter(modelPath + Path.DirectorySeparatorChar + "values"))
                        {
                            foreach (string s in em.References)
                            {
                                file.WriteLine(s);
                            }
                        }
                        //Save vectors:
                        foreach (DiscreteMappingVector mv in em.Matrix)
                        {
                            using (StreamWriter file = new StreamWriter(modelPath + Path.DirectorySeparatorChar + mv.From.ToConvenientString(em.References).Replace(" ", "")))
                            {
                                file.WriteLine(String.Format("{0} elements", mv.From.Card));
                                file.WriteLine(mv.From.ToStringLines(em.References));
                                file.WriteLine(String.Format("{0} conversions", mv.To.Count));
                                foreach (DiscreteMappingPoint mp in mv.To)
                                {
                                    file.WriteLine(String.Format("{0} elements", mp.Element.Card));
                                    file.WriteLine(mp.Element.ToStringLines(_refList));
                                    file.WriteLine(mp.MappingFactor.ToString());
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
                    doc.WriteStartElement("belief-from-beliefs");
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
                    //Write mappings:
                    doc.WriteStartElement("evidential-mappings");
                    foreach (DiscreteEvidentialMapping em in _mappings)
                    {
                        doc.WriteStartElement("evidential-mapping");
                        //Write subframe:
                        doc.WriteStartElement("subframe");
                        doc.WriteAttributeString("name", em.FrameName);
                        foreach (string s in em.References)
                        {
                            doc.WriteStartElement("state");
                            doc.WriteString(s);
                            doc.WriteEndElement();
                        }
                        doc.WriteEndElement();
                        //Write vectors:
                        foreach (DiscreteMappingVector mv in em.Matrix)
                        {
                            doc.WriteStartElement("mapping-vector");
                            doc.WriteStartElement("from");
                            doc.WriteAttributeString("element", mv.From.ToConvenientString(em.References));
                            doc.WriteEndElement();
                            foreach (DiscreteMappingPoint mp in mv.To)
                            {
                                doc.WriteStartElement("to");
                                doc.WriteAttributeString("element", mp.Element.ToConvenientString(this._refList));
                                doc.WriteString(mp.MappingFactor.ToString());
                                doc.WriteEndElement();
                            }
                            doc.WriteEndElement();
                        }
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
            foreach (DiscreteEvidentialMapping mapping in _mappings)
            {
                if (!mapping.IsValid())
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
        /// Constructs the mass functions from the given mass functions. The passed objects should be
        /// of the type <see cref="NamedMassFunction{TFunction, TElement}"/> or arrays of such structure.
        /// </summary>
        /// <param name="obj">The mass functions to build mass functions from.</param>
        /// <returns>Returns a list of discrete mass functions corresponding to the propagation of
        /// mass functions from one frame to another. If no evidential mapping was found for any mass 
        /// function, an empty list is returned.</returns>
        /// <exception cref="NotSupportedException">Thrown if at least one of the given objects
        /// is not of the type <see cref="NamedMassFunction{TFunction, TElement}"/>.</exception>
        public List<DiscreteMassFunction> ConstructEvidence(params Object[] obj)
        {
            //Checking:
            for (int i = 0; i < obj.Length; i++)
            {
                if ((obj[i] == null) ||
                    (!obj[i].GetType().Equals(typeof(NamedMassFunction<DiscreteMassFunction, DiscreteElement>)) &
                        !obj[i].GetType().Equals(typeof(NamedMassFunction<DiscreteMassFunction, DiscreteElement>[]))))
                {
                    throw new NotSupportedException("The given mass functions should be of the NamedMassFunction<DiscreteMassFunction, DiscreteElement> type!");
                }
            }
            //Convert:
            List<NamedMassFunction<DiscreteMassFunction, DiscreteElement>> masses = new List<NamedMassFunction<DiscreteMassFunction, DiscreteElement>>();
            for (int i = 0; i < obj.Length; i++)
            {
                if (obj[i].GetType().Equals(typeof(NamedMassFunction<DiscreteMassFunction, DiscreteElement>)))
                {
                    masses.Add((NamedMassFunction<DiscreteMassFunction, DiscreteElement>)obj[i]);
                }
                else if (obj[i].GetType().Equals(typeof(NamedMassFunction<DiscreteMassFunction, DiscreteElement>[])))
                {
                    NamedMassFunction<DiscreteMassFunction, DiscreteElement>[] table = (NamedMassFunction<DiscreteMassFunction, DiscreteElement>[])obj[i];
                    foreach (NamedMassFunction<DiscreteMassFunction, DiscreteElement> mass in table)
                    {
                        masses.Add((NamedMassFunction<DiscreteMassFunction, DiscreteElement>)mass);
                    }
                }
            }

            return ConstructEvidence(masses);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Constructs the mass functions from the given mass functions.
        /// </summary>
        /// <param name="masses">The mass functions to build mass functions from.</param>
        /// <returns>Returns a list of discrete mass functions corresponding to the propagation of
        /// mass functions from one frame to another. If no evidential mapping was found for any mass 
        /// function, an empty list is returned.</returns>
        public List<DiscreteMassFunction> ConstructEvidence(params NamedMassFunction<DiscreteMassFunction, DiscreteElement>[] masses)
        {
            List<DiscreteMassFunction> evidence = new List<DiscreteMassFunction>();
            foreach (NamedMassFunction<DiscreteMassFunction, DiscreteElement> mass in masses)
            {
                foreach (DiscreteEvidentialMapping em in _mappings)
                {
                    if (mass.Name == em.FrameName)
                    {
                        evidence.Add(em.GetEvidence(mass.Mass));
                    }
                }
            }
            foreach (DiscreteMassFunction m in evidence)
            {
                m.Clean();
            }
            return evidence;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Constructs the mass functions from the given mass functions.
        /// </summary>
        /// <param name="masses">The mass functions to build mass functions from.</param>
        /// <returns>Returns a list of discrete mass functions corresponding to the propagation of
        /// mass functions from one frame to another. If no evidential mapping was found for any mass 
        /// function, an empty list is returned.</returns>
        public List<DiscreteMassFunction> ConstructEvidence(List<NamedMassFunction<DiscreteMassFunction, DiscreteElement>> masses)
        {
            return ConstructEvidence(masses.ToArray());
        }

        #endregion

        //------------------------------------------------------------------------------------------------

        #region Utility methods

        /// <summary>
        /// Gives a string representation of the current generator, more specifically the current model.
        /// Not used for any parsing purpose as it is done only for readibility.
        /// </summary>
        /// <returns>Returns a string representing the current model in the generator.</returns>
        public override string ToString()
        {
            if (_mappings.Count == 0)
            {
                return String.Format("<<< Belief from Beliefs >>>\nFrame name: {0}\nNothing loaded :(", FrameName);
            }
            StringBuilder toReturn = new StringBuilder(String.Format("<<< Belief from Sensors >>>\nFrame name: {0}\n{1}\n", FrameName, References));
            for (int i = 0; i < _mappings.Count; i++)
            {
                if (i == _mappings.Count)
                {
                    toReturn.Append(String.Format("{0}", _mappings[i].ToString(_refList)));
                }
                else
                {
                    toReturn.Append(String.Format("{0}\n", _mappings[i].ToString(_refList)));
                }
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
        private void LoadDiscreteEvidentialMapping(string path)
        {
            //Load the reference list:
            string refListFileName = path + "/values";
            if (!File.Exists(refListFileName))
            {
                throw new InvalidBeliefModelException(String.Format("The path \"{0}\" should contain a file named \"values\" with the names of the possible worlds (one per line)!", path));
            }
            DiscreteEvidentialMapping mapping = new DiscreteEvidentialMapping(new DirectoryInfo(path).Name, refListFileName);
            //Load files one by one:
            foreach (string filename in Directory.GetFiles(path))
            {
                //Avoid temp files and the "values" file:
                if (filename[filename.Length - 1] == '~' ||
                    new FileInfo(filename).Name == "values")
                {
                    continue;
                }
                using (StreamReader file = new StreamReader(filename))
                {
                    string[] lines = file.ReadToEnd().Split('\n');
                    try
                    {
                        int lineCounter = 0;
                        int nbAtoms = Convert.ToInt32(lines[0].Split(' ')[0]);
                        if (nbAtoms <= 0)
                        {
                            throw new InvalidBeliefModelException("There must be at least one atom to build an Element!");
                        }
                        lineCounter++;
                        string[] atoms = new string[nbAtoms];
                        for (int i = 0; i < nbAtoms; i++)
                        {
                            atoms[i] = lines[i + lineCounter];
                        }
                        lineCounter += nbAtoms;
                        DiscreteMappingVector vector = new DiscreteMappingVector(new DiscreteElement(mapping.References, atoms));
                        //Add the points:
                        int nbConversions = Convert.ToInt32(lines[lineCounter].Split(' ')[0]);
                        if (nbConversions < 1)
                        {
                            throw new InvalidBeliefModelException("There must be at least one element to convert mass to!");
                        }
                        lineCounter++;
                        for (int i = 0; i < nbConversions; i++)
                        {
                            int nbAtomsTo = Convert.ToInt32(lines[lineCounter].Split(' ')[0]);
                            if (nbAtomsTo <= 0)
                            {
                                throw new InvalidBeliefModelException("There must be at least one atom to build an Element!");
                            }
                            lineCounter++;
                            string[] atomsTo = new string[nbAtomsTo];
                            for (int j = 0; j < nbAtomsTo; j++)
                            {
                                atomsTo[j] = lines[j + lineCounter];
                            }
                            lineCounter += nbAtomsTo;
                            double factor = Convert.ToDouble(lines[lineCounter].Split(' ')[0]);
                            lineCounter++;
                            vector.AddPoint(new DiscreteElement(this.References, atomsTo), factor);
                        }
                        mapping.AddVector(vector);
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
            _mappings.Add(mapping);
        }

        //------------------------------------------------------------------------------------------------

        private void ParseDiscreteEvidentialMapping(XmlNode node)
        {
            //Create the ReferenceList:
            XmlNode frame = node.SelectSingleNode("subframe");
            if (frame == null)
            {
                throw new InvalidBeliefModelException("The xml model element \"evidential-mapping\" should contain an element called \"subframe\" to define the frame of discernment!");
            }
            string frameName = frame.Attributes["name"].InnerText;
            StringReferenceList references = new StringReferenceList();
            try
            {
                foreach (XmlNode n in frame.ChildNodes)
                {
                    references.Add(n.InnerText);
                }
            }
            catch (IncompatibleReferenceListException)
            {
                throw new InvalidBeliefModelException("The xml model element file \"evidential-mapping\" has a state defined multiple times in the frame node!");
            }
            DiscreteEvidentialMapping mapping = new DiscreteEvidentialMapping(frameName, references);
            //Load evidential vectors:
            XmlNodeList vectors = node.SelectNodes("mapping-vector");
            if (vectors == null)
            {
                throw new InvalidBeliefModelException("An evidential mapping should contain mapping vectors!");
            }
            foreach (XmlNode n in vectors)
            {
                XmlNode fr = n.SelectSingleNode("from");
                DiscreteMappingVector mp = new DiscreteMappingVector(new DiscreteElement(references, fr.Attributes["element"].InnerText.Split(' ')));
                XmlNodeList to = n.SelectNodes("to");
                foreach (XmlNode t in to)
                {
                    mp.AddPoint(new DiscreteMappingPoint(new DiscreteElement(_refList, t.Attributes["element"].InnerText.Split(' ')), Convert.ToDouble(t.InnerText)));
                }
                mapping.AddVector(mp);
            }
            _mappings.Add(mapping);
        }

        #endregion

    } //Class
} //Namespace

