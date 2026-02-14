using Ardalis.GuardClauses;
using Utils.Enums;
using DataLayer.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utils.Constants;
using System.Reflection;

namespace DataLayer.Storage
{
    public class FileStorage : IFileStorage
    {
        private readonly IWebHostEnvironment _env;

        public FileStorage(IWebHostEnvironment env)
        {
            _env = env;
        }
        
        public string GetDirectory(FileStorageDirectory dir, bool fullPath = false)
        {
            var imgStr = "img";
            var relativePath = string.Empty;

            //Get path where the static files live
            switch (dir)
            {
                case FileStorageDirectory.Temp:
                    //path = Path.Combine(_env.WebRootPath, imgStr, "temp");
                    relativePath = Path.Combine(imgStr, "temp");
                    break;
                case FileStorageDirectory.Upload:
                    relativePath = Path.Combine(imgStr, "uploads");
                    break;
                case FileStorageDirectory.Default:
                    relativePath = Path.Combine(imgStr, "default");
                    break;
                default:
                    Guard.Against.EnumOutOfRange(dir, nameof(dir));
                    break;
            }

            var absolutePath = Path.Combine(_env.WebRootPath, relativePath);
            if (!Directory.Exists(absolutePath))
            {
                Directory.CreateDirectory(absolutePath);
            }

            if (fullPath) return absolutePath;
            else return relativePath;
        }

        public bool MoveFile(string oldFileName, FileStorageDirectory newDir)
        {
            Guard.Against.NullOrWhiteSpace(oldFileName, nameof(oldFileName));
            Guard.Against.EnumOutOfRange(newDir, nameof(newDir));

            //Get new absolute path (destiny)
            var directoryPath = GetDirectory(newDir, true);
            var newFilePath = Path.Combine(directoryPath, oldFileName);

            var oldDirectoryPath = string.Empty;
            var oldFilePath = string.Empty;

            switch (newDir)
            {
                case FileStorageDirectory.Temp:
                    oldDirectoryPath = GetDirectory(FileStorageDirectory.Upload, true);
                    break;
                case FileStorageDirectory.Upload:
                    oldDirectoryPath = GetDirectory(FileStorageDirectory.Temp, true);
                    break;
                default:
                    throw new Exception("Something went wrong.");
            }

            //Get old absolute path (source)
            oldFilePath = Path.Combine(oldDirectoryPath, oldFileName);

            if (File.Exists(oldFilePath))
            {
                File.Move(oldFilePath, newFilePath, true);
                return true;
            }
            else return false;
        }

        //Save attachment in File System
        public async Task<bool> SaveFileToDiskAsync(Stream file, string fileName, FileStorageDirectory dir)
        {
            Guard.Against.Null(file, nameof(file));
            Guard.Against.NullOrWhiteSpace(fileName, nameof(fileName));
            Guard.Against.EnumOutOfRange(dir, nameof(dir));

            var directoryPath = GetDirectory(dir, true);
            var filePath = Path.Combine(directoryPath, fileName);
            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return true;
        }

        public void DeleteFile(string fileName)
        {
            Guard.Against.NullOrWhiteSpace(fileName, nameof(fileName));
            var uploadPath = GetDirectory(FileStorageDirectory.Upload, true);
            var filePath = Path.Combine(uploadPath, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        //This method is called by FileDeletionService
        public void DeleteTempFiles(TimeSpan maxAge)
        {
            Guard.Against.Null(maxAge, nameof(maxAge));
            var absolutePath = GetDirectory(FileStorageDirectory.Temp, true);
            var now = DateTime.UtcNow;

            var files = Directory.GetFiles(absolutePath);
            foreach (var file in files)
            {
                //If the file was created longer than the specified time
                if (now - File.GetCreationTimeUtc(file) > maxAge)
                {
                    File.Delete(file);
                }
            }
        }   
    }
}
