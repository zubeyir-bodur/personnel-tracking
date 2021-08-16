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
                    var leaveList = dbContext.Leaves.Where(u => true).ToList();
                    response.Data = leaveList;
                }
                catch (Exception ex)
                {
                    response.HasError = true;
                    response.ErrorMessage = ex.Message;
                }

                return Ok(response);
            }

            // TODO: Rearrange here later
            [HttpPost]
            public IActionResult Post(Leave leaveDto)
            {
                ResponseModel response = new ResponseModel();

                try
                {
                    Leave newLeave = new Leave();
                newLeave.LeaveId = leaveDto.LeaveId;
                newLeave.PersonnelId = leaveDto.PersonnelId;
                newLeave.LeaveStart = leaveDto.LeaveStart;
                newLeave.LeaveEnd = leaveDto.LeaveEnd;

                    dbContext.Add<Leave>(newLeave);

                    response.Data = newLeave;
                }
                catch (Exception e)
                {
                    response.HasError = true;
                    response.ErrorMessage = e.Message;
                }

                return Ok(response);
            }


            [HttpPut]
            public IActionResult Put(Leave leaveDto)
            {
                ResponseModel response = new ResponseModel();

                try
                {
                Leave leave = dbContext.Leaves.Where(u => u.LeaveId == leaveDto.LeaveId).FirstOrDefault();
                leave.LeaveId = leaveDto.LeaveId;
                leave.PersonnelId = leaveDto.PersonnelId;
                leave.LeaveStart = leaveDto.LeaveStart;
                leave.LeaveEnd = leaveDto.LeaveEnd;
                dbContext.Update<Leave>(leave);

                }
                catch (Exception e)
                {
                    response.HasError = true;
                    response.ErrorMessage = e.Message;
                }

                return Ok(response);
            }

            [HttpDelete]
            public IActionResult Delete(Leave leaveDto)
            {
                ResponseModel response = new ResponseModel();

                try
            {
                    Leave leave = dbContext.Leaves.Where(u => u.LeaveId == leaveDto.LeaveId).FirstOrDefault();
                    dbContext.Leaves.Remove(leave);
                }
                catch (Exception e)
                {
                    response.HasError = true;
                    response.ErrorMessage = e.Message;
                }

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
    }
