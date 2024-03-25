using System.Net.NetworkInformation;

namespace KonataNT.Utility.Network;

internal static class Icmp
{
    public static async Task<long> PingAsync(Uri hostIp, int timeout = 1000)
    {
        using var ping = new Ping();
        var reply = await ping.SendPingAsync(hostIp.Host, timeout);
        return reply?.RoundtripTime ?? long.MaxValue;
    }
}