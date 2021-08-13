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
    public class AreaController : ControllerBase {
        private readonly PersonnelTrackingDBContext dbContext;
        public AreaController() {
            if (dbContext == null) dbContext = new PersonnelTrackingDBContext();
        }

        [HttpGet]
        public IActionResult Get() {
            ResponseModel response = new ResponseModel();

            try {
                var areaList = dbContext.Areas.Where(u => true).ToList();
                for(int i = 0; i < areaList.Count; i++) {
                    Console.WriteLine(areaList[i].AreaName);
                }
                response.Data = areaList;
            } catch (Exception ex) {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }

            return Ok(response);
        }
        // TODO: Rearrange here later
        [HttpPost]
        public IActionResult Post(int areaId, int companyId, string areaName, decimal latitude, decimal longitude, byte[] qrCode) {
            ResponseModel response = new ResponseModel();

            try {
                Area newArea = new Area();
                newArea.AreaId = areaId;
                newArea.CompanyId = areaId;
                newArea.Latitude = latitude;
                newArea.Longitude = longitude;
                newArea.QrCode = qrCode;
                dbContext.Add<Area>(newArea);
                response.Data = newArea;
            } catch (Exception e) {
                response.HasError = true;
                response.ErrorMessage = e.Message;
            }

            return Ok(response);
        }
        //TODO: Implement later
        [HttpPut("{areaId}")]
        public IActionResult Put(int areaId) {
            ResponseModel response = new ResponseModel();

            try {
                Area area = dbContext.Areas.Where(u => u.AreaId == areaId).FirstOrDefault();
            } catch (Exception e) {
                response.HasError = true;
                response.ErrorMessage = e.Message;
            }

            return Ok(response);
        }

        [HttpDelete("{areaId}")]
        public IActionResult Delete(int areaId)
        {
            ResponseModel response = new ResponseModel();

            try {
                Area area = dbContext.Areas.Where(u => u.AreaId == areaId).FirstOrDefault();
                dbContext.Areas.Remove(area);
            } catch (Exception e) {
                response.HasError = true;
                response.ErrorMessage = e.Message;
            }

            return Ok(response);
        }
        
        public IActionResult SaveChanges() {
            ResponseModel response = new ResponseModel();
            try {
                dbContext.SaveChanges();
            } catch (Exception e) {
                response.HasError = true;
                response.ErrorMessage = e.Message;
            }

            return Ok(response);
        }
    }
}
