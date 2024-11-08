using SetorDeCompras.Models;
using System.Threading.Tasks;

namespace SetorDeCompras.Repository.AuthRepo
{
    public interface AuthInterface
    {
        int GenerateRandomCode();
        Task<AuthValidationModel> FindFirstByEmail(string email);
        Task CreateAuthVaidation(string name, string email);
        Task<int> GenerateNewCode(string email);
    }
}
