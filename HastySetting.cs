using Zorro.Settings;

// Modified from https://github.com/NaokoAF/HastyControls, fitting my style better.
namespace MoreLandings;

public class HastyFloat : FloatSetting, IExposedSetting
{
	private HastySetting config;

	private float defaultValue;

	private UnityEngine.Localization.LocalizedString displayName;

	private Unity.Mathematics.float2 minMax;

	/// <summary>
	/// Creates a new float setting in a <see cref="HastySetting"/> menu.
	/// </summary>
	/// <param name="cfg">The config this setting belongs to.</param>
	/// <param name="name">Name of the setting (used as a key).</param>
	/// <param name="description">Displayed description in the UI.</param>
	/// <param name="min">Minimum allowed value.</param>
	/// <param name="max">Maximum allowed value.</param>
	/// <param name="defaultValue">Default value when nothing is set.</param>
	public HastyFloat(HastySetting cfg, string name, string description, float min, float max, float defaultValue)
	{
		config = cfg;
		this.defaultValue = defaultValue;
		minMax = new Unity.Mathematics.float2(min, max);
		displayName = cfg.CreateDisplayName(name, description);
		cfg.Add(this);
	}

	public event Action<float>? Applied;

	public override void ApplyValue() => Applied?.Invoke(Value);

	public string GetCategory() => config.ModName;

	public override float GetDefaultValue() => defaultValue;

	public UnityEngine.Localization.LocalizedString GetDisplayName() => displayName;

	public override Unity.Mathematics.float2 GetMinMaxValue() => minMax;
}

public class HastySetting
{
	// Access references to the settings list and the save/load system.
	private static HarmonyLib.AccessTools.FieldRef<HasteSettingsHandler, List<Setting>> settingsRef = HarmonyLib.AccessTools.FieldRefAccess<HasteSettingsHandler, List<Setting>>("settings");

	private static HarmonyLib.AccessTools.FieldRef<HasteSettingsHandler, ISettingsSaveLoad> settingsSaveLoadRef = HarmonyLib.AccessTools.FieldRefAccess<HasteSettingsHandler, ISettingsSaveLoad>("_settingsSaveLoad");

	/// <summary>
	/// Sets up a new setting handler for a mod, linking the mod's name and GUID.
	/// </summary>
	/// <param name="modName">The name of the mod.</param>
	/// <param name="modGUID">The unique GUID of the mod.</param>
	public HastySetting(string modName, string modGUID)
	{
		ModName = modName;
		SettingsUIPage.LocalizedTitles.Add(ModName, new(Main.GUID, ModName));
	}

	public string ModName { get; private set; }

	/// <summary>
	/// Adds a new setting to the mod's settings list and applies its initial value.
	/// </summary>
	/// <typeparam name="T">The type of setting being added (e.g., HastyFloat, HastyBool).</typeparam>
	/// <param name="setting">The setting to add.</param>
	public void Add<T>(T setting) where T : Setting
	{
		var handler = GameHandler.Instance.SettingsHandler;
		settingsRef(handler).Add(setting);
		setting.Load(settingsSaveLoadRef(handler));
		setting.ApplyValue();
	}

	/// <summary>
	/// Creates a localized display name for a setting with an optional description.
	/// </summary>
	/// <param name="name">The name of the setting.</param>
	/// <param name="description">A brief description for the setting.</param>
	/// <returns>A <see cref="LocalizedString"/> for the setting's name and description.</returns>
	internal UnityEngine.Localization.LocalizedString CreateDisplayName(string name, string description = "") => new(Main.GUID, $"{name}\n<size=60%><alpha=#50>{description}");
}

public class HastyBool : BoolSetting, IExposedSetting, IEnumSetting
{
	private List<string> choices;

	private HastySetting config;

	private bool defaultValue;

	private UnityEngine.Localization.LocalizedString displayName;

	/// <summary>
	/// Creates a new <see cref="bool"/> setting with custom choices for "On" and "Off" values.
	/// </summary>
	/// <param name="cfg">The config object the setting belongs to.</param>
	/// <param name="name">Name of the setting (used as a key).</param>
	/// <param name="description">Displayed description in the UI.</param>
	/// <param name="defaultValue">Default boolean value (true or false).</param>
	/// <param name="offChoice">Text for the "Off" option (defaults to "Off").</param>
	/// <param name="onChoice">Text for the "On" option (defaults to "On").</param>
	public HastyBool(HastySetting cfg, string name, string description, bool defaultValue, string offChoice = "Off", string onChoice = "On")
	{
		config = cfg;
		this.defaultValue = defaultValue;
		displayName = cfg.CreateDisplayName(name, description);
		choices = new List<string> { offChoice, onChoice };
		cfg.Add(this);
	}

	public event Action<bool>? Applied;

	public override UnityEngine.Localization.LocalizedString OffString => null!;

	public override UnityEngine.Localization.LocalizedString OnString => null!;

	public override void ApplyValue() => Applied?.Invoke(Value);

	public string GetCategory() => config.ModName;

	public override bool GetDefaultValue() => defaultValue;

	public UnityEngine.Localization.LocalizedString GetDisplayName() => displayName;

	/// <summary>
	/// Gets the unlocalized choices for this setting ("On" and "Off").
	/// </summary>
	List<string> IEnumSetting.GetUnlocalizedChoices() => choices;
}
