using Arcora.Api.Services.Interfaces;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace Arcora.Api.Services.Implementations
{
    public class HtmlToPdfService : IHtmlToPdfService
    {
        private const decimal PxToPt = 0.75m;
        private const decimal PageHeightPt = 841.89m;
        private const decimal MarginTopPt = 34.02m;
        private const decimal MarginBottomPt = 34.02m;
        private const decimal MarginLeftPt = 28.35m;
        private const decimal MarginRightPt = 28.35m;

        public async Task<byte[]> ConvertHtmlToPdfAsync(string html, CancellationToken cancellationToken = default)
        {
            var result = await ConvertHtmlToPdfWithAnchorsAsync(html, cancellationToken: cancellationToken);
            return result.PdfBytes;
        }

        public async Task<HtmlToPdfResult> ConvertHtmlToPdfWithAnchorsAsync(string html, string anchorCssSelector = ".sig-line", CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var content = EnsureHtmlDocument(html);

            try
            {
                using var playwright = await Playwright.CreateAsync();
                await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = true,
                    Args = new[] { "--no-sandbox", "--disable-dev-shm-usage" }
                });

                var page = await browser.NewPageAsync(new BrowserNewPageOptions
                {
                    ViewportSize = new ViewportSize { Width = 1240, Height = 1754 }
                });

                await page.SetContentAsync(content, new PageSetContentOptions
                {
                    WaitUntil = WaitUntilState.NetworkIdle,
                    Timeout = 60000
                });

                await page.AddStyleTagAsync(new PageAddStyleTagOptions
                {
                    Content = BuildPdfBorderFixCss()
                });

                List<DomRect> domAnchors;
                try
                {
                    domAnchors = await GetAnchorRectsAsync(page, anchorCssSelector, cancellationToken);
                }
                catch
                {
                    domAnchors = new List<DomRect>();
                }

                cancellationToken.ThrowIfCancellationRequested();

                var pdf = await page.PdfAsync(new PagePdfOptions
                {
                    Format = "A4",
                    PrintBackground = true,
                    PreferCSSPageSize = true,
                    Margin = new Margin
                    {
                        Top = "12mm",
                        Right = "10mm",
                        Bottom = "12mm",
                        Left = "10mm"
                    }
                });

                return new HtmlToPdfResult
                {
                    PdfBytes = pdf,
                    SignatureAnchors = ConvertAnchorsToPdfCoordinates(domAnchors)
                };
            }
            catch (PlaywrightException ex)
            {
                throw new InvalidOperationException("HTML-to-PDF rendering failed. Ensure Playwright Chromium is installed for this environment.", ex);
            }
        }

        private static async Task<List<DomRect>> GetAnchorRectsAsync(IPage page, string selector, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var effectiveSelector = string.IsNullOrWhiteSpace(selector) ? ".sig-line" : selector.Trim();
            var escapedSelector = effectiveSelector.Replace("\\", "\\\\").Replace("'", "\\'");

            var rectsJson = await page.EvaluateAsync<string>($@"
(() => {{
  const nodes = Array.from(document.querySelectorAll('{escapedSelector}'));
  return JSON.stringify(nodes
    .map(node => {{
      const rect = node.getBoundingClientRect();
      return {{
        Left: rect.left + window.scrollX,
        Top: rect.top + window.scrollY,
        Width: rect.width,
        Height: rect.height
      }};
    }})
    .filter(x => x.Width > 0 && x.Height >= 0));
}})();
");

            if (string.IsNullOrWhiteSpace(rectsJson))
                return new List<DomRect>();

            var rects = JsonSerializer.Deserialize<List<DomRect>>(rectsJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return rects ?? new List<DomRect>();
        }

        private static List<PdfSignatureAnchor> ConvertAnchorsToPdfCoordinates(IEnumerable<DomRect> domAnchors)
        {
            var output = new List<PdfSignatureAnchor>();
            var contentHeightPt = PageHeightPt - MarginTopPt - MarginBottomPt;
            var contentHeightPx = contentHeightPt / PxToPt;
            var maxXPt = 595.28m - MarginRightPt;

            foreach (var anchor in domAnchors)
            {
                var topPx = (decimal)Math.Max(anchor.Top, 0d);
                var leftPx = (decimal)Math.Max(anchor.Left, 0d);
                var widthPx = (decimal)Math.Max(anchor.Width, 0d);
                var heightPx = (decimal)Math.Max(anchor.Height, 0d);

                var page = (int)Math.Floor(topPx / contentHeightPx) + 1;
                var withinPageTopPx = topPx % contentHeightPx;

                var widthPt = widthPx > 0 ? widthPx * PxToPt : 180m;
                var heightPt = heightPx > 0 ? Math.Max(heightPx * PxToPt, 30m) : 45m;

                var xPt = MarginLeftPt + (leftPx * PxToPt);
                var yPt = MarginTopPt + (withinPageTopPx * PxToPt);

                if (xPt + widthPt > maxXPt)
                    xPt = Math.Max(MarginLeftPt, maxXPt - widthPt);

                output.Add(new PdfSignatureAnchor
                {
                    Page = Math.Max(page, 1),
                    X = Math.Round(xPt, 2),
                    Y = Math.Round(yPt, 2),
                    Width = Math.Round(widthPt, 2),
                    Height = Math.Round(heightPt, 2)
                });
            }

            return output;
        }

        private static string EnsureHtmlDocument(string html)
        {
            var raw = string.IsNullOrWhiteSpace(html) ? "<div>Lease agreement</div>" : html;

            if (Regex.IsMatch(raw, "<html[\\s>]", RegexOptions.IgnoreCase))
                return raw;

            return """
<!doctype html>
<html>
<head>
  <meta charset="utf-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1" />
  <style>
    html, body { margin: 0; padding: 0; }
    * { -webkit-print-color-adjust: exact; print-color-adjust: exact; }
  </style>
</head>
<body>
""" + raw + """
</body>
</html>
""";
        }

        private static string BuildPdfBorderFixCss()
        {
            return """
@page {
  size: A4;
  margin: 12mm 10mm 12mm 10mm;
}

* {
  box-sizing: border-box;
  -webkit-print-color-adjust: exact !important;
  print-color-adjust: exact !important;
}

html, body {
  margin: 0 !important;
  padding: 0 !important;
  background: #ffffff !important;
}

body {
  overflow: hidden;
}

.page {
  margin: 0 !important;
  max-width: none !important;
  box-shadow: none !important;
  border: none !important;
}

.grid {
  border-collapse: collapse !important;
}

.grid td,
.grid th {
  border: 1px solid #d1d5db !important;
}

.section {
  page-break-inside: avoid;
}
""";
        }

        public sealed class DomRect
        {
            public double Left { get; set; }
            public double Top { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
        }
    }
}
