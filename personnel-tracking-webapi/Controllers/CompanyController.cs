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
    [TokenCheck]
    public class CompanyController : Controller
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
        [HttpGet]
        public IActionResult Get()
        {
            ResponseModel response = new ResponseModel();

            try
            {
                var companyList = dbContext.Companies.ToList();
                response.Data = companyList;
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }

            return Ok(response);
        }

        /// <summary>
        /// Registers a company to the table, provided that
        /// Form is submitted from the frontend
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}")]
        public async Task<IActionResult> Post(Company company)
        {
            var response = new ResponseModel();
            try
            {
                dbContext.Add(company);
                await dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }
            return Ok(response);
        }

        /// <summary>
        /// Edits a company row provided that the form is submitted
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Put(Company company)
        {
            var response = new ResponseModel();
            try
            {
                dbContext.Update(company);
                await dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }
            return Ok(response);
        }

        /// <summary>
        /// Checks if the id is valid and asks user's permission to delete
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            var response = new ResponseModel();
            if (id == null)
            {
                response.HasError = true;
                response.ErrorMessage = "ID was missing";
                return NotFound(response);
            }
            try
            {
                var company = await dbContext.Companies.FirstOrDefaultAsync(c => c.CompanyId == id);
                if (company == null)
                {
                    response.HasError = true;
                    response.ErrorMessage = "The company to be deleted was not found";
                    return NotFound(response);
                }
                response.Data = company; // company to be deleted is valid
                response.HasError = false;
            }
            catch (Exception ex) {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }
            return Ok(response);
        }

        /// <summary>
        /// With the deletion confirmed, deletes the company and returns the index
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var company = await dbContext.Companies.FindAsync(id);
            dbContext.Companies.Remove(company);
            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Get));
        }
    }
}
