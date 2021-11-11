using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProEventos.Application.Contratos;
using ProEventos.Application.DTOs;
using ProEventos.Domain.Identity;
using ProEventos.Persistence.Contratos;

namespace ProEventos.Application
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;
        private readonly IUserPersist _userPersist;

        public AccountService(UserManager<User> userManager,
                              SignInManager<User> signInManager,
                              IMapper mapper,
                              IUserPersist userPersist)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _userPersist = userPersist;
        }

        #region CheckUserPasswordAsync
        /// <summary>
        /// CheckUserPasswordAsync
        /// </summary>
        /// <param name="model"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<SignInResult> CheckUserPasswordAsync(UserUpdateDTO model, string password)
        {
            try
            {
                var user = await _userManager.Users
                                             .SingleOrDefaultAsync(user => user.UserName == model.UserName.ToLower());

                return await _signInManager.CheckPasswordSignInAsync(user, password, false);
            }
            catch (System.Exception ex)
            {
                throw new Exception($"Erro ao tentar verificar password. Erro: {ex.Message}");
            }
        }
        #endregion

        #region CreateAccountAsync
        /// <summary>
        /// CreateAccountAsync
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<UserDTO> CreateAccountAsync(UserDTO model)
        {
            try
            {
                var user = _mapper.Map<User>(model);
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return _mapper.Map<UserDTO>(user);
                }

                return null;
            }
            catch (System.Exception ex)
            {
                throw new Exception($"Erro ao tentar criar usuário. Erro: {ex.Message}");
            }
        }

        #endregion

        #region GetUserByUserNameAsync
        /// <summary>
        /// GetUserByUserNameAsync
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<UserUpdateDTO> GetUserByUserNameAsync(string userName)
        {
            try
            {
                var user = await _userPersist.GetUserByUserNameAsync(userName);
                if (user == null) return null;

                return _mapper.Map<UserUpdateDTO>(user);
            }
            catch (System.Exception ex)
            {
                throw new Exception($"Erro ao tentar obter usuário por username. Erro: {ex.Message}");
            }
        }
        #endregion

        #region UpdateAccount
        /// <summary>
        /// UpdateAccount
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<UserUpdateDTO> UpdateAccount(UserUpdateDTO model)
        {
            try
            {
                var user = await _userPersist.GetUserByUserNameAsync(model.UserName);
                if (user == null) return null;

                _mapper.Map(model, user);

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, model.Password);

                _userPersist.Update<User>(user);

                if (await _userPersist.SaveChangesAsync())
                {
                    var userToReturn = await _userPersist.GetUserByUserNameAsync(user.UserName);

                    return _mapper.Map<UserUpdateDTO>(userToReturn);
                }

                return null;
            }
            catch (System.Exception ex)
            {
                throw new Exception($"Erro ao tentar atualizar usuário. Erro: {ex.Message}");
            }
        }
        #endregion

        #region UserExists
        /// <summary>
        /// UserExists
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<bool> UserExists(string userName)
        {
            try
            {
                return await _userManager.Users
                                         .AnyAsync(user => user.UserName == userName.ToLower());
            }
            catch (System.Exception ex)
            {
                throw new Exception($"Erro ao verificar se usuário existe. Erro: {ex.Message}");
            }
        }
        #endregion
    }
}