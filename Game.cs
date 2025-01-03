using System;

namespace CheckersGameLogic
{
	public class Game
	{
		private CheckersBoard m_Board;
		private Player m_Player1;
		private Player m_Player2;

		public Player PlayerToMove { get; private set; }
		public Player PlayerWhoPlayedLast { get; private set; }
		public Player GameWinner { get; private set; }
		public eErrorCode? ErrorCode { get; private set; }
		public Move? LastMovePlayed { get; private set; }
		public bool IsGameRunning { get; private set; }
		internal bool IsInConsecutiveCapturesMode { get; private set; }
		public int BoardSizeInt
		{
			get
			{
				return m_Board.BoardSizeInt;
			}
		}
		public string PlayerToMoveName
		{
			get
			{
				return PlayerToMove.PlayerName;
			}
		}
		public string PlayerWhoPlayedLastName
		{
			get
			{
				string name = string.Empty;

				if (PlayerWhoPlayedLast != null)
				{
					name = PlayerWhoPlayedLast.PlayerName;
				}

				return name;
			}
		}
		public ePlayerType PlayerToMoveType {
			get
			{
				return PlayerToMove.PlayerType;
			}
		}

		public string PlayerOneName
		{
			get
			{
				return m_Player1.PlayerName;
			}
		}
		public string PlayerTwoName
		{
			get
			{
				return m_Player2.PlayerName;
			}
		}
		
		public int PlayerOneTotalScore
		{
			get
			{
				return m_Player1.PlayerTotalScore;
			}
			set
			{
				m_Player1.PlayerTotalScore = value;
			}
		}
		public int PlayerTwoTotalScore 
		{
			get
			{
				return m_Player2.PlayerTotalScore;
			}
			set
			{
				m_Player2.PlayerTotalScore = value;
			}
		}

		public Game(eBoardSize i_BoardSize, string i_PlayerOneName, string i_PlayerTwoName, ePlayerType i_PlayerType)
		{
			m_Board = new CheckersBoard(i_BoardSize, this);
			m_Player1 = new Player(i_PlayerOneName, ePlayerType.Human, eColor.White);
			m_Player2 = new Player(i_PlayerTwoName, i_PlayerType, eColor.Black);			
			PlayerOneTotalScore = 0;
			PlayerTwoTotalScore = 0;
			StartNewGame();
		}

		public void StartNewGame()
		{
			m_Board.InitializeBoardStartingState();
			IsGameRunning = true;
			PlayerToMove = m_Player1; // This enables future flexiblity of selecting first player to move.
			PlayerWhoPlayedLast = null; 
			ErrorCode = null;
			IsInConsecutiveCapturesMode = false;
			ErrorCode = null;
			GameWinner = null;
			LastMovePlayed = null;
			GenerateListOfMovesForPlayer(PlayerToMove);
		}

		public void GenerateNextConsecutiveCaptureMoves(Move i_Move)
		{
			Disc disc = m_Board.GetDiscFromPositionOrNullIfEmpty(i_Move.End);
			bool isKing = disc.DiscType == eDiscType.King;

			PlayerToMove.m_PlayerListOfValidMoves.Clear();
			m_Board.GenerateMovesFromPosition(i_Move.End, PlayerToMove, isKing);
			m_Board.ClearNonCaptureMovesFromMovesList(PlayerToMove);
		}

		public void GenerateListOfMovesBothForPlayers()
		{
			GenerateListOfMovesForPlayer(m_Player1);
			GenerateListOfMovesForPlayer(m_Player2);
		}

		public void GenerateListOfMovesForPlayer(Player i_Player)
		{
			eColor discColor;
			bool isKing = false;
			Disc disc = null;
			Player discOwner = null;
			eColor playerColor = i_Player.PlayerColor;

			i_Player.m_PlayerListOfValidMoves.Clear();

			for (int row = 0; row < BoardSizeInt; row++)
			{
				for (int col = 0; col < BoardSizeInt; col++)
				{
					Position startPos = new Position(row, col);
					disc = m_Board.GetDiscFromPositionOrNullIfEmpty(startPos);

					if (disc != null && disc.DiscColor == playerColor)
					{
						isKing = disc.DiscType == eDiscType.King;
						discColor = disc.DiscColor;
						discOwner = (discColor == eColor.White) ? m_Player1 : m_Player2;
						m_Board.GenerateMovesFromPosition(startPos, discOwner, isKing);
					}
				}
			}
		}

