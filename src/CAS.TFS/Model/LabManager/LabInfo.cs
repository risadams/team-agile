#region References

using System;
using Microsoft.TeamFoundation.Lab.Client;

#endregion

namespace CAS.TFS.Model.LabManager
{
    /// <summary>
    /// Class LabInfo.
    /// </summary>
    [Serializable]
    public class LabInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LabInfo"/> class.
        /// </summary>
        /// <param name="lab">The lab.</param>
        public LabInfo(LabEnvironment lab)
        {
        }
    }
}