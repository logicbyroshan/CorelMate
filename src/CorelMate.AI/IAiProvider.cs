using System.Threading;
using System.Threading.Tasks;

namespace CorelMate.AI;

public interface IAiProvider
{
    string ProviderId { get; }
    Task<string> SendTextAsync(string prompt, CancellationToken cancellationToken);
}