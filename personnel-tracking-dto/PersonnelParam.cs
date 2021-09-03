using System;
using System.Collections.Generic;
using System.Text;

namespace personnel_tracking_dto
{
    public class PersonnelParam
    {
        /// <summary>
        /// Helper dto class for receiving report parameters
        /// to create files for a given area
        /// </summary>
        public int PersonnelId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
