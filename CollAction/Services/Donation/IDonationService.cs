using System.Threading.Tasks;

namespace CollAction.Services.Donation
{
    public interface IDonationService
    {
        Task Charge(string email, string token, long amount, string currency);
    }
}