using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace TestProject.Models
{
    public class FileModel
    {
        public FileStream Stream { get; set; }
        public string Name { get; set; }
    }
}