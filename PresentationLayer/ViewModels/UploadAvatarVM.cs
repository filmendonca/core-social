using Microsoft.AspNetCore.Http;
using PresentationLayer.CustomValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PresentationLayer.ViewModels
{
    public class UploadAvatarVM
    {
        [Required(ErrorMessage = "Image is required")]
        //[ImageFile(3 * 1024 * 1024, ErrorMessage = "File size must not exceed 3 MB")]
        [ImageFile(3 * 1024 * 1024)]
        [DataType(DataType.Upload)]
        public IFormFile Image { get; set; }

        //[BindNever]
        //public string ImageFileName { get; set; }
        //[BindNever]
        //public string ImageFileType { get; set; }
        //[BindNever]
        //public long ImageFileSize { get; set; }
    }
}
