using DataLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#nullable enable

namespace BusinessLayer.DTOs.Attachment
{
    public class AttachmentDTO
    {

    }

    public class AttachmentGetDTO
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; } //Content-Type
        public long FileSize { get; set; }
        public byte[]? FileData { get; set; } //Database
        public string? FilePath { get; set; } //File System
        public string? BlobId { get; set; } //Blob Storage
        //public FileStorageType FileStorageType { get; set; }
        //public string UserId { get; set; }

        //public DataCreation CreatedBy { get; set; } = DataCreation.User;
        //public DataCreation UpdatedBy { get; set; } = DataCreation.User;
    }

    public class AttachmentEditDTO
    {
        public int Id { get; set; }
        public string FileName { get; set; } = null!;
        public string FileType { get; set; } = null!; //Content-Type
        public long FileSize { get; set; }
        public byte[]? FileData { get; set; } = null!; //Database
        public string? FilePath { get; set; } //File System
        public string? BlobId { get; set; } //Blob Storage
        public FileStorageType FileStorageType { get; set; }
        public string UserId { get; set; } = null!;

        public DateTime DateUpdated { get; set; } = DateTime.UtcNow;

        //public DataCreation CreatedBy { get; set; } = DataCreation.User;
        //public DataCreation UpdatedBy { get; set; } = DataCreation.User;
    }

    public class AttachmentCreateDTO
    {
        public string FileName { get; set; } = null!;
        public string FileType { get; set; } = null!; //Content-Type
        public long FileSize { get; set; }
        public byte[]? FileData { get; set; } = null!; //Database
        public string? FilePath { get; set; } //File System
        public string? BlobId { get; set; } //Blob Storage
        public FileStorageType FileStorageType { get; set; }
        public string UserId { get; set; } = null!;
        public DataCreation CreatedBy { get; set; } = DataCreation.User;
        public DataCreation UpdatedBy { get; set; } = DataCreation.User;
    }
}
