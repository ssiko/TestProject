using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using TestProject.Models;

namespace TestProject.Services
{
    public class FileSystemService
    {
        private string _rootPath { get; set; }

        public FileSystemService()
        {
            _rootPath = ConfigurationManager.AppSettings["RootPath"];
        }

        public FileSystemModel GetFilteredFileSystem(string search)
        {            
            DirectoryInfo di = new DirectoryInfo(_rootPath);
            var fsm = CreateInitialModel(di);
            FilterFileSystem(search, di, fsm);
            return fsm;
        }

        public FileSystemModel GetFileSystem()
        {
            DirectoryInfo di = new DirectoryInfo(_rootPath);
            var fsm = CreateInitialModel(di);
            PopulateFileSystem(di, fsm);
            return fsm;
        }

        private FileSystemModel CreateInitialModel(DirectoryInfo di)
        {            
            var fsm = new FileSystemModel
            {
                FullName = di.FullName,
                RelativePath = di.FullName.Replace(_rootPath, "\\"),
                Name = di.Name,
                IsFolder = true,
                Children = new List<FileSystemModel>()
            };
            return fsm;
        }

        private void FilterFileSystem(string filter, DirectoryInfo di, FileSystemModel fsm)
        {
            var subFiles = di.GetFiles("*", SearchOption.AllDirectories).Where(x => x.Name.Contains(filter));
            //get folders that contain a file that matches or where the folder name matches
            var subDirs = di.GetDirectories("*", SearchOption.AllDirectories).Where(x => 
                x.Name.Contains(filter) || 
                x.GetFiles("*", SearchOption.AllDirectories).Any(dir => dir.Name.Contains(filter))
            );

            long bytes = subFiles.Select(x => x.Length).Sum();
            fsm.FileCount = subFiles.Count();
            fsm.SubDirectoryCount = subDirs.Count();
            fsm.Bytes = bytes;

            foreach (FileInfo file in di.GetFiles("*", SearchOption.TopDirectoryOnly).Where(x => x.Name.Contains(filter)))
            {
                FileSystemModel fileModel = new FileSystemModel
                {
                    RelativePath = file.FullName.Replace(_rootPath, ""),
                    FullName = file.FullName,
                    Bytes = file.Length,
                    Name = file.Name,
                    IsFolder = false,
                    Children = null
                };

                fsm.Children.Add(fileModel);
            }
            foreach (DirectoryInfo folder in di.GetDirectories("*", SearchOption.TopDirectoryOnly))
            {
                if (folder.Name.Contains(filter) || folder.GetFiles("*", SearchOption.AllDirectories).Any(x => x.Name.Contains(filter)))
                {
                    FileSystemModel folderModel = new FileSystemModel
                    {
                        RelativePath = folder.FullName.Replace(_rootPath, ""),
                        FullName = folder.FullName,
                        Name = folder.Name,
                        IsFolder = true,
                        Children = new List<FileSystemModel>()
                    };
                    fsm.Children.Add(folderModel);
                    FilterFileSystem(filter, folder, folderModel);
                }                
            }
        }

        private void PopulateFileSystem(DirectoryInfo di, FileSystemModel fsm)
        {
            var subFiles = di.GetFiles("*", SearchOption.AllDirectories);
            var subDirs = di.GetDirectories("*", SearchOption.AllDirectories);

            long bytes = subFiles.Select(x => x.Length).Sum();
            fsm.FileCount = subFiles.Count();
            fsm.SubDirectoryCount = subDirs.Count();
            fsm.Bytes = bytes;

            foreach (FileInfo file in di.GetFiles("*", SearchOption.TopDirectoryOnly))
            {
                FileSystemModel fileModel = new FileSystemModel {
                    RelativePath = file.FullName.Replace(_rootPath, ""), 
                    FullName = file.FullName,
                    Bytes = file.Length, 
                    Name = file.Name, 
                    IsFolder = false, 
                    Children = null
                };

                fsm.Children.Add(fileModel);
            }
            foreach (DirectoryInfo folder in di.GetDirectories("*", SearchOption.TopDirectoryOnly))
            {
                FileSystemModel folderModel = new FileSystemModel {
                    RelativePath = folder.FullName.Replace(_rootPath, ""),
                    FullName = folder.FullName,
                    Name = folder.Name, 
                    IsFolder = true, 
                    Children = new List<FileSystemModel>()
                };
                fsm.Children.Add(folderModel);
                PopulateFileSystem(folder, folderModel);
            }
        }
    }
}