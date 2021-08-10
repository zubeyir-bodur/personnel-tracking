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
    public class AreaController : ControllerBase
    {
        private readonly PersonnelTrackingDBContext dbContext;
        public AreaController()
        {
            if (dbContext == null) dbContext = new PersonnelTrackingDBContext();
        }

        [HttpGet]
        public IActionResult Get()
        {
            ResponseModel response = new ResponseModel();

            try
            {
                var areaList = dbContext.Set<Area>().ToList();

                response.Data = areaList;
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }

            return Ok(response);
        }

        [HttpPost]
        public IActionResult Post()
        {
            return Ok();
        }

        [HttpPut]
        public IActionResult Put()
        {
            return Ok();
        }

        [HttpDelete]
        public IActionResult Delete()
        {
            return Ok();
        }
    }
}
