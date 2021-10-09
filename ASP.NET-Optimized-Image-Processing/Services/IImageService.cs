using ASP.NET_Optimized_Image_Processing.Models.Images;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ASP.NET_Optimized_Image_Processing.Services
{
    public interface IImageService
    {
        Task Process(IEnumerable<ImageInputModel> images);

        Task<Stream> GetThumbnail(string id);

        Task<Stream> GetFullscreen(string id);

        Task<List<string>> GetAllImages();
    }
}
