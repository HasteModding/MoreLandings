namespace MoreLandings;

public class Config
{
	public Config(HastySetting cfg)
	{
		CustomFreq = new HastyFloat(cfg, "Roll chance", "The chance to display custom text over the original.", new()
		{
			MinMax = new Unity.Mathematics.float2(0, 1),
			DefaultValue = 0.5f
		});

		ForceType = new HastyEnum<MoreLandingType>(cfg, "Force Landing Type", "Forces the landing type to be a specific one. Useful for testing.", new()
		{
			Choices = Enum.GetValues(typeof(MoreLandingType)).Cast<MoreLandingType>().Select(l => l.ToString()).ToList(),
			DefaultValue = MoreLandingType.None
		});

		MinTextTime = new HastyFloat(cfg, "Text Time", "How long landing text is present on screen (lower is longer)", new()
		{
			MinMax = new(0.1f, 0.99f),
			DefaultValue = 0.25f
		});

		new HastyButton(cfg, "Reset", "Resets values to their default state", new()
		{
			OnClick = () =>
			{
				CustomFreq.Reset();
				ForceType.Reset();
				MinTextTime.Reset();
			},
			Text = "Reset Settings"
		});

		LoadStrings();
	}

	public enum MoreLandingType
	{
		Bad,
		Ok,
		Good,
		Perfect,
		None,
		Saved,
	}

	/// <summary>
	/// Gets the custom frequency setting for displaying custom landing text.
	/// </summary>
	public static HastyFloat CustomFreq { get; private set; } = null!;

	/// <summary>
	/// Gets the force landing type setting, which allows forcing a specific landing type for testing purposes.
	/// </summary>
	public static HastyEnum<MoreLandingType> ForceType { get; private set; } = null!;

	/// <summary>
	/// Get's the path of the json file
	/// </summary>
	public static string jsonPath => Path.Combine(UnityEngine.Application.persistentDataPath, "MoreLandings.json");

	/// <summary>
	/// Stores all the different landingtype strings
	/// </summary>
	public static Dictionary<MoreLandingType, List<string>> LandingStrings { get; private set; } = null!;

	/// <summary>
	/// How long should the long texts stay on screen for.
	/// </summary>
	public static HastyFloat MinTextTime { get; private set; } = null!;

	/// <summary>
	/// Loads landing strings from the JSON file or uses default values if the file does not exist or errors.
	/// </summary>
	public static void LoadStrings()
	{
		// Defining the default JSON string
		string defaultJson = """{"Bad":["Please take a shower","HORSE","You are not him","Skill Issue","Not good enough","That didn't land","Nobody clapped","-100 Aura","What are you doing","Hell..."],"Ok":["Here, you deserve a star","Everybody clapped","Learning The Slopes","It's Okay","Happens","Could've done better","Daro Proud","Find Better Angle"],"Good":["Do that again","Congratulations!","Mushroom","Nice Job","Well Done","You can do better","Captain Proud","Nice!","Good Angle"],"Perfect":["King","What a Sigma","Awesome","Just like a God planned this","Peak","Peam","Wild","This all over my screen","THE POWER OF AN ANGEL","Perfection","+Rice bowl","+Neko wife","+1 social credit","Ideal","Rolling Rolling Rolling","Faster Than Light","+10 style points","Haste"],"Saved":["You got lucky there", "Really?", "Perfect. I guess.", "Yeah alright mate"]}""";

		// Initialize LandingStrings with the default values
		/// "Possible null reference" HOW???
		LandingStrings = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<MoreLandingType, List<string>>>(defaultJson)!;

		try
		{
			// If the json file exists, we load it
			if (File.Exists(jsonPath))
			{
				// Load json data into a temporary dict
				Dictionary<MoreLandingType, List<string>>? loadedStrings
					= Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<MoreLandingType, List<string>>>(File.ReadAllText(jsonPath));

				// If we could not load the json data, exit.
				if (loadedStrings == null) { Informer.Inform("Somehow the loadedStrings is null...?"); return; }

				// Go through the loaded data and check if it's missing entries
				foreach (KeyValuePair<MoreLandingType, List<string>> entry in LandingStrings)
				{
					// Check if the LandingType is in the loaded data, if not, add the default value
					/// OR
					// If the entry exists, ensure it has at least one string
					if (!loadedStrings.ContainsKey(entry.Key) || loadedStrings[entry.Key].Count == 0)
					{
                        loadedStrings[entry.Key] = [.. LandingStrings[entry.Key]];
                    }
				}

				// Then finally set the landing strings to the loaded data once we've verified it has all the right data
				/// 👍👍👍👍👍👍👍👍 I forgot this line for so long, wondering why it wasn't loading new strings... 👎👎👎👎👎👎👎👎
				LandingStrings = loadedStrings;
			}
			// Else, save the full default data to the file for future use
			else
			{
				Informer.Inform($"No MoreLandings file was found. Creating a new one.");
				File.WriteAllText(jsonPath, Newtonsoft.Json.JsonConvert.SerializeObject(LandingStrings, Newtonsoft.Json.Formatting.Indented));
			}
		}
		catch (Newtonsoft.Json.JsonException jex)
		{
			Informer.Inform(jex, "Error deseralizing json file. Using default settings.", InformType.Error);
		}
		catch (Exception ex)
		{
			Informer.Inform(ex, "Error loading landing strings from file. Using default settings.", InformType.Error);
		}
	}
}
