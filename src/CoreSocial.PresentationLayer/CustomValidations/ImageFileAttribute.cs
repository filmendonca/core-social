using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Utils.Constants;

namespace PresentationLayer.CustomValidations
{
    public class ImageFileAttribute : ValidationAttribute
    {
        //private readonly string[] _validExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
        private readonly long _maxSizeInBytes;  // Max size in bytes

        public ImageFileAttribute(long maxSizeInBytes)
        {
            _maxSizeInBytes = maxSizeInBytes;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                // Check if file is empty
                if (file.Length == 0)
                {
                    return new ValidationResult("The file is empty.");
                }

                // Check file extension
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (Array.IndexOf(FileExtensions.IMAGE_EXTENSIONS, fileExtension) < 0)
                {
                    return new ValidationResult($"Only the following file types are allowed: {string.Join(", ", FileExtensions.IMAGE_EXTENSIONS)}.");
                }

                // Check file size
                if (file.Length > _maxSizeInBytes)
                {
                    return new ValidationResult($"The file size must not exceed {_maxSizeInBytes / (1024 * 1024)} MB.");
                }
            }

            return ValidationResult.Success;
        }
    }
}