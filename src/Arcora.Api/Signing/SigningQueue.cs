using System.Threading.Channels;
using Arcora.Api.Models;

namespace Arcora.Api.Signing
{
    public interface ISigningQueue
    {
        ValueTask EnqueueAsync(SigningWorkItem item, CancellationToken cancellationToken = default);
        ChannelReader<SigningWorkItem> Reader { get; }
    }

    public sealed class SigningQueue : ISigningQueue
    {
        private readonly Channel<SigningWorkItem> _channel;

        public SigningQueue()
        {
            _channel = Channel.CreateUnbounded<SigningWorkItem>(new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            });
        }

        public ValueTask EnqueueAsync(SigningWorkItem item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);
            return _channel.Writer.WriteAsync(item, cancellationToken);
        }

        public ChannelReader<SigningWorkItem> Reader => _channel.Reader;
    }
}
