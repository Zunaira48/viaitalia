using ViaitaliaAPI.Data;
using ViaitaliaAPI.Models;

namespace ViaitaliaAPI.Repositories
{
    public class ImageRepository: IImageRepository
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly TravelDBContext _travelDbContext;

        public ImageRepository(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, TravelDBContext travelDbContext)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
            this._travelDbContext = travelDbContext;
        }
        public async Task<Image> Upload(Image image)
        {
            // Save image to wwwroot/Images (publicly accessible)
            var imageFileName = $"{Guid.NewGuid()}{image.FileExtension}";
            var wwwRootPath = webHostEnvironment.WebRootPath;
            var localFilePath = Path.Combine(wwwRootPath, "Images", imageFileName);

            // Create directory if it doesn't exist
            var imageFolder = Path.GetDirectoryName(localFilePath);
            if (!Directory.Exists(imageFolder))
                Directory.CreateDirectory(imageFolder);

            using var stream = new FileStream(localFilePath, FileMode.Create);
            await image.File.CopyToAsync(stream);

            // This will be used as the <img src="..."> path
            var urlFilePath = $"/Images/{imageFileName}";

            image.FilePath = urlFilePath;
            image.FileName = Path.GetFileNameWithoutExtension(imageFileName);

            await _travelDbContext.Images.AddAsync(image);
            await _travelDbContext.SaveChangesAsync();

            return image;
        }
    }
}
