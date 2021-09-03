using System;
using System.Collections.Generic;

#nullable disable

namespace personnel_tracking_dto
{
    public partial class AreaDTO
    {
        public int areaId { get; set; }
        public int companyId { get; set; }
        public string companyName { get; set; }
        public string areaName { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string qr_code { get; set; }
    }
}
