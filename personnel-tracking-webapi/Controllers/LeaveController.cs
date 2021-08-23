using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public class LeaveController : ControllerBase
        {
            private readonly PersonnelTrackingDBContext dbContext;
            public LeaveController()
            {
                if (dbContext == null) dbContext = new PersonnelTrackingDBContext();
            }

        [HttpGet]
        public IActionResult Get()
        {
            ResponseModel response = new ResponseModel();

            try
            {
                var leaveList = dbContext.Set<Leave>().ToList();
                var leaveDTOlist = new List<LeaveDTO>();
                for (int i = 0; i < leaveList.Count; i++)
                {
                    leaveDTOlist.Add(new LeaveDTO
                    {
                        leaveId = leaveList[i].LeaveId,
                        personnelName = dbContext.Personnel.Where(e => e.PersonnelId == leaveList[i].PersonnelId).FirstOrDefault().PersonnelName,
                        personnelSurname = dbContext.Personnel.Where(e => e.PersonnelId == leaveList[i].PersonnelId).FirstOrDefault().PersonnelSurname,
                        leaveStart = leaveList[i].LeaveStart,
                        leaveEnd = leaveList[i].LeaveEnd,
                    });
                }

                response.Data = leaveDTOlist;

            }



            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }

            return Ok(response);
        }

        [HttpPost]
        public IActionResult Post(Leave leave)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                dbContext.Leaves.Add(leave);
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }
            var leaveList = dbContext.Set<Leave>().ToList();
            var leaveDTOlist = new List<LeaveDTO>();
            for (int i = 0; i < leaveList.Count; i++)
            {
                leaveDTOlist.Add(new LeaveDTO
                {
                    leaveId = leaveList[i].LeaveId,
                    personnelName = dbContext.Personnel.Where(e => e.PersonnelId == leaveList[i].PersonnelId).FirstOrDefault().PersonnelName,
                    personnelSurname = dbContext.Personnel.Where(e => e.PersonnelId == leaveList[i].PersonnelId).FirstOrDefault().PersonnelSurname,
                    leaveStart = leaveList[i].LeaveStart,
                    leaveEnd = leaveList[i].LeaveEnd,
                });
            }
            response.Data = leaveDTOlist;
            return Ok(response);
        }

        [HttpPut]
        public IActionResult Put(Leave leave)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                if (leave != null)
                {
                    Leave leaveDeleted = dbContext.Set<Leave>().Where(x => x.LeaveId == leave.LeaveId).FirstOrDefault<Leave>();

                    leaveDeleted.LeaveStart = leave.LeaveStart;
                    leaveDeleted.LeaveEnd = leave.LeaveEnd;

                    dbContext.Update<Leave>(leaveDeleted);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }
            var leaveList = dbContext.Set<Leave>().ToList();
            var leaveDTOlist = new List<LeaveDTO>();
            for (int i = 0; i < leaveList.Count; i++)
            {
                leaveDTOlist.Add(new LeaveDTO
                {
                    leaveId = leaveList[i].LeaveId,
                    personnelName = dbContext.Personnel.Where(e => e.PersonnelId == leaveList[i].PersonnelId).FirstOrDefault().PersonnelName,
                    personnelSurname = dbContext.Personnel.Where(e => e.PersonnelId == leaveList[i].PersonnelId).FirstOrDefault().PersonnelSurname,
                    leaveStart = leaveList[i].LeaveStart,
                    leaveEnd = leaveList[i].LeaveEnd,
                });
            }
            response.Data = leaveDTOlist;
            return Ok(response);
        }

        [HttpDelete]
        public IActionResult Delete(Leave leave)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                Leave leaveDeleted = dbContext.Leaves.Where(x => x.LeaveId== leave.LeaveId).FirstOrDefault<Leave>();
                dbContext.Leaves.Remove(leaveDeleted).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }
            var leaveList = dbContext.Set<Leave>().ToList();
            var leaveDTOlist = new List<LeaveDTO>();
            for (int i = 0; i < leaveList.Count; i++)
            {
                leaveDTOlist.Add(new LeaveDTO
                {
                    leaveId = leaveList[i].LeaveId,
                    personnelName = dbContext.Personnel.Where(e => e.PersonnelId == leaveList[i].PersonnelId).FirstOrDefault().PersonnelName,
                    personnelSurname = dbContext.Personnel.Where(e => e.PersonnelId == leaveList[i].PersonnelId).FirstOrDefault().PersonnelSurname,
                    leaveStart = leaveList[i].LeaveStart,
                    leaveEnd = leaveList[i].LeaveEnd,
                });
            }
            response.Data = leaveDTOlist;
            return Ok(response);
        }

   

    public IActionResult SaveChanges()
        {
            ResponseModel response = new ResponseModel();
            try
            {
                dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                response.HasError = true;
                response.ErrorMessage = e.Message;
            }

            return Ok(response);
        }
    }
    public class LeaveDTO
    {
        public int leaveId { get; set; }
        public string personnelName { get; set; }
        public string personnelSurname { get; set; }

        public DateTime leaveStart { get; set; }
        public DateTime leaveEnd { get; set; }
    }


}
