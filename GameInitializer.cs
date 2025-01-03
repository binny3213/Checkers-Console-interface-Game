using CheckersGameLogic;

namespace FlowAndUI
{
	internal class GameInitializer
	{
		internal static void InitializeGame()
		{
			string playerTwoName = string.Empty;
			string playerOneName = PlayerInteraction.GetPlayerName();
			eBoardSize boardSize = PlayerInteraction.RequestBoardSizeInput();
			ePlayerType gameMode = PlayerInteraction.RequestGameMode();

			if (gameMode == ePlayerType.Human)
			{
				playerTwoName = PlayerInteraction.GetPlayerName();
			}

			Game game = new Game(boardSize, playerOneName, playerTwoName, gameMode);
			GameUI gameUI = new GameUI(game);
			gameUI.StartGameSession();
		}
	}
}

