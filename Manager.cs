namespace MoreLandings;

public class Config
{
	public Config()
	{
		HastySetting cfg = new(Main.NAME, Main.GUID);

		CustomFreq = new HastyFloat(cfg, "Roll chance", "The chance to display custom text over the original", 0, 1, 0.5f);
	}

	public static HastyFloat? CustomFreq { get; private set; }
}

internal static class NumberUtils
{
	/// <summary>
	/// Represents a shared instance of a random number generator.
	/// </summary>
	internal static readonly Random random = new(GenerateTrulyRandomNumber());

	/// <summary>
	/// Generates a truly random number using cryptographic random number generation.
	/// </summary>
	/// <returns>A truly random number within a specified range.</returns>
	internal static int GenerateTrulyRandomNumber()
	{
		using System.Security.Cryptography.RNGCryptoServiceProvider rng = new();
		byte[] bytes = new byte[4]; // 32 bities :3c
		rng.GetBytes(bytes);

		// Convert the random bytes to an integer and ensure it falls within the specified range
		int randomInt = BitConverter.ToInt32(bytes, 0);
		return Math.Abs(randomInt % (50 - 10)) + 10;
	}

	/// <summary>
	/// Returns a random integer within the specified range.
	/// </summary>
	/// <param name="min">The inclusive lower bound of the random number to be generated.</param>
	/// <param name="max">The exclusive upper bound of the random number to be generated.</param>
	/// <returns>A random integer within the specified range.</returns>
	internal static int Next(int min, int max) => random.Next(min, max);

	/// <summary>
	/// Returns a random double number between 0.0 and 1.0.
	/// </summary>
	/// <returns>A random double number between 0.0 and 1.0... Duh</returns>
	internal static double NextD() => random.NextDouble();
}

[HarmonyLib.HarmonyPatch]
public class Patching
{
	/// <summary>
	/// Patches the "Land" method to change the text after landing.
	/// </summary>
	/// <param name="type">The landing type.</param>
	/// <param name="savedLanding">Indicates if the landing was saved.</param>
	/// <param name="__instance">The instance of UI_LandingScore being patched.</param>

	[HarmonyLib.HarmonyPatch(typeof(UI_LandingScore), "Land")]
	[HarmonyLib.HarmonyPostfix]
	private static void Land(LandingType type, bool savedLanding, UI_LandingScore __instance)
	{
		// Get the non-public "text" field from UI_LandingScore
		System.Reflection.FieldInfo textField = typeof(UI_LandingScore).GetField("text", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		if (textField != null)
		{
			// Extract the TextMeshProUGUI object from the field
			TMPro.TextMeshProUGUI tmpText = (TMPro.TextMeshProUGUI)textField.GetValue(__instance);
			if (tmpText != null) Main.ChangeTextOnLand(type, tmpText, __instance);
		}
	}

	/// <summary>
	/// Patches the "OnStringChanged" method to update the localized text based on the mod's GUID.
	/// </summary>
	/// <param name="__instance">The instance of LocalizeUIText being patched.</param>
	[HarmonyLib.HarmonyPatch(typeof(Zorro.Localization.LocalizeUIText), "OnStringChanged")]
	[HarmonyLib.HarmonyPostfix]
	private static void OnStringChangedPostfix(Zorro.Localization.LocalizeUIText __instance)
	{
		// Ensure the string's table reference matches the mod's GUID
		if (__instance.String?.TableReference.TableCollectionName != Main.GUID) return;
		__instance.Text.text = __instance.String.TableEntryReference.Key;
	}

	/// <summary>
	/// Patches the "RegisterPage" method to create a new Config page.
	/// </summary>
	/// <param name="__instance">The instance of HasteSettingsHandler being patched.</param>
	[HarmonyLib.HarmonyPatch(typeof(HasteSettingsHandler), "RegisterPage")]
	[HarmonyLib.HarmonyPrefix]
	private static void RegisterPagePrefix(HasteSettingsHandler __instance) => new Config();
}
