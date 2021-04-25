using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace TestProject.Services
{
    public class FolderService
    {
        private string _rootPath { get; set; }

        public FolderService()
        {
            _rootPath = ConfigurationManager.AppSettings["RootPath"];
        }

        public void DeleteFolder(string relativePath)
        {
            Directory.Delete(_rootPath + relativePath, true);
        }

        public void CreateFolder(string relativePath, string folderName)
        {
            Directory.CreateDirectory(_rootPath + relativePath + "\\" + folderName);
        }
    }
}