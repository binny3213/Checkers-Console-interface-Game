using System.Collections.Generic;

namespace CheckersGameLogic
{
	public class Player
	{
		private readonly string r_PlayerName;
		private readonly ePlayerType r_PlayerType;
		private readonly eColor r_PlayerColor;
		internal List<Move> m_PlayerListOfValidMoves;

		internal int PlayerTotalScore { get; set; }
		internal string PlayerName
		{
			get { return r_PlayerName; }
		}
		public ePlayerType PlayerType
		{
			get { return r_PlayerType; }
		}
		public eColor PlayerColor
		{
			get { return r_PlayerColor; }
		}

		internal Player(string i_PlayerName, ePlayerType i_PlayerType, eColor i_PlayerColor)
		{
			r_PlayerName = i_PlayerName == string.Empty ? "Computer" : i_PlayerName;
			r_PlayerType = i_PlayerType;
			r_PlayerColor = i_PlayerColor;
			PlayerTotalScore = 0;
			m_PlayerListOfValidMoves = new List<Move>();
		}
	}
}
