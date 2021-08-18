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
    [Route("api/company")]
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
                var companyDTOList = dbContext.Companies.Select(u => new
                {
                    u.CompanyId,
                    u.CompanyName
                }).ToList();
                response.Data = companyDTOList;
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }
            return Ok(response);
        }

        /// <summary>
        /// Registers a company to the table
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post([FromBody] CompanyDTO companyDto)
        {
            var response = new ResponseModel();
            try
            {
                var newCompany = new Company
                {
                    CompanyId = companyDto.CompanyId,
                    CompanyName = companyDto.CompanyName
                };
                dbContext.Add<Company>(newCompany).State = EntityState.Added;
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
                return NotFound(response);
            }
            Console.WriteLine(dbContext.SaveChanges() + " rows affected.");

            response.Data = dbContext.Companies.Select(u => new {
                u.CompanyId,
                u.CompanyName
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
                dbContext.Update<Company>(newCompany).State = EntityState.Modified;
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
                return NotFound(response);
            }
            Console.WriteLine(dbContext.SaveChanges() + " rows affected.");

            response.Data = dbContext.Companies.Select(u => new {
                u.CompanyId,
                u.CompanyName
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
                var company = dbContext.Companies.Where(u => u.CompanyId == companyDto.CompanyId).FirstOrDefault();
                dbContext.Companies.Remove(company).State = EntityState.Deleted;
            }
            catch (Exception e)
            {
                response.HasError = true;
                response.ErrorMessage = e.Message;
            }
            Console.WriteLine(dbContext.SaveChanges() + " rows affected.");
            response.Data = dbContext.Companies.Select(u => new {
                u.CompanyId,
                u.CompanyName
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

    public class CompanyDTO
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
    }
}