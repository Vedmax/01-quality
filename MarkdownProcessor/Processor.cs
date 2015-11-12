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
		public string Text;
		private readonly int _depth;

		public Dictionary<string, string> SignatureOfTags = new Dictionary<string, string>()
		{
			{ "`", "code"},
			{ "_", "em"},
			{ "__", "strong" },
		};

		public Processor(string inputText, int depth)
		{
			Text = inputText;
			_depth = depth;
		}

		public string MarkText()
		{
			var paragraphs = Regex.Split(Text, "(\r\n[ ]*\r\n)");
			var result = new StringBuilder();
			foreach (var paragraph in paragraphs)
			{
				Text = paragraph;
				var tags = FindFontTags();
				if (!tags.Any())
				{ 
					if (Text[0] != '\r' && paragraphs.Length != 1)
						result.Append("<p>\n" + Text + "\n</p>\n");
					else
						result.Append(Text);
					continue;
				}
				if (_depth == 0 || paragraphs.Length != 1)
					result.Append("<p>\n" + GetMarkedText(tags) + "\n</p>\n");
				else
					result.Append(GetMarkedText(tags));
			}
			return result.ToString();
		}

		private string GetMarkedText(Queue<Tag> tags)
		{
			var lastIndex = tags.Peek().Index;
			var resultString = new StringBuilder(Text.Substring(0, lastIndex));
			while (tags.Count != 0)
			{
				lastIndex = tags.Peek().Index;
				var openTag = GetOpenTag(tags);
				if (openTag != null)
				{
					resultString.Append(GetMissingText(lastIndex, openTag));
					lastIndex = openTag.Index;
				}
				else
					return Text;
				var closeTag = GetPairTag(openTag, tags);
				if (closeTag != null)
				{
					resultString.Append(JoinTagsAndText(openTag, closeTag, GetSubstringBetweenTags(openTag, closeTag)));
					DeleteUsedTags(closeTag, tags);
					resultString.Append(GetTextAfterPosition(closeTag.Index + closeTag.Length, tags));
				}
				else
					resultString.Append(GetTextAfterPosition(openTag.Index, tags));
			}
			return resultString.ToString();
		}

		public string GetSubstringBetweenTags(Tag openTag, Tag closeTag)
		{
			var endOfOpenTag = openTag.Index + openTag.Length;
			var substr = Text.Substring(endOfOpenTag, closeTag.Index - endOfOpenTag);
			if (closeTag.Type != "`")
				substr = new Processor(substr, 1).MarkText();
			return substr;
		}

		public string GetMissingText(int lastIndex, Tag openTag)
		{
			return Text.Substring(lastIndex ,openTag.Index - lastIndex);
		}

		public string GetTextAfterPosition(int leftBorder, Queue<Tag> tags)
		{
			var rightBorder = Text.Length;
			if (tags.Count != 0)
			{
				rightBorder = tags.Peek().Index;
			}
			return Text.Substring(leftBorder, rightBorder - leftBorder);
		}

		public void DeleteUsedTags(Tag closeTag, Queue<Tag> tags)
		{
			while (tags.Count != 0 && tags.Peek().Index <= closeTag.Index)
				tags.Dequeue();
		}

		public string JoinTagsAndText(Tag openTag, Tag closeTag, string substring)
		{
			return "<" + SignatureOfTags[openTag.Type] + ">" + substring + "</" + SignatureOfTags[closeTag.Type] + ">";
		}

		public Tag GetPairTag(Tag openTag, Queue<Tag> tags)
		{
			try
			{
				return tags
					.ToList()
					.Where(x => x.Index != 0)
					.Where(x => Text[x.Index - 1] != '\\')
					.Where(x => x.Length + x.Index == Text.Length ||
						char.IsWhiteSpace(Text[x.Index + x.Length]) || char.IsPunctuation(Text[x.Index + x.Length]))
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
				if (tag.Index + tag.Length >= Text.Length - 1)
					continue;
				if (tag.Index == 0)
					return tag;
				if (!char.IsLetterOrDigit(Text[tag.Index - 1]) && Text[tag.Index - 1] != '\\')
					return tag;
			}
			return null;
		}

		public Queue<Tag> FindFontTags()
		{
			var matches = Regex.Matches(Text, 
				string.Concat(SignatureOfTags
				.Keys
				.OrderByDescending(x => x.Length)
				.Select(x => "(" + x + ")")
					.ToArray()).Replace(")(", ")|("));
			return new Queue<Tag>(matches
				.Cast<Match>()
				.Select(match => new Tag(match.Index, match.Value))
				.OrderBy(x => x.Index)
				.ThenByDescending(x => x.Length));
		}

		public string GetHtmlCode()
		{
			CoverExistedTags(Text);
			return "<!DOCTYPE html>\n" +
				   "<html>\n" +
				   "<head>\n" +
				   "<title>Vedmax</title>\n" +
				   "<meta charset='utf-8'>\n" +
				   "</head>\n" +
				   "<body>\n" +
				   MarkText() +
				   "</body>\n" +
				   "</html>";
		}

		public void CoverExistedTags(string text)
		{
			Text = Regex.Replace(text, "<", "&lt;");
			Text = Regex.Replace(Text, ">", "&gt;");
		}
	}
}
