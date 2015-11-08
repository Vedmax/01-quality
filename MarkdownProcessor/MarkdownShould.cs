using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;


namespace MarkdownProcessor
{
	class MarkdownShould
	{
		[Test]
		public void FindOpenTag()
		{
			var proc = new Processor("ol _ol_");
			var tags = proc.FindFontTags();
			var openTags = proc.FilterByOpen(tags);
			CollectionAssert.AreEquivalent(new List<Tag>() { new Tag(3, "_") }, openTags);
		}

		[Test]
		public void FindOpenTagInBegining()
		{
			var proc = new Processor("_olol_");
			var tags = proc.FindFontTags();
			var openTags = proc.FilterByOpen(tags);
			CollectionAssert.AreEquivalent(new List<Tag>() {new Tag(0, "_")}, openTags);
		}

		[Test]
		public void FindOpenMultipleTag()
		{
			var proc = new Processor("__ol _ol_");
			var tags = proc.FindFontTags();
			var openTags = proc.FilterByOpen(tags);
			CollectionAssert.AreEquivalent(new List<Tag>() { new Tag(5, "_"), new Tag(0, "__") }, openTags);
		}

		[Test]
		public void NotFindShadeOpenTag()
		{
			var proc = new Processor("\\_olol_");
			var tags = proc.FindFontTags();
			var openTags = proc.FilterByOpen(tags);
			CollectionAssert.IsEmpty(openTags);
		}

		[Test]
		public void FindCloseTag()
		{
			var proc = new Processor("_ol_ ol");
			var tags = proc.FindFontTags();
			var openTags = proc.FilterByClosed(tags);
			CollectionAssert.AreEquivalent(new List<Tag>() { new Tag(3, "_") }, openTags);
		}

		[Test]
		public void FindCloseTagInTheEnd()
		{
			var proc = new Processor("olol_");
			var tags = proc.FindFontTags();
			var openTags = proc.FilterByClosed(tags);
			CollectionAssert.AreEquivalent(new List<Tag>() { new Tag(4, "_") }, openTags);
		}

		[Test]
		public void FindCloseMultipleTag()
		{
			var proc = new Processor("ol_ ol__");
			var tags = proc.FindFontTags();
			var openTags = proc.FilterByClosed(tags);
			CollectionAssert.AreEquivalent(new List<Tag>() { new Tag(2, "_"), new Tag(6, "__") }, openTags);
		}

		[Test]
		public void NotFindShadeCloseTag()
		{
			var proc = new Processor("olol\\_");
			var tags = proc.FindFontTags();
			var closedTags = proc.FilterByClosed(tags);
			CollectionAssert.IsEmpty(closedTags);
		}

		[Test]
		public void GetRightPairTag()
		{
			var proc = new Processor("_o__lol_");
			var tags = proc.FindFontTags();
			var closedTags = proc.FilterByClosed(tags);
			var pairTag = proc.GetPairTag(new Tag(0, "_"), closedTags);
			Assert.AreEqual(pairTag, new Tag(7, "_"));
		}

		[Test]
		public void ReturnNullIfPairTagDoesntExist()
		{
			var proc = new Processor("_o __lol__");
			var tags = proc.FindFontTags();
			var closedTags = proc.FilterByClosed(tags);
			var pairTag = proc.GetPairTag(new Tag(0, "_"), closedTags);
			Assert.Null(pairTag);
		}

		[Test]
		public void JoinTagsAndText()
		{
			var proc = new Processor("");
			var res = proc.JoinTagsAndText(new Tag(0, "`"), new Tag(1, "`"), "ololo");
			Assert.AreEqual(res, "<code>ololo<\\code>");
		}

		[Test]
		public void GetTextAfterPositionToEnd()
		{
			var proc = new Processor("o _op_ kek");
			var res = proc.GetTextAfterPosition(6, new Queue<Tag>());
			Assert.AreEqual(res, " kek");
		}
		
		[Test]
		public void GetTextAfterPositionToNextOpenTag()
		{
			var proc = new Processor("_op_ kek _op_");
			var openTags = proc.FilterByOpen(proc.FindFontTags().ToArray());
			openTags.Dequeue();
			var res = proc.GetTextAfterPosition(4, openTags);
			Assert.AreEqual(res, " kek ");
		}

		[Test]
		public void NotMarkTagsInsideCodeTag()
		{
			var proc = new Processor("`o _op_ kek`");
			var res = proc.MarkText();
			Assert.AreEqual(res, "<code>o _op_ kek<\\code>");
		}

		[Test]
		public void MarkTagsInsideOverTags()
		{
			var proc = new Processor("__o _op_ kek__");
			var res = proc.MarkText();
			Assert.AreEqual(res, "<strong>o <em>op<\\em> kek<\\strong>");
		}
	}
}
