using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public partial class BS_Bill
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
        public string directorsFlag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public string supervisorsFlag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public string shareholdersFlag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public string noticeTypeCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public string name { get; set; }
    }
}
