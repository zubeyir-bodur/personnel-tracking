using System;
using System.Collections.Generic;

#nullable disable

namespace personnel_tracking_entity
{
    public partial class Company
    {
        public Company()
        {
            Areas = new HashSet<Area>();
            Personnel = new HashSet<Personnel>();
        }

        public int CompanyId { get; set; }
        public string CompanyName { get; set; }

        public virtual ICollection<Area> Areas { get; set; }
        public virtual ICollection<Personnel> Personnel { get; set; }
    }
}
