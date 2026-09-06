using DRGames.Games;
using Dalamud.Plugin.Services;
using ECommons.Automation;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace DRGames.Services
{
	public sealed class GameChat : IGameChat
	{
		private readonly IFramework framework;
		private readonly Channel<string> messageQueue = Channel.CreateUnbounded<string>();
		private readonly Channel<string> readyMessageQueue = Channel.CreateUnbounded<string>();
		private readonly CancellationTokenSource cancellationTokenSource = new();
		private readonly Task worker;

		public GameChat(IFramework framework)
		{
			this.framework = framework;
			framework.Update += OnFrameworkUpdate;
			worker = ProcessMessagesAsync();
		}

		public void SendPartyMessage(string message)
		{
			messageQueue.Writer.TryWrite($"/p {message}");
		}

		public void SendTell(string playerName, string world, string message)
		{
			messageQueue.Writer.TryWrite($"/tell {playerName}@{world} {message}");
		}

		public void Dispose()
		{
			framework.Update -= OnFrameworkUpdate;
			messageQueue.Writer.TryComplete();
			readyMessageQueue.Writer.TryComplete();
			cancellationTokenSource.Cancel();
			worker.GetAwaiter().GetResult();
			cancellationTokenSource.Dispose();
		}

		private void OnFrameworkUpdate(IFramework framework)
		{
			while (readyMessageQueue.Reader.TryRead(out var message))
			{
				Chat.SendMessage(message);
			}
		}

		private async Task ProcessMessagesAsync()
		{
			var isFirstMessage = true;

			try
			{
				await foreach (var message in messageQueue.Reader.ReadAllAsync(cancellationTokenSource.Token))
				{
					if (!isFirstMessage)
					{
						await Task.Delay(Random.Shared.Next(1000, 2001), cancellationTokenSource.Token);
					}

					readyMessageQueue.Writer.TryWrite(message);
					isFirstMessage = false;
				}
			}
			catch (OperationCanceledException) when (cancellationTokenSource.IsCancellationRequested)
			{
			}
		}
	}
}
