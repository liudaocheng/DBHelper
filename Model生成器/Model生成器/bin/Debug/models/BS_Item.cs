using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public partial class BS_Item
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
        public string subType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public string directorsId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public string supervisorsId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public string shareholdersId { get; set; }
    }
}
