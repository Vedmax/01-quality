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
				File.ReadAllText(@"sample.md", Encoding.UTF8);
			var proc = new Processor(text, 0);
			var code = proc.GetHtmlCode();
			Console.WriteLine(code);
			//File.WriteAllText(@"..\..\index.html", code, Encoding.UTF8);
		}
	}
}
