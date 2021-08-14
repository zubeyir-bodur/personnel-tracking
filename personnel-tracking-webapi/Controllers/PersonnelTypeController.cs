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
    [Route("api/personnel-type")]
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
        /// GET: http://localhost:5000/api/personnel-type
        [HttpGet]
        public IActionResult Get()
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var ptList = dbContext.PersonnelTypes.ToList();
                response.Data = ptList;
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
