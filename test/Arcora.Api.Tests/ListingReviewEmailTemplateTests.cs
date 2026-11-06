using Arcora.Api;

namespace Arcora.Api.Tests;

public sealed class ListingReviewEmailTemplateTests
{
    [Fact]
    public void RejectionEmailEscapesReviewMessageAndUsesBrandedTemplate()
    {
        const string reviewMessage = "Remove <script>alert(1)</script> from the description.";

        var html = EmailTemplates.BuildListingReviewDecisionEmail(
            "Taylor Host",
            "Harbor View Suite",
            "REJECTED",
            reviewMessage,
            "Arcora",
            "support@example.test");

        Assert.Contains("Arcora", html);
        Assert.Contains("Harbor View Suite", html);
        Assert.Contains("Remove &lt;script&gt;alert(1)&lt;/script&gt; from the description.", html);
        Assert.DoesNotContain("<script>alert(1)</script>", html);
        Assert.DoesNotContain("{{", html);
    }

    [Fact]
    public void ApprovalEmailDoesNotIncludeRejectionMessage()
    {
        var html = EmailTemplates.BuildListingReviewDecisionEmail(
            "Taylor Host",
            "Harbor View Suite",
            "APPROVED",
            "This message should not appear.",
            "Arcora");

        Assert.Contains("Your listing is approved", html);
        Assert.DoesNotContain("This message should not appear.", html);
        Assert.DoesNotContain("{{", html);
    }
}