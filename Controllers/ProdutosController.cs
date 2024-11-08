using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SetorDeCompras.Models;
using SetorDeCompras.Repository.ProdutosRepo;
using SetorDeCompras.Repository.UserRepo;

namespace SetorDeCompras.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly ProdutosRepository _produtosRepository;


        public ProdutosController(UserRepository userRepository, ProdutosRepository produtosRepository)
        {
            _userRepository = userRepository;
            _produtosRepository = produtosRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> MeuCarrinho(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("O email não pode estar vazio.");
            }

            var user = await _userRepository.GetUserByEmailWithInclude(email);

            if (user == null)
            {
                return NotFound("Usuário não encontrado");
            }

            var meuCarrinho = user.PurchasesList;

            return View(meuCarrinho);
        }


        public async Task<IActionResult> ListagemDeProdutos()
        {
            var produtos = await _produtosRepository.GetAllProdutos();

            return View(produtos);
        }

        public async Task<IActionResult> ListaDeAdicionarProdutos()
        {
            var produtos = await _produtosRepository.GetAllProdutos();

            return View(produtos);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ProdutosPost([FromBody] ProdutosModel request)
        {
            Console.WriteLine($"Dados {request.Preco}");
            Console.WriteLine($"Dados {request.Name}"); 

            if (string.IsNullOrWhiteSpace(request.Name) || request.Quantidade<= 0 || request.Preco <= 0 || string.IsNullOrWhiteSpace(request.Imagem))
            {
                ModelState.AddModelError(string.Empty, "Dados inválidos. Por favor, preencha todos os campos corretamente.");
                return View("ProdutosForm");
            }

            var dados = await _produtosRepository.FindFirstByName(request.Name);

            if (dados == null)
            {
                await _produtosRepository.CreateProduto(request.Name, request.Quantidade, request.Preco, request.Imagem);
            }
            else
            {
                dados.Quantidade += request.Quantidade;
                dados.Preco = request.Preco;
                await _produtosRepository.UpdateProdutos(dados);
            }

            return RedirectToAction("ListagemDeProdutos", "Produtos");

        }

        public class ProdutosRequestModel
        {
            public ProdutosModel[] Produtos { get; set; }
            public string Email { get; set; }
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AdicionarProdutosPost([FromBody] ProdutosRequestModel request)
        {
            Console.WriteLine($"Produtos: {request.Produtos}");
            Console.WriteLine($"Email: {request.Email}");

            if (request.Produtos == null || !request.Produtos.Any())
            {
                ModelState.AddModelError(string.Empty, "Nenhum produto foi selecionado.");
                return View("ListaDeAdicionarProdutos");
            }

            var user = await _userRepository.GetUserByEmailWithInclude(request.Email);

            if (user != null)
            {
                foreach (var produto in request.Produtos)
                {
                    if (produto.Quantidade > 0)
                    {
                        var existingPurchase = user.PurchasesList.FirstOrDefault(p => p.Name == produto.Name);

                        if (existingPurchase != null)
                        {
                            existingPurchase.Price = produto.Preco;
                            existingPurchase.Quantidade = produto.Quantidade + existingPurchase.Quantidade;
                        }
                        else
                        {
                            var newPurchase = new PurchaseList
                            {
                                Name = produto.Name,
                                Price = produto.Preco,
                                Quantidade = produto.Quantidade
                            };
                            user.PurchasesList.Add(newPurchase);
                        }

                        var existingProduct = await _produtosRepository.FindFirstByName(produto.Name);

                        if (existingProduct != null)
                        {
                            existingProduct.Quantidade = existingProduct.Quantidade - produto.Quantidade;
                            await _produtosRepository.UpdateProdutos(existingProduct);
                        }
                    }
                }

                await _userRepository.UpdateUser(user);
                return RedirectToAction("MeuCarrinho", "Produtos", new { email = request.Email });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Usuário não encontrado.");
                return View("ListaDeAdicionarProdutos");
            }
        }



    }
}
