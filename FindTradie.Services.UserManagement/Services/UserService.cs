// Services/UserService.cs
using AutoMapper;
using FindTradie.Services.UserManagement.DTOs;
using FindTradie.Services.UserManagement.Entities;
using FindTradie.Services.UserManagement.Repositories;
using FindTradie.Shared.Contracts.Common;
using FindTradie.Shared.Contracts.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FindTradie.Services.UserManagement.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;
    private readonly IConfiguration _configuration;

    public UserService(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<UserService> logger,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<ApiResponse<UserProfileDto>> CreateUserAsync(CreateUserRequest request)
    {
        try
        {
            // Check if email already exists
            if (await _userRepository.EmailExistsAsync(request.Email))
            {
                return ApiResponse<UserProfileDto>.ErrorResult("Email already registered");
            }

            // Check if phone already exists
            if (await _userRepository.PhoneExistsAsync(request.PhoneNumber))
            {
                return ApiResponse<UserProfileDto>.ErrorResult("Phone number already registered");
            }

            var user = new User
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                UserType = request.UserType
            };

            var createdUser = await _userRepository.CreateAsync(user);
            var userDto = _mapper.Map<UserProfileDto>(createdUser);

            _logger.LogInformation("Created user {UserId} with email {Email}", createdUser.Id, request.Email);

            return ApiResponse<UserProfileDto>.SuccessResult(userDto, "User created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user with email {Email}", request.Email);
            return ApiResponse<UserProfileDto>.ErrorResult("Failed to create user");
        }
    }

    public async Task<ApiResponse<UserProfileDto>> GetUserAsync(Guid id)
    {
        try
        {
            var user = await _userRepository.GetByIdWithDetailsAsync(id);
            if (user == null)
            {
                return ApiResponse<UserProfileDto>.ErrorResult("User not found");
            }

            var userDto = _mapper.Map<UserProfileDto>(user);
            return ApiResponse<UserProfileDto>.SuccessResult(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user {UserId}", id);
            return ApiResponse<UserProfileDto>.ErrorResult("Failed to retrieve user");
        }
    }

    public async Task<ApiResponse<UserProfileDto>> GetUserByEmailAsync(string email)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return ApiResponse<UserProfileDto>.ErrorResult("User not found");
            }

            var userDto = _mapper.Map<UserProfileDto>(user);
            return ApiResponse<UserProfileDto>.SuccessResult(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with email {Email}", email);
            return ApiResponse<UserProfileDto>.ErrorResult("Failed to retrieve user");
        }
    }

    public async Task<ApiResponse<string>> LoginAsync(string email, string password)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return ApiResponse<string>.ErrorResult("Invalid email or password");
            }

            var key = _configuration["JWT:Secret"];
            if (string.IsNullOrEmpty(key))
            {
                _logger.LogError("JWT secret not configured");
                return ApiResponse<string>.ErrorResult("Authentication configuration error");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.UserType.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(int.TryParse(_configuration["JWT:ExpirationInMinutes"], out var exp) ? exp : 60),
                Issuer = _configuration["JWT:Issuer"],
                Audience = _configuration["JWT:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            return ApiResponse<string>.SuccessResult(tokenHandler.WriteToken(token), "Login successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging in user {Email}", email);
            return ApiResponse<string>.ErrorResult("Login failed");
        }
    }

    public async Task<ApiResponse<UserProfileDto>> UpdateUserAsync(Guid id, UpdateUserRequest request)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return ApiResponse<UserProfileDto>.ErrorResult("User not found");
            }

            // Check if email is being changed and if new email already exists
            if (user.Email != request.Email && await _userRepository.EmailExistsAsync(request.Email, id))
            {
                return ApiResponse<UserProfileDto>.ErrorResult("Email already registered");
            }

            // Check if phone is being changed and if new phone already exists
            if (user.PhoneNumber != request.PhoneNumber && await _userRepository.PhoneExistsAsync(request.PhoneNumber, id))
            {
                return ApiResponse<UserProfileDto>.ErrorResult("Phone number already registered");
            }

            // Update user properties
            user.Email = request.Email;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;

            var updatedUser = await _userRepository.UpdateAsync(user);
            var userDto = _mapper.Map<UserProfileDto>(updatedUser);

            return ApiResponse<UserProfileDto>.SuccessResult(userDto, "User updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            return ApiResponse<UserProfileDto>.ErrorResult("Failed to update user");
        }
    }

    public async Task<ApiResponse<List<UserProfileDto>>> GetAllUsersAsync(int pageNumber = 1, int pageSize = 20)
    {
        try
        {
            var users = await _userRepository.GetAllAsync(pageNumber, pageSize);
            var userDtos = _mapper.Map<List<UserProfileDto>>(users);
            return ApiResponse<List<UserProfileDto>>.SuccessResult(userDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return ApiResponse<List<UserProfileDto>>.ErrorResult("Failed to retrieve users");
        }
    }

    public async Task<ApiResponse<bool>> DeleteUserAsync(Guid id)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return ApiResponse<bool>.ErrorResult("User not found");
            }

            user.IsDeleted = true;
            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("Deleted user {UserId}", id);

            return ApiResponse<bool>.SuccessResult(true, "User deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", id);
            return ApiResponse<bool>.ErrorResult("Failed to delete user");
        }
    }

    public async Task<ApiResponse<bool>> VerifyEmailAsync(Guid id)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return ApiResponse<bool>.ErrorResult("User not found");
            }

            user.IsEmailVerified = true;
            await _userRepository.UpdateAsync(user);

            return ApiResponse<bool>.SuccessResult(true, "Email verified successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying email for user {UserId}", id);
            return ApiResponse<bool>.ErrorResult("Failed to verify email");
        }
    }

    public async Task<ApiResponse<bool>> VerifyPhoneAsync(Guid id)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return ApiResponse<bool>.ErrorResult("User not found");
            }

            user.IsPhoneVerified = true;
            await _userRepository.UpdateAsync(user);

            return ApiResponse<bool>.SuccessResult(true, "Phone verified successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying phone for user {UserId}", id);
            return ApiResponse<bool>.ErrorResult("Failed to verify phone");
        }
    }
}