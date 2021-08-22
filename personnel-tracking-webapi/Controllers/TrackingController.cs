using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personnel_tracking_entity;
using personnel_tracking_webapi.Filters;
using personnel_tracking_webapi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace personnel_tracking_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TokenCheck]
    public class TrackingController : ControllerBase
    {
        private readonly PersonnelTrackingDBContext dbContext;
        public TrackingController()
        {
            if (dbContext == null) dbContext = new PersonnelTrackingDBContext();
        }

        [HttpGet]
        public IActionResult Get()
        {
            ResponseModel response = new ResponseModel();

            try
            {
                var trackingList = dbContext.Set<Tracking>().ToList();
                var tList = new List<TrackingDTO>();
                for (int i = 0; i < trackingList.Count; i++)
                {
                    tList.Add(new TrackingDTO
                    {
                        trackingId = trackingList[i].TrackingId,
                        personnelName = dbContext.Personnel.Where(e => e.PersonnelId == trackingList[i].PersonnelId).FirstOrDefault().PersonnelName,
                        personnelSurname = dbContext.Personnel.Where(e => e.PersonnelId == trackingList[i].PersonnelId).FirstOrDefault().PersonnelSurname,
                        areaName = dbContext.Areas.Where(a => a.AreaId == trackingList[i].AreaId).FirstOrDefault().AreaName,
                        entranceDate = trackingList[i].EntranceDate,
                        exitDate = trackingList[i].ExitDate,
                        autoExit = trackingList[i].AutoExit
                    });
                }

                response.Data = tList;

            }



            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }

            return Ok(response);
        }


        [HttpPost]
        public IActionResult Post(Tracking t)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                dbContext.Trackings.Add(t);
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }

            var trackingList = dbContext.Set<Tracking>().ToList();
            var trackingDTOList = new List<TrackingDTO>();
            for (int i = 0; i < trackingList.Count; i++)
            {
                trackingDTOList.Add(new TrackingDTO
                {
                    trackingId = trackingList[i].TrackingId,
                    personnelName = dbContext.Personnel.Where(e => e.PersonnelId == trackingList[i].PersonnelId).FirstOrDefault().PersonnelName,
                    personnelSurname = dbContext.Personnel.Where(e => e.PersonnelId == trackingList[i].PersonnelId).FirstOrDefault().PersonnelSurname,
                    areaName = dbContext.Areas.Where(a => a.AreaId == trackingList[i].AreaId).FirstOrDefault().AreaName,
                    entranceDate = trackingList[i].EntranceDate,
                    exitDate = trackingList[i].ExitDate,
                    autoExit = trackingList[i].AutoExit,
                });
            }

            response.Data = trackingDTOList;
           return Ok(response);
        }

        [HttpPut]
        public IActionResult Put(Tracking t)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                if (t != null)
                {
                    Tracking trackingData = dbContext.Set<Tracking>().Where(p => p.TrackingId == t.TrackingId).FirstOrDefault<Tracking>();

                    trackingData.PersonnelId = t.PersonnelId;
                    trackingData.AreaId = t.AreaId;
                    trackingData.EntranceDate = t.EntranceDate;
                    trackingData.ExitDate = t.ExitDate;
                    trackingData.AutoExit = t.AutoExit;

                    dbContext.Update<Tracking>(trackingData);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }

            var trackingList = dbContext.Set<Tracking>().ToList();
            var trackingDTOList = new List<TrackingDTO>();
            for (int i = 0; i < trackingList.Count; i++)
            {
                trackingDTOList.Add(new TrackingDTO
                {
                    trackingId = trackingList[i].TrackingId,
                    personnelName = dbContext.Personnel.Where(e => e.PersonnelId == trackingList[i].PersonnelId).FirstOrDefault().PersonnelName,
                    personnelSurname = dbContext.Personnel.Where(e => e.PersonnelId == trackingList[i].PersonnelId).FirstOrDefault().PersonnelSurname,
                    areaName = dbContext.Areas.Where(a => a.AreaId == trackingList[i].AreaId).FirstOrDefault().AreaName,
                    entranceDate = trackingList[i].EntranceDate,
                    exitDate = trackingList[i].ExitDate,
                    autoExit = trackingList[i].AutoExit,
                });
            }

            response.Data = trackingDTOList;
            return Ok(response);
        }

        [HttpDelete]
        public IActionResult Delete(Tracking t)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                Tracking trackingData = dbContext.Trackings.Where(x => x.TrackingId == t.TrackingId ).FirstOrDefault<Tracking>();
                dbContext.Trackings.Remove(trackingData).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }

            var trackingList = dbContext.Set<Tracking>().ToList();
            var trackingDTOList = new List<TrackingDTO>();
            for (int i = 0; i < trackingList.Count; i++)
            {
                trackingDTOList.Add(new TrackingDTO
                {
                    trackingId = trackingList[i].TrackingId,
                    personnelName = dbContext.Personnel.Where(e => e.PersonnelId == trackingList[i].PersonnelId).FirstOrDefault().PersonnelName,
                    personnelSurname = dbContext.Personnel.Where(e => e.PersonnelId == trackingList[i].PersonnelId).FirstOrDefault().PersonnelSurname,
                    areaName = dbContext.Areas.Where(a => a.AreaId == trackingList[i].AreaId).FirstOrDefault().AreaName,
                    entranceDate = trackingList[i].EntranceDate,
                    exitDate = trackingList[i].ExitDate,
                    autoExit = trackingList[i].AutoExit,
                });
            }

            response.Data = trackingDTOList;
            return Ok(response);
        }

    }


    public class TrackingDTO
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
