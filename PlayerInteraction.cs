using System;
using System.Text;
using System.Linq;

using CheckersGameLogic;

namespace FlowAndUI
{
	public class PlayerInteraction
	{
		internal GameUI m_GameUI;
		internal Game m_Game;

		public PlayerInteraction(GameUI i_GameUI, Game i_Game)
		{
			m_GameUI = i_GameUI;
			m_Game = i_Game;
		}
		
		internal static string GetPlayerName()
		{
			string playerNameInput = string.Empty;
			bool isValidName = false;

			while (!isValidName)
			{
				Console.Write("Please Enter player name up to length 20 and with no spaces: ");
				playerNameInput = Console.ReadLine();
				isValidName = IsValidPlayerName(playerNameInput);
			}

			return playerNameInput;
		}

		internal static bool IsValidPlayerName(string i_Input)
		{
			bool isValid = true;

			isValid = !string.IsNullOrEmpty(i_Input) && !i_Input.Any(char.IsWhiteSpace) && Game.IsValidPlayerNameLength(i_Input);

			if (!isValid)
			{
				Console.WriteLine("Invalid Input Please try again");
			}

			return isValid;
		}

		internal static eBoardSize RequestBoardSizeInput()
		{
			string inputString = string.Empty;

			while (true)
			{
				Console.Write("Please enter board size - 6, 8 or 10: ");
				inputString = Console.ReadLine();
				if (int.TryParse(inputString, out int o_BoardSize) && Game.IsValidBoardSize(o_BoardSize))
				{
					return (eBoardSize)o_BoardSize;
				}
				else
				{
					Console.WriteLine("Invalid Input, please Try again.");
				}
			}
		}

		internal static ePlayerType RequestGameMode()
		{
			string userInput = string.Empty;
			ePlayerType gameModeDecided;

			while (true)
			{
				Console.Write("Please enter game mode (1) for Player vs Computer or (2) for Player vs Player: ");
				userInput = Console.ReadLine();

				if (int.TryParse(userInput, out int parseResult) && Game.IsValidGameMode((ePlayerType)parseResult))
				{
					gameModeDecided = (ePlayerType)parseResult;
					break;
				}

				Console.WriteLine("Invalid Input, Please try again.");
			}
			
			return gameModeDecided;
		}

		internal void PrintCurrentTotalScores()
		{
			Console.WriteLine(string.Format("Player One score = {0}", m_GameUI.m_Game.PlayerOneTotalScore));
			Console.WriteLine(string.Format("Player Two score = {0}", m_GameUI.m_Game.PlayerTwoTotalScore));
		}

		internal string GetValidInputFormatOrResignation(out bool o_UserResigned)
		{
			o_UserResigned = false;
			string validatedMoveString = String.Empty;
			string userInput = String.Empty;

			Console.WriteLine(string.Format("{0}'s Turn ({1}):", 
				m_GameUI.m_Game.PlayerToMoveName, 
				GetDiscSymbolToMove(m_Game.PlayerToMove.PlayerColor)));
			PrintMoveOrQuitRequest();
			userInput = Console.ReadLine();

			while (true)
			{
				o_UserResigned = m_GameUI.UserWantsToQuit(userInput);

				if (o_UserResigned)
				{
					break; 
				}

				if (ValidateMoveFormat(userInput))
				{
					validatedMoveString = userInput; 
					break; 
				}

				Console.WriteLine("Invalid Move format.");
				Console.Write("Please enter move in format ROWcol>ROWcol: ");
				userInput = Console.ReadLine();
			}

			return o_UserResigned ? String.Empty : validatedMoveString;
		}

		internal bool ValidateMoveFormat(string i_Move)
		{
			char rowStart, colStart, rowEnd, colEnd;
			bool isValidMoveFormat = false;
			string[] moves = i_Move.Split('>');
			
			if (moves.Length == 2 && moves[0].Length == 2 && moves[1].Length == 2)
			{
				rowStart = moves[0][0];
				colStart = moves[0][1];
				rowEnd = moves[1][0];
				colEnd = moves[1][1];
				isValidMoveFormat =
					char.IsUpper(rowStart) && 
					char.IsUpper(rowEnd) && 
					char.IsLower(colStart) && 
					char.IsLower(colEnd);
			}

			return isValidMoveFormat;
		}

		internal string PrepareRowSeparator()
		{
			StringBuilder rowSeparatorBuilder = new StringBuilder();

			rowSeparatorBuilder.Append(" ");
			rowSeparatorBuilder.Append("=");
			rowSeparatorBuilder.Append(new string('=', m_GameUI.BoardSize * 4));
			rowSeparatorBuilder.Append("=");
			rowSeparatorBuilder.Append(Environment.NewLine);

			return rowSeparatorBuilder.ToString();
		}

		internal void PrintMoveOrQuitRequest()
		{
			string quitString = "Q";
			Console.Write(string.Format("Enter {0} to quit, enter move in format ROWcol > ROWcol: ", quitString));
		}

		internal void PrintSessionScoreBoard()
		{
			Console.WriteLine(string.Format("{0} has {1} points{2}{3} has {4} points{5}",
				m_Game.PlayerOneName,
				m_Game.PlayerOneTotalScore,
				Environment.NewLine,
				m_Game.PlayerTwoName,
				m_Game.PlayerTwoTotalScore,
				Environment.NewLine));
		}


		internal string GetErrorMessage(eErrorCode i_ErrorCode)
		{
			string errorMsg = string.Empty;
			
			switch (i_ErrorCode)
			{
				case eErrorCode.PositionOutOfBoardBounds:
					errorMsg = string.Format("Position out of Board Bounds: Row[{0}-{1}]col[{2}-{3}] are the valid values.", 'A', (char)('A' + m_GameUI.BoardSize), 'a', (char)('a' + m_GameUI.BoardSize));
					break;
				case eErrorCode.InvalidStartOrEndPosition:
					errorMsg = string.Format("You entered a wrong start or end position (or both).", m_GameUI.m_Game.PlayerToMoveName); 
					break;
				case eErrorCode.EndPositionOccupied:
					errorMsg = string.Format("{0} You entered an end position you can't move to", m_GameUI.m_Game.PlayerToMoveName);
					break;
				case eErrorCode.EntereredRegularMoveWhenHadCapture:
					errorMsg = string.Format("{0} You must capture if you can.", m_GameUI.m_Game.PlayerToMoveName);
					break;
				case eErrorCode.InvalidMove:
					errorMsg = string.Format("{0} enterd an invalid move.", m_GameUI.m_Game.PlayerToMoveName);
					break;
				default:
					break;					
			}

			return errorMsg;
		}

		internal char GetDiscSymbolToMove(eColor i_PlayerColor)
		{
			char discCharRepresentation;

			if (i_PlayerColor == eColor.White)
			{
				discCharRepresentation =  m_GameUI.r_CellStateDictionary[eCellState.WhiteRegular];
			}
			else
			{
				discCharRepresentation = m_GameUI.r_CellStateDictionary[eCellState.BlackRegular];
			}

			return discCharRepresentation;
		}
	}
}
