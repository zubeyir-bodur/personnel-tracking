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

namespace personnel_tracking_webapi.Controllers {
    [Route("api/company")]
    [ApiController]
    [TokenCheck]
    public class CompanyController : ControllerBase {
        private readonly PersonnelTrackingDBContext dbContext;
        public CompanyController() {
            if (dbContext == null) dbContext = new PersonnelTrackingDBContext();
        }

        /// <summary>
        /// Gets all companies to show to the admin
        /// </summary>
        /// <returns></returns>
        /// GET: http://localhost:5000/api/company
        [HttpGet]
        public IActionResult Get() {
            ResponseModel response = new ResponseModel();
            try {
                var companyList = dbContext.Companies.ToList();
                response.Data = companyList;
            } catch (Exception ex) {
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
        public IActionResult Post(Company companyDto) {
            var response = new ResponseModel();
            try {
                // dto to entity
                var company = new Company {
                    CompanyId = companyDto.CompanyId,
                    CompanyName = companyDto.CompanyName
                };
                // don't add the same company twice
                bool exists = dbContext.Companies.
                    FirstOrDefault(c => company.CompanyName.Equals(c.CompanyName)) != null;
                if (!exists) {
                    dbContext.Add(company);
                    dbContext.SaveChanges();
                    response.Data = company;
                }
                else {
                    response.HasError = true;
                    response.ErrorMessage = "Same entry already exists";
                    response.Data = companyDto; // send back the dto
                }
                return Ok(response);
            } catch (Exception ex) {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
                return NotFound(response);
            }
        }

        /// <summary>
        /// Edits a company row provided that the form is submitted
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public IActionResult Put(Company companyDto) {
            var response = new ResponseModel();
            try {
                // todo: dto integration
                var company = new Company {
                    CompanyId = companyDto.CompanyId,
                    CompanyName = companyDto.CompanyName
                };
                bool exists = dbContext.Companies.
                    FirstOrDefault(c => c.CompanyId == companyDto.CompanyId) != null;
                if (exists) {
                    dbContext.Update(company);
                    dbContext.SaveChanges();
                    response.Data = company;
                }
                else {
                    response.HasError = true;
                    response.ErrorMessage = "Company edit error";
                }
                return Ok(response);
            } catch (Exception ex) {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }
            return Ok(response);
        }

        /// <summary>
        /// Checks if the id is valid and asks user's permission to delete
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public IActionResult Delete(int? id) {
            var response = new ResponseModel();
            if (id == null) {
                response.HasError = true;
                response.ErrorMessage = "ID was missing";
                return NotFound(response);
            }
            try {
                var company = dbContext.Companies.FirstOrDefault(c => c.CompanyId == id);
                if (company == null) {
                    response.HasError = true;
                    response.ErrorMessage = "The company to be deleted was not found";
                    return NotFound(response);
                }
                response.Data = company; // company to be deleted is valid
                dbContext.Remove(company);
                dbContext.SaveChanges();
                response.HasError = false;
            } catch (Exception ex) {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }
            return Ok(response);
        }
    }
}