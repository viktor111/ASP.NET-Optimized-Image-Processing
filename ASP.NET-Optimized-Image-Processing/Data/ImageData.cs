using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASP.NET_Optimized_Image_Processing.Data
{
    public class ImageData
    {
        public ImageData()
        {
            Id = new Guid();
        }

        public Guid Id { get; set; }

        public string OriginalFileName { get; set; }

        public string OriginalType { get; set; }

        public byte[] OriginalContent { get; set; }

        public byte[] ThumbnailContent { get; set; }

        public byte[] FullscreenContent { get; set; }
    }
}
