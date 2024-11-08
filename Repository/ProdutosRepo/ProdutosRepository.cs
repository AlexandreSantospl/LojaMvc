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
            try
            {
                var dados = await _context.Produtos.FirstOrDefaultAsync(u => u.Name == name);
                if (dados == null)
                {
                    return null;
                }
                return dados;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar produto no banco de dados.", ex);
            }
        }

            public async Task CreateProduto(string name, int quantidade, float preco, string img)
        {
            ProdutosModel novoProduto = new ProdutosModel
            {
                Name = name,
                Quantidade = quantidade,
                Preco = preco,
                Imagem = img
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
