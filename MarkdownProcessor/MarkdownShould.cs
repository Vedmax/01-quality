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
		public void FindOpenTagInBegining()
		{
			var proc = new Processor("_olol_");
			var tags = proc.FindFontTags();
			var openTags = proc.FilterByOpen(tags);
			CollectionAssert.AreEquivalent(new List<Tag>() {new Tag(0, "_")}, openTags);
		}

		[Test]
		public void NotFindShadeOpenTag()
		{
			var proc = new Processor("\\_olol_");
			var tags = proc.FindFontTags();
			var openTags = proc.FilterByOpen(tags);
			CollectionAssert.IsEmpty(openTags);
		}
	}
}
