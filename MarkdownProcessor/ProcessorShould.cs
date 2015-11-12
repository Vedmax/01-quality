using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;


namespace MarkdownProcessor
{
	class ProcessorShould
	{
		[Test]
		public void GetEmTags()
		{
			var proc = new Processor("_olol_", 0);
			var tags = proc.FindFontTags();
			CollectionAssert.AreEquivalent(tags, new List<Tag>() { new Tag(0, "_"), new Tag(5, "_")});
		}

		[Test]
		public void GetStrongTags()
		{
			var proc = new Processor("__olol_", 0);
			var tags = proc.FindFontTags();
			CollectionAssert.AreEquivalent(tags, new List<Tag>() { new Tag(0, "__"), new Tag(6, "_") });
		}

		[Test]
		public void GetCodeTags()
		{
			var proc = new Processor("`olol`", 0);
			var tags = proc.FindFontTags();
			CollectionAssert.AreEquivalent(tags, new List<Tag>() { new Tag(0, "`"), new Tag(5, "`") });
		}

		[Test]
		public void JoinTagsAndText()
		{
			var proc = new Processor("", 1);
			var res = proc.JoinTagsAndText(new Tag(0, "`"), new Tag(10, "`"), "o 'lo' lo");
			Assert.AreEqual(res, "<code>o 'lo' lo</code>");
		}

		[Test]
		public void GetOpenTag()
		{
			var proc = new Processor("_ololo_", 0);
			var openTag = new Tag(0, "_");
			var tags = proc.FindFontTags();
			var res = proc.GetOpenTag(tags);
			Assert.AreEqual(res, openTag);
		}

		[Test]
		public void NotGetInsideTag()
		{
			var proc = new Processor("ol_olo", 1);
			var openTag = new Tag(0, "_");
			var tags = proc.FindFontTags();
			var res = proc.GetOpenTag(tags);
			Assert.Null(res);
		}

		[Test]
		public void NotGetCloseTagInsteadOfOpen()
		{
			var proc = new Processor("ololo__", 1);
			var tags = proc.FindFontTags();
			var res = proc.GetOpenTag(tags);
			Assert.Null(res);
		}

		[Test]
		public void GetTextAfterPositionToEnd()
		{
			var proc = new Processor("o _op_ kek", 1);
			var res = proc.GetTextAfterPosition(6, new Queue<Tag>());
			Assert.AreEqual(res, " kek");
		}

		[Test]
		public void GetTextAfterPositionToNextTag()
		{
			var proc = new Processor("o _op_ kek __lol__", 1);
			var tags = proc.FindFontTags();
			var openTag = tags.Dequeue();
			proc.GetPairTag(openTag, tags);
			tags.Dequeue();
			var res = proc.GetTextAfterPosition(6, tags);
			Assert.AreEqual(res, " kek ");
		}
		

		[Test]
		public void NotMarkTagsInsideCodeTag()
		{
			var proc = new Processor("`o _op_ kek`", 0);
			var res = proc.GetSubstringBetweenTags(new Tag(0, "`"), new Tag(11, "`"));
			Assert.AreEqual(res, "o _op_ kek");
		}

		[Test]
		public void MarkTagsInsideOverTags()
		{
			var proc = new Processor("__o _op_ kek__", 0);
			var res = proc.MarkText();
			Assert.AreEqual(res, "<p>\n<strong>o <em>op</em> kek</strong>\n</p>\n");
		}

		[Test]
		public void PutParagraphsInsideOverTags()
		{
			var proc = new Processor("__o _op_ `kek`__", 0);
			var res = proc.MarkText();
			Assert.AreEqual(res, "<p>\n<strong>o <em>op</em> <code>kek</code></strong>\n</p>\n");
		}

		[Test]
		public void NotMarkOnlyOpenTags()
		{
			var proc = new Processor("__o _op `kek", 0);
			var res = proc.MarkText();
			Assert.AreEqual(res, "<p>\n__o _op `kek\n</p>\n");
		}

		[Test]
		public void NotMarkTagsInsideWord()
		{
			var proc = new Processor("keks_peks_shmeks", 0);
			var res = proc.MarkText();
			Assert.AreEqual(res, "<p>\nkeks_peks_shmeks\n</p>\n");
		}

		[Test]
		public void GetMissingText()
		{
			var proc = new Processor("kek _pi_", 1);
			var tags = proc.FindFontTags();
			var openTag = proc.GetOpenTag(tags);
			var lastIndex = 0;
			var res = proc.GetMissingText(lastIndex, openTag);
			Assert.AreEqual(res, "kek ");
		}

		[Test]
		public void GetPairTag()
		{
			var proc = new Processor("kek _pi_", 1);
			var tags = proc.FindFontTags();
			var openTag = proc.GetOpenTag(tags);
			var res = proc.GetPairTag(openTag, tags);
			Assert.AreEqual(res, new Tag(7, "_"));
		}

		[Test]
		public void GetNullThenDoesntExistPairTag()
		{
			var proc = new Processor("kek _pi", 1);
			var tags = proc.FindFontTags();
			var openTag = proc.GetOpenTag(tags);
			var res = proc.GetPairTag(openTag, tags);
			Assert.Null(res);
		}

		[Test]
		public void CoverExistedTags()
		{
			var proc = new Processor("<code>", 0);
			proc.CoverExistedTags(proc.Text);
			Assert.AreEqual(proc.Text, "&lt;code&gt;");
		}
	}
}
