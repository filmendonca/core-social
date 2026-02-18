using DataLayer.Base;
using DataLayer.Enums;
using DataLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace DataLayer.Models
{
    public class Attachment : BaseEntity
    {
        public string FileName { get; set; } = null!;
        public string FileType { get; set; } = null!; //Content-Type
        public long FileSize { get; set; }
        public byte[]? FileData { get; set; } //Database
        public string? FilePath { get; set; } = "img\\default"; //File System
        public string? BlobId { get; set; } //Blob Storage
        public FileStorageType FileStorageType { get; set; }

        #region Navigation Properties

        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;
        public virtual User User { get; set; } = null!;

        //[ForeignKey(nameof(Post))]
        //public int PostId { get; set; }
        //public Post Post { get; set; }

        #endregion
    }
}
