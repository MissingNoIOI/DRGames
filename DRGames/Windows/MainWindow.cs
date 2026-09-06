using Dalamud.Interface.Internal;
using Dalamud.Interface.Windowing;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Bindings.ImGui;
using System;
using System.Numerics;

namespace DRGames.Windows;

public class MainWindow : Window, IDisposable
{
	private readonly Plugin plugin;

	public MainWindow(Plugin plugin) : base(
		"DRGames", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
	{
		SizeConstraints = new WindowSizeConstraints
		{
			MinimumSize = new Vector2(375, 330),
			MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
		};
		this.plugin = plugin;
	}

	public void Dispose()
	{
	}

	public override void Draw()
	{
		ImGui.Text($"The random config bool is {plugin.Configuration.SomePropertyToBeSavedAndWithADefault}");

		if (ImGui.Button("Show Settings"))
		{
			plugin.DrawConfigUI();
		}

		ImGui.Spacing();

		if (ImGui.Button("Play Poker"))
		{
			plugin.DrawPokerUI();
		}

		ImGui.Spacing();

		if (ImGui.Button("Play Roulette"))
		{
			plugin.DrawRouletteUI();
		}
	}
}
