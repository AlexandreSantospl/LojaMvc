using Data;
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

        public async Task<IActionResult> ListagemDeProdutos()
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


    }
}
