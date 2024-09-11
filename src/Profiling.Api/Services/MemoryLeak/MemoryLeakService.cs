using System.Collections.Concurrent;
using System.Text;

namespace Profiling.Api.Services.MemoryLeak;

public class MemoryLeakService : IMemoryLeakService
{
    private ConcurrentDictionary<Guid, object> Cache { get; } = new();
    private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
    private static readonly int MessageBytesSize = 32000;

    public void Run() => AddToCache();

    private void AddToCache() => Cache.GetOrAdd(Guid.NewGuid(), GetObject());

#pragma warning disable CA1859 // We want this to be inefficient
    private static object GetObject()
    {
        int charsAvailable = Alphabet.Length;
        StringBuilder sb = new();

        for (int i = 0; i < MessageBytesSize - 4; i++)
        {
            sb.Append(Alphabet[new Random().Next(charsAvailable - 1)]);
        }

        return sb.ToString();
    }
}
#pragma warning restore