using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using SetorDeCompras.Models;
using SetorDeCompras.Services;

namespace SetorDeCompras.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly AppDbContext _context;

        public ProdutosController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> MeuCarrinho(string email)
        {
            var user = await _context.Users.Include(u => u.PurchasesList).FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return NotFound("Usuário não encontrado");
            }

            var meuCarrinho = user.PurchasesList;

            return View(meuCarrinho);
        }


        public async Task<IActionResult> ListagemDeProdutos()
        {
            var produtos = await _context.Produtos.ToListAsync();

            return View(produtos);
        }

        public async Task<IActionResult> ListaDeAdicionarProdutos()
        {
            var produtos = await _context.Produtos.ToListAsync();

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

            var dados = await _context.Produtos.FirstOrDefaultAsync(u => u.Name == name);

            if (dados == null)
            {
                ProdutosModel novoProduto = new ProdutosModel
                {
                    Name = name,
                    Quantidade = qntd,
                    Preco = valor
                };
                await _context.Produtos.AddAsync(novoProduto);
            }
            else
            {
                dados.Quantidade += qntd;
                dados.Preco = valor;
                _context.Produtos.Update(dados);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("ListagemDeProdutos", "Produtos");

        }


        [HttpPost]
        public async Task<IActionResult> AdicionarProdutosPost2(ProdutosModel[] produtos, string email)
        {
            Console.WriteLine($"Seu email: {email}");
            if (produtos == null || !produtos.Any())
            {
                ModelState.AddModelError(string.Empty, "Nenhum produto foi selecionado.");
                return View("ListaDeAdicionarProdutos");
            }

            List<PurchaseList> listaDeCriacao = new List<PurchaseList>();

            foreach (var produto in produtos)
            {
                if (produto.Quantidade > 0)
                {
                    listaDeCriacao.Add(new PurchaseList
                    {
                        Name = produto.Name,
                        Price = produto.Preco,
                        Quantidade = produto.Quantidade
                    });
                }
            }

            var user = await _context.Users.Include(u => u.PurchasesList).FirstOrDefaultAsync(u => u.Email == email);

            if (user != null)
            {
                user.PurchasesList.AddRange(listaDeCriacao);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("ListagemDeProdutos", "Produtos");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Usuário não encontrado.");
                return View("ListaDeAdicionarProdutos");
            }
        }


        [HttpPost]
        public async Task<IActionResult> AdicionarProdutosPost(ProdutosModel[] produtos, string email)
        {
            if (produtos == null || !produtos.Any())
            {
                ModelState.AddModelError(string.Empty, "Nenhum produto foi selecionado.");
                return View("ListaDeAdicionarProdutos");
            }

            var user = await _context.Users.Include(u => u.PurchasesList).FirstOrDefaultAsync(u => u.Email == email);

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
                    }
                }

                _context.Users.Update(user);
                await _context.SaveChangesAsync(); 
                return RedirectToAction("ListagemDeProdutos", "Produtos");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Usuário não encontrado.");
                return View("ListaDeAdicionarProdutos");
            }
        }


    }
}
