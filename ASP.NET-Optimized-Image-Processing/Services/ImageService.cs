using ASP.NET_Optimized_Image_Processing.Data;
using ASP.NET_Optimized_Image_Processing.Models.Images;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ASP.NET_Optimized_Image_Processing.Services
{
    public class ImageService : IImageService
    {
        private const int ThumbnailWidth = 300;
        private const int FullscreenWidth = 1000;

        private readonly IServiceScopeFactory _serviceFactory;
        private readonly ApplicationDbContext _dbContext;

        public ImageService(IServiceScopeFactory serviceFactory, ApplicationDbContext dbContext)
        {
            _serviceFactory = serviceFactory;
            _dbContext = dbContext;
        }

        public Task<List<string>> GetAllImages()
        {
            var result = _dbContext.Images.Select(i => i.Id.ToString()).ToListAsync();

            return result;
        }

        public Task<Stream> GetFullscreen(string id)
        {
            return GetImageData(id, "Fullscreen");
        }

        public Task<Stream> GetThumbnail(string id)
        {
            return GetImageData(id, "Thumbnail");
        }

        public async Task Process(IEnumerable<ImageInputModel> images)
        {
            var tasks = images
            .Select(image => Task.Run(async () => 
            {
                try
                {
                    using var imageResult = await Image.LoadAsync(image.Content);

                    var original = await SaveImage(imageResult, imageResult.Width);
                    var fullscreen = await SaveImage(imageResult, FullscreenWidth);
                    var thumbnail = await SaveImage(imageResult, ThumbnailWidth);

                    var dbContext = _serviceFactory
                    .CreateScope()
                    .ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                    var imageModel = new ImageData();
                    imageModel.OriginalFileName = image.Name;
                    imageModel.OriginalType = image.Type;
                    imageModel.ThumbnailContent = thumbnail;
                    imageModel.FullscreenContent = fullscreen;
                    imageModel.OriginalContent = original;


                    await dbContext.Images.AddAsync(imageModel);
                    await dbContext.SaveChangesAsync();
                }
                catch
                {

                }
            }));           

            await Task.WhenAll(tasks);
        }

        private async Task<Stream> GetImageData(string id, string size)
        {
            var database = _dbContext.Database;

            var dbConnection = (SqlConnection)database.GetDbConnection();

            var command = new SqlCommand(
                $"SELECT {size}Content FROM Images WHERE Id = @id",
                dbConnection
                );

            command.Parameters.Add(new SqlParameter("@id", id));

            await dbConnection.OpenAsync();

            var reader = await command.ExecuteReaderAsync();

            Stream result = null;

            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    result = reader.GetStream(0);
                }
            }

            await reader.CloseAsync();
            await dbConnection.CloseAsync();

            return result;
        }
       
        private async Task<byte[]> SaveImage(Image image, int resizeWidth)
        {
            var width = image.Width;
            var height = image.Height;

            if (width > resizeWidth)
            {
                height = (int)((double)resizeWidth / width * height);
                width = resizeWidth;
            }

            image
                .Mutate(i => i
                    .Resize(new Size(width, height)));

            image.Metadata.ExifProfile = null;

            var memoryStream = new MemoryStream();


            await image.SaveAsJpegAsync(memoryStream, new JpegEncoder
            {
                Quality = 75
            });

            return memoryStream.ToArray();
        }
    }
}
