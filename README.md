# AutoSave for Unity

A lightweight Unity Editor plugin that automatically saves your scenes at a configurable interval, so you never lose work to an unexpected crash.

---

## Features

- Automatically saves all dirty scenes on a configurable timer
- Optionally saves all dirty scenes before entering Play mode
- Skips saving during Play mode to avoid interruptions
- Saves prefab and ScriptableObject changes outside of scenes
- Configurable settings panel under Edit → Preferences → AutoSave
- Optional Console logging with a timestamp on every save
- Settings persist across sessions and are not committed to source control

---

## Requirements

- Unity 2020.1 or newer
- No third-party dependencies

---

## Installation

1. Open your Unity project
2. In the Project window, navigate to your `Assets` folder
3. Create a folder named `Editor` if one does not already exist — the name must be exact
4. Inside `Assets/Editor/`, create two new C# scripts:
   - `AutoSave.cs`
   - `AutoSavePreferencesProvider.cs`
5. Replace the contents of each file with the corresponding script
6. Save both files and wait for Unity to finish recompiling

The plugin activates automatically after recompilation. No further setup is required.

---

## Configuration

Open **Edit → Preferences → AutoSave** to access the settings panel.

| Setting | Default | Description |
|---|---|---|
| Enable AutoSave | On | Master on/off switch for the entire plugin |
| Interval (minutes) | 5 | How often to save dirty scenes. Minimum is 1 minute |
| Save on Play | On | Saves all dirty scenes before entering Play mode |
| Log Saves to Console | On | Prints a timestamped message to the Console on each save |
| Save Now | — | Triggers an immediate manual save |

All settings are stored in `EditorPrefs` on your local machine and are not included in source control.

---

## How It Works

- `[InitializeOnLoad]` causes the plugin to activate on every Editor startup and recompile, so it is always running without any manual setup
- A hook on `EditorApplication.update` checks elapsed time each frame and triggers a save when the configured interval is reached
- A hook on `EditorApplication.playModeStateChanged` saves scenes just before Play mode begins
- Only scenes that have unsaved changes are written to disk — scenes with no changes are skipped
- `AssetDatabase.SaveAssets()` is called on every save to capture prefab and ScriptableObject changes that exist outside of scenes

---


## Notes

- The `Editor` folder is a special Unity folder. Its contents are automatically excluded from game builds, so this plugin adds zero overhead to your final build
- Saving is skipped entirely while the Editor is in Play mode to avoid interruptions mid-session
- If a scene has never been saved before, Unity will not know where to write the file. Make sure all new scenes are saved manually at least once before relying on AutoSave

---
