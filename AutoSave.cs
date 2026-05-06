#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class AutoSave
{
  // ── Preference Keys ────────────────────────────────────────────
  private const string AutoSaveEnabledKey = "AutoSave_Enabled";
  private const string AutoSaveIntervalKey = "AutoSave_IntervalMinutes";
  private const string SaveOnPlayEnabledKey = "AutoSave_SaveOnPlay";
  private const string ConsoleLoggingEnabledKey = "AutoSave_LogEnabled";

  // ── Defaults ───────────────────────────────────────────────────
  private const bool DefaultAutoSaveEnabled = true;
  private const float DefaultIntervalInMinutes = 5f;
  private const bool DefaultSaveOnPlayEnabled = true;
  private const bool DefaultConsoleLoggingEnabled = true;

  private const float MinimumIntervalInMinutes = 1f;
  private const float SecondsPerMinute = 60f;

  private static double nextScheduledSaveTime;

  // ── Preferences ────────────────────────────────────────────────
  public static bool AutoSaveEnabled
  {
    get => EditorPrefs.GetBool(AutoSaveEnabledKey, DefaultAutoSaveEnabled);
    set => EditorPrefs.SetBool(AutoSaveEnabledKey, value);
  }

  public static float IntervalInMinutes
  {
    get => EditorPrefs.GetFloat(AutoSaveIntervalKey, DefaultIntervalInMinutes);
    set => EditorPrefs.SetFloat(AutoSaveIntervalKey, Mathf.Max(MinimumIntervalInMinutes, value));
  }

  public static bool SaveOnPlayEnabled
  {
    get => EditorPrefs.GetBool(SaveOnPlayEnabledKey, DefaultSaveOnPlayEnabled);
    set => EditorPrefs.SetBool(SaveOnPlayEnabledKey, value);
  }

  public static bool ConsoleLoggingEnabled
  {
    get => EditorPrefs.GetBool(ConsoleLoggingEnabledKey, DefaultConsoleLoggingEnabled);
    set => EditorPrefs.SetBool(ConsoleLoggingEnabledKey, value);
  }

  // ── Initialization ─────────────────────────────────────────────
  // InitializeOnLoad triggers this constructor on every Editor startup
  // and recompile, so hooks are always registered without manual setup.
  static AutoSave()
  {
    ScheduleNextSave();
    EditorApplication.update += OnEditorUpdate;
    EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
  }

  // ── Timed Save ─────────────────────────────────────────────────
  private static void OnEditorUpdate()
  {
    if (!AutoSaveEnabled) return;
    if (EditorApplication.isPlaying) return;
    if (!HasReachedScheduledSaveTime()) return;

    SaveAllDirtyScenes("Timed AutoSave");
    ScheduleNextSave();
  }

  private static bool HasReachedScheduledSaveTime()
  {
    return EditorApplication.timeSinceStartup >= nextScheduledSaveTime;
  }

  private static void ScheduleNextSave()
  {
    double intervalInSeconds = IntervalInMinutes * SecondsPerMinute;
    nextScheduledSaveTime = EditorApplication.timeSinceStartup + intervalInSeconds;
  }

  // ── Save on Play ───────────────────────────────────────────────
  private static void OnPlayModeStateChanged(PlayModeStateChange playModeState)
  {
    if (!SaveOnPlayEnabled) return;

    bool isAboutToEnterPlayMode = playModeState == PlayModeStateChange.ExitingEditMode;
    if (isAboutToEnterPlayMode)
      SaveAllDirtyScenes("AutoSave Before Play");
  }

  // ── Core Save Logic ────────────────────────────────────────────
  public static void SaveAllDirtyScenes(string saveReason)
  {
    bool savedAtLeastOneScene = false;

    for (int sceneIndex = 0; sceneIndex < EditorSceneManager.sceneCount; sceneIndex++)
    {
      var scene = EditorSceneManager.GetSceneAt(sceneIndex);

      bool sceneCanBeSaved = scene.IsValid() && scene.isDirty;
      if (sceneCanBeSaved)
      {
        EditorSceneManager.SaveScene(scene);
        savedAtLeastOneScene = true;
      }
    }

    // SaveAssets catches prefab and ScriptableObject changes outside scenes
    AssetDatabase.SaveAssets();

    if (ConsoleLoggingEnabled && savedAtLeastOneScene)
      LogSaveToConsole(saveReason);
  }

  private static void LogSaveToConsole(string saveReason)
  {
    string timestamp = System.DateTime.Now.ToString("HH:mm:ss");
    Debug.Log($"[AutoSave] {saveReason} — {timestamp}");
  }
}
#endif