using Data;
using Microsoft.EntityFrameworkCore;
using SetorDeCompras.Models;
using System.Drawing;
using System.Xml.Linq;

namespace SetorDeCompras.Repository.ProdutosRepo
{
    public class ProdutosRepository : ProdutosInterface
    {
        private readonly AppDbContext _context;

        public ProdutosRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProdutosModel>> GetAllProdutos()
        {
            var produtos = await _context.Produtos.ToListAsync();

            if (produtos == null)
            {
                throw new Exception();
            }
            return produtos;

        }

        public async Task<ProdutosModel> FindFirstByName(string name)
        {

            var dados = await _context.Produtos.FirstOrDefaultAsync(u => u.Name == name);
            if (dados == null)
            {
                throw new Exception();
            }
            return dados;

        }

        public async Task CreateProduto(string name, int quantidade, float preco)
        {
            ProdutosModel novoProduto = new ProdutosModel
            {
                Name = name,
                Quantidade = quantidade,
                Preco = preco
            };
            await _context.Produtos.AddAsync(novoProduto);
            await _context.SaveChangesAsync();

        }

        public async Task UpdateProdutos(ProdutosModel produto)
        {
            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();
        }
    }
}
