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
                response.Data = areaList;
            } catch (Exception ex) {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }

            return Ok(response);
        }

        // TODO: Rearrange here later
        [HttpPost]
        public IActionResult Post(Area areaDto) {
            ResponseModel response = new ResponseModel();

            try {
                Area newArea = new Area();
                newArea.AreaId = areaDto.AreaId;
                newArea.CompanyId = areaDto.CompanyId;
                newArea.Latitude = areaDto.Latitude;
                newArea.Longitude = areaDto.Longitude;
                newArea.QrCode = areaDto.QrCode;
               
                dbContext.Add<Area>(newArea);
               
                response.Data = newArea;
            } catch (Exception e) {
                response.HasError = true;
                response.ErrorMessage = e.Message;
            }

            return Ok(response);
        }

        
        [HttpPut]
        public IActionResult Put(Area areaDto) {
            ResponseModel response = new ResponseModel();

            try {
                Area area = dbContext.Areas.Where(u => u.AreaId == areaDto.AreaId).FirstOrDefault();
                area.CompanyId = areaDto.CompanyId;
                area.Latitude = areaDto.Latitude;
                area.Longitude = areaDto.Longitude;
                area.QrCode = areaDto.QrCode;
                dbContext.Update<Area>(area);

            } catch (Exception e) {
                response.HasError = true;
                response.ErrorMessage = e.Message;
            }

            return Ok(response);
        }

        [HttpDelete]
        public IActionResult Delete(Area areaDto)
        {
            ResponseModel response = new ResponseModel();

            try {
                Area area = dbContext.Areas.Where(u => u.AreaId == areaDto.AreaId).FirstOrDefault();
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
