using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Editor
{
    public class AudioAuditTool : EditorWindow
    {
        // Settings
        private string yarnProjectFolder = "Assets"; // Where to search for .yarn files
        private string audioResourcesPath = "Audio"; // Inside Resources folder (e.g. Resources/Audio)

        // Internal Data
        private Vector2 scrollPos;
        private List<MissingLineData> missingLines = new List<MissingLineData>();
        private int totalLinesFound = 0;
        private int filesScanned = 0;

        private struct MissingLineData
        {
            public string id;
            public string text;
            public string filename; // Which .yarn file it came from
        }

        [MenuItem("Tools/Audio Audit Tool")]
        public static void ShowWindow()
        {
            GetWindow<AudioAuditTool>("Audio Audit");
        }

        void OnGUI()
        {
            GUILayout.Label("Audio Audit (Direct Scan)", EditorStyles.boldLabel);
            GUILayout.Space(5);

            // 1. Configuration
            EditorGUILayout.BeginVertical("box");
            yarnProjectFolder = EditorGUILayout.TextField(
                new GUIContent("Script Folder", "Where your .yarn files live (starts with Assets)"), yarnProjectFolder);
            audioResourcesPath = EditorGUILayout.TextField(
                new GUIContent("Audio Resources Path", "Path inside a Resources folder (e.g. 'Audio')"),
                audioResourcesPath);
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            // 2. Action Button
            if (GUILayout.Button("Scan .yarn Files & Check Audio", GUILayout.Height(30)))
            {
                RunAudit();
            }

            GUILayout.Space(10);

            // 3. Results Info
            if (filesScanned > 0)
            {
                string color = missingLines.Count == 0 ? "green" : "red";
                GUILayout.Label($"Scanned {filesScanned} files. Found {totalLinesFound} lines.",
                    EditorStyles.miniLabel);
                GUILayout.Label($"<color={color}>Missing Audio: {missingLines.Count}</color>",
                    new GUIStyle(EditorStyles.boldLabel) { richText = true });
            }

            // 4. Scrollable List
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            if (missingLines.Count > 0)
            {
                foreach (var line in missingLines)
                {
                    DrawMissingLineItem(line);
                }
            }
            else if (filesScanned > 0 && totalLinesFound > 0)
            {
                GUILayout.Label("✨ All audio files found! Perfect!", EditorStyles.wordWrappedLabel);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawMissingLineItem(MissingLineData line)
        {
            EditorGUILayout.BeginVertical("helpbox");

            // Top Row: ID
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label($"<b>ID:</b> {line.id}", new GUIStyle(EditorStyles.label) { richText = true });
            if (GUILayout.Button("Copy", GUILayout.Width(50)))
            {
                GUIUtility.systemCopyBuffer = line.id;
            }

            EditorGUILayout.EndHorizontal();

            // Text Content
            GUILayout.Label($"<i>\"{line.text}\"</i>",
                new GUIStyle(EditorStyles.label) { richText = true, wordWrap = true });

            // Footer: Source File
            GUILayout.Label($"Src: {line.filename}", EditorStyles.miniLabel);

            EditorGUILayout.EndVertical();
            GUILayout.Space(2);
        }

        private void RunAudit()
        {
            missingLines.Clear();
            totalLinesFound = 0;
            filesScanned = 0;

            // 1. Find all .yarn files recursively
            if (!Directory.Exists(yarnProjectFolder))
            {
                EditorUtility.DisplayDialog("Error", $"Folder not found: {yarnProjectFolder}", "OK");
                return;
            }

            string[] yarnFiles = Directory.GetFiles(yarnProjectFolder, "*.yarn", SearchOption.AllDirectories);
            filesScanned = yarnFiles.Length;

            // 2. Regex to find IDs:  Matches any content ending with #line:12345
            // Capture Group 1: The text
            // Capture Group 2: The ID
            Regex lineRegex = new Regex(@"(.*?)\s*#line:([a-zA-Z0-9_:]+)");

            foreach (string filePath in yarnFiles)
            {
                string[] lines = File.ReadAllLines(filePath);

                foreach (string lineContent in lines)
                {
                    // Skip comments
                    if (lineContent.Trim().StartsWith("//")) continue;

                    Match match = lineRegex.Match(lineContent);
                    if (match.Success)
                    {
                        totalLinesFound++;

                        // Extract Data
                        string rawText = match.Groups[1].Value.Trim();
                        string id = match.Groups[2].Value.Trim();

                        // Cleanup Text (Remove character name "Mom: " if present)
                        string displayText = rawText;
                        if (displayText.Contains(":"))
                        {
                            var parts = displayText.Split(new[] { ':' }, 2);
                            if (parts.Length > 1) displayText = parts[1].Trim();
                        }

                        // 3. Check if Audio Exists
                        CheckAudioExistence(id, displayText, Path.GetFileName(filePath));
                    }
                }
            }
        }

        private void CheckAudioExistence(string id, string text, string filename)
        {
            // Yarn IDs might contain "line:", we strip it for the filename usually, 
            // OR we keep it depending on your naming convention. 
            // Based on previous chats, your files are "line_1a2b3c"

            // If ID is "line:12345", we want "line_12345"
            string cleanID = id.Replace(":", "_");

            // Try to load
            string pathToCheck = $"{audioResourcesPath}/{cleanID}";
            Object clip = Resources.Load(pathToCheck);

            if (clip == null)
            {
                missingLines.Add(new MissingLineData
                {
                    id = id,
                    text = text,
                    filename = filename
                });
            }
        }
    }
}