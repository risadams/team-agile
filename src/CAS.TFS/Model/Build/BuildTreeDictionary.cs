#region References

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion

namespace CAS.TFS.Model.Build
{
    /// <summary>
    ///     Class BuildTree.
    /// </summary>
    [Serializable]
    public class BuildTreeDictionary : Dictionary<string, IList<BuildDefinitionInfo>>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BuildTreeDictionary" /> class.
        /// </summary>
        public BuildTreeDictionary()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Collections.Generic.Dictionary`2" /> class with serialized
        ///     data.
        /// </summary>
        /// <param name="info">
        ///     A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object containing the information
        ///     required to serialize the <see cref="T:System.Collections.Generic.Dictionary`2" />.
        /// </param>
        /// <param name="context">
        ///     A <see cref="T:System.Runtime.Serialization.StreamingContext" /> structure containing the source
        ///     and destination of the serialized stream associated with the
        ///     <see cref="T:System.Collections.Generic.Dictionary`2" />.
        /// </param>
        protected BuildTreeDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}