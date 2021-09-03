using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using personnel_tracking_entity;
using personnel_tracking_dto;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using personnel_tracking_webapi.Models;
using personnel_tracking_webapi.Filters;

namespace personnel_tracking_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TokenCheck]
    public class ReportController : ControllerBase
    {
        private readonly PersonnelTrackingDBContext dbContext;
        public ReportController()
        {
            if (dbContext == null) dbContext = new PersonnelTrackingDBContext();
        }

        /// <summary>
        /// Gets the personnels of a given company
        /// </summary>
        /// <returns></returns>
        [HttpPut("personnelsOfCompany")]
        public IActionResult PersonnelsOfCompany([FromBody]CompanyDTO companyDTO) {
            var response = new ResponseModel();
            try
            {
                response.Data = dbContext.Personnel
                    .AsNoTracking()
                    .Where(p => p.CompanyId == companyDTO.CompanyId)
                    .ToList();
            }
            catch (Exception ex)
            {
                response.Data = null;
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
        /// Get files for the trackings of a given area
        /// </summary>
        /// <param name="areaParam"></param>
        /// <returns></returns>
        [HttpPut("area")]
        public IActionResult FilesArea(AreaParam areaParam)
        {
            var response = new ResponseModel();
            try
            {
                // 1. filter the trackings table
                var company = dbContext.Companies
                    .AsNoTracking()
                    .FirstOrDefault(c => c.CompanyId == areaParam.CompanyId);
                var area = dbContext.Areas
                    .AsNoTracking()
                    .Where(a => a.CompanyId == company.CompanyId) // search for the areas inside that company only
                    .FirstOrDefault(a => a.AreaId == areaParam.AreaId);

                if (area == null)
                {
                    response.HasError = true;
                    response.ErrorMessage = "An error occurred while processing your request, please contact the website owner.";
                    return Ok(response);
                }
                // include the necessary information
                var filter = dbContext.Trackings
                    .Select(u => new {
                        u.AreaId,
                        u.Area.CompanyId,
                        Name = u.Personnel.PersonnelName,
                        Surname = u.Personnel.PersonnelSurname,
                        Role = u.Personnel.PersonnelType.PersonnelTypeName,
                        Company = u.Area.Company.CompanyName,
                        Area = u.Area.AreaName,
                        EntranceDate = u.EntranceDate,
                        ExitDate = u.ExitDate,
                        ExitType = PutWhiteSpace(TrackReport.GetExitType(u.AutoExit, u.ExitDate != null).ToString())
                    })// filter
                    .AsNoTracking().Where(
                    t => t.AreaId == area.AreaId
                    && t.CompanyId == company.CompanyId
                    && t.EntranceDate.CompareTo(areaParam.Start) >= 0
                    && t.EntranceDate.CompareTo(areaParam.End) <= 0);
                // remove id's
                var list = filter.Select(u => new TrackReport
                {
                    Name = u.Name,
                    Surname = u.Surname,
                    Role = u.Role,
                    Company = u.Company,
                    Area = u.Area,
                    EntranceDate = u.EntranceDate.ToString("g", new CultureInfo("fr-FR")),
                    ExitDate = u.ExitDate.Value.ToString("g", new CultureInfo("fr-FR")),
                    ExitType = u.ExitType
                }).ToList();

                // TO DO
                // insert an empty tracking for each leave,
                // into the position where leave start is in the right order according to
                // entrance date. and all other information will say on leave

                // 2. output files
                // create directory
                var path = "files\\";
                var fileInfo = new FileInfo(path);
                fileInfo.Directory.Create();
                var JsonString = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
                System.IO.File.WriteAllText(path + "area-personnels.json", JsonString);
                // 2.2 generate excel table
                ExportExcel(list, areaParam.Start, path + "area-personnels");
                // return the path so that frontend can send download requests using this path
                string[] paths = { fileInfo.FullName + "personnel-areas.json", fileInfo.FullName + "personnel-areas.xlsx" };
                response.Data = paths;
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
        /// Get files for the trackings of a given personnel
        /// </summary>
        /// <param name="personnelParam"></param>
        /// <returns></returns>
        [HttpPut("personnel")]
        public IActionResult FilesPersonnel(PersonnelParam personnelParam)
        {
            var response = new ResponseModel();
            try
            {
                // 1. filter the trackings table
                var personnel = dbContext.Personnel
                    .AsNoTracking()
                    .FirstOrDefault(p => p.PersonnelId == personnelParam.PersonnelId);
                
                if (personnel == null)
                {
                    response.HasError = true;
                    response.ErrorMessage = "An error occurred while processing your request, please contact the website owner.";
                    return Ok(response);
                }

                // include the necessary information
                var filter = dbContext.Trackings
                    .AsNoTracking().Where(
                    t => t.PersonnelId == personnelParam.PersonnelId
                    && t.EntranceDate.CompareTo(personnelParam.Start) >= 0
                    && t.EntranceDate.CompareTo(personnelParam.End) <= 0);
                // remove id's
                var list = filter.Select(u => new TrackReport
                {
                    Name = u.Personnel.PersonnelName,
                    Surname = u.Personnel.PersonnelSurname,
                    Role = u.Personnel.PersonnelType.PersonnelTypeName,
                    Company = u.Area.Company.CompanyName,
                    Area = u.Area.AreaName,
                    EntranceDate = u.EntranceDate.ToString("g", new CultureInfo("fr-FR")),
                    ExitDate = u.ExitDate.Value.ToString("g", new CultureInfo("fr-FR")),
                    ExitType = PutWhiteSpace(TrackReport.GetExitType(u.AutoExit, u.ExitDate != null).ToString())
                }).ToList();

                // TO DO
                // insert an empty tracking for each leave,
                // into the position where leave start is in the right order according to
                // entrance date. and all other information will say on leave

                // 2. output files
                // create directory
                var path = "files\\";
                var fileInfo = new FileInfo(path);
                fileInfo.Directory.Create();
                var JsonString = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
                System.IO.File.WriteAllText(path + "personnel-areas.json", JsonString);
                // 2.2 generate excel table
                ExportExcel(list, personnelParam.Start, path + "personnel-areas");
                // return the paths so that frontend can send download requests using this path
                string[] paths = { fileInfo.FullName + "personnel-areas.json", fileInfo.FullName + "personnel-areas.xlsx" };
                response.Data = paths;
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
        /// Have the user download a file that is written into their local storage
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpGet("download")]
        public async Task<IActionResult> Download(string path) {
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open)) 
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return File(memory, GetContentType()[ext], Path.GetFileName(path));
        }

        /// <summary>
        /// Dictionary of extensions/content types
        /// Only three needed to download an excel sheet and a json file
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetContentType() {
            return new Dictionary<string, string>
            {
                { "xlsx","application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                { ".json", "application/json"}
            };
        }

        /// <summary>
        /// Helper method for exporting a list of objects into excel
        /// </summary>
        /// <author>Zubeyir Bodur</author>
        /// <param name=""></param>
        private static void ExportExcel<T>(List<T> list, DateTime date, string path)
        {
            // Creating an instance
            // of ExcelPackage
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage excel = new ExcelPackage();

            // name of the sheet
            var workSheet = excel.Workbook.Worksheets.Add(date.Month + "-" + date.Year);

            // setting the properties of the work sheet 
            workSheet.TabColor = System.Drawing.Color.Black;
            workSheet.DefaultRowHeight = 12;

            // Setting the properties of the first row
            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;

            int i = 1;
            // First row headers
            var properties = typeof(T).GetProperties();
            foreach (var propertyInfo in properties)
            {
                workSheet.Cells[1, i].Value = PutWhiteSpace(propertyInfo.Name);
                i++;
            }
            i = 2;
            // Data
            foreach (var item in list)
            {
                int j = 1;
                foreach (var propertyInfo in properties)
                {
                    workSheet.Cells[i, j].Value = propertyInfo.GetValue(item);
                    j++;
                }
                i++;
            }
            for (int col = 1; col <= properties.Length; col++)
                workSheet.Column(col).AutoFit();

            path += ".xlsx";
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            // Create excel file on physical disk 
            FileStream stream = System.IO.File.Create(path);
            stream.Close();

            // Write content to excel file 
            System.IO.File.WriteAllBytes(path, excel.GetAsByteArray());

            //Close Excel package
            excel.Dispose();
        }

        /// <summary>
        /// Returns whitespaced verison of camel typed string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string PutWhiteSpace(string s)
        {

            int index = 1;
            char[] arr = s.ToCharArray();
            while (index < arr.Length)
            {
                if (arr[index] > 64 && arr[index] < 90)
                {
                    s = s.Insert(index, " ");
                    arr = s.ToCharArray();
                    index++;
                }
                index++;
            }
            return s;
        }
    }

    public class TrackReport
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Role { get; set; }
        public string Company { get; set; }
        public string Area { get; set; }
        public string EntranceDate { get; set; }
        public string ExitDate { get; set; }

        public string ExitType { get; set; }

        public enum Exit
        {
            Automatic,
            Manual,
            NotExitedYet
        }


        public static Exit GetExitType(bool AutoExit, bool ExitDateExists)
        {
            if (AutoExit)
                return Exit.Automatic;
            else if (!AutoExit && !ExitDateExists)
                return Exit.NotExitedYet;
            else
                return Exit.Manual;
        }
    }
}
