using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using HotelBooking.Core.ServiceContracts.IHotelService;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Services.HotelService
{
    public class CloudinaryImageStorageService : IImageStorageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryImageStorageService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<string> UploadAsync(IFormFile file, string folder)
        {
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder
            };
            var result = await _cloudinary.UploadAsync(uploadParams);
            return result.SecureUrl.AbsoluteUri;
        }

        public async Task DeleteAsync(string imageUrl)
        {
            var publicId = GetPublicIdFromUrl(imageUrl);
            if (!string.IsNullOrEmpty(publicId))
            {
                await _cloudinary.DestroyAsync(new DeletionParams(publicId));
            }
        }

        private string GetPublicIdFromUrl(string imageUrl)
        {
            try
            {
                var uri = new Uri(imageUrl);
                var segments = uri.AbsolutePath.Split('/');
                var fileName = segments.Last(); // myimage.jpg
                var folder = segments[segments.Length - 2]; // hotels

                return $"{folder}/{Path.GetFileNameWithoutExtension(fileName)}";
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
