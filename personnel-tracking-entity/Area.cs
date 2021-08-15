using Newtonsoft.Json;
using System;
using System.Collections.Generic;

#nullable disable

namespace personnel_tracking_entity
{
    public partial class Area
    {
        public Area()
        {
            Trackings = new HashSet<Tracking>();
        }

        public int AreaId { get; set; }
        public int CompanyId { get; set; }
        public string AreaName { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public byte[] QrCode { get; set; }
        [JsonIgnore]
        public virtual Company Company { get; set; }
        [JsonIgnore]
        public virtual ICollection<Tracking> Trackings { get; set; }
    }
}
