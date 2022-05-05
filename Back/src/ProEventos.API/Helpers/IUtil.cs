using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ProEventos.API.Helpers
{
    public interface IUtil
    {
        Task<string> SaveImage(IFormFile imageFile, string destination);
        void DeleteImage(string imageName, string destination);
    }
}