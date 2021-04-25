using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using TestProject.Models;

namespace TestProject.Services
{
    public class FileService
    {
        private string _rootPath { get; set; }

        public FileService()
        {
            _rootPath = ConfigurationManager.AppSettings["RootPath"];
        }
        public string SaveFile(string folder, HttpPostedFile file)
        {
            var filename = _rootPath + folder + "\\" + file.FileName;
            file.SaveAs(filename);
            return filename;
        }

        public FileInfo GetFile(string relativePath)
        {
            return new FileInfo(_rootPath + relativePath);            
        }

        public void DeleteFile(string relativePath)
        {
            File.Delete(_rootPath + relativePath);
        }

        public bool MoveTo(string relativeDestination, string relativePath)
        {
            var fullDestination = _rootPath + relativeDestination;
            var fullPath = _rootPath + relativePath;
            if (ValidateCopyMove(fullDestination, fullPath))
            {
                File.Move(fullPath, fullDestination);
                return true;
            }
            return false;
        }

        public bool CopyTo(string relativeDestination, string relativePath)
        {
            var fullDestination = _rootPath + relativeDestination;
            var fullPath = _rootPath + relativePath;
            if (ValidateCopyMove(fullDestination, fullPath))
            {
                File.Copy(fullPath, fullDestination);
                return true;
            }
            return false;
        }

        private bool ValidateCopyMove(string fullDestination, string fullPath)
        {
            if (fullDestination != fullPath && !File.Exists(fullDestination))
            {                
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}