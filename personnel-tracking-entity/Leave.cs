using System;
using System.Collections.Generic;

#nullable disable

namespace personnel_tracking_entity
{
    public partial class Leave
    {
        public int LeaveId { get; set; }
        public int PersonnelId { get; set; }
        public DateTime LeaveStart { get; set; }
        public DateTime LeaveEnd { get; set; }

        public virtual Personnel Personnel { get; set; }
    }
}