		public eCellState[,] GetBoardStateSnapshot() // Encapsulated board state for UI. UI does not receive actual Disc objects.
		{
			int size = m_Board.BoardSizeInt;
			eCellState[,] boardStateSnapshot = new eCellState[size, size];
			
			for (int row = 0; row < size; row++)
			{
				for (int col = 0; col < size; col++)
				{
					boardStateSnapshot[row, col] = m_Board.GetCellStateFromBoard(new Position(row,col));
				}
			}

			return boardStateSnapshot;
		}

		public static bool IsValidPlayerNameLength(string i_InputName)
		{
			return i_InputName.Length <= 20;
		}

		private void switchTurn()
		{
			PlayerToMove = (PlayerToMove == m_Player1) ? m_Player2 : m_Player1;
			PlayerWhoPlayedLast = (PlayerToMove == m_Player1) ? m_Player2 : m_Player1;
		}

		public static bool IsValidBoardSize(int i_BoardSizeInput)
		{
			return Enum.IsDefined(typeof(eBoardSize), i_BoardSizeInput);
		}

		public static bool IsValidGameMode(ePlayerType i_InputMode)
		{
			return Enum.IsDefined(typeof(ePlayerType), i_InputMode);
		}

		public void IsValidMove(Move i_Move)
		{
			bool isHasCapture = false;
			ErrorCode = null;
			Disc startPosdisc = null;
			Disc endPosDisc = null;

			GenerateListOfMovesForPlayer(PlayerToMove);

			if (PlayerToMove.m_PlayerListOfValidMoves.Count == 0)
			{
				IsGameRunning = false;
			}

			if (IsGameRunning) 
			{ 
				if ( m_Board.IsMoveInBounds(i_Move))
				{
					if (IsMoveInPlayerValidMoveList(i_Move, out isHasCapture))
					{
						if (i_Move.IsCapture == false && isHasCapture)
						{
							ErrorCode = eErrorCode.EntereredRegularMoveWhenHadCapture;			
						}
						else
						{
							ExecuteMoveAndCheckConsecutiveCaptures(i_Move);
						}
					}
					else
					{
						startPosdisc = m_Board.GetDiscFromPositionOrNullIfEmpty(i_Move.Start);
						endPosDisc = m_Board.GetDiscFromPositionOrNullIfEmpty(i_Move.End);

						if (startPosdisc == null || endPosDisc != null) // if start position is empty or end position is occupied -> invalid
						{
							ErrorCode = eErrorCode.InvalidStartOrEndPosition;
						}
						else
						{
							ErrorCode = eErrorCode.InvalidMove; // other type of invalid move
						}
					}
				}
				else
				{
					ErrorCode = eErrorCode.PositionOutOfBoardBounds;
				}
			}
		}
		
		public bool IsMoveInPlayerValidMoveList(Move i_Move, out bool o_HasCaptureInList)
		{
			
			bool isInValidMovesList = false;
			o_HasCaptureInList = false; // assume no capture in list

			foreach (Move move in PlayerToMove.m_PlayerListOfValidMoves)
			{
				if (move.IsCapture)
				{
					o_HasCaptureInList = true;
				}

				if (i_Move.Equals(move))
				{
					isInValidMovesList = true;
				}
			}

			return isInValidMovesList;
		}

		public void ExecuteMoveAndCheckConsecutiveCaptures(Move i_Move)
		{
			if (i_Move.IsCapture)
			{
				m_Board.ExecuteCapture(i_Move.Start,i_Move.End, m_Board.GetMidPositionForCapture(i_Move.Start, i_Move.End));
				GenerateNextConsecutiveCaptureMoves(i_Move);
				IsInConsecutiveCapturesMode = PlayerToMove.m_PlayerListOfValidMoves.Count > 0 ? true : false;
			}
			else
			{
				m_Board.ExecuteRegularMove(i_Move.Start, i_Move.End);
			}

			LastMovePlayed = i_Move;

			if (m_Board.HasReachedEndRow(i_Move.End))
			{
				m_Board.GetDiscFromPositionOrNullIfEmpty(i_Move.End).PromoteToKing();
			}

			if (IsInConsecutiveCapturesMode)
			{
				PlayerWhoPlayedLast = PlayerToMove;
			}
			else
			{				
				switchTurn();
				GenerateListOfMovesForPlayer(PlayerToMove); 
			}

			checkIfGameOverAndDeclareWinnerOrTie();
		}

