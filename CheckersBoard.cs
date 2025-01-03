using System.Collections.Generic;

namespace CheckersGameLogic
{
	internal class CheckersBoard
	{
		private Game m_Game;
		private readonly List<List<Disc>> r_DiscBoard; 
		public eBoardSize BoardSize { get; }
		public int BoardSizeInt
		{
			get
			{
				return (int)BoardSize;
			}
		}

		internal CheckersBoard(eBoardSize i_BoardSize, Game i_Game)
		{
			BoardSize = i_BoardSize;
			r_DiscBoard = new List<List<Disc>>();
			m_Game = i_Game;

			for (int row = 0; row < BoardSizeInt; row++)
			{
				List<Disc> rowInGrid = new List<Disc>(new Disc[BoardSizeInt]);
				r_DiscBoard.Add(rowInGrid);
			}
		}

		internal void InitializeBoardStartingState()
		{

			int rowsPerPlayer = (BoardSizeInt - 2) / 2;

			ClearBoard();

			// White discs (upper board) placement
			for (int row = 0; row < rowsPerPlayer; row++)
			{
				for (int col = (row + 1) % 2; col < BoardSizeInt; col += 2)
				{
					r_DiscBoard[row][col] = new Disc(eColor.White);
				}
			}

			// Black discs (lower board) placement
			for (int row = (BoardSizeInt - rowsPerPlayer); row < BoardSizeInt; row++)
			{
				for (int col = (row + 1) % 2; col < BoardSizeInt; col += 2)
				{
					r_DiscBoard[row][col] = new Disc(eColor.Black);
				}
			}
		}

		internal void ClearBoard()
		{
			int row = 0;
			int col = 0;

			for (row = 0; row < BoardSizeInt; row++)
			{
				for (col = 0; col < BoardSizeInt; col++)
				{
					r_DiscBoard[row][col] = null;
				}
			}
		}

		internal void GenerateMovesFromPosition(Position i_StartPos, Player i_Player, bool i_IsKing)
		{
			int[] rowOffsets;
			int[] colOffsets = new int[] { 1, -1 };

			if (i_IsKing)
			{
				rowOffsets = new int[] { 1, -1 }; //if king can go both up and down rows
			}
			else
			{
				// (O) Discs move down, in direction A-> ... F/H/J (depends on board size) 
				// (X) Discs move up, in direction F/H/J ... -> A
				rowOffsets = (i_Player.PlayerColor == eColor.White) ? new int[] { 1 } : new int[] { -1 };
			}

			foreach (int rowOff in rowOffsets)
			{
				foreach (int colOff in colOffsets)
				{
					Position regularEndPosition = new Position(i_StartPos.Row + rowOff, i_StartPos.Col + colOff);
					Position captureEndPosition = new Position(i_StartPos.Row + rowOff * 2, i_StartPos.Col + colOff * 2);
					Move ?regularMove = GetRegularMoveIfValidOrNull(i_StartPos, regularEndPosition, i_Player.PlayerColor);
					Move ?captureMove = GetCaptureMoveIfValidOrNull(i_StartPos, captureEndPosition, i_Player);

					if (regularMove.HasValue)
					{
						i_Player.m_PlayerListOfValidMoves.Add(regularMove.Value);
					}
					
					if (captureMove.HasValue)
					{
						i_Player.m_PlayerListOfValidMoves.Add(captureMove.Value);
					}
				}
			}
		}

		internal Move? GetRegularMoveIfValidOrNull(Position i_Start, Position i_End, eColor i_PlayerColor)
		{
			Move ?move = null;

			if (isMoveInBounds(i_Start,i_End) &&
				isEndPositionFree(i_End) &&
				isStartPositionDiscOwnedBySameColorPlayer(i_Start, i_PlayerColor))
			{
				move = new Move(i_Start, i_End, false);
			}

			return move;
		}

