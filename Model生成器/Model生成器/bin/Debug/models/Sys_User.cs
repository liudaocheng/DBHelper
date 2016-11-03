using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public partial class Sys_User
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
        public string userName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public string showName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public string pwd { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [IsDBField]
        public string remarks { get; set; }
    }
}
