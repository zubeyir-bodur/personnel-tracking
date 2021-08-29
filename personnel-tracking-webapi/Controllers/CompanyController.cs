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
using System.Threading.Tasks;

namespace personnel_tracking_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TokenCheck]
    public class CompanyController : ControllerBase
    {
        private readonly PersonnelTrackingDBContext dbContext;
        public CompanyController()
        {
            if (dbContext == null) dbContext = new PersonnelTrackingDBContext();
        }

        /// <summary>
        /// Gets all companies to show to the admin
        /// </summary>
        /// <returns></returns>
        /// GET: http://localhost:5000/api/company
        [HttpGet]
        public IActionResult Get()
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var companyDTOList = dbContext.Companies.Select(u => new CompanyDTO
                {
                    CompanyId = u.CompanyId,
                    CompanyName = u.CompanyName
                }).ToList();
                response.Data = companyDTOList;
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

        /// <summary>
        /// Registers a company to the table
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post(CompanyDTO companyDto)
        {
            var response = new ResponseModel();
            try
            {
                var newCompany = new Company
                {
                    CompanyId = companyDto.CompanyId,
                    CompanyName = companyDto.CompanyName
                };
                // don't add same company twice
                bool exists = dbContext.Companies.
                    FirstOrDefault(c => c.CompanyName.Equals(companyDto.CompanyName)) != null;
                if (exists)
                    throw new Exception("Same company already exists.");
                else
                {
                    dbContext.Add<Company>(newCompany).State = EntityState.Added;
                    Console.WriteLine(dbContext.SaveChanges() + " rows affected.");
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
            response.Data = dbContext.Companies.Select(u => new CompanyDTO
            {
                CompanyId = u.CompanyId,
                CompanyName = u.CompanyName
            }).ToList();
            return Ok(response);
        }

        /// <summary>
        /// Edits a company row provided that the form is submitted
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public IActionResult Put(CompanyDTO companyDto)
        {
            var response = new ResponseModel();
            try
            {
                var newCompany = new Company
                {
                    CompanyId = companyDto.CompanyId,
                    CompanyName = companyDto.CompanyName
                };
                // don't let the user change the company's name to another company's name
                bool exists = dbContext.Companies.
                    FirstOrDefault(c => c.CompanyName.Equals(companyDto.CompanyName)) != null;
                if (exists)
                    throw new Exception("Same company already exists.");
                else
                {
                    dbContext.Update<Company>(newCompany).State = EntityState.Modified;
                    Console.WriteLine(dbContext.SaveChanges() + " rows affected.");
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
            response.Data = dbContext.Companies.Select(u => new CompanyDTO
            {
                CompanyId = u.CompanyId,
                CompanyName = u.CompanyName
            }).ToList();
            return Ok(response);
        }

        /// <summary>
        /// Deletes the company from the database
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public IActionResult Delete(CompanyDTO companyDto)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                var delCompany = new Company
                {
                    CompanyId = companyDto.CompanyId,
                    CompanyName = companyDto.CompanyName
                };

                bool isRelationalArea, isRelationalPersonnel;
                isRelationalArea = dbContext.Areas.AsNoTracking().FirstOrDefault(a => a.CompanyId == companyDto.CompanyId) != null;
                isRelationalPersonnel = dbContext.Personnel.AsNoTracking().FirstOrDefault(a => a.CompanyId == companyDto.CompanyId) != null;
                if (isRelationalArea && !isRelationalPersonnel)
                    throw new Exception("The company being deleted has areas related to it, please delete areas first.");
                if (isRelationalPersonnel&& !isRelationalArea)
                    throw new Exception("The company being deleted has personnel related to it, please delete those personnel first.");
                if (isRelationalPersonnel && isRelationalArea)
                    throw new Exception("The company being deleted has personnel and areas related to it, please delete those first.");

                dbContext.Companies.Remove(delCompany).State = EntityState.Deleted;
                Console.WriteLine(dbContext.SaveChanges() + " rows affected.");
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
                if (ex.InnerException != null) {
                    response.ErrorMessage += ": " + ex.InnerException.Message;
                }
            }
            response.Data = dbContext.Companies.Select(u => new CompanyDTO {
                CompanyId = u.CompanyId, 
                CompanyName = u.CompanyName 
            }).ToList();
            return Ok(response);
        }

        /* No wizard base admin page
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
        }*/
    }
}