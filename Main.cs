namespace MoreLandings;

[Landfall.Modding.LandfallPlugin]
public class MoreLandings
{
	/// <summary>
	/// Static constructor. Shocker right?
	/// </summary>
	static MoreLandings()
	{
		// Create the config
		HastySetting cfg = new("MoreLandings");

		// Tell our config class to load the UI
		cfg.OnConfig += () => new Config(cfg);

		// Add event
		On.UI_LandingScore.Land += ChangeTextOnLand;
	}

	/// <summary>
	/// Console command that displays the current JSON path.
	/// </summary>
	[Zorro.Core.CLI.ConsoleCommand]
	public static void JSONPath()
	{
		Informer.Inform($"Your JSON path is: {Config.jsonPath}", InformType.Info);
	}

	/// <summary>
	/// Console command that displays the contents of each landing type.
	/// </summary>
	[Zorro.Core.CLI.ConsoleCommand]
	public static void Landings()
	{
		foreach (KeyValuePair<Config.MoreLandingType, List<string>> kvp in Config.LandingStrings)
		{
			List<string> strings = kvp.Value.ToList();
			Informer.Inform($"{kvp.Key}: [", InformType.Info);
			strings.ForEach(s => Informer.Inform($" -  {s}", InformType.Warn));
			Informer.Inform("]", InformType.Info);
		}
	}

	/// <summary>
	/// Console command to reload landing strings from the JSON file.
	/// </summary>
	[Zorro.Core.CLI.ConsoleCommand]
	public static void Reload()
	{
		Informer.Inform("Loading json strings. Please wait...", InformType.Info);
		Config.LoadStrings();
	}

	/// <summary>
	/// Hook method that changes the landing text when a landing event occurs.
	/// </summary>
	/// <param name="orig">The original landing method delegate.</param>
	/// <param name="self">The UI_LandingScore instance.</param>
	/// <param name="type">The type of landing.</param>
	/// <param name="savedLanding">Indicates if the landing was saved.</param>
	private static void ChangeTextOnLand(On.UI_LandingScore.orig_Land orig, UI_LandingScore self, PlayerCharacter player, LandingType type, bool savedLanding)
	{
		// Check if we should force a landing type
		switch (Config.ForceType?.Value)
		{
			case Config.MoreLandingType.None:
				break; // Do nothing, use original landing type

			case Config.MoreLandingType.Bad:
			case Config.MoreLandingType.Ok:
			case Config.MoreLandingType.Good:
			case Config.MoreLandingType.Perfect:
				type = (LandingType)Config.ForceType.Value; // Force the landing type
				break;

			case Config.MoreLandingType.Saved:
				savedLanding = true;
				break;

			default:
				Informer.Inform($"Unknown landing type: {Config.ForceType!.Value}", InformType.Error);
				return;
		}

		// Run original method
		orig(self, player, type, savedLanding);

		// Roll
		if (UnityEngine.Random.Range(0f, 1f) > Config.CustomFreq?.Value) { return; }

		// Get animator
		if (self.TryGetComponent(out UnityEngine.Animator anim))
		{
			// Get text
			System.Reflection.FieldInfo textField = typeof(UI_LandingScore).GetField("text", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			if (textField != null && textField.GetValue(self) is TMPro.TextMeshProUGUI text)
			{
				// Get random string from landingstrings
				string landText = RandomElement(Config.LandingStrings[savedLanding ? Config.MoreLandingType.Saved : (Config.MoreLandingType)type]);

				// Calculate the animation speed based on the length of the landing text, clamping it between 0.25f and 1f
				float speed = UnityEngine.Mathf.Clamp(1f / (landText.Length / 6), Config.MinTextTime.Value, 1f);

				// Set the animation speed and update the displayed text with the random landing string
				anim.speed = speed;
				text.text = landText;
			}
			else { Informer.Inform("Somehow failed to get the text field", InformType.Error); }
		}
		else { Informer.Inform("Failed to get animator", InformType.Error); }
	}

	/// <summary>
	/// Returns a random element from the given enumerable.
	/// </summary>
	/// <typeparam name="T">The type of elements in the enumerable.</typeparam>
	/// <param name="enumerable">The enumerable to select from.</param>
	/// <returns>A randomly selected element.</returns>
	private static T RandomElement<T>(IEnumerable<T> enumerable)
	{
		IReadOnlyList<T> readOnlyList = (enumerable as IReadOnlyList<T>) ?? enumerable.ToArray();
		return readOnlyList[UnityEngine.Random.Range(0, readOnlyList.Count)];
	}
}
