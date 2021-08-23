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
    //[TokenCheck]
    public class AreaController : ControllerBase
    {
        private readonly PersonnelTrackingDBContext dbContext;
        public AreaController()
        {
            if (dbContext == null) dbContext = new PersonnelTrackingDBContext();
        }

        [HttpGet]
        public IActionResult Get()
        {
            ResponseModel response = new ResponseModel();

            try
            {
                var areaList = dbContext.Areas.Include(x => x.Company).ToList();
                var areaDTOList = dbContext.Areas.Select(u => new
                {
                    u.AreaId,
                    u.Company.CompanyName,
                    u.AreaName,
                    u.Latitude,
                    u.Longitude,
                    string.Empty//u.QrCode
                });

                response.Data = areaDTOList;
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }
            return Ok(response);
        }


        [HttpPost]
        public IActionResult Post([FromBody] AreaDTO areaDto)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                Area newArea = new Area();
                Company company = dbContext.Companies.Where<Company>(u => u.CompanyName == areaDto.company).FirstOrDefault();
                newArea.AreaName = areaDto.area;
                newArea.CompanyId = company.CompanyId;

                newArea.Latitude = Convert.ToDecimal(areaDto.latitude);
                newArea.Longitude = Convert.ToDecimal(areaDto.longitude);
                newArea.QrCode = Convert.FromBase64String(areaDto.qr_code.Substring(22));

                dbContext.Add<Area>(newArea);

            }
            catch (Exception e)
            {
                response.HasError = true;
                response.ErrorMessage = e.Message;
            }
            Console.WriteLine("Affected row number is " + dbContext.SaveChanges());

            response.Data = dbContext.Areas.Select(u => new
            {
                u.AreaId,
                u.Company.CompanyName,
                u.AreaName,
                u.Latitude,
                u.Longitude,
                u.QrCode
            }).ToList();
            return Ok(response);
        }


        [HttpPut]
        public IActionResult Put(AreaDTO areaDto)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                Area area = dbContext.Areas.Where(u => u.AreaId == areaDto.areaId).FirstOrDefault();
                Company company = dbContext.Companies.Where<Company>(u => u.CompanyName == areaDto.company).FirstOrDefault();
                area.AreaName = areaDto.area;
                area.CompanyId = company.CompanyId;
                area.Latitude = Convert.ToDecimal(areaDto.latitude);
                area.Longitude = Convert.ToDecimal(areaDto.longitude);

                dbContext.Update<Area>(area);

            }
            catch (Exception e)
            {
                response.HasError = true;
                response.ErrorMessage = e.Message;
            }

            Console.WriteLine("Affected row number is " + dbContext.SaveChanges());

            response.Data = dbContext.Areas.Select(u => new
            {
                u.AreaId,
                u.Company.CompanyName,
                u.AreaName,
                u.Latitude,
                u.Longitude,
                u.QrCode
            }).ToList();
            return Ok(response);
        }

        [HttpDelete]
        public IActionResult Delete(AreaDTO areaDto)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                Area area = dbContext.Areas.Where(u => u.AreaId == areaDto.areaId).FirstOrDefault();
                dbContext.Areas.Remove(area);
            }
            catch (Exception e)
            {
                response.HasError = true;
                response.ErrorMessage = e.Message;
            }
            Console.WriteLine("Affected row number is " + dbContext.SaveChanges());

            response.Data = dbContext.Areas.Select(u => new
            {
                u.AreaId,
                u.Company.CompanyName,
                u.AreaName,
                u.Latitude,
                u.Longitude,
                u.QrCode
            }).ToList();
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
    public class AreaDTO
    {
        public int areaId { get; set; }
        public string company { get; set; }
        public string area { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string qr_code { get; set; }
    }
}
