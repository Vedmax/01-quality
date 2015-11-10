using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
			var tags = FindFontTags();
			if (!tags.Any())
				return _text;
			return GetMarkedText(tags);
		}

		private string GetMarkedText(Queue<Tag> tags)
		{
			var lastIndex = tags.Peek().Index;
			var resultString = new StringBuilder(_text.Substring(0, lastIndex));
			while (tags.Count != 0)
			{
				lastIndex = tags.Peek().Index;
				var openTag = GetOpenTag(tags);
				if (openTag == null)
					return _text;
				else
				{
					resultString.Append(GetMissingText(lastIndex, openTag));
					lastIndex = openTag.Index;
				}
				var closeTag = GetPairTag(openTag, tags);
				if (closeTag == null)
				{
					resultString.Append(GetTextAfterPosition(openTag.Index, tags));
					continue;
				}
				var endOfOpenTag = openTag.Index + openTag.Length;
				var substr = _text.Substring(endOfOpenTag, closeTag.Index - endOfOpenTag);
				if (closeTag.Type != "`")
					substr = new Processor(substr).MarkText();
				resultString.Append(JoinTagsAndText(openTag, closeTag, substr));
				DeleteUsedTags(closeTag, tags);
				resultString.Append(GetTextAfterPosition(closeTag.Index + closeTag.Length, tags));
			}
			return resultString.ToString();
		}

		public string GetMissingText(int lastIndex, Tag openTag)
		{
			return _text.Substring(lastIndex ,openTag.Index - lastIndex);
		}

		public string GetTextAfterPosition(int leftBorder, Queue<Tag> tags)
		{
			var rightBorder = _text.Length;
			if (tags.Count != 0)
			{
				rightBorder = tags.Peek().Index;
			}
			return _text.Substring(leftBorder, rightBorder - leftBorder);
		}

		public void DeleteUsedTags(Tag closeTag, Queue<Tag> tags)
		{
			while (tags.Count != 0 && tags.Peek().Index <= closeTag.Index)
				tags.Dequeue();
		}

		public string JoinTagsAndText(Tag openTag, Tag closeTag, string substring)
		{
			return "<" + SignatureOfTags[openTag.Type] + ">" + substring + "<\\" + SignatureOfTags[closeTag.Type] + ">";
		}

		public Tag GetPairTag(Tag openTag, Queue<Tag> tags)
		{
			try
			{
				return tags
					.ToList()
					.Where(x => x.Index != 0)
					.Where(x => _text[x.Index - 1] != '\\')
					.Where(x => x.Length + x.Index == _text.Length ||
						char.IsWhiteSpace(_text[x.Index + x.Length]) || char.IsPunctuation(_text[x.Index + x.Length]))
					.First(x => x.Index > openTag.Index && string.Equals(x.Type, openTag.Type));
			}
			catch (Exception)
			{
				return null;
			}
		}

		public Tag GetOpenTag(Queue<Tag> tags)
		{
			while (tags.Count != 0)
			{
				var tag = tags.Dequeue();
				if (tag.Index + tag.Length >= _text.Length - 1)
					continue;
				if (tag.Index == 0)
					return tag;
				if (!char.IsLetterOrDigit(_text[tag.Index - 1]) && _text[tag.Index - 1] != '\\')
					return tag;
			}
			return null;
		}

		public Queue<Tag> FindFontTags()
		{
			var matches = Regex.Matches(_text, "(__)|(_)|(`)");
			return new Queue<Tag>(matches
				.Cast<Match>()
				.Select(match => new Tag(match.Index, match.Value))
				.OrderBy(x => x.Index)
				.ThenByDescending(x => x.Length));
		}
	}
}
