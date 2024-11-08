using Data;
using Microsoft.EntityFrameworkCore;
using Models;
using SetorDeCompras.Services;
using ModelUser = Models.User;



namespace SetorDeCompras.Repository.UserRepo
{
    public class UserRepository : UserInterface
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;
        private readonly JwtService _jwtService;

        public UserRepository(AppDbContext context, EmailService emailService, JwtService jwtService)
        {
            _context = context;
            _emailService = emailService;
            _jwtService = jwtService;
        }

        public async Task<ModelUser> FindFirstByEmail(string email)
        {
            var response = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            return response;
        }

        public async Task CreateUser(string name, int age, string email)
        {
            User novoUsuario = new User
            {
                Name = name,
                Email = email,
                Age = age
            };

            await _context.Users.AddAsync(novoUsuario);
            await _context.SaveChangesAsync();

        }

        public async Task<ModelUser> GetUserByEmailWithInclude(string email)
        {
            var user = await _context.Users.Include(u => u.PurchasesList).FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                throw new ArgumentException();
            }
            return user;
        }

        public async Task UpdateUser(ModelUser user)
        {
            if(user == null)
            {
                throw new Exception();
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }


    }
}
