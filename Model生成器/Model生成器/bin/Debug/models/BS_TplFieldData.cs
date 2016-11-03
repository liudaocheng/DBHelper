using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public partial class BS_TplFieldData
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
        public string itemId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public string fieldId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public string fieldValue { get; set; }
    }
}
