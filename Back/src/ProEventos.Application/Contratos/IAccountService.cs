using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ProEventos.Application.DTOs;

namespace ProEventos.Application.Contratos
{
    public interface IAccountService
    {
        Task<bool> UserExists(string username);
        Task<UserUpdateDTO> GetUserByUserNameAsync(string username);
        Task<SignInResult> CheckUserPasswordAsync(UserUpdateDTO model, string password);
        Task<UserUpdateDTO> CreateAccountAsync(UserDTO model);
        Task<UserUpdateDTO> UpdateAccount(UserUpdateDTO model);
    }
} 