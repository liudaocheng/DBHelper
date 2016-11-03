using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public partial class BS_Folder
    {
        /// <summary>
        /// 
        /// </summary>
        [IsId]
        [IsDBField]
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public string code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public string name { get; set; }
    }
}
