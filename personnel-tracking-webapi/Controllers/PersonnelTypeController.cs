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
    public class PersonnelTypeController : Controller
    {
        private readonly PersonnelTrackingDBContext dbContext;
        public PersonnelTypeController()
        {
            if (dbContext == null) dbContext = new PersonnelTrackingDBContext();
        }

        /// <summary>
        /// Gets all companies to show to the admin
        /// </summary>
        /// <returns></returns>
        /// GET: http://localhost:5000/api/personnelType
        [HttpGet]
        public IActionResult Get()
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var personnelTypeList = dbContext.PersonnelTypes.ToList();
                response.Data = personnelTypeList;
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }
            return Ok(response);
        }
    }
}
