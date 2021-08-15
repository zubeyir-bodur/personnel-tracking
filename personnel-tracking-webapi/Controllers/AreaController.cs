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
                var areaList = dbContext.Areas.ToList();
                var areaDTOList = new List<AreaDTO>();
                for(int i = 0; i < areaList.Count; i++) {
                    areaDTOList.Add(new AreaDTO {
                        areaId = areaList[i].AreaId,
                        area = areaList[i].AreaName,
                        company = dbContext.Companies.Where(u => u.CompanyId == areaList[i].CompanyId).FirstOrDefault().CompanyName,
                        latitude = Convert.ToDouble(areaList[i].Latitude),
                        longitude = Convert.ToDouble(areaList[i].Longitude)
                    });
                }
                response.Data = areaDTOList;
            } catch (Exception ex) {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }
            Console.WriteLine("Get");
            return Ok(response);
        }

        // TODO: Rearrange here later
        [HttpPost]
        public IActionResult Post([FromBody]AreaDTO areaDto) {
            ResponseModel response = new ResponseModel();
            Console.WriteLine(areaDto.area);
            Console.WriteLine(areaDto.company);
            Console.WriteLine(areaDto.latitude);
            Console.WriteLine(areaDto.longitude);
            try {
                Area newArea = new Area();
                Company company = dbContext.Companies.Where<Company>(u => u.CompanyName == areaDto.company).FirstOrDefault();
                newArea.AreaName = areaDto.area;
                newArea.CompanyId = company.CompanyId;
                
                newArea.Latitude = Convert.ToDecimal(areaDto.latitude);
                newArea.Longitude = Convert.ToDecimal(areaDto.longitude);

                dbContext.Add<Area>(newArea);
            } catch (Exception e) {
                response.HasError = true;
                response.ErrorMessage = e.Message;
            }
            Console.WriteLine("Affected row number is " + dbContext.SaveChanges());
            Console.WriteLine("******");
            for(int i = 0; i < dbContext.Areas.ToList().Count; i++) {
                Console.WriteLine(dbContext.Areas.Where(u => true).ToList()[i].AreaName);
                Console.WriteLine(".");
            }
            response.Data = dbContext.Areas.ToList();
            return Ok(response);
        }

        
        [HttpPut]
        public IActionResult Put(AreaDTO areaDto) {
            ResponseModel response = new ResponseModel();

            try {
                Area area = dbContext.Areas.Where(u => u.AreaId == areaDto.areaId).FirstOrDefault();
                Company company = dbContext.Companies.Where<Company>(u => u.CompanyName == areaDto.company).FirstOrDefault();
                area.AreaName = areaDto.area;
                area.CompanyId = company.CompanyId;
                area.Latitude = Convert.ToDecimal(areaDto.latitude);
                area.Longitude = Convert.ToDecimal(areaDto.longitude);
          //      area.QrCode = areaDto.QrCode;
                dbContext.Update<Area>(area);

            } catch (Exception e) {
                response.HasError = true;
                response.ErrorMessage = e.Message;
            }

            Console.WriteLine("Affected row number is " + dbContext.SaveChanges());
            Console.WriteLine("******");
            for (int i = 0; i < dbContext.Areas.ToList().Count; i++) {
                Console.WriteLine(dbContext.Areas.Where(u => true).ToList()[i].AreaName);
                Console.WriteLine(".");
            }

            response.Data = dbContext.Areas.ToList();
            return Ok(response);
        }

        [HttpDelete]
        public IActionResult Delete(AreaDTO areaDto)
        {
            ResponseModel response = new ResponseModel();

            try {
                Area area = dbContext.Areas.Where(u => u.AreaId == areaDto.areaId).FirstOrDefault();
                dbContext.Areas.Remove(area);
            } catch (Exception e) {
                response.HasError = true;
                response.ErrorMessage = e.Message;
            }
            Console.WriteLine("Affected row number is " + dbContext.SaveChanges());
            Console.WriteLine("******");
            for (int i = 0; i < dbContext.Areas.ToList().Count; i++) {
                Console.WriteLine(dbContext.Areas.Where(u => true).ToList()[i].AreaName);
                Console.WriteLine(".");
            }

            response.Data = dbContext.Areas.ToList();
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
    public class AreaDTO {
        public int areaId { get; set; }
        public string company { get; set; }
        public string area { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        //Add qr code
    }
}
