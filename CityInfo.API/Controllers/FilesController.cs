using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.API.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FilesController : Controller
    {
        private readonly FileExtensionContentTypeProvider _fex;

        public FilesController(FileExtensionContentTypeProvider fex)
        {
            this._fex = fex;
        }
        
        [HttpGet("{fileId}")]
        public ActionResult GetFiles(string fileId)
        {
            var pathToFile  = "/mnt/sdb1/courses/books/Programming/progit.pdf";

            if(!System.IO.File.Exists(pathToFile))
            {
                return NotFound();
            }

            if(!_fex.TryGetContentType(pathToFile, out var contentType))
            {
                contentType = "application/octet-stream";
            }
 
            var bytes = System.IO.File.ReadAllBytes(pathToFile);

            return File(bytes, contentType, Path.GetFileName(pathToFile));
        }
    }
}