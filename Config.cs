using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MoreLandings;

/// <summary>
/// Configuration class for the MoreLandings mod. Handles setting up custom frequency and loading landing strings.
/// </summary>
public class Config
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Config"/> class and loads landing strings.
	/// </summary>
	/// <param name="cfg">The HastySetting configuration instance.</param>
	public Config(HastySetting cfg)
	{
		CustomFreq = new(cfg, "Roll chance", "The change to display custom text over the original", new() { MinMax = new(0, 1), DefaultValue = 0.5f });

		new HastyButton(cfg, "Open JSON Path", "Tries to open the persistent data path.", new("Open", OpenPath));

		/* Placing these in here becuase I added a log in the LoadStrings method which errors since the debug UI doesn't exist at that time. Lol */
		// Create the file path to store the JSON data (shoutout to Alice for this part).
		MoreLandings.jsonPath = Path.Combine(UnityEngine.Application.persistentDataPath, "MoreLandings.json");

		// Load the landing strings into the dictionary from the JSON file or default values.
		MoreLandings.LoadStrings();
	}

	/// <summary>
	/// Gets the custom frequency setting for displaying custom landing text.
	/// </summary>
	public static HastyFloat? CustomFreq { get; private set; } = null!;

	/// <summary>
	/// Opens the persistent data path in the system's file explorer.
	/// </summary>
	public void OpenPath()
	{
		string path = UnityEngine.Application.persistentDataPath;

		try
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{ Process.Start(new ProcessStartInfo("explorer.exe", $"\"{path}\"") { UseShellExecute = true }); }
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{ Process.Start("open", $"\"{path}\""); }
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{ Process.Start("xdg-open", $"\"{path}\""); }
			else
			{ UnityEngine.Debug.LogWarning("Unsupported platform"); }
		}
		catch (Exception e) { e.InformTrace("Failed to open the persistent data path.", InformType.Error); }
	}
}
