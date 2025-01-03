
namespace CheckersGameLogic
{
	internal class Disc
	{
		internal eDiscType DiscType { get; set; }
		internal eColor DiscColor { get; }
		public Disc(eColor i_Color)
		{
			DiscColor = i_Color;
			DiscType = eDiscType.Regular;
		}

		internal void PromoteToKing()
		{
			DiscType = eDiscType.King;
		}
	}
}
