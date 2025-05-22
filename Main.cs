namespace MoreLandings;

/// <summary>
/// Main plugin class for the MoreLandings mod. Handles configuration, hooks, and landing string management.
/// </summary>
[Landfall.Modding.LandfallPlugin]
public class MoreLandings
{
	/// <summary>
	/// Static constructor. Initializes configuration and hooks into the landing score event.
	/// </summary>
	static MoreLandings()
	{
		// Create config
		HastySetting cfg = new("MoreLandings");
		cfg.OnConfig += () => new Config(cfg);

		// Moving to Hooks, removing dependancy.
		On.UI_LandingScore.Land += ChangeTextOnLand;
	}

	/// <summary>
	/// Gets the dictionary of landing types and their associated messages.
	/// </summary>
	public static Dictionary<LandingType, List<string>> LandingTypes { get; private set; } = null!;

	/// <summary>
	/// Gets the file path to the JSON file containing landing strings.
	/// </summary>
	internal static string jsonPath { get; set; } = string.Empty;

	/// <summary>
	/// Console command that displays the current JSON path.
	/// </summary>
	[Zorro.Core.CLI.ConsoleCommand]
	internal static void JSONPath() => Informer.Inform($"Your JSON path is: {jsonPath}", InformType.Warn);

	/// <summary>
	/// Loads landing strings from the JSON file or uses default values if the file does not exist or errors.
	/// </summary>
	internal static void LoadStrings()
	{
		// Default JSON string if the file doesn't exist
		string defaultJson = """{ "Bad": [ "Please take a shower", "HORSE", "You are not him", "Skill Issue", "Not good enough", "That didn't land", "Nobody clapped", "-100 Aura", "What are you doing", "Hell...", ], "Ok": [ "Here, you deserve a star", "Everybody clapped", "Learning The Slopes", "It's Okay", "Happens", "Could've done better", "Daro Proud", "Find Better Angle" ], "Good": [ "Do that again", "Congratulations!", "Mushroom", "Nice Job", "Well Done", "You can do better", "Captain Proud", "Nice!", "Good Angle" ], "Perfect": [ "King", "What a Sigma", "Awesome", "Just like a God planned this", "Peak", "Peam", "Wild", "This all over my screen", "THE POWER OF AN ANGEL", "Perfection", "+Rice bowl", "+Neko wife", "+1 social credit", "Ideal", "Rolling Rolling Rolling", "Faster Than Light", "+10 style points", "Haste" ] }""";
		try
		{
			// Deserialize the JSON into the dictionary.
			// If the JSON file exists, load it; otherwise, use the default JSON string.
			LandingTypes = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<LandingType, List<string>>>(File.Exists(jsonPath) ? File.ReadAllText(jsonPath) : defaultJson)!;

			// If the JSON file doesn't exist, save the loaded dictionary into the file for future use.
			if (!File.Exists(jsonPath)) File.WriteAllText(jsonPath, Newtonsoft.Json.JsonConvert.SerializeObject(LandingTypes));
		}
		catch (Exception e)
		{
			Informer.InformTrace(e, "How did this fuck up? Anyway, loading default settings.", InformType.Error);

			// Since it somehow errored, maybe due to a dumbass not setting the json properly. We load default but not override file. Suffer.
			LandingTypes = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<LandingType, List<string>>>(defaultJson)!;
		}
	}

	/// <summary>
	/// Console command to reload landing strings from the JSON file.
	/// </summary>
	[Zorro.Core.CLI.ConsoleCommand]
	internal static void Reload()
	{ Informer.Inform("Loading json strings. Please wait.", InformType.Warn); LoadStrings(); Informer.Inform("Loaded JSON Strings. Happy landing!", InformType.Info); }

	/// <summary>
	/// Console command that displays the counts and contents of each landing type.
	/// </summary>
	[Zorro.Core.CLI.ConsoleCommand]
	internal static void What()
	{
		Informer.Inform($"Counts: [Bad: {LandingTypes[LandingType.Bad].Count}], [Ok: {LandingTypes[LandingType.Ok].Count}], [Good: {LandingTypes[LandingType.Good].Count}]," +
			$"[Perfect: {LandingTypes[LandingType.Perfect].Count}]", InformType.Info);
		Informer.Inform(string.Join(", ", LandingTypes[LandingType.Bad]), InformType.Info);
		Informer.Inform(string.Join(", ", LandingTypes[LandingType.Ok]), InformType.Info);
		Informer.Inform(string.Join(", ", LandingTypes[LandingType.Good]), InformType.Info);
		Informer.Inform(string.Join(", ", LandingTypes[LandingType.Perfect]), InformType.Info);
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
		// Run original method
		orig(self, player, type, savedLanding);

		// Roll
		if (UnityEngine.Random.Range(0.0f, 1.0f) > Config.CustomFreq?.Value) { return; }

		// Get animator
		UnityEngine.Animator anim = self.GetComponent<UnityEngine.Animator>();
		if (anim == null) throw new Exception("Fuck");

		// Get text
		System.Reflection.FieldInfo textField = typeof(UI_LandingScore).GetField("text", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		if (textField != null && textField.GetValue(self) is TMPro.TextMeshProUGUI text)
		{
			// Get a random landing text from the dictionary based on the landing type
			string landText = RandomElement(LandingTypes[type]);

			// Calculate the animation speed based on the length of the landing text, clamping it between 0.25f and 1f
			float speed = UnityEngine.Mathf.Clamp(1f / (landText.Length / 6), 0.25f, 1f);

			// Set the animation speed and update the displayed text with the random landing string
			anim.speed = speed;
			text.text = landText;
		}
		else { throw new Exception("Somehow failed to get the text field"); }
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
