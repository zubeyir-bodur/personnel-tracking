﻿using Microsoft.AspNetCore.Http;
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
    public class PersonnelController : Controller
    {
        private readonly PersonnelTrackingDBContext dbContext;

        public PersonnelController()
        {
            if (dbContext == null) dbContext = new PersonnelTrackingDBContext();

        }
        [HttpGet]
        public IActionResult Get()
        {
            ResponseModel response = new ResponseModel();

            try
            {
                var personnelList = dbContext.Personnel.ToList();
                var personnelDTOList = dbContext.Personnel.Select(u => new {
                    u.PersonnelId,
                    u.Company.CompanyName,
                    u.PersonnelType.PersonnelTypeName,
                    u.IdentityNumber,
                    u.PersonnelName,
                    u.PersonnelSurname,
                    u.UserName,
                    u.Password
                });

                response.Data = personnelDTOList;
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }
            Console.WriteLine("Get");
            return Ok(response);
        }

        [HttpPost]
        public IActionResult Post([FromBody] PersonnelDTO personnelDTO)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                Personnel newPersonnel = new Personnel();
                Company company = dbContext.Companies.Where<Company>(u => u.CompanyName == personnelDTO.company).FirstOrDefault();
                PersonnelType personnelType = dbContext.PersonnelTypes.Where<PersonnelType>(u => u.PersonnelTypeName == personnelDTO.personnelType).FirstOrDefault();

                newPersonnel.IdentityNumber = personnelDTO.identityNumber;
                newPersonnel.PersonnelName = personnelDTO.personnelName;
                newPersonnel.PersonnelSurname = personnelDTO.personnelSurname;
                newPersonnel.UserName = personnelDTO.username;
                newPersonnel.Password = personnelDTO.password;
                newPersonnel.CompanyId = company.CompanyId;
                newPersonnel.PersonnelTypeId = personnelType.PersonnelTypeId;
                dbContext.Add<Personnel>(newPersonnel).State = EntityState.Added;
            }
            catch (Exception e)
            {
                response.HasError = true;
                response.ErrorMessage = e.Message;
            }

            dbContext.SaveChanges();

            response.Data = dbContext.Personnel.Select(u => new {
                u.PersonnelId,
                u.Company.CompanyName,
                u.PersonnelType.PersonnelTypeName,
                u.IdentityNumber,
                u.PersonnelName,

                u.PersonnelSurname,
                u.UserName,
                u.Password
            });

            return Ok(response);
        }

        [HttpPut]
        public IActionResult Put(PersonnelDTO personnelDTO)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                Personnel personnel = dbContext.Personnel.Where(u => u.PersonnelId == personnelDTO.personnelId).FirstOrDefault();
                Company company = dbContext.Companies.Where<Company>(u => u.CompanyName == personnelDTO.company).FirstOrDefault();
                PersonnelType personnelType = dbContext.PersonnelTypes.Where<PersonnelType>(u => u.PersonnelTypeName == personnelDTO.personnelType).FirstOrDefault();

                personnel.CompanyId = company.CompanyId;
                personnel.PersonnelTypeId = personnelType.PersonnelTypeId;
                personnel.IdentityNumber = personnelDTO.identityNumber;
                personnel.PersonnelName = personnelDTO.personnelName;
                personnel.PersonnelSurname = personnelDTO.personnelSurname;
                personnel.UserName = personnelDTO.username;
                personnel.Password = personnelDTO.password;
                dbContext.Update<Personnel>(personnel).State = EntityState.Modified;

            }
            catch (Exception e)
            {
                response.HasError = true;
                response.ErrorMessage = e.Message;
            }
            dbContext.SaveChanges();

            response.Data = dbContext.Personnel.Select(u => new {
                u.PersonnelId,
                u.Company.CompanyName,
                u.PersonnelType.PersonnelTypeName,
                u.IdentityNumber,
                u.PersonnelName,
                u.PersonnelSurname,
                u.UserName,
                u.Password
            });
            return Ok(response);
        }

        [HttpDelete]
        public IActionResult Delete(PersonnelDTO personnelDTO)
        {
            Console.WriteLine("here");
            ResponseModel response = new ResponseModel();

            try
            {
                Personnel personnel = dbContext.Personnel.Where(u => u.PersonnelId == personnelDTO.personnelId).FirstOrDefault();
                dbContext.Personnel.Remove(personnel).State = EntityState.Deleted;
                //save

            }
            catch (Exception e)
            {
                response.HasError = true;
                response.ErrorMessage = e.Message;
            }

            dbContext.SaveChanges();
            response.Data = dbContext.Personnel.Select(u => new {
                u.PersonnelId,
                u.Company.CompanyName,
                u.PersonnelType.PersonnelTypeName,
                u.IdentityNumber,
                u.PersonnelName,
                u.PersonnelSurname,
                u.UserName,
                u.Password
                
            });
            ///save chanege

            return Ok(response);
        }

        //Check if user with given username exists

        [HttpGet("checkUsername")]
        public IActionResult checkUsername([FromBody] string username)
        {
            ResponseModel response = new ResponseModel();
            Console.WriteLine(username +"smth");

            Personnel personnel = dbContext.Personnel.Where(u => u.UserName == username).FirstOrDefault();
            if (personnel == null)
            {
                response.Data = "Does not exist";
            }
            else
            {
                response.Data = "Exists";
            }
            return Ok(response);
        }

        [HttpGet("checkIDNum")]
        public IActionResult checkIDNum([FromBody]  string idNum)
        {
            ResponseModel response = new ResponseModel();
            //System.Threading.Thread.Sleep(3000);
            Console.WriteLine(idNum+ "sd;klvmsd");
            long l = long.Parse(idNum);
            Personnel personnel = dbContext.Personnel.Where(u => u.IdentityNumber == l).FirstOrDefault();
            if (personnel == null)
            {
                response.Data = "Does not exist";
            }
            else
            {
                response.Data = "Exists";
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
