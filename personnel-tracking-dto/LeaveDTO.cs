using System;
using System.Collections.Generic;

#nullable disable

namespace personnel_tracking_dto
{
    public partial class LeaveDTO
    {
        public int leaveId { get; set; }
        public string personnelName { get; set; }
        public string personnelSurname { get; set; }

        public DateTime leaveStart { get; set; }
        public DateTime leaveEnd { get; set; }
    }
}
