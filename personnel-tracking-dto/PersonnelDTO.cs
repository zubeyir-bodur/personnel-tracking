using System;
using System.Collections.Generic;

#nullable disable

namespace personnel_tracking_dto
{
    public partial class PersonnelDTO
    {
        public int personnelId { get; set; }
        public string company { get; set; }
        public string personnelType { get; set; }
        public long identityNumber { get; set; }
        public string personnelName { get; set; }
        public string personnelSurname { get; set; }
        public string username { get; set; }
        public string password { get; set; }

        //maybe add virtual ones too?
    }
}
