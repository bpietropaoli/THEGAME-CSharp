
namespace THEGAME.Construction.Generic
{
    /// <summary>
    /// The different type of formats currently supported to load/save belief models.
    /// </summary>
    public enum ModelFormat { 
        /// <summary>
        /// This format creates, in the given directory, a subdirectory for each type of data
        /// with the model associated to this data in a list of files easily read by humans.
        /// </summary>
        MODEL_CUSTOM_DIRECTORY, 

        /// <summary>
        /// This format saves/loads the entire model from a unique XML format.
        /// </summary>
        MODEL_XML_FILE 
    };
}
