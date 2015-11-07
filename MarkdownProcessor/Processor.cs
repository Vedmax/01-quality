using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MarkdownProcessor
{
	public class Processor
	{
		public string Text;
		public Stack<Tag> Tags;

		public Dictionary<string, string> SignatureOfTags = new Dictionary<string, string>()
		{
			{ "`", "code"},
			{ "_", "em"},
			{ "__", "strong" }
		};

		public Processor(string inputText)
		{
			Text = inputText;
		}

		public string PlaceTags()
		{
			var tags = FindFontTags();
			var openTags = FilterByOpen(tags);
			foreach (var openTag in openTags)
			{
				Console.WriteLine(openTag.Type);
			}
			return null;
		}

		public IEnumerable<Tag> FilterByOpen(IEnumerable<Tag> tags)
		{
			var openTags = new List<Tag>();
			foreach (var tag in tags)
			{
				if (tag.Index == 0)
				{
					openTags.Add(tag);
					continue;
				}
				if (tag.Index + tag.Length >= Text.Length - 1)
					continue;
				if (char.IsWhiteSpace(Text[tag.Index - 1]) && !char.IsDigit(Text[tag.Index + tag.Length]))
					openTags.Add(tag);
			}
			return openTags;
		}

		public IEnumerable<Tag> FindFontTags()
		{
			MatchCollection matches = Regex.Matches(this.Text, "(__)|(_)|(`)");
			return matches
				.Cast<Match>()
				.Select(match => new Tag(match.Index, match.Value))
				.OrderByDescending(x => x.Length);
		}

	}
}
