# MoreLandings

### 🪂 How It Works

- Overrides the default landing messages with random entries based on landing quality: `Bad`, `Ok`, `Good`, or `Perfect`.
- Configurable display rate via the **Settings Menu**.
- Includes console commands to reload message data or locate the JSON file.

---
<!--------------------------------------------------------------------------------------->

### 📝 Customizing Messages

1. **Find the config file**  
   - Stored in `UnityEngine.Application.persistentDataPath`.  
   - Use the console command `MoreLandings.JSONPath` to print the full path.

2. **Edit `MoreLandings.json`**  
   - Open with any text editor (Notepad, VS Code, etc.).
   - JSON format:
     ```json
     {
       "Bad":     ["Try again!", "Ouch.", "Not your best."],
       "Ok":      ["Not bad!", "Getting there."],
       "Good":    ["Nice!", "Well done!"],
       "Perfect": ["Legendary!", "Unstoppable!"],
       "Saved":   ["You got lucky there."]
     }
     ```

3. **Save the file**  
   - Make sure it's still valid JSON.

4. **Reload in-game**  
   - Use `MoreLandings.Reload` in the console to hot-reload your messages.
   - If for some reason your file is broken, delete it and relead in-game. It will create a new file.

---
<!--------------------------------------------------------------------------------------->

### 💻 Console Commands

Open the console (`F1`) and type:

| Command                     | Description                                      |
|-----------------------------|--------------------------------------------------|
| `MoreLandings.Reload`       | Reloads the JSON file without restarting         |
| `MoreLandings.JSONPath`     | Prints the exact file path of `MoreLandings.json`|
| `MoreLandings.Landings`     | Lists all messages currently loaded by category  |

---
<!--------------------------------------------------------------------------------------->

### ⚙️ Settings

> [!CAUTION]
> Unity’s UI system isn’t always efficient with frequent updates.  
> While performance impact is negligible, it’s best to tweak settings from the **main menu**.

---
<!--------------------------------------------------------------------------------------->

### 🙌 Credits

- **koshachiyglushitel** – Original concept  
- **Aezurian** – Provided most of the message flavor  
- **justatiredartist** – Kept me sane through it all ♥

---
<!--------------------------------------------------------------------------------------->
### Final Words

```txt
████████╗██╗░░██╗███████╗        ██████╗░░█████╗░░██╗░░░░░░░██╗███████╗██████╗░
╚══██╔══╝██║░░██║██╔════╝        ██╔══██╗██╔══██╗░██║░░██╗░░██║██╔════╝██╔══██╗
░░░██║░░░███████║█████╗░░        ██████╔╝██║░░██║░╚██╗████╗██╔╝█████╗░░██████╔╝
░░░██║░░░██╔══██║██╔══╝░░        ██╔═══╝░██║░░██║░░████╔═████║░██╔══╝░░██╔══██╗
░░░██║░░░██║░░██║███████╗        ██║░░░░░╚█████╔╝░░╚██╔╝░╚██╔╝░███████╗██║░░██║
░░░╚═╝░░░╚═╝░░╚═╝╚══════╝        ╚═╝░░░░░░╚════╝░░░░╚═╝░░░╚═╝░░╚══════╝╚═╝░░╚═╝

░█████╗░███████╗  ░█████╗░███╗░░██╗  ░█████╗░███╗░░██╗░██████╗░███████╗██╗░░░░░
██╔══██╗██╔════╝  ██╔══██╗████╗░██║  ██╔══██╗████╗░██║██╔════╝░██╔════╝██║░░░░░
██║░░██║█████╗░░  ███████║██╔██╗██║  ███████║██╔██╗██║██║░░██╗░█████╗░░██║░░░░░
██║░░██║██╔══╝░░  ██╔══██║██║╚████║  ██╔══██║██║╚████║██║░░╚██╗██╔══╝░░██║░░░░░
╚█████╔╝██║░░░░░  ██║░░██║██║░╚███║  ██║░░██║██║░╚███║╚██████╔╝███████╗███████╗
░╚════╝░╚═╝░░░░░  ╚═╝░░╚═╝╚═╝░░╚══╝  ╚═╝░░╚═╝╚═╝░░╚══╝░╚═════╝░╚══════╝╚══════╝
```

---
<!--------------------------------------------------------------------------------------->

</br>
<h3>Anyway, here's the update journey...</h3></br>
</br>

# Updates

### v1.3.0
- Added support for Saved landings.
- Reworked json saving/loading, checking for missing entries.
- Previous versions' settings are replaced by new ones since I changed how they're saved. Rip.

### v1.2.0
- Updated internal SettingsLib.
- Overhauled README
- Removed HarmonyX as a dependancy. Moved to Hooks.

### v1.1.2
- Forgot to chance the default value in the config to 0.5...

### v1.1.1
- Fixed bugs.

### v1.1.0
- Added config.
- Allows the game to have a chance to pick a random text or display the original text.

### v1.0.0
- idk man.
