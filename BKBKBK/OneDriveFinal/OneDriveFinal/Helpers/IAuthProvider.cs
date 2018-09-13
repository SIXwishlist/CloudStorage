using System.Threading.Tasks;


namespace OneDriveFinal.Helpers
{
    public interface IAuthProvider
    {
        Task<string> GetUserAccessTokenAsync();
    }
}