using WebBanQuanAo.Models;

namespace WebBanQuanAo.Serveice
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        bool ValidateToken(string token);
    }
}