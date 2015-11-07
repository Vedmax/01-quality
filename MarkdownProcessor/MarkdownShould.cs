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
	}
}
