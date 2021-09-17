using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace personnel_tracking_dto
{
    /// <summary>
    /// QR Scan class
    /// </summary>
    public class QRScan
    {
        /// <summary>
        /// Comes from login
        /// </summary>
        public long IdentityNumber { get; set; }
        /**
         * Comes from the qr code, following json string:
        {
            areaId: int
            company: string,
            area: string,
            latitude: float in js, decimal in c#
            longitude: same
        }
         */
        public int AreaId { get; set; }
        public int CompanyId { get; set; }
        public string AreaName { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitute { get; set; }
        /**
         * Scan time 
         */
        public DateTime scanTime { get; set; }
    }
}
