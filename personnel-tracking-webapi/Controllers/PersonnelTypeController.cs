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
                var personnelTypeDTOList = dbContext.PersonnelTypes.Select(u => new PersonnelTypeDTO
                {
                    PersonnelTypeId = u.PersonnelTypeId,
                    PersonnelTypeName = u.PersonnelTypeName
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
            response.Data = dbContext.PersonnelTypes.Select(u => new PersonnelTypeDTO
            {
                PersonnelTypeId = u.PersonnelTypeId,
                PersonnelTypeName = u.PersonnelTypeName
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
                // don't let the user change the role into another role
                bool exists = dbContext.PersonnelTypes.
                    FirstOrDefault(pt => pt.PersonnelTypeName.Equals(personnelTypeDto.PersonnelTypeName)) != null;
                if (exists)
                    throw new Exception("Same role already exists.");
                else
                {
                    dbContext.Update<PersonnelType>(newPersonnelType).State = EntityState.Modified;
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
            response.Data = dbContext.PersonnelTypes.Select(u => new PersonnelTypeDTO
            {
                PersonnelTypeId = u.PersonnelTypeId,
                PersonnelTypeName = u.PersonnelTypeName
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
                var delPersonnelType = new PersonnelType
                {
                    PersonnelTypeId = personnelTypeDto.PersonnelTypeId,
                    PersonnelTypeName = personnelTypeDto.PersonnelTypeName
                };

                bool isRelationalPersonnel = dbContext.Personnel
                    .AsNoTracking()
                    .FirstOrDefault(a => a.PersonnelTypeId == personnelTypeDto.PersonnelTypeId) != null;
                if (isRelationalPersonnel)
                    throw new Exception("The role being deleted has personnel related to it, please delete those personnel first.");

                dbContext.PersonnelTypes.Remove(delPersonnelType).State = EntityState.Deleted;
                dbContext.SaveChanges();
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
            response.Data = dbContext.PersonnelTypes.Select(u => new PersonnelTypeDTO
            {
                PersonnelTypeId = u.PersonnelTypeId,
                PersonnelTypeName = u.PersonnelTypeName
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