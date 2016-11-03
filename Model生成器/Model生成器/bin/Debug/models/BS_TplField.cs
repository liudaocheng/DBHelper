using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public partial class BS_TplField
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
        public string fieldName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public string remarks { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public string relationId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public string fieldType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public string ope { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public string opeValue { get; set; }
    }
}
