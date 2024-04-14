using System.IO;
using System.Windows.Media.Imaging;
using TaskManager.Common.Models;

namespace TaskManager.Client.Models.Extensions
{
    public static class CommonModelExtensions
    {
        public static BitmapImage LoadImage(this CommonModel model)
        {
            if (model?.Photo == null || model.Photo.Length == 0)
            {
                return null;
            }

            var image = new BitmapImage();

            using (var memoryStream = new MemoryStream(model.Photo))
            {
                memoryStream.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = memoryStream;
                image.EndInit();
            }

            image.Freeze();
            return image;
        }
    }
}
