namespace MoreLandings;

[Landfall.Modding.LandfallPlugin]
public class Main
{
	static Main()
	{
		// Create a new Harmony instance using the mod's GUID for patching.
		harmony = new(GUID);

		// Apply all patches defined in the Patching class.
		harmony.PatchAll(typeof(Patching));

		// Create the file path to store the JSON data (shoutout to Alice for this part).
		jsonPath = Path.Combine(UnityEngine.Application.persistentDataPath, "MoreLandings.json");

		// Load the landing strings into the dictionary from the JSON file or default values.
		LoadStrings();
	}

	// The mod's unique identifier (GUID) for internal use and patching.
	public static string GUID { get; private set; } = "com.github.ignoredsoul.MoreLandings";

	// Dictionary to store different landing types with associated messages.
	public static Dictionary<LandingType, List<string>> LandingTypes { get; private set; }

	// The name of the mod, used for the config menu.
	public static string NAME { get; private set; } = "MoreLandings";

	// The file path to the JSON file that stores landing messages.
	protected static string jsonPath { get; private set; }

	// The Harmony instance used to patch methods at runtime.
	private static HarmonyLib.Harmony harmony { get; set; }

	public static void ChangeTextOnLand(LandingType _type, TMPro.TextMeshProUGUI _text, UI_LandingScore instance)
	{
		// Get the Animator component from the UI_LandingScore instance, needed to adjust animation speed
		UnityEngine.Animator anim = instance.GetComponent<UnityEngine.Animator>();

		// If the random value (NextD) is greater than the custom frequency, show the original animation speed
		if (NumberUtils.NextD() > Config.CustomFreq?.Value) { anim.speed = 1f; return; }

		// Get a random landing text from the dictionary based on the landing type
		string landText = LandingTypes[_type][NumberUtils.Next(0, LandingTypes[_type].Count)];

		// Calculate the animation speed based on the length of the landing text, clamping it between 0.25f and 1f
		float speed = UnityEngine.Mathf.Clamp(1f / (landText.Length / 6), 0.25f, 1f);

		// Set the animation speed and update the displayed text with the random landing string
		anim.speed = speed;
		_text.text = landText;
	}

	// Mainly used for development but avaliable for those who want it
	[Zorro.Core.CLI.ConsoleCommand]
	internal static void Reload() => LoadStrings();

	// Load the list of landings into the dictionary
	private static void LoadStrings()
	{
		// Default JSON string if the file doesn't exist
		string defaultJson = """{ "Bad": [ "Please take a shower", "HORSE", "You are not him", "Skill Issue", "Not good enough", "That didn't land", "Nobody clapped", "-100 Aura", "What are you doing", "Hell...", ], "Ok": [ "Here, you deserve a star", "Everybody clapped", "Learning The Slopes", "It's Okay", "Happens", "Could've done better", "Daro Proud", "Find Better Angle" ], "Good": [ "Do that again", "Congratulations!", "Mushroom", "Nice Job", "Well Done", "You can do better", "Captain Proud", "Nice!", "Good Angle" ], "Perfect": [ "King", "What a Sigma", "Awesome", "Just like a God planned this", "Peak", "Peam", "Wild", "This all over my screen", "THE POWER OF AN ANGEL", "Perfection", "+Rice bowl", "+Neko wife", "+1 social credit", "Ideal", "Rolling Rolling Rolling", "Faster Than Light", "+10 style points", "Haste" ] }""";

		// Deserialize the JSON into the dictionary.
		// If the JSON file exists, load it; otherwise, use the default JSON string.
		LandingTypes = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<LandingType, List<string>>>(File.Exists(jsonPath) ? File.ReadAllText(jsonPath) : defaultJson)!;

		// If the JSON file doesn't exist, save the loaded dictionary into the file for future use.
		if (!File.Exists(jsonPath)) File.WriteAllText(jsonPath, Newtonsoft.Json.JsonConvert.SerializeObject(LandingTypes));
	}
}
