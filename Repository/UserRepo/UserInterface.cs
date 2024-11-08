using Models;
using System.Threading.Tasks;

namespace SetorDeCompras.Repository.UserRepo
{
    public interface UserInterface
    {
        Task<User> FindFirstByEmail(string email);
        Task CreateUser(string name, int age, string email);
        Task<User> GetUserByEmailWithInclude(string email);
        Task UpdateUser(User user);
    }
}
