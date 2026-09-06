namespace DRGames.Roulette
{
	public enum RouletteBetType
	{
		SingleNumber,
		Even,
		Odd,
		Low,
		High,
		Dozen,
		Street,
		DoubleStreet
	}

	public sealed class RouletteBet
	{
		public RouletteBetType Type { get; set; }
		public int Value { get; set; }

		public int PayoutMultiplier => Type switch
		{
			RouletteBetType.SingleNumber => 35,
			RouletteBetType.Even or RouletteBetType.Odd or RouletteBetType.Low or RouletteBetType.High => 1,
			RouletteBetType.Dozen => 2,
			RouletteBetType.Street => 11,
			RouletteBetType.DoubleStreet => 5,
			_ => 0
		};

		public bool Wins(int result)
		{
			if (result is < 1 or > 36)
			{
				return false;
			}

			return Type switch
			{
				RouletteBetType.SingleNumber => result == Value,
				RouletteBetType.Even => result % 2 == 0,
				RouletteBetType.Odd => result % 2 != 0,
				RouletteBetType.Low => result is >= 1 and <= 18,
				RouletteBetType.High => result is >= 19 and <= 36,
				RouletteBetType.Dozen => result >= Value && result <= Value + 11,
				RouletteBetType.Street => result >= Value && result <= Value + 2,
				RouletteBetType.DoubleStreet => result >= Value && result <= Value + 5,
				_ => false
			};
		}

		public override string ToString()
		{
			return Type switch
			{
				RouletteBetType.SingleNumber => $"Single {Value}",
				RouletteBetType.Even => "Even",
				RouletteBetType.Odd => "Odd",
				RouletteBetType.Low => "Low (1-18)",
				RouletteBetType.High => "High (19-36)",
				RouletteBetType.Dozen => $"Dozen {Value}",
				RouletteBetType.Street => $"Street {Value}-{Value + 2}",
				RouletteBetType.DoubleStreet => $"Double Street {Value}-{Value + 5}",
				_ => Type.ToString()
			};
		}
	}
}
