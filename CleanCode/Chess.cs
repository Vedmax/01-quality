using System.IO;

namespace CleanCode
{
	public class Chess
	{
		private readonly Board board;
		public string Result;

		public Chess(StreamReader reader)
		{
			board = new Board(reader);
		}

		// Определяет мат, шах или пат белым.
		public string Solve()
		{
			var hasMoves = false;
			foreach (var from in board.GetPieces(PieceColor.White))
			{
				foreach (var to in board.Get(from).Piece.GetMoves(from, board))
				{
					var old = board.Get(to);
					if (HasMoves(to, @from, old))
						hasMoves = true;
				}
			}
			if (IsCheck())
				if (hasMoves)
					return "check";
				else return "mate";
			if (hasMoves) return "ok";
			return "stalemate";
		}

		private bool HasMoves(Location to, Location @from, Cell old)
		{
			var hasMoves = false;
			board.Set(to, board.Get(@from));
			board.Set(@from, Cell.Empty);
			if (!IsCheck())
				hasMoves = true;
			board.Set(@from, board.Get(to));
			board.Set(to, old);
			return hasMoves;
		}

		private bool IsCheck()
		{
			foreach (var loc in board.GetPieces(PieceColor.Black))
			{
				var cell = board.Get(loc);
				var moves = cell.Piece.GetMoves(loc, board);
				foreach (var to in moves)
				{
					if (board.Get(to).IsWhiteKing)
						return true;
				}
			}
			return false;
		}
	}
}