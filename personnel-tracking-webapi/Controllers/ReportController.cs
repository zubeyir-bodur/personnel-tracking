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

namespace personnel_tracking_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly PersonnelTrackingDBContext dbContext;
        public ReportController()
        {
            if (dbContext == null) dbContext = new PersonnelTrackingDBContext();
        }


        /// <summary>
        /// Get files for the trackings of a given area
        /// </summary>
        /// <param name="areaParam"></param>
        /// <returns></returns>
        [HttpGet("area")]
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
                var includedTrackings = dbContext.Trackings.Select(u => new TrackReport
                {
                    Name = u.Personnel.PersonnelName,
                    Surname = u.Personnel.PersonnelSurname,
                    Role = u.Personnel.PersonnelType.PersonnelTypeName,
                    Company = u.Area.Company.CompanyName,
                    Area = u.Area.AreaName,
                    EntranceDate = u.EntranceDate.ToString("g", new CultureInfo("fr-FR")),
                    ExitDate = u.ExitDate.Value.ToString("g", new CultureInfo("fr-FR")),
                    ExitType = TrackReport.GetExitType(u.AutoExit, u.ExitDate != null)
                }).AsNoTracking();
                var list = includedTrackings.Where(
                    tr => tr.Area.Equals(area.AreaName)
                    && tr.Company.Equals(company.CompanyName) // also filter according to that company
                    && tr.EntranceDate.CompareTo(areaParam.Start) >= 0
                    && tr.EntranceDate.CompareTo(areaParam.End) <= 0).ToList();

                // 2.1 use json serializer
                // TODO change the path
                var path = "files\\area-personnels";
                var JsonString = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
                System.IO.File.WriteAllText(path + ".json", JsonString);
                // 2.2 generate excel table
                ExportExcel(list, areaParam.Start, path);
                // return the path so that frontend can send download requests using this path
                response.Data = path;
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
        [HttpGet("personnel")]
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

                var filter = dbContext.Trackings.AsNoTracking().Where(
                    t => t.PersonnelId == personnelParam.PersonnelId
                    && t.EntranceDate.CompareTo(personnelParam.Start) >= 0
                    && t.EntranceDate.CompareTo(personnelParam.End) <= 0);
                var list = filter.Select(u => new TrackReport
                {
                    Name = u.Personnel.PersonnelName,
                    Surname = u.Personnel.PersonnelSurname,
                    Role = u.Personnel.PersonnelType.PersonnelTypeName,
                    Company = u.Area.Company.CompanyName,
                    Area = u.Area.AreaName,
                    EntranceDate = u.EntranceDate.ToString("g", new CultureInfo("fr-FR")),
                    ExitDate = u.ExitDate.Value.ToString("g", new CultureInfo("fr-FR")),
                    ExitType = TrackReport.GetExitType(u.AutoExit, u.ExitDate != null)
                }).ToList();

                // 2.1 use json serializer
                // TODO change the path
                var path = "files\\personnel-areas";
                var JsonString = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
                System.IO.File.WriteAllText(path + ".json", JsonString);
                // 2.2 generate excel table
                ExportExcel(list, personnelParam.Start, path);
                // return the path so that frontend can send download requests using this path
                response.Data = path;
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
                { ".xls", "application/vnd.ms-excel"},
                { ".json", "application/json"}
            };
        }

        /// <summary>
        /// Helper method for exporting a list of objects into excel
        /// </summary>
        /// <author>Zubeyir Bodur</author>
        /// <param name=""></param>
        private void ExportExcel<T>(List<T> list, DateTime date, string path)
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
        private string PutWhiteSpace(string s)
        {

            int index = 1;
            char[] arr = s.ToCharArray();
            while (index < arr.Length)
            {
                if (arr[index] > 40 && arr[index] < 91)
                    s = s.Insert(index, " ");
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

        public Exit ExitType;

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