		public void ExecuteComputerMove()
		{
			bool isKing;
			int randomIndex;
			Move randomMove;
			int validMovesListCount;
			Random random = new Random();

			GenerateListOfMovesForPlayer(PlayerToMove);
			validMovesListCount = PlayerToMove.m_PlayerListOfValidMoves.Count;

			if (validMovesListCount > 0)
			{
				randomIndex = random.Next(validMovesListCount);
				randomMove = PlayerToMove.m_PlayerListOfValidMoves[randomIndex];
				PlayerWhoPlayedLast = PlayerToMove;
				LastMovePlayed = randomMove;

				if (randomMove.IsCapture)
				{
					while (validMovesListCount > 0) // consecutive captures for computer
					{
						m_Board.ExecuteCapture(randomMove.Start, randomMove.End, m_Board.GetMidPositionForCapture(randomMove.Start, randomMove.End)); 

						if (m_Board.HasReachedEndRow(randomMove.End))
						{
							m_Board.GetDiscFromPositionOrNullIfEmpty(randomMove.End).PromoteToKing();
						}

						LastMovePlayed = randomMove; 
						GenerateNextConsecutiveCaptureMoves(LastMovePlayed.Value); 
						validMovesListCount = PlayerToMove.m_PlayerListOfValidMoves.Count;

						if (validMovesListCount == 0)
						{
							break;
						}
						
						randomIndex = random.Next(validMovesListCount);
						randomMove = PlayerToMove.m_PlayerListOfValidMoves[randomIndex];
						LastMovePlayed = randomMove;
					}

					GenerateListOfMovesBothForPlayers();
					checkIfGameOverAndDeclareWinnerOrTie(); // computer may have remaining moves (non capture) while player has no more discs
					switchTurn();
				}
				else
				{
					m_Board.ExecuteRegularMove(randomMove.Start,randomMove.End);

					if (m_Board.HasReachedEndRow(randomMove.End))
					{
						m_Board.GetDiscFromPositionOrNullIfEmpty(randomMove.End).PromoteToKing();
					}

					switchTurn();
				}
			}
			else
			{
				if (IsInConsecutiveCapturesMode) // was in ConsecutiveCaptureMode and has no more moves
				{
					IsInConsecutiveCapturesMode = false;
					switchTurn();
					
				}
				else
				{
					checkIfGameOverAndDeclareWinnerOrTie();
				}
			}

		}

		private void checkIfGameOverAndDeclareWinnerOrTie()
		{
			Player otherPlayer = null;
			int playerToMoveValidMovesCount;
			int otherPlayerValidMovesCount;

			otherPlayer = (PlayerToMove == m_Player1) ? m_Player2 : m_Player1;
			GenerateListOfMovesForPlayer(otherPlayer);
			playerToMoveValidMovesCount = PlayerToMove.m_PlayerListOfValidMoves.Count;
			otherPlayerValidMovesCount = otherPlayer.m_PlayerListOfValidMoves.Count;

			IsGameRunning = false;

			if (playerToMoveValidMovesCount == 0 && otherPlayerValidMovesCount == 0)
			{
				GameWinner = null;
			}
			else if (playerToMoveValidMovesCount > 0 && otherPlayerValidMovesCount == 0)
			{
				GameWinner = PlayerToMove;
			}
			else if (playerToMoveValidMovesCount == 0 && otherPlayerValidMovesCount > 0)
			{
				GameWinner = otherPlayer;
			}
			else
			{
				IsGameRunning = true; 
			}
		}

		public void SetWinnerOnResignation()
		{
			GameWinner = (PlayerToMove == m_Player1) ? m_Player2 : m_Player1;
		}

		public void CalculatePoints(Player i_Loser)
		{
			eCellState [,]cellStatesSnapshot = GetBoardStateSnapshot();
			int playerOneRoundTotal = 0;
			int playerTwoRoundTotal = 0;

			if (GameWinner != null)
			{
				GameWinner = (i_Loser == m_Player1) ? m_Player2 : m_Player1;

				for (int row = 0; row < BoardSizeInt; row++)
				{
					for (int col = 0; col < BoardSizeInt; col++)
					{
						switch (cellStatesSnapshot[row, col])
						{
							case eCellState.Empty:
								break;
							case eCellState.WhiteRegular:
								playerOneRoundTotal += 1;
								break;
							case eCellState.BlackRegular:
								playerTwoRoundTotal += 1;
								break;
							case eCellState.WhiteKing:
								playerOneRoundTotal += 4;
								break;
							case eCellState.BlackKing:
								playerTwoRoundTotal += 4;
								break;
							default:
								break;
						}
					}
				}
			}

			GameWinner.PlayerTotalScore += Math.Abs(playerOneRoundTotal - playerTwoRoundTotal);
		}	
	}
}
