using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using TestProject.Services;

namespace TestProject.Controllers
{
    public class FolderController : ApiController
    {
        private FolderService _service { get; set; }
        public FolderController()
        {
            _service = new FolderService();
        }
        public HttpResponseMessage Post([FromUri] string relativePath, [FromUri]string folderName)
        {            
            Regex containsABadCharacter = new Regex("[" + Regex.Escape(Path.GetInvalidPathChars().ToString()) + "]");
            if (containsABadCharacter.IsMatch(folderName))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            _service.CreateFolder(relativePath, folderName);
            return new HttpResponseMessage(HttpStatusCode.OK);            
        }
        
        public void Delete([FromUri]string relativePath)
        {            
            _service.DeleteFolder(relativePath);
        }
    }
}
