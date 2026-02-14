using Utils.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Interfaces
{
    public interface IFileStorage
    {
        string GetDirectory(FileStorageDirectory dir, bool fullPath = false);
        Task<bool> SaveFileToDiskAsync(Stream file, string fileName, FileStorageDirectory dir);
        void DeleteFile(string fileName);
        bool MoveFile(string oldFileName, FileStorageDirectory newDir);
        void DeleteTempFiles(TimeSpan maxAge);
    }
}
