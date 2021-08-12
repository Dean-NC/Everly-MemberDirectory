using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MemberDirectory.App.Api.Services
{
    /// <summary>
    /// Provides HTML parsing capability, like getting the text for Heading tags
    /// </summary>
    public class HtmlParser : Interfaces.IHtmlParser
    {
        // This class uses the HtmlAgilityPack open-source library to parse the HTML.
        // https://github.com/zzzprojects/html-agility-pack

        private readonly ILogger<HtmlParser> _logger;

        public HtmlParser(ILogger<HtmlParser> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets the text of all H1 through H3 tags
        /// </summary>
        /// <param name="htmlStream">A stream with the HTML content to parse</param>
        /// <returns>A list of strings for all H1 through H3 tags combined</returns>
        public ICollection<string> GetTextForHeadingTags(System.IO.Stream htmlStream)
        {
            List<string> result = new(10);

            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.Load(htmlStream);

                // Get H1 tags
                result.AddRange(
                    GetTextForTags(htmlDoc, "h1")
                );

                // Get H2 tags
                result.AddRange(
                    GetTextForTags(htmlDoc, "h2")
                );

                // Get H3 tags
                result.AddRange(
                    GetTextForTags(htmlDoc, "h3")
                );

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);
            }

            return result;
        }

        private static IList<string> GetTextForTags(HtmlDocument document, string tagType)
        {
            var tagItems = document.DocumentNode.Descendants(tagType);
            List<string> result = new();

            if (tagItems != null)
            {
                foreach (var tag in tagItems)
                {
                    string text = tag?.InnerText;
                    if (!string.IsNullOrWhiteSpace(tag?.InnerText))
                    {
                        result.Add(text);
                    }
                }
            }

            return result;
        }
    }
}
