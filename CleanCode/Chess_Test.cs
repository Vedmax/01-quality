using System;
using System.IO;
using NUnit.Framework;

namespace CleanCode
{
	[TestFixture]
	public class Chess_Test
	{
		[Test]
		public void Test()
		{
            int testsCount = 0;
			foreach (var file in Directory.GetFiles("ChessTests"))
			{
				if (Path.GetExtension(file) != string.Empty) continue;
				using (var f = File.OpenText(file))
				{
					var chess = new Chess(f);
				    Console.WriteLine("Loaded " + file);
				    var expectedAnswer = File.ReadAllText(file + ".ans").Trim();
                    chess.Solve();
                    Assert.AreEqual(expectedAnswer, chess.Solve(), "error in file " + file);
				}
				testsCount++;
			}
			Console.WriteLine("Tests count: " + testsCount);
		}
	}
}