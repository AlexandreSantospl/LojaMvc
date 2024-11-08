using Microsoft.AspNetCore.Mvc;
using SetorDeCompras.Services;
using SetorDeCompras.Repository.AuthRepo;
using SetorDeCompras.Repository.UserRepo;

namespace SetorDeCompras.Controllers
{
    public class UserController : Controller
    {
        private readonly EmailService _emailService;
        private readonly JwtService _jwtService;
        private readonly AuthRepository _authRepository;
        private readonly UserRepository _userRepository;

        public UserController(EmailService emailService, JwtService jwtService, AuthRepository authRepository, UserRepository userRepository)
        {
            _emailService = emailService;
            _jwtService = jwtService;
            _authRepository = authRepository;
            _userRepository = userRepository;
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

                var response = await _userRepository.FindFirstByEmail(email);

                if (response == null)
                {
                    await _authRepository.CreateAuthVaidation(name, email);
                    await _userRepository.CreateUser(name, idade, email);
                    await _emailService.SendEmailAsync(email, "Usuario registrado!", "Você foi cadastrado com sucesso!");

                    return RedirectToAction("Login");
                }

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
                var response = await _userRepository.FindFirstByEmail(email);

                if (response == null)
                {
                    ModelState.AddModelError(string.Empty, "Usuário não encontrado!");
                    return RedirectToAction("Login");
                }

                var randomCode = await _authRepository.GenerateNewCode(email);

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
            Console.WriteLine("emailAAAAAAAAAAAAAAAAAAAAA",email);
            var dados = await _authRepository.FindFirstByEmail(email);

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
