using EPiServer.Core;

namespace Geta.oEmbed.Sample.Models.Pages
{
    public interface IHasRelatedContent
    {
        ContentArea RelatedContentArea { get; }
    }
}
