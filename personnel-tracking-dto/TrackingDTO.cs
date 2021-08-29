using System;

#nullable disable

namespace personnel_tracking_dto
{
    public partial class TrackingDTO
    {
        public int trackingId { get; set; }
        public string personnelName { get; set; }
        public string personnelSurname { get; set; }
        public string areaName { get; set; }
        public DateTime entranceDate { get; set; }
        public DateTime? exitDate { get; set; }
        public bool autoExit { get; set; }
    }
}
