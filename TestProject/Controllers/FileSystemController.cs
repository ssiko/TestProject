using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using TestProject.Models;
using TestProject.Services;

namespace TestProject.Controllers
{
    public class FileSystemController : ApiController
    {
        private FileSystemService _service { get; set; }
        public FileSystemController()
        {
            _service = new FileSystemService();
        }
        public JsonResult<FileSystemModel> Get()
        {
            return Json(_service.GetFileSystem());
        }

        public JsonResult<FileSystemModel> Get([FromUri] string filter)
        {
            return Json(_service.GetFilteredFileSystem(filter));
        }
    }
}
