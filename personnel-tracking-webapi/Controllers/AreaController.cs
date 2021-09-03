using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personnel_tracking_entity;
using personnel_tracking_dto;
using personnel_tracking_webapi.Filters;
using personnel_tracking_webapi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace personnel_tracking_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TokenCheck]
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
                var areaDTOList = dbContext.Areas.Select(u => new AreaDTO
                {
                    areaId = u.AreaId,
                    company = u.Company.CompanyName,
                    area = u.AreaName,
                    latitude = Convert.ToDouble(u.Latitude),
                    longitude = Convert.ToDouble(u.Longitude),
                    qr_code = "data:image/png;base64," + Convert.ToBase64String(u.QrCode)
                }).ToList();

                response.Data = areaDTOList;
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    response.ErrorMessage += ": " + ex.InnerException.Message;
                }
            }
            return Ok(response);
        }


        [HttpPost]
        public IActionResult Post([FromBody] AreaDTO areaDto)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var company = dbContext.Companies.AsNoTracking().Where<Company>(u => u.CompanyName == areaDto.company).FirstOrDefault();
                Area newArea = new Area {
                        AreaName = areaDto.area,
                        CompanyId = company.CompanyId,
                        Latitude = Convert.ToDecimal(areaDto.latitude),
                        Longitude = Convert.ToDecimal(areaDto.longitude),
                        QrCode = Convert.FromBase64String("")// empty string for now
                };
                // 1- Coordinates must be unique
                bool existsCoordinates = dbContext.Areas.AsNoTracking().
                    FirstOrDefault(a => 
                    a.Latitude == newArea.Latitude
                    && a.Longitude == newArea.Longitude
                    ) != null;

                // 2- A company can't have two areas with same names
                var companyAreas = dbContext.Areas.AsNoTracking().Where(a => a.CompanyId == newArea.CompanyId).ToList();
                bool existsName = companyAreas.
                    FirstOrDefault(a =>
                    a.AreaName.Equals(newArea.AreaName)
                    ) != null; ;
                if (existsCoordinates)
                    throw new Exception("An area with same coordinates already exist!");
                else if (existsName)
                    throw new Exception("A company can't have two areas with same names!");
                else {
                    dbContext.Add<Area>(newArea).State = EntityState.Added;
                    Console.WriteLine("Affected row number is " + dbContext.SaveChanges());
                }
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    response.ErrorMessage += ": " + ex.InnerException.Message;
                }
            }
            response.Data = dbContext.Areas.Select(u => new AreaDTO
            {
                areaId = u.AreaId,
                company = u.Company.CompanyName,
                area = u.AreaName,
                latitude = Convert.ToDouble(u.Latitude),
                longitude = Convert.ToDouble(u.Longitude),
                qr_code = "data:image/png;base64," + Convert.ToBase64String(u.QrCode)
            }).ToList();
            return Ok(response);
        }
        
        [HttpPut]
        public IActionResult Put(AreaDTO areaDto)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                var company = dbContext.Companies.AsNoTracking().Where<Company>(u => u.CompanyName == areaDto.company).FirstOrDefault();
                Area newArea = new Area
                {
                    AreaId = areaDto.areaId,
                    AreaName = areaDto.area,
                    CompanyId = company.CompanyId,
                    Latitude = Convert.ToDecimal(areaDto.latitude),
                    Longitude = Convert.ToDecimal(areaDto.longitude),
                    QrCode = Convert.FromBase64String(areaDto.qr_code.Substring(22))
                };
                // 1- Coordinates must be unique, but may be equal to old one
                decimal oldLat, oldLong;
                var oldArea = dbContext.Areas.AsNoTracking().FirstOrDefault(a => a.AreaId == areaDto.areaId);
                if (oldArea != null) {
                    oldLat = oldArea.Latitude;
                    oldLong = oldArea.Longitude;
                    bool isOldCoordinate = newArea.Latitude == oldLat && newArea.Longitude == oldLong;
                    bool existsCoordinates = dbContext.Areas.AsNoTracking().
                        FirstOrDefault(a =>
                        (a.Latitude == newArea.Latitude
                        && a.Longitude == newArea.Longitude)
                        ) != null;

                    // 2- A company can't have two areas with same names, but may be equal to old one
                    var companyAreas = dbContext.Areas.AsNoTracking().Where(a => a.CompanyId == newArea.CompanyId).ToList();
                    var isOldName = oldArea.AreaName.Equals(newArea.AreaName);
                    bool existsName = companyAreas.
                        FirstOrDefault(a =>
                        a.AreaName.Equals(newArea.AreaName)
                        ) != null;
                    if (existsName && !isOldName)
                        throw new Exception("A company can't have two areas with same names!");
                    else if (existsCoordinates && !isOldCoordinate)
                        throw new Exception("An area with same coordinates already exist!");
                    else
                    {
                        dbContext.Update<Area>(newArea).State = EntityState.Modified;
                        Console.WriteLine("Affected row number is " + dbContext.SaveChanges());
                    }
                }
                else
                {
                    dbContext.Update<Area>(newArea).State = EntityState.Modified;
                    Console.WriteLine("Affected row number is " + dbContext.SaveChanges());
                }
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    response.ErrorMessage += ": " + ex.InnerException.Message;
                }
            }
            response.Data = dbContext.Areas.Select(u => new AreaDTO
            {
                areaId = u.AreaId,
                company = u.Company.CompanyName,
                area = u.AreaName,
                latitude = Convert.ToDouble(u.Latitude),
                longitude = Convert.ToDouble(u.Longitude),
                qr_code = "data:image/png;base64," + Convert.ToBase64String(u.QrCode)
            }).ToList();
            return Ok(response);
        }

        [HttpDelete]
        public IActionResult Delete(AreaDTO areaDto)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                var company = dbContext.Companies.AsNoTracking().Where<Company>(u => u.CompanyName.Equals(areaDto.company)).FirstOrDefault();
                var delArea = new Area {
                    AreaId = areaDto.areaId,
                    AreaName = areaDto.area,
                    CompanyId = company.CompanyId,
                    Latitude = Convert.ToDecimal(areaDto.latitude),
                    Longitude = Convert.ToDecimal(areaDto.longitude)
                    //QrCode = Convert.FromBase64String(areaDto.qr_code)
                };
                dbContext.Areas.Remove(delArea).State = EntityState.Deleted;
                Console.WriteLine("Affected row number is " + dbContext.SaveChanges());
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    response.ErrorMessage += ": " + ex.InnerException.Message;
                }
            }
            response.Data = dbContext.Areas.Select(u => new AreaDTO
            {
                areaId = u.AreaId,
                company = u.Company.CompanyName,
                area = u.AreaName,
                latitude = Convert.ToDouble(u.Latitude),
                longitude = Convert.ToDouble(u.Longitude),
                qr_code = "data:image/png;base64," + Convert.ToBase64String(u.QrCode)
            }).ToList();
            return Ok(response);
        }

        /// <summary>
        /// Verify the range of coordinates
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        [HttpPost("range")]
        public IActionResult InRange([FromBody] Coordinate coordinate) {
            decimal latitude = coordinate.latitude;
            decimal longitude = coordinate.longitude;
            bool[] success = {false, false};
            ResponseModel response = new ResponseModel();
            bool latInRange = !(latitude > 90 || latitude < -90);
            bool longInRange = !(longitude > 180 || longitude < -180);
            if (latInRange && !longInRange) {
                response.HasError = true;
                success[0] = true;
                response.Data = success;
            }
            else if (!latInRange && longInRange) {
                response.HasError = true;
                success[1] = true;
                response.Data = success;
            }
            else if (!latInRange && !longInRange) {
                response.HasError = true;
                response.Data = success;
            }
            else
            {
                response.HasError = false;
                success[0] = true;
                success[1] = true;
                response.Data = success;
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
