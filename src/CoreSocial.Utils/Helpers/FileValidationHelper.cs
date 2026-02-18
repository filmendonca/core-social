using Ardalis.GuardClauses;
using FileSignatures;
using FileSignatures.Formats;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using Utils.Constants;
using Utils.Enums;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Img = SixLabors.ImageSharp.Image;
using Image = FileSignatures.Formats.Image;

namespace Utils.Helpers
{
    //Class used for the validation of files uploaded by the user
    public static class FileValidationHelper
    {
        public static string ValidateFileName(string fileName, FileStorageDirectory dir)
        {
            Guard.Against.NullOrWhiteSpace(fileName, nameof(fileName));
            Guard.Against.EnumOutOfRange(dir, nameof(dir));

            fileName = fileName.Trim();
            fileName = fileName.Replace(" ", "_");

            //Filter file name and discard non alphanumeric chars
            var sanitizedChars = fileName.Where(c => char.IsLetterOrDigit(c) || c == '.' || c == '_').ToArray();
            var newFileName = new string(sanitizedChars);

            //Add timestamp or random Guid to the start of the file
            switch (dir)
            {
                case FileStorageDirectory.Temp:
                    newFileName = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff") + "_" + newFileName;
                    break;
                case FileStorageDirectory.Upload:
                    newFileName = Guid.NewGuid() + "_" + newFileName;
                    break;
                default:
                    throw new Exception("Something went wrong.");
            }

            if (newFileName.Length > 255) throw new InvalidOperationException("File name is too long.");

            return newFileName;
        }

        public static FileResult ValidateFile(IFormFile file, FileCategory category)
        {   
            Guard.Against.Null(file, nameof(file));
            Guard.Against.EnumOutOfRange(category, nameof(category));
            using var stream = file.OpenReadStream();

            #region Check Image Dimensions

            using var img = Img.Load<Rgba32>(stream);

            var check = false;
            switch (category)
            {
                case FileCategory.Avatar:
                    check = img.Width < ImageDimensions.AVATAR_IMAGE_MIN_WIDTH || img.Height < ImageDimensions.AVATAR_IMAGE_MIN_HEIGHT ||
                            img.Width > ImageDimensions.AVATAR_IMAGE_MAX_WIDTH || img.Height > ImageDimensions.AVATAR_IMAGE_MAX_HEIGHT;
                    break;
                case FileCategory.PostImage:
                    check = img.Width < ImageDimensions.POST_IMAGE_MIN_WIDTH || img.Height < ImageDimensions.POST_IMAGE_MIN_HEIGHT ||
                            img.Width > ImageDimensions.POST_IMAGE_MAX_WIDTH || img.Height > ImageDimensions.POST_IMAGE_MAX_HEIGHT;
                    break;
                default:
                    throw new Exception("Something went wrong.");
            }

            if (check)
                return FileResult.InvalidDimensions;

            #endregion

            #region Check File Extension

            //Get file extension from the header signature
            var inspector = new FileFormatInspector();
            var format = inspector.DetermineFileFormat(stream);

            //Check if file uploaded is an image
            Guard.Against.InvalidInput(format, nameof(format), form => form is Image);

            //Check if file extension is valid
            if (!FileExtensions.IMAGE_EXTENSIONS.Contains("." + format.Extension))
                return FileResult.InvalidFormat;

            #endregion

            return FileResult.Success;
        }
    }
}
