using ViaitaliaAPI.Models;

namespace ViaitaliaAPI.Repositories
{
    public interface IImageRepository
    {
        Task<Image> Upload(Image image);
    }
}
