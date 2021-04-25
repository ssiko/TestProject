using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestProject.Models
{
    public class FileSystemModel
    {
        public string RelativePath { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public long Bytes { get; set; }
        public int SubDirectoryCount { get; set; }
        public int FileCount { get; set; }
        public bool IsFolder { get; set; }
        public List<FileSystemModel> Children { get; set; }
    }
}