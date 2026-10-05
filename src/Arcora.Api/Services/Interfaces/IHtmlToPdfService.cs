namespace Arcora.Api.Services.Interfaces
{
    public class PdfSignatureAnchor
    {
        public int Page { get; set; } = 1;
        public decimal X { get; set; }
        public decimal Y { get; set; }
        public decimal Width { get; set; } = 180;
        public decimal Height { get; set; } = 45;
    }

    public class HtmlToPdfResult
    {
        public byte[] PdfBytes { get; set; } = Array.Empty<byte>();
        public List<PdfSignatureAnchor> SignatureAnchors { get; set; } = new();
    }

    public interface IHtmlToPdfService
    {
        /// <summary>
        /// Converts HTML content into a PDF byte array. Implementations may call an external renderer.
        /// </summary>
        Task<byte[]> ConvertHtmlToPdfAsync(string html, CancellationToken cancellationToken = default);

        /// <summary>
        /// Converts HTML content into a PDF byte array and returns positional anchors for matching elements.
        /// </summary>
        Task<HtmlToPdfResult> ConvertHtmlToPdfWithAnchorsAsync(string html, string anchorCssSelector = ".sig-line", CancellationToken cancellationToken = default);
    }
}
