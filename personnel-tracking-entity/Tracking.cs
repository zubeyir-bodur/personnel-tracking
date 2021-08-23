using Newtonsoft.Json;
using System;
using System.Collections.Generic;

#nullable disable

namespace personnel_tracking_entity
{
    public partial class Tracking
    {
        public int TrackingId { get; set; }
        public int PersonnelId { get; set; }
        public int AreaId { get; set; }
        public DateTime EntranceDate { get; set; }
        public DateTime? ExitDate { get; set; }
        public bool AutoExit { get; set; }
        [JsonIgnore]
        public virtual Area Area { get; set; }
        [JsonIgnore]
        public virtual Personnel Personnel { get; set; }
    }
}
