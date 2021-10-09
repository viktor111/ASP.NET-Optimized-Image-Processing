using ASP.NET_Optimized_Image_Processing.Infrastructure.Filters;
using ASP.NET_Optimized_Image_Processing.Models.Images;
using ASP.NET_Optimized_Image_Processing.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ASP.NET_Optimized_Image_Processing.Controllers
{
    public class FileImagesController : Controller
    {
        private readonly IFileImageService _imageService;

        public FileImagesController(IFileImageService imageService)
            => _imageService = imageService;

        [HttpGet]
        public IActionResult Upload() => View();

        [HttpPost]
        [RequestSizeLimitChecker]
        public async Task<IActionResult> Upload(IFormFile[] images)
        {
            await _imageService.Process(images.Select(i => new ImageInputModel
            {
                Name = i.FileName,
                Type = i.ContentType,
                Content = i.OpenReadStream()
            }));

            return RedirectToAction(nameof(Done));
        }

        public async Task<IActionResult> All()
            => this.View(await _imageService.GetAllImages());

        public IActionResult Done() => View();
    }
}
