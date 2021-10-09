using ASP.NET_Optimized_Image_Processing.Models.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASP.NET_Optimized_Image_Processing.Services
{
    public interface IFileImageService
    {
        Task Process(IEnumerable<ImageInputModel> images);

        Task<List<string>> GetAllImages();
    }
}
