using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using THEGAME.Exceptions;

namespace THEGAME.Core.DiscreteStates
{
    /// <summary>
    /// A simple class to get a string reference list for the discrete elements.
    /// Keeps the unicity of the stored references.
    /// </summary>
    public sealed class StringReferenceList : ReferenceList<string>,
                                              IEquatable<StringReferenceList>
    {
        #region Constructors

        /*					
         * Constructors
         */
        /// <summary>
        /// Builds an empty string reference list.
        /// </summary>
        public StringReferenceList() : base() {}

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Builds an empty string reference list then fills it with the given strings.
        /// </summary>
        /// <param name="references">The references to store in the list.</param>
        public StringReferenceList(params string[] references) : base()
        {
            foreach (string r in references)
            {
                this.Add(new string(r.ToCharArray()));
            }
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Builds a string reference list by loading the given file which should contain one reference
        /// per line.
        /// </summary>
        /// <param name="fileName"></param>
        public StringReferenceList(string fileName) : base()
        {
            using (StreamReader file = new StreamReader(fileName))
            {
                string[] lines = file.ReadToEnd().Split('\n');
                foreach (string line in lines)
                {
                    if (line != "")
                    {
                        this.Add(line);
                    }
                }
            }
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Methods

        /*
         * Methods
         */

        /// <summary>
        /// Tests the equality of the given list with the current one.
        /// </summary>
        /// <param name="list">The list to compare to.</param>
        /// <returns>Returns true if both list are equal, false otherwise.</returns>
        public bool Equals(StringReferenceList list)
        {
            return base.Equals(list);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Compares the current reference list with the given object.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>Returns true if the given object is an equal reference list, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as StringReferenceList);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the hash code for the current reference list.
        /// </summary>
        /// <returns>Returns the hash code of the current reference list.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------

        #region Operator overrides

        /*
         * Overriding operators
         */

        /// <summary>
        /// Overrides the "==" operator to test for the equality of two lists.
        /// </summary>
        /// <param name="a">The first list to compare.</param>
        /// <param name="b">The second list to compare.</param>
        /// <returns>Returns true if both lists are equal, false otherwise.</returns>
        public static bool operator ==(StringReferenceList a, StringReferenceList b)
        {
            if ((object)a == null) return (object)b == null;
            if ((object)b == null) return (object)a == null;
            return !a.Equals(b);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Overrides the "!=" operator to test for the inequality of two lists.
        /// </summary>
        /// <param name="a">The first list to compare.</param>
        /// <param name="b">The second list to compare.</param>
        /// <returns>Returns true if both lists are not equal, false otherwise.</returns>
        public static bool operator !=(StringReferenceList a, StringReferenceList b)
        {
            if ((object)a == null) return (object)b != null;
            if ((object)b == null) return (object)a != null;
            return !a.Equals(b);
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Overrides the "+" operator to add a new reference to the reference list.
        /// </summary>
        /// <param name="list">The list to add a reference to.</param>
        /// <param name="reference">The reference to add.</param>
        /// <returns>Returns the list with the reference added (WARNING: It modifies the given list!)</returns>
        /// <exception cref="IncompatibleReferenceListException">Thrown if the given reference is
        /// already contained.</exception>
        public static StringReferenceList operator +(StringReferenceList list, string reference)
        {
            list.Add(reference);
            return list;
        }

        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// Overrides the "-" operator to remove a reference from the reference list.
        /// </summary>
        /// <param name="list">The list to remove a reference from.</param>
        /// <param name="reference">The reference to remove.</param>
        /// <returns>Returns the list with the reference removed (WARNING: It modifies the given list!)</returns>
        public static StringReferenceList operator -(StringReferenceList list, string reference)
        {
            list.Remove(reference);
            return list;
        }

        #endregion

    } //Class
} //Namespace

