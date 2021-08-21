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
    [Route("api/personnelType")]
    [ApiController]
    [TokenCheck]
    public class PersonnelTypeController : ControllerBase
    {

        private readonly PersonnelTrackingDBContext dbContext;
        public PersonnelTypeController()
        {
            if (dbContext == null) dbContext = new PersonnelTrackingDBContext();
        }

        /// <summary>
        /// Lists all the roles available
        /// </summary>
        /// <returns></returns>
        /// GET: http://localhost:5000/api/personnelType
        [HttpGet]
        public IActionResult Get()
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var personnelTypeDTOList = dbContext.PersonnelTypes.Select(u => new
                {
                    u.PersonnelTypeId,
                    u.PersonnelTypeName
                }).ToList();
                response.Data = personnelTypeDTOList;
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
        /// Registers a personnelType to the table
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post(PersonnelTypeDTO personnelTypeDto)
        {
            var response = new ResponseModel();
            try
            {
                var newPersonnelType = new PersonnelType
                {
                    PersonnelTypeId = personnelTypeDto.PersonnelTypeId,
                    PersonnelTypeName = personnelTypeDto.PersonnelTypeName
                };
                // don't add same company twice
                bool exists = dbContext.PersonnelTypes.
                    FirstOrDefault(pt => pt.PersonnelTypeName.Equals(personnelTypeDto.PersonnelTypeName)) != null;
                if (exists)
                    throw new Exception("Same role already exists.");
                else
                {
                dbContext.Add<PersonnelType>(newPersonnelType).State = EntityState.Added;
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
            response.Data = dbContext.PersonnelTypes.Select(u => new {
                u.PersonnelTypeId,
                u.PersonnelTypeName
            }).ToList();
            return Ok(response);
        }

        /// <summary>
        /// Edits a personnelType row provided that the form is submitted
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public IActionResult Put(PersonnelTypeDTO personnelTypeDto)
        {
            var response = new ResponseModel();
            try
            {
                var newPersonnelType = new PersonnelType
                {
                    PersonnelTypeId = personnelTypeDto.PersonnelTypeId,
                    PersonnelTypeName = personnelTypeDto.PersonnelTypeName
                };
                dbContext.Update<PersonnelType>(newPersonnelType).State = EntityState.Modified;
                Console.WriteLine(dbContext.SaveChanges() + " rows affected.");
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

            response.Data = dbContext.PersonnelTypes.Select(u => new {
                u.PersonnelTypeId,
                u.PersonnelTypeName
            }).ToList();
            return Ok(response);
        }

        /// <summary>
        /// Deletes the personnelType from the database
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public IActionResult Delete(PersonnelTypeDTO personnelTypeDto)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                var personnelType = dbContext.PersonnelTypes.FirstOrDefault(u => u.PersonnelTypeId == personnelTypeDto.PersonnelTypeId);
                dbContext.PersonnelTypes.Remove(personnelType).State = EntityState.Deleted;
                Console.WriteLine(dbContext.SaveChanges() + " rows affected.");
            }
            catch (Exception e)
            {
                response.HasError = true;
                response.ErrorMessage = e.Message;
                if (e.InnerException != null)
                {
                    response.ErrorMessage += ": " + e.InnerException.Message;
                }
            }
            response.Data = dbContext.PersonnelTypes.Select(u => new {
                u.PersonnelTypeId,
                u.PersonnelTypeName
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

    public class PersonnelTypeDTO
    {
        public int PersonnelTypeId { get; set; }
        public string PersonnelTypeName { get; set; }
    }
}