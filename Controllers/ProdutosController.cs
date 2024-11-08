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

        [HttpPost]
        public async Task<IActionResult> ProdutosPost(string name, int qntd, float valor)
        {

            if (string.IsNullOrWhiteSpace(name) || qntd <= 0 || valor <= 0)
            {
                ModelState.AddModelError(string.Empty, "Dados inválidos. Por favor, preencha todos os campos corretamente.");
                return View("ProdutosForm");
            }

            var dados = await _produtosRepository.FindFirstByName(name);

            if (dados == null)
            {
                await _produtosRepository.CreateProduto(name, qntd, valor);
            }
            else
            {
                dados.Quantidade += qntd;
                dados.Preco = valor;
                await _produtosRepository.UpdateProdutos(dados);
            }

            return RedirectToAction("ListagemDeProdutos", "Produtos");

        }


        [HttpPost]
        public async Task<IActionResult> AdicionarProdutosPost(ProdutosModel[] produtos, string email)
        {
            if (produtos == null || !produtos.Any())
            {
                ModelState.AddModelError(string.Empty, "Nenhum produto foi selecionado.");
                return View("ListaDeAdicionarProdutos");
            }

            var user = await _userRepository.GetUserByEmailWithInclude(email);

            if (user != null)
            {
                foreach (var produto in produtos)
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
                return RedirectToAction("MeuCarrinho", "Produtos", new { email });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Usuário não encontrado.");
                return View("ListaDeAdicionarProdutos");
            }
        }


    }
}
