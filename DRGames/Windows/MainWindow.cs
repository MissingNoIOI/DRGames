using Dalamud.Interface.Internal;
using Dalamud.Interface.Windowing;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Bindings.ImGui;
using System;
using System.Numerics;

namespace DRGames.Windows;

public class MainWindow : Window, IDisposable
{
	private readonly Plugin Plugin;

	public MainWindow(Plugin plugin) : base(
		"DRGames", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
	{
		SizeConstraints = new WindowSizeConstraints
		{
			MinimumSize = new Vector2(375, 330),
			MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
		};
		Plugin = plugin;
	}

	public void Dispose()
	{
	}

	public override void Draw()
	{
		ImGui.Text($"The random config bool is {Plugin.Configuration.SomePropertyToBeSavedAndWithADefault}");

		if (ImGui.Button("Show Settings"))
		{
			Plugin.DrawConfigUI();
		}

		ImGui.Spacing();

		if (ImGui.Button("Play Poker"))
		{
			Plugin.DrawPokerUI();
		}

		ImGui.Spacing();
	}
}
