
namespace CheckersGameLogic
{
	public struct Move
	{
		public Position Start { get;  }
		public Position End { get; }
		public bool IsCapture { get; }

		public Move(Position i_StartPos, Position i_EndPos, bool i_IsCapture)
		{
			Start = i_StartPos;
			End = i_EndPos;
			IsCapture = i_IsCapture;
		}
	}
}
