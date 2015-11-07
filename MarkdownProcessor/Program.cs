using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownProcessor
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var text =
				File.ReadAllText("C:\\Users\\Max\\Documents\\Visual Studio 2012\\Projects\\01-quality\\Markdown\\sample.md");
			var proc = new Processor(text);
			var stt = proc.PlaceTags();
		}
	}
}
