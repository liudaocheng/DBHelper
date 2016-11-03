using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public partial class BS_Meeting
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
        public string type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public string term { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public string num { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public string keywords { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public string reviewTime { get; set; }
    }
}
