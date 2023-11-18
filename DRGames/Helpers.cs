using System;
using System.Collections.Generic;
using System.Threading;

namespace DRGames
{
	internal static class Helpers
	{
		public static class ThreadSafeRandom
		{
			[ThreadStatic] private static Random? Local;

			public static Random ThisThreadsRandom => Local ??= new Random(unchecked((Environment.TickCount * 31) + Thread.CurrentThread.ManagedThreadId));
		}

		public static void Shuffle<T>(this IList<T> list)
		{
			var n = list.Count;
			while (n > 1)
			{
				n--;
				var k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
				(list[n], list[k]) = (list[k], list[n]);
			}
		}
	}
}
