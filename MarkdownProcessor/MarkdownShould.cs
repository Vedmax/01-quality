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
			var proc = new Processor("_olol_");
			var tags = proc.FindFontTags();
			CollectionAssert.AreEquivalent(tags, new List<Tag>() { new Tag(0, "_"), new Tag(5, "_")});
		}

		[Test]
		public void GetStrongTags()
		{
			var proc = new Processor("__olol_");
			var tags = proc.FindFontTags();
			CollectionAssert.AreEquivalent(tags, new List<Tag>() { new Tag(0, "__"), new Tag(6, "_") });
		}

		[Test]
		public void GetCodeTags()
		{
			var proc = new Processor("`olol`");
			var tags = proc.FindFontTags();
			CollectionAssert.AreEquivalent(tags, new List<Tag>() { new Tag(0, "`"), new Tag(5, "`") });
		}

		[Test]
		public void JoinTagsAndText()
		{
			var proc = new Processor("");
			var res = proc.JoinTagsAndText(new Tag(0, "`"), new Tag(1, "`"), "o 'lo' lo");
			Assert.AreEqual(res, "<code>o 'lo' lo<\\code>");
		}

		[Test]
		public void GetOpenTag()
		{
			var proc = new Processor("_ololo_");
			var openTag = new Tag(0, "_");
			var tags = proc.FindFontTags();
			var res = proc.GetOpenTag(tags);
			Assert.AreEqual(res, openTag);
		}

		[Test]
		public void NotGetInsideTag()
		{
			var proc = new Processor("ol_olo");
			var openTag = new Tag(0, "_");
			var tags = proc.FindFontTags();
			var res = proc.GetOpenTag(tags);
			Assert.Null(res);
		}

		[Test]
		public void NotGetCloseTagInsteadOfOpen()
		{
			var proc = new Processor("ololo__");
			var tags = proc.FindFontTags();
			var res = proc.GetOpenTag(tags);
			Assert.Null(res);
		}

		[Test]
		public void GetTextAfterPositionToEnd()
		{
			var proc = new Processor("o _op_ kek");
			var res = proc.GetTextAfterPosition(6, new Queue<Tag>());
			Assert.AreEqual(res, " kek");
		}

		[Test]
		public void GetTextAfterPositionToNextTag()
		{
			var proc = new Processor("o _op_ kek __lol__");
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

		[Test]
		public void NotMarkOnlyOpenTags()
		{
			var proc = new Processor("__o _op `kek");
			var res = proc.MarkText();
			Assert.AreEqual(res, "__o _op `kek");
		}

		[Test]
		public void NotMarkTagsInsideWord()
		{
			var proc = new Processor("keks_peks_shmeks");
			var res = proc.MarkText();
			Assert.AreEqual(res, "keks_peks_shmeks");
		}

		[Test]
		public void GetMissingText()
		{
			var proc = new Processor("kek _pi_");
			var tags = proc.FindFontTags();
			var openTag = proc.GetOpenTag(tags);
			var lastIndex = 0;
			var res = proc.GetMissingText(lastIndex, openTag);
			Assert.AreEqual(res, "kek ");
		}

		[Test]
		public void GetPairTag()
		{
			var proc = new Processor("kek _pi_");
			var tags = proc.FindFontTags();
			var openTag = proc.GetOpenTag(tags);
			var res = proc.GetPairTag(openTag, tags);
			Assert.AreEqual(res, new Tag(7, "_"));
		}

		[Test]
		public void GetNullThenDoesntExistPairTag()
		{
			var proc = new Processor("kek _pi");
			var tags = proc.FindFontTags();
			var openTag = proc.GetOpenTag(tags);
			var res = proc.GetPairTag(openTag, tags);
			Assert.Null(res);
		}
	}
}
