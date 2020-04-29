using System.Threading.Tasks;

namespace CollAction.Services.Initialization
{
    public interface IInitializationService
    {
        Task InitializeDatabase();
    }
}