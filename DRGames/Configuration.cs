using Dalamud.Configuration;
using System;

namespace DRGames
{
	[Serializable]
	public class Configuration : IPluginConfiguration
	{
		public int Version { get; set; } = 0;

		public bool SomePropertyToBeSavedAndWithADefault { get; set; } = true;

		public void Save()
		{
			Plugin.PluginInterface.SavePluginConfig(this);
		}
	}
}
