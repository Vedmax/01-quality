using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MarkdownProcessor
{
	public class Processor
	{
		private readonly string _text;
		public Stack<Tag> Tags;

		public Dictionary<string, string> SignatureOfTags = new Dictionary<string, string>()
		{
			{ "`", "code"},
			{ "_", "em"},
			{ "__", "strong" }
		};

		public Processor(string inputText)
		{
			_text = inputText;
		}

		public string MarkText()
		{
			var tags = FindFontTags().ToArray();
			var openTags = FilterByOpen(tags);
			var closeTags = FilterByClosed(tags);
			if (openTags.Count()*closeTags.Count() == 0)
				return _text;
			return GetMarkedText(openTags, closeTags);
		}

		private string GetMarkedText(Queue<Tag> openTags, Queue<Tag> closeTags)
		{
			var resultString = new StringBuilder(_text.Substring(0, openTags.Peek().Index));
			while (openTags.Count != 0)
			{
				var openTag = openTags.Dequeue();
				var closeTag = GetPairTag(openTag, closeTags);
				if (closeTag == null)
				{
					resultString.Append(GetTextAfterPosition(openTag.Index + openTag.Length, openTags));
					continue;
				}
				var endOfOpenTag = openTag.Index + openTag.Length;
				var substr = _text.Substring(endOfOpenTag, closeTag.Index - endOfOpenTag);
				if (closeTag.Type != "`")
					substr = new Processor(substr).MarkText();
				resultString.Append(JoinTagsAndText(openTag, closeTag, substr));
				DeleteUsedTags(closeTag, openTags, closeTags);
				resultString.Append(GetTextAfterPosition(closeTag.Index + closeTag.Length, openTags));
			}
			return resultString.ToString();
		}

		public string GetTextAfterPosition(int leftBorder, Queue<Tag> openTags)
		{
			var rightBorder = _text.Length;
			if (openTags.Count != 0)
			{
				rightBorder = openTags.Peek().Index;
			}
			return _text.Substring(leftBorder, rightBorder - leftBorder);
		}

		public void DeleteUsedTags(Tag closeTag, Queue<Tag> openTags, Queue<Tag> closeTags)
		{
			while (openTags.Count != 0 && openTags.Peek().Index <= closeTag.Index)
				openTags.Dequeue();
			while (closeTags.Count != 0 && closeTags.Peek().Index <= closeTag.Index)
				closeTags.Dequeue();
		}

		public string JoinTagsAndText(Tag openTag, Tag closeTag, string substring)
		{
			return "<" + SignatureOfTags[openTag.Type] + ">" + substring + "<\\" + SignatureOfTags[closeTag.Type] + ">";
		}

		public Tag GetPairTag(Tag openTag, Queue<Tag> closeTags)
		{
			try
			{
				return closeTags
					.ToList()
					.First(x => x.Index > openTag.Index && string.Equals(x.Type, openTag.Type));
			}
			catch (Exception)
			{
				return null;
			}
		}

		public Queue<Tag> FilterByClosed(IEnumerable<Tag> tags)
		{
			return new Queue<Tag>(tags
				.Where(tag => tag.Index != 0)
				.Where(tag => char.IsLetter(_text[tag.Index - 1]))
				.Where(tag => tag.Length + tag.Index == _text.Length ||
					char.IsWhiteSpace(_text[tag.Index + tag.Length]))
				.ToList());
		}

		public Queue<Tag> FilterByOpen(IEnumerable<Tag> tags)
		{
			var openTags = new Queue<Tag>();
			foreach (var tag in tags.Where(tag => tag.Index + tag.Length < _text.Length - 1))
			{
				if (tag.Index == 0)
				{
					openTags.Enqueue(tag);
					continue;
				}
				if (char.IsWhiteSpace(_text[tag.Index - 1]) && !char.IsDigit(_text[tag.Index + tag.Length]))
					openTags.Enqueue(tag);
			}
			return openTags;
		}

		public IEnumerable<Tag> FindFontTags()
		{
			var matches = Regex.Matches(_text, "(__)|(_)|(`)");
			return matches
				.Cast<Match>()
				.Select(match => new Tag(match.Index, match.Value))
				.OrderBy(x => x.Index)
				.ThenByDescending(x => x.Length);
		}
	}
}
