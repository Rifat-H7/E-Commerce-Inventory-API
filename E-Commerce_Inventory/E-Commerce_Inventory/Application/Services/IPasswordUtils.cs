namespace E_Commerce_Inventory.Application.Services
{
    public interface IPasswordUtils
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}
