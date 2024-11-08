using SetorDeCompras.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SetorDeCompras.Repository.ProdutosRepo
{
    public interface ProdutosInterface
    {
        Task<List<ProdutosModel>> GetAllProdutos();
        Task<ProdutosModel> FindFirstByName(string name);
        Task CreateProduto(string name, int quantidade, float preco, string img);
        Task UpdateProdutos(ProdutosModel produto);
    }
}
