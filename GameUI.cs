using System;
using System.Collections.Generic;
using System.Text;

using CheckersGameLogic;
using Ex02.ConsoleUtils;

namespace FlowAndUI
{
	public class GameUI
	{
		
		private readonly int r_RowStringLength;
		private readonly int r_WidthPerColumn = 5; // hardcoded for column width.
		private readonly string r_RowSeparatorString;
		internal readonly Dictionary<eCellState, char> r_CellStateDictionary;
		public Game m_Game;
		public PlayerInteraction m_PlayerInteraction;

		public int BoardSize { get; }

		public GameUI(Game i_Game)
		{
			m_Game = i_Game;
			BoardSize = m_Game.BoardSizeInt;
			m_PlayerInteraction = new PlayerInteraction(this, i_Game);
			r_RowSeparatorString = m_PlayerInteraction.PrepareRowSeparator();
			r_CellStateDictionary = generateECellStateCharMapping();
			r_RowStringLength = BoardSize * r_WidthPerColumn;
		}

		public void StartGameSession()
		{
			string validInputOrQuitRequest = string.Empty;
			string[] startAndEndPosAsStrings;
			bool isSessionRunning = true;
			bool isUserResigned = false;

			while (isSessionRunning)
			{
				
				while (m_Game.IsGameRunning)
				{
					printBoardState(m_Game.GetBoardStateSnapshot());

					if (m_Game.ErrorCode.HasValue)
					{
						Console.WriteLine(m_PlayerInteraction.GetErrorMessage(m_Game.ErrorCode.Value));
					}
					else
					{
						printMove(m_Game.LastMovePlayed, m_Game.PlayerWhoPlayedLastName);
					}

					if (m_Game.PlayerToMoveType == ePlayerType.Human)
					{
						validInputOrQuitRequest = m_PlayerInteraction.GetValidInputFormatOrResignation(out isUserResigned);

						if (isUserResigned)
						{
							m_Game.SetWinnerOnResignation();
							break;
						}
						else
						{
							startAndEndPosAsStrings = validInputOrQuitRequest.Split('>');
							Move moveObj = parseStringToMove(startAndEndPosAsStrings[0], startAndEndPosAsStrings[1]);
							m_Game.IsValidMove(moveObj);

							if (m_Game.ErrorCode.HasValue)
							{
								m_PlayerInteraction.GetErrorMessage(m_Game.ErrorCode.Value);
							}
						}
					}
					else // Computer move block
					{
						Console.Write("Please press any key other than Q to generate cpu move");
						ConsoleKey consoleKey = Console.ReadKey().Key;
						m_Game.ExecuteComputerMove();
					}
				} // end of while GameIsOver loop

				printBoardState(m_Game.GetBoardStateSnapshot());

				if (m_Game.GameWinner != null)
				{
					m_Game.CalculatePoints(m_Game.PlayerToMove);
				}
				else
				{
					Console.WriteLine("It's a tie in this round. No points added to any player.");
				}

				m_PlayerInteraction.PrintSessionScoreBoard();
				Console.Write("Do you want to start a new game? enter (1) for Yes or any other key to end session: ");
				ConsoleKey wantsNewRound = Console.ReadKey().Key;

				if (wantsNewRound == ConsoleKey.D1)
				{
					m_Game.StartNewGame();
				}
				else
				{
					isSessionRunning = false;
				}
			}
		}
			
		private void printBoardState(eCellState[,] i_BoardState)
		{
			int rows = i_BoardState.GetLength(0);
			int cols = i_BoardState.GetLength(1);
			eCellState [] eCellsRow = new eCellState[cols];

			Screen.Clear(); // The reference file received from Guy Ronen
			printColumnsHeader(BoardSize);

			for (int r = 0; r < rows; r++)
			{
				printRowSeparator();
				eCellsRow = GetRow(i_BoardState, r);
				printRowState(eCellsRow, r, cols);			
			}
			printRowSeparator();
			Console.WriteLine();
		}
		private void printRowState(eCellState[] i_RowState, int i_RowIndex, int i_NumColumns)
		{
			StringBuilder rowStringState = new StringBuilder(r_RowStringLength);

			rowStringState.Append($"{(char)('A' + i_RowIndex)}|");

			for (int col = 0; col < i_NumColumns; col++)
			{ 
				rowStringState.Append($" {r_CellStateDictionary[i_RowState[col]]} |");
			}

			rowStringState.Append(Environment.NewLine);
			Console.Write(rowStringState.ToString());
		}
		private void printColumnsHeader(int i_BoardSize)
		{
			Console.Write("  "); 

			for (int c = 0; c < i_BoardSize; c++)
			{
				Console.Write($" {(char)('a' + c)}");
				Console.Write("  ");
			}

			Console.WriteLine();
		}

		private void printRowSeparator()
		{
			Console.Write(r_RowSeparatorString);
		}

		private Dictionary<eCellState,char> generateECellStateCharMapping()
		{
			Dictionary<eCellState, char> cellMapping = new Dictionary<eCellState, char>
			{
				{ eCellState.Empty, ' ' },
				{ eCellState.WhiteRegular, 'O' },
				{ eCellState.WhiteKing, 'U' },
				{ eCellState.BlackRegular, 'X' },
				{ eCellState.BlackKing, 'K' }
			};

			return cellMapping;
		}

		public eCellState[] GetRow(eCellState[,] board, int rowIndex)
		{
			int columns = board.GetLength(1);
			eCellState[] row = new eCellState[columns];

			for (int col = 0; col < columns; col++)
			{
				row[col] = board[rowIndex, col];
			}

			return row;
		}

		private Move parseStringToMove(string i_StartPos, string i_EndPos)
		{
			Position startPos = generatePosition(i_StartPos);
			Position endPos = generatePosition(i_EndPos);
			bool isPotentialCapture = (Math.Abs(startPos.Row - endPos.Row) == 2 && (Math.Abs(startPos.Col-endPos.Col) == 2));
			Move move = new Move(startPos, endPos, isPotentialCapture);

			return move;
		}

		private Position generatePosition(string i_PositionString)
		{
			Position pos = new Position(i_PositionString[0] - 'A', i_PositionString[1] -'a');

			return pos;
		}

		private void printMove(Move ?i_Move, string i_PlayerName)
		{
			if (i_Move.HasValue)
			{
				Console.WriteLine(string.Format("{0}'s last move was ({1}): {2}{3}",
					i_PlayerName,
					m_PlayerInteraction.GetDiscSymbolToMove(m_Game.PlayerWhoPlayedLast.PlayerColor),
					parseMoveToString(i_Move.Value),
					Environment.NewLine));
			}
		}

		private string parseMoveToString(Move i_Move)
		{
			string moveString = $"{(char)(i_Move.Start.Row + 'A')}{(char)(i_Move.Start.Col + 'a')}>{(char)(i_Move.End.Row + 'A')}{(char)(i_Move.End.Col + 'a')}";

			return moveString;
		}

		internal bool UserWantsToQuit(string i_Input)
		{
			return i_Input.Length == 1 && i_Input[0] == 'Q';
		}
	}
}
