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
			var tags = FindFontTags().ToArray();
			var openTags = FilterByOpen(tags);
			var closeTags = FilterByClosed(tags);
			return null;
		}

		public IEnumerable<Tag> FilterByClosed(IEnumerable<Tag> tags)
		{
			return tags
				.Where(tag => tag.Index != 0)
				.Where(tag => char.IsLetter(Text[tag.Index - 1]))
				.Where(tag => tag.Length + tag.Index == Text.Length ||
					char.IsWhiteSpace(Text[tag.Index + tag.Length]))
				.ToList();
		}

		public IEnumerable<Tag> FilterByOpen(IEnumerable<Tag> tags)
		{
			var openTags = new List<Tag>();
			foreach (var tag in tags.Where(tag => tag.Index + tag.Length < Text.Length - 1))
			{
				if (tag.Index == 0)
				{
					openTags.Add(tag);
					continue;
				}
				if (char.IsWhiteSpace(Text[tag.Index - 1]) && !char.IsDigit(Text[tag.Index + tag.Length]))
					openTags.Add(tag);
			}
			return openTags;
		}

		public IEnumerable<Tag> FindFontTags()
		{
			var matches = Regex.Matches(Text, "(__)|(_)|(`)");
			return matches
				.Cast<Match>()
				.Select(match => new Tag(match.Index, match.Value))
				.OrderByDescending(x => x.Length);
		}

	}
}
