using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ASP.NET_Optimized_Image_Processing.Models.Images
{
    public class ImageInputModel
    {
        public string Name { get; set; }
        
        public string Type { get; set; }

        public Stream Content { get; set; }
    }
}
