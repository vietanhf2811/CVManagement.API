using CVManagement.Application.DTOs.Auth;
using CVManagement.Application.Interfaces;
using CVManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVManagement.Application.Services
{
    internal class RegistrationService : IRegistrationService
    {
        private const string CandidateRoleName = "Candidate";

        private readonly IRegistrationRepository _repository;
        private readonly IPasswordHasherService _passwordHasher;

        public RegistrationService(IRegistrationRepository repository,IPasswordHasherService passwordHasher)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
        {
            var fullName = request.FullName.Trim();
            var email = request.Email.Trim().ToLowerInvariant();
            if (await _repository.EmailExistsAsync(email, cancellationToken))
            {
                throw new ArgumentException(
                    "This email address is already registered.");
            }

            var role = await _repository.GetRoleByNameAsync(CandidateRoleName, cancellationToken);

            if (role is null){
                throw new InvalidOperationException(
                    "The Candidate role was not found.");
            }

            var user = new User
            {
                RoleId = role.Id,
                Email = email,
                FullName = fullName,
                Phone = string.IsNullOrWhiteSpace(request.Phone)? null: request.Phone.Trim(),
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            var candidate = new Candidate
            {
                CreatedAt = DateTime.UtcNow
            };
            await _repository.CreateCandidateAsync(user, candidate, cancellationToken);
            return new RegisterResponseDto
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = role.Name
            };
        }
    }
}
