using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using TestProject.Services;

namespace TestProject.Controllers
{
    public class FileController : ApiController
    {
        private FileService _service { get; set; }
        public FileController()
        {
            _service = new FileService();
        }        
        public HttpResponseMessage Get([FromUri] string relativePath)
        {
            var fileInfo = _service.GetFile(relativePath);            
            var dataBytes = File.ReadAllBytes(fileInfo.FullName);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new ByteArrayContent(dataBytes);

            //Set the Response Content Length.
            response.Content.Headers.ContentLength = dataBytes.LongLength;

            //Set the Content Disposition Header Value and FileName.
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = fileInfo.Name;

            //Set the File Content Type.
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(fileInfo.Name));
            return response;
        }

        public HttpResponseMessage Post()
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                var folder = httpRequest.Form["folder"];
                var postedFile = httpRequest.Files[0];

                var saveFileResult = _service.SaveFile(folder, postedFile);

                result = Request.CreateResponse(HttpStatusCode.Created, saveFileResult);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            return result;
        }
        
        public void Delete([FromUri] string relativePath)
        {
            _service.DeleteFile(relativePath);
        }

        [HttpPut]
        [Route("api/File/Move")]
        public HttpResponseMessage Move([FromUri] string relativeDestination, [FromUri] string relativePath)
        {
            var result = _service.MoveTo(relativeDestination, relativePath);
            if (!result)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPut]
        [Route("api/File/Copy")]
        public HttpResponseMessage Copy([FromUri] string relativeDestination, [FromUri] string relativePath)
        {
            var result = _service.CopyTo(relativeDestination, relativePath);
            if (!result)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
