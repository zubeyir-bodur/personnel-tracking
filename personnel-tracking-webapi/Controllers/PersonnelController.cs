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
                var personnelDTOList = new List<PersonnelDTO>();
                for (int i = 0; i < personnelList.Count; i++)
                {
                    personnelDTOList.Add(new PersonnelDTO
                    {
                        PersonnelId = personnelList[i].PersonnelId,
                        CompanyId = personnelList[i].CompanyId,
                        PersonnelTypeId = personnelList[i].PersonnelTypeId,
                        IdentityNumber = personnelList[i].IdentityNumber,
                        PersonnelName = personnelList[i].PersonnelName,
                        PersonnelSurname = personnelList[i].PersonnelSurname,
                        UserName = personnelList[i].UserName,
                        Password = personnelList[i].Password,
                        //Need to actually get the company and the tracking not just the ids


                    }) ;
                }
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
                newPersonnel.PersonnelId = personnelDTO.PersonnelId;
                newPersonnel.CompanyId = personnelDTO.CompanyId;
                newPersonnel.PersonnelTypeId = personnelDTO.PersonnelTypeId;
                newPersonnel.IdentityNumber = personnelDTO.IdentityNumber;
                newPersonnel.PersonnelName = personnelDTO.PersonnelName;
                newPersonnel.PersonnelSurname = personnelDTO.PersonnelSurname;
                newPersonnel.UserName = personnelDTO.UserName;
                newPersonnel.Password = personnelDTO.Password;

                dbContext.Add<Personnel>(newPersonnel);
            }
            catch (Exception e)
            {
                response.HasError = true;
                response.ErrorMessage = e.Message;
            }
          
            response.Data = dbContext.Personnel.ToList();
            return Ok(response);
        }

        [HttpPut]
        public IActionResult Put(PersonnelDTO personnelDTO)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                Personnel personnel = dbContext.Personnel.Where(u => u.PersonnelId == personnelDTO.PersonnelId).FirstOrDefault();

                personnel.PersonnelId = personnelDTO.PersonnelId;
                personnel.CompanyId = personnelDTO.CompanyId;
                personnel.PersonnelTypeId = personnelDTO.PersonnelTypeId;
                personnel.IdentityNumber = personnelDTO.IdentityNumber;
                personnel.PersonnelName = personnelDTO.PersonnelName;
                personnel.PersonnelSurname = personnelDTO.PersonnelSurname;
                personnel.UserName = personnelDTO.UserName;
                personnel.Password = personnelDTO.Password;
                dbContext.Update<Personnel>(personnel);

            }
            catch (Exception e)
            {
                response.HasError = true;
                response.ErrorMessage = e.Message;
            }

            response.Data = dbContext.Personnel.ToList();
            return Ok(response);
        }

        [HttpDelete]
        public IActionResult Delete(PersonnelDTO personnelDTO)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                Personnel personnel = dbContext.Personnel.Where(u => u.PersonnelId == personnelDTO.PersonnelId).FirstOrDefault();
                dbContext.Personnel.Remove(personnel);
            }
            catch (Exception e)
            {
                response.HasError = true;
                response.ErrorMessage = e.Message;
            }
           
            response.Data = dbContext.Personnel.ToList();
            return Ok(response);
        }
    }


    public class PersonnelDTO
    {
        public int PersonnelId { get; set; }
        public int CompanyId { get; set; }
        public int PersonnelTypeId { get; set; }
        public int IdentityNumber { get; set; }
        public string PersonnelName { get; set; }
        public string PersonnelSurname { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        //maybe add virtual ones too?
    }


}
