using ASP.NET_Optimized_Image_Processing.Infrastructure.Filters;
using ASP.NET_Optimized_Image_Processing.Models.Images;
using ASP.NET_Optimized_Image_Processing.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ASP.NET_Optimized_Image_Processing.Controllers
{
    public class ImagesController : Controller
    {
        private readonly IImageService _imageService;

        public ImagesController(IImageService imageService)
        {
            _imageService = imageService;
        }

        public IActionResult Upload()
        {
            return View();
        }

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

        [HttpGet]
        public async Task<IActionResult> All()
        {
            return View(await _imageService.GetAllImages());
        }

        [HttpGet]
        public async Task<IActionResult> Thumbnail(string id)
        {
            return ReturnImageCache(await _imageService.GetThumbnail(id));
        }

        [HttpGet]
        public async Task<IActionResult> Fullscreen(string id)
        {
            return ReturnImageCache(await _imageService.GetFullscreen(id));
        }

        [HttpGet]
        public IActionResult Done()
        {
            return View();
        }

        private IActionResult ReturnImageCache(Stream image)
        {
            var headers = Response.GetTypedHeaders();

            headers.CacheControl = new CacheControlHeaderValue {
                Public = true,
                MaxAge = TimeSpan.FromDays(30)
            };

            headers.Expires = new DateTimeOffset(DateTime.UtcNow.AddDays(30));

            return File(image, "image/jpeg");
        }
    }
}
