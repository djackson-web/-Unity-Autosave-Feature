#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class AutoSavePreferencesProvider : SettingsProvider
{
  private const float SaveNowButtonWidth = 100f;
  private const float HeaderSpacingTop = 8f;
  private const float HeaderSpacingBottom = 4f;
  private const float FooterSpacing = 12f;

  public AutoSavePreferencesProvider()
      : base("Preferences/AutoSave", SettingsScope.User) { }

  [SettingsProvider]
  public static SettingsProvider CreateAutoSaveSettingsProvider()
  {
    return new AutoSavePreferencesProvider();
  }

  public override void OnGUI(string searchContext)
  {
    EditorGUILayout.Space(HeaderSpacingTop);
    GUILayout.Label("AutoSave Settings", EditorStyles.boldLabel);
    EditorGUILayout.Space(HeaderSpacingBottom);

    DrawAutoSaveEnabledToggle();

    EditorGUI.BeginDisabledGroup(!AutoSave.AutoSaveEnabled);
    DrawIntervalField();
    DrawSaveOnPlayToggle();
    DrawConsoleLoggingToggle();
    EditorGUI.EndDisabledGroup();

    EditorGUILayout.Space(FooterSpacing);

    DrawSaveNowButton();
    DrawEditorPrefsNotice();
  }

  private static void DrawAutoSaveEnabledToggle()
  {
    var label = new GUIContent(
        "Enable AutoSave",
        "Periodically saves all scenes that have unsaved changes."
    );
    AutoSave.AutoSaveEnabled = EditorGUILayout.Toggle(label, AutoSave.AutoSaveEnabled);
  }

  private static void DrawIntervalField()
  {
    var label = new GUIContent(
        "Interval (minutes)",
        "How many minutes to wait between automatic saves. Minimum is 1 minute."
    );
    float newInterval = EditorGUILayout.FloatField(label, AutoSave.IntervalInMinutes);
    AutoSave.IntervalInMinutes = newInterval;
  }

  private static void DrawSaveOnPlayToggle()
  {
    var label = new GUIContent(
        "Save on Play",
        "Saves all unsaved scenes before entering Play mode."
    );
    AutoSave.SaveOnPlayEnabled = EditorGUILayout.Toggle(label, AutoSave.SaveOnPlayEnabled);
  }

  private static void DrawConsoleLoggingToggle()
  {
    var label = new GUIContent(
        "Log Saves to Console",
        "Prints a message to the Console window each time an automatic save occurs."
    );
    AutoSave.ConsoleLoggingEnabled = EditorGUILayout.Toggle(label, AutoSave.ConsoleLoggingEnabled);
  }

  private static void DrawSaveNowButton()
  {
    EditorGUI.BeginDisabledGroup(!AutoSave.AutoSaveEnabled);
    if (GUILayout.Button("Save Now", GUILayout.Width(SaveNowButtonWidth)))
      AutoSave.SaveAllDirtyScenes("Manual Save");
    EditorGUI.EndDisabledGroup();
  }

  private static void DrawEditorPrefsNotice()
  {
    EditorGUILayout.HelpBox(
        "Settings are stored in EditorPrefs on this machine and are not included in source control.",
        MessageType.Info
    );
  }
}
#endif