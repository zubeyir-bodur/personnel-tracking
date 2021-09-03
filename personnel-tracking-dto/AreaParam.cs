using System;
using System.Collections.Generic;
using System.Text;

namespace personnel_tracking_dto
{
    /// <summary>
    /// Helper dto class for receiving report parameters
    /// to create files for a given area.
    /// Also includes company id, since 
    /// the frontend already knows which company registered the system
    /// </summary>
    public class AreaParam
    {
        public int AreaId { get; set; }
        public int CompanyId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End{ get; set; }
    }
}
