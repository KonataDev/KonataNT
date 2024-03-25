using KonataNT.Common;
using KonataNT.Core;

namespace KonataNT.Test;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        var client = BotFather.CreateBot(new BotConfig
        {
            Protocol = Protocols.Linux,
            Logger = new Logger(),
            AutoReconnect = true,
            GetOptimumServer = true
        }, 0, "");

        var qrCode = await client.FetchQrCode() ?? throw new Exception("Failed to fetch QR code");
        await File.WriteAllBytesAsync("qrcode.png", qrCode.Image);
        
        while (true)
        {
            var state = await client.QueryQrCodeState();
            if (state is QrCodeState.Confirmed)
            {
                await client.QrCodeLogin();
                break;
            }
            
            await Task.Delay(2000);
        }
    }
}

internal class Logger : ILogger
{
    public void Log(string tag, LogLevel level, string message)
    {
        Console.WriteLine($"[{DateTime.Now}] [{tag}] [{level}] {message}");
    }
}