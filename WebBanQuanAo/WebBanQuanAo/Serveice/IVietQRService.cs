namespace WebBanQuanAo.Serveice
{
    public interface IVietQRService
    {
        Task<string> GenerateQRCodeAsync(string accountNumber, decimal amount, string note);
    }

}
