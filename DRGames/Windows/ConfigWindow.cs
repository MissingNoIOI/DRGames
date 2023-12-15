using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
using System.Numerics;

namespace DRGames.Windows;

public class ConfigWindow : Window, IDisposable
{
	private readonly Configuration configuration;

	public ConfigWindow(Plugin plugin) : base(
		"A Wonderful Configuration Window",
		ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
		ImGuiWindowFlags.NoScrollWithMouse)
	{
		Size = new Vector2(232, 75);
		SizeCondition = ImGuiCond.Always;

		configuration = plugin.Configuration;
	}

	public void Dispose() { }

	public override void Draw()
	{
		// can't ref a property, so use a local copy
		var configValue = configuration.SomePropertyToBeSavedAndWithADefault;
		if (ImGui.Checkbox("Random Config Bool", ref configValue))
		{
			configuration.SomePropertyToBeSavedAndWithADefault = configValue;
			// can save immediately on change, if you don't want to provide a "Save and Close" button
			configuration.Save();
		}
	}
}
