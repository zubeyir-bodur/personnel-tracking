using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using personnel_tracking_entity;
using personnel_tracking_dto;
using personnel_tracking_webapi.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using personnel_tracking_webapi.Models;
using Microsoft.EntityFrameworkCore;

namespace personnel_tracking_webapi.Controllers
{
    /// <summary>
    /// Home controller that is responsible from managing login and qr code operations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [TokenCheck]
    public class HomeController : ControllerBase
    {
        private readonly PersonnelTrackingDBContext dbContext;
        public HomeController()
        {
            if (dbContext == null) dbContext = new PersonnelTrackingDBContext();
        }

        /// <summary>
        /// Check credenntials of the user
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns>The response model</returns>
        [HttpPost("login")]
        public IActionResult Login(UserInfo userInfo)
        {
            Console.WriteLine(userInfo.Identitynumber);
            var response = new ResponseModel();
            try
            {
                // 1. Check if the user id exists in personnel table
                var personnel = dbContext.Personnel.AsNoTracking()
                    .FirstOrDefault(p => p.IdentityNumber == userInfo.Identitynumber);
                bool exists = personnel != null;
                Console.WriteLine(exists);
                // 1.1 If not, send a error response
                if (!exists)
                {
                    Console.WriteLine("nope");
                    response.HasError = true;
                    response.ErrorMessage = "Please check your credentials";
                }
                // 1.2 If yes, send all the personel info except their password
                else
                {
                    // Don't send password to frontend
                    Console.WriteLine("yep");
                    response.Data = new {
                        personnelId = personnel.PersonnelId,
                        companyName = dbContext.Companies
                            .FirstOrDefault(c => c.CompanyId == personnel.CompanyId).CompanyName,
                        personnelTypeName = dbContext.PersonnelTypes
                            .FirstOrDefault(pt => pt.PersonnelTypeId == personnel.PersonnelTypeId).PersonnelTypeName,
                        personnelName = personnel.PersonnelName,
                        personnelSurname = personnel.PersonnelSurname,
                        username = personnel.UserName,
                        identityNumber = personnel.IdentityNumber
                    };
                }
            }
            catch (Exception ex) {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    response.ErrorMessage += ": " + ex.InnerException.Message;
                }
            }
            return Ok(response);
        }

        /// <summary>
        /// Registering QR code to database after a user logged in successfully
        /// </summary>
        /// <param name="scan"></param>
        /// <returns></returns>
        [HttpPost("processScan")]
        public IActionResult ProcessScan(QRScan scan)
        {
            var response = new ResponseModel();
            try
            {
                var personnel = dbContext.Personnel.
                    AsNoTracking().FirstOrDefault(p => p.IdentityNumber == scan.IdentityNumber);
                Tracking newTracking = new Tracking
                {
                    PersonnelId = personnel.PersonnelId,
                    AreaId = scan.AreaId,
                    // assuming a simple entry
                    EntranceDate = scan.scanTime,
                    ExitDate = null,
                    AutoExit = false
                };

                //Auto Exit procedure
                // 1. find the last recent tracking
                var trackingsOfP = dbContext.Trackings.AsNoTracking().Where(t => t.PersonnelId == newTracking.PersonnelId).ToList();
                var lastTrackingOfP = trackingsOfP.OrderBy(t => t.EntranceDate).LastOrDefault();

                if (lastTrackingOfP != null) // base case for the first scan
                {
                    if (lastTrackingOfP.ExitDate == null)
                    {
                        // haven't exit the old area yet
                        if (lastTrackingOfP.AreaId == scan.AreaId)
                        {
                            // old area is the same area with the scanned area
                            // second scan from same sector == exit
                            newTracking.TrackingId = lastTrackingOfP.TrackingId; // same row, where the entry has been made should be changed
                            newTracking.EntranceDate = lastTrackingOfP.EntranceDate; // no change should be in entry date
                            newTracking.ExitDate = scan.scanTime; // exit date is the scan time
                            dbContext.Update<Tracking>(newTracking).State = EntityState.Modified; // update that row
                            response.Data = "Exit from "
                                + dbContext.Areas.AsNoTracking().FirstOrDefault(a => a.AreaId == newTracking.AreaId).AreaName
                                + " was successful";
                        }
                        else
                        {   
                            // New scan in another sector without exitting
                            // 1.1 Entry to a new sector 
                            newTracking.AreaId = scan.AreaId;
                            newTracking.PersonnelId = personnel.PersonnelId;
                            newTracking.EntranceDate = scan.scanTime;
                            newTracking.ExitDate = null;
                            newTracking.AutoExit = false;
                            // 1.2 AutoExit from lastTrackingOfP
                            var oldTracking = new Tracking
                            {
                                TrackingId = lastTrackingOfP.TrackingId,
                                AreaId = lastTrackingOfP.AreaId,
                                PersonnelId = lastTrackingOfP.PersonnelId,
                                EntranceDate = lastTrackingOfP.EntranceDate,
                                ExitDate = scan.scanTime,
                                AutoExit = true
                            };
                            // update old tracking, add new tracking
                            dbContext.Update<Tracking>(oldTracking).State = EntityState.Modified;
                            dbContext.Add<Tracking>(newTracking).State = EntityState.Added;
                            response.Data = "You have been automatically exited from "
                                + dbContext.Areas.AsNoTracking().FirstOrDefault(a => a.AreaId == lastTrackingOfP.AreaId).AreaName
                                + ". Entry into "
                                + dbContext.Areas.AsNoTracking().FirstOrDefault(a => a.AreaId == newTracking.AreaId).AreaName
                                + " is successful.";
                        }
                    }
                    else
                    {
                        // exit date was not null, so it was an entry
                        dbContext.Add<Tracking>(newTracking).State = EntityState.Added;
                        response.Data = "Entry into "
                                + dbContext.Areas.AsNoTracking().FirstOrDefault(a => a.AreaId == newTracking.AreaId).AreaName
                                + " was successful.";
                    }
                }
                else 
                {
                    dbContext.Add<Tracking>(newTracking).State = EntityState.Added;
                    response.Data = "Entry into "
                                    + dbContext.Areas.AsNoTracking().FirstOrDefault(a => a.AreaId == newTracking.AreaId).AreaName
                                    + " was successful.";
                }
                dbContext.SaveChanges();
            }
            catch (Exception ex) {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    response.ErrorMessage += ": " + ex.InnerException.Message;
                }
            }
            return Ok(response);
        }
    }
}
