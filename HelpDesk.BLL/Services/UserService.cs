using HelpDesk.BLL.DTOs;
using HelpDesk.BLL.Exceptions;
using HelpDesk.BLL.Interfaces;
using HelpDesk.BLL.Security;
using HelpDesk.DAL.Repositories;
using HelpDesk.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _tokenGenerator;
        private readonly ILogger<UserService> _logger;

        public UserService(IRepository<User> userRepo, IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IJwtTokenGenerator tokenGenerator, ILogger<UserService> logger)
        {
            this._userRepo = userRepo;
            this._unitOfWork = unitOfWork;
            this._passwordHasher = passwordHasher;
            this._tokenGenerator = tokenGenerator;
            this._logger = logger;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            //查用户
            User? user = await _userRepo.Query().FirstOrDefaultAsync(u => u.UserName == request.Username);
            if(user == null)
            {
                throw new BusinessException("用户名或密码错误");
            }

            //校验密码
            bool pwdVerification = _passwordHasher.Verify(request.Password, user.PasswordHash);
            if (pwdVerification == false)
            {
                _logger.LogWarning($"登录失败：用户名 {request.Username} 密码错误");
                throw new BusinessException("用户名或密码错误");
            }

            //签发 JWT
            string token = _tokenGenerator.GenerateToken(user);

            //返回
            return new LoginResponse
            {
                Token = token,
                UserId = user.Id,
                UserName = user.UserName,
                Role = user.Role.ToString(),
            };
        }

        public async Task<UserDto> RegisterAsync(RegisterRequest request)
        {
            //查重
            bool exists = await _userRepo.Query().AnyAsync(u => u.UserName == request.Username);
            if (exists)
            {
                throw new BusinessException("用户名已存在");
            }

            //哈希密码
            string hash = _passwordHasher.Hash(request.Password);

            //构建实体
            User user = new User()
            {
                UserName = request.Username,
                PasswordHash = hash,
                Email = request.Email,
                Role = request.Role,
                CreatedAt = DateTime.Now
            };

            //存入数据库
            await _userRepo.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            //返回 DTO
            return new UserDto()
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Role = user.Role.ToString(),
                CreatedAt = user.CreatedAt
            };
        }
    }
}
