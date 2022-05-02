using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProEventos.API.Extensions;
using ProEventos.Application.Contratos;
using ProEventos.Application.DTOs;

namespace ProEventos.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ITokenService _tokenService;

        public AccountController(IAccountService accountService,
                                 ITokenService tokenService)
        {
            _accountService = accountService;
            _tokenService = tokenService;
        }

        #region GetUser
        /// <summary>
        /// GetUser
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser()
        {
            try
            {
                var userName = User.GetUserName();
                var user = await _accountService.GetUserByUserNameAsync(userName);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar recuperar usuário. Erro: {ex.Message}");
            }
        }
        #endregion

        #region Register
        /// <summary>
        /// Register
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserDTO model)
        {
            try
            {
                if (await _accountService.UserExists(model.UserName))
                    return BadRequest("Usuário já existe");

                var user = await _accountService.CreateAccountAsync(model);
                if (user != null)
                    return Ok(new
                    {
                        userName = user.UserName,
                        primeiroNome = user.PrimeiroNome,
                        token = _tokenService.CreateToken(user).Result
                    });

                return BadRequest("Usuário não criado, tente novamente mais tarde!");
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar registrar usuário. Erro: {ex.Message}");
            }
        }
        #endregion

        #region Login
        /// <summary>
        /// Login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDTO model)
        {
            try
            {
                var user = await _accountService.GetUserByUserNameAsync(model.Username);
                if (user == null) return Unauthorized("Usuário ou Senha está incorreto");

                var result = await _accountService.CheckUserPasswordAsync(user, model.Password);
                if (!result.Succeeded) return Unauthorized();

                return Ok(new
                {
                    userName = user.UserName,
                    primeiroNome = user.PrimeiroNome,
                    token = _tokenService.CreateToken(user).Result
                });
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar realizar login. Erro: {ex.Message}");
            }
        }
        #endregion

        #region UpdateUser
        /// <summary>
        /// UpdateUser
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser(UserUpdateDTO model)
        {
            try
            {
                if (model.UserName != User.GetUserName())
                    return Unauthorized("Usuário Inválido");

                var user = await _accountService.GetUserByUserNameAsync(User.GetUserName());
                if (user == null) return Unauthorized("Usuário Inválido");

                var userToReturn = await _accountService.UpdateAccount(model);
                if (userToReturn == null) return NoContent();

                return Ok(new
                {
                    userName = userToReturn.UserName,
                    primeiroNome = userToReturn.PrimeiroNome,
                    token = _tokenService.CreateToken(userToReturn).Result
                });
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar atualizar usuário. Erro: {ex.Message}");
            }
        }
        #endregion
    }
}