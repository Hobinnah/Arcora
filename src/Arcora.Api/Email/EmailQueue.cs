using System.Threading.Channels;

namespace Arcora.Api.Email
{
    /// <summary>
    /// A single email work item to be delivered by the background email dispatcher.
    /// </summary>
    /// <param name="To">Recipient email address.</param>
    /// <param name="Subject">Email subject line.</param>
    /// <param name="HtmlBody">HTML body of the email.</param>
    public sealed record EmailMessage(string To, string Subject, string HtmlBody);

    /// <summary>
    /// Abstraction for enqueuing emails for out-of-band (background) delivery so request handling is
    /// never blocked on SMTP latency.
    /// </summary>
    public interface IEmailQueue
    {
        /// <summary>
        /// Enqueues an email for background delivery. Returns immediately.
        /// </summary>
        ValueTask EnqueueAsync(EmailMessage message, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads queued emails for the background dispatcher to process.
        /// </summary>
        ChannelReader<EmailMessage> Reader { get; }
    }

    /// <summary>
    /// In-memory, thread-safe email queue backed by an unbounded <see cref="Channel{T}"/>.
    /// Registered as a singleton and drained by <see cref="EmailQueueBackgroundService"/>.
    /// </summary>
    public sealed class EmailQueue : IEmailQueue
    {
        private readonly Channel<EmailMessage> _channel;

        public EmailQueue()
        {
            // Single reader (the background service), many writers (request handlers).
            _channel = Channel.CreateUnbounded<EmailMessage>(new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            });
        }

        /// <inheritdoc/>
        public ValueTask EnqueueAsync(EmailMessage message, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(message);
            return _channel.Writer.WriteAsync(message, cancellationToken);
        }

        /// <inheritdoc/>
        public ChannelReader<EmailMessage> Reader => _channel.Reader;
    }
}
