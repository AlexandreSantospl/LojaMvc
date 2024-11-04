using Microsoft.AspNetCore.Mvc;
using Data;
using Models;
using System.Threading.Tasks;
using SetorDeCompras.Services;
using Microsoft.EntityFrameworkCore;
using SetorDeCompras.Models;

namespace SetorDeCompras.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;
        private readonly JwtService _jwtService;
        private static Random _random = new Random();


        public UserController(AppDbContext context, EmailService emailService, JwtService jwtService)
        {
            _context = context;
            _emailService = emailService;
            _jwtService = jwtService;
        }

        public int GenerateRandomCode()
        {
            return _random.Next(1000, 10000);
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Registro()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Codigo(string email)
        {
            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegistroPost(string name, string email, int idade)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || idade <= 0)
            {
                ModelState.AddModelError(string.Empty, "Dados inválidos. Por favor, preencha todos os campos corretamente.");
                return View("Registro");
            }

            try
            {
                var response = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);


                if (response == null)
                {

                    User novoUsuario = new User
                    {
                        Name = name,
                        Email = email,
                        Age = idade
                    };

                    int randomCode = GenerateRandomCode();

                    AuthValidationModel newAuthValidationUser = new AuthValidationModel
                    {
                        Email = email,
                        Name = name,
                        Code = randomCode
                    };

                    await _context.AuthValidation.AddAsync(newAuthValidationUser);
                    await _context.Users.AddAsync(novoUsuario);
                    await _context.SaveChangesAsync();

                    await _emailService.SendEmailAsync(email, "Usuario registrado!", "Você foi cadastrado com sucesso!");

                    return RedirectToAction("Login");
                }

                await _emailService.SendEmailAsync(email, "Usuario já registrado!", "Você já possui registro");

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao registrar usuário: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Ocorreu um erro ao registrar o usuário. Tente novamente.");
                return View("Registro");
            }
        }

        [HttpPost]
        public async Task<IActionResult> LoginPost(string email)
        {
            try
            {
                var response = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

                if (response == null)
                {
                    ModelState.AddModelError(string.Empty, "Usuário não encontrado!");
                    return RedirectToAction("Login");
                }

                var authDates = await _context.AuthValidation.FirstOrDefaultAsync(u => u.Email == email);

                int randomCode = GenerateRandomCode();

                authDates!.Code = randomCode;

                _context.AuthValidation.Update(authDates);

                await _context.SaveChangesAsync();

                await _emailService.SendEmailAsync(email, "Seu Codigo Chegou!", $"Aqui está seu codigo {randomCode}");

                return RedirectToAction("Codigo", new { email = email });
            }
            catch
            {
                Console.WriteLine($"Erro");
                return View("Registro");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CodigoPost(int code, string email)
        {
            Console.WriteLine(email);
            var dados = await _context.AuthValidation.FirstOrDefaultAsync(u => u.Email == email);

            if (dados!.Code == code)
            {
                var token = _jwtService.GenerateJwtToken(email);

                return RedirectToAction("Index", "Home", new { token });
            }
            ModelState.AddModelError(string.Empty, "Dados inválidos. Por favor, preencha todos os campos corretamente.");
            return View("Codigo");

        }

    }
}
