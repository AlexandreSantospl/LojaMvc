using Data;
using SetorDeCompras.Services;
using SetorDeCompras.Models;
using System;
using Microsoft.EntityFrameworkCore;

namespace SetorDeCompras.Repository.AuthRepo
{
    public class AuthRepository : AuthInterface
    {
        private readonly AppDbContext _context;
        private static Random _random = new Random();

        public AuthRepository(AppDbContext context)
        {
            _context = context;
        }

        public int GenerateRandomCode()
        {
            return _random.Next(1000, 10000);
        }

        public async Task<AuthValidationModel> FindFirstByEmail(string email)
        {
            var response = await _context.AuthValidation.FirstOrDefaultAsync(u => u.Email == email);

            if (response == null)
            {
                throw new ArgumentException();
            }
            return response;
        }

        public async Task CreateAuthVaidation(string name, string email)
        {
            int randomCode = GenerateRandomCode();

            AuthValidationModel newAuthValidationUser = new AuthValidationModel
            {
                Email = email,
                Name = name,
                Code = randomCode
            };

            await _context.AuthValidation.AddAsync(newAuthValidationUser);
            await _context.SaveChangesAsync();

        }

        public async Task<int> GenerateNewCode(string email)
        {
            var authDates = await this.FindFirstByEmail(email);

            int randomCode = GenerateRandomCode();

            authDates!.Code = randomCode;

            _context.AuthValidation.Update(authDates);

            await _context.SaveChangesAsync();

            return randomCode;
        }


    }
}