		internal Move? GetCaptureMoveIfValidOrNull(Position i_Start, Position i_End, Player i_Player)
		{
			Move? move = null;
			Position midPos = GetMidPositionForCapture(i_Start, i_End);

			if (isMoveInBounds(i_Start,i_End) &&
				isEndPositionFree(i_End) && 
				isStartPositionDiscOwnedBySameColorPlayer(i_Start, i_Player.PlayerColor) && 
				isOpponentDiscAt(midPos, i_Player.PlayerColor)) 
			{
				move = new Move(i_Start, i_End, true);
			}

			return move;
		}

		private bool isOpponentDiscAt(Position i_Pos, eColor i_CurrentPlayerColor)
		{
			Disc opponenetDisc = GetDiscFromPositionOrNullIfEmpty(i_Pos);

			return opponenetDisc != null && opponenetDisc.DiscColor != i_CurrentPlayerColor;
		}

		private bool isMoveInBounds(Position i_PosStart, Position i_PosEnd)
		{
			return isPositionInBounds(i_PosStart) && isPositionInBounds(i_PosEnd);
		}

		private bool isPositionInBounds(Position i_Position)
		{
			return i_Position.Row >= 0 && i_Position.Row < BoardSizeInt && i_Position.Col >= 0 && i_Position.Col < BoardSizeInt;
		}

		private bool isEndPositionFree(Position i_Position)
		{
			return r_DiscBoard[i_Position.Row][i_Position.Col] == null;
		}

		private bool isStartPositionDiscOwnedBySameColorPlayer(Position i_Position, eColor i_PlayerColor)
		{
			Disc disc = GetDiscFromPositionOrNullIfEmpty(i_Position);

			return disc != null && disc.DiscColor == i_PlayerColor;
		}

		internal Position GetMidPositionForCapture(Position i_StartPos, Position i_EndPos)
		{
			int midPosRow = (i_StartPos.Row + i_EndPos.Row) / 2;
			int midPosCol = (i_StartPos.Col + i_EndPos.Col) / 2;
			Position midPos = new Position(midPosRow, midPosCol);

			return midPos;
		}

		internal void ExecuteCapture(Position i_Start, Position i_End, Position i_CaptureDiscPosition)
		{
			r_DiscBoard[i_End.Row][i_End.Col] = r_DiscBoard[i_Start.Row][i_Start.Col];
			r_DiscBoard[i_Start.Row][i_Start.Col] = null;
			r_DiscBoard[i_CaptureDiscPosition.Row][i_CaptureDiscPosition.Col] = null;
		}

		internal void ExecuteRegularMove(Position i_Start, Position i_End)
		{
			r_DiscBoard[i_End.Row][i_End.Col] = r_DiscBoard[i_Start.Row][i_Start.Col];
			r_DiscBoard[i_Start.Row][i_Start.Col] = null;
		}

		internal Disc GetDiscFromPositionOrNullIfEmpty(Position i_Position)
		{
			return r_DiscBoard[i_Position.Row][i_Position.Col];
		}

		internal eCellState GetCellStateFromBoard(Position i_Pos)
		{
			Disc disc = r_DiscBoard[i_Pos.Row][i_Pos.Col];
			eCellState cellState = eCellState.Empty;

			if (disc != null)
			{
				if (disc.DiscType == eDiscType.King)
				{
					cellState = (disc.DiscColor == eColor.White) ? eCellState.WhiteKing : eCellState.BlackKing;
				}
				else if (disc.DiscType == eDiscType.Regular)
				{
					cellState = (disc.DiscColor == eColor.White) ? eCellState.WhiteRegular : eCellState.BlackRegular;
				}
			}

			return cellState;
		} 
		internal bool HasReachedEndRow(Position i_Position)
		{
			int promotionRow = m_Game.PlayerToMove.PlayerColor == eColor.White ? BoardSizeInt - 1 : 0;

			return i_Position.Row == promotionRow;
		}

		internal void ClearNonCaptureMovesFromMovesList(Player i_Player)
		{
			i_Player.m_PlayerListOfValidMoves.RemoveAll(move => !move.IsCapture);
		}

		internal bool IsMoveInBounds(Move i_Move)
		{
			return isPositionInBounds(i_Move.Start) && isPositionInBounds(i_Move.End);
		}
	}
}
