using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using System.IO;

namespace MmoKitCE
{
    public class MmoKitCE_InstallWizard : EditorWindow
    {
        [System.Serializable]
        private class PackageManifest
        {
            public string version = "";
        }

        private const string PREF_KEY_SHOWN = "MmoKitCE_WizardShown_1.3.0";

        private Texture2D iconTexture;
        private int currentStep = 1;

        private const string SETTINGS_PACKAGE_PATH = "Packages/com.mmokitce.installer/MmoKitCE_Settings.unitypackage";
        private const string MMOKITCE_PACKAGE_PATH = "Packages/com.mmokitce.installer/MmoKitCE.unitypackage";
        private const string PACKAGE_NAME = "com.mmokitce.installer";
        private PackageManifest currentInstall;

		GUIStyle richTextStyle;
		GUIStyle bulletStyle;

        [InitializeOnLoadMethod]
        private static void InitOnLoad()
        {
            // Show only once per session/project open
            if (!EditorPrefs.GetBool(PREF_KEY_SHOWN, false))
            {
                EditorApplication.delayCall += () =>
                {
                    ShowWizard();
                    EditorPrefs.SetBool(PREF_KEY_SHOWN, true);
                };
            }
        }

        public static void ShowWizard()
        {
            var window = GetWindow<MmoKitCE_InstallWizard>(true, "MmoKitCE Setup Wizard");
            window.minSize = new Vector2(600, 580);
            window.maxSize = new Vector2(600, 580);
            window.Show();
        }

        private void OnEnable()
        {
            UpdateInstalledVersion();

            iconTexture = Resources.Load<Texture2D>("MmoKitCE");
            if (iconTexture == null)
            {
                Debug.LogWarning("MmoKitCE.png not found in Resources");
            }

        	richTextStyle = new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                richText = true
            };
            bulletStyle = new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                richText = true,
                wordWrap = true
            };
        }

        private void OnGUI()
        {
            DrawHeader();
            DrawStepsBar();
            DrawContent();
            DrawFooter();
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);

            if (iconTexture != null)
            {
				GUILayout.BeginVertical();
				EditorGUILayout.Space(10);
                GUILayout.Box(iconTexture, GUILayout.Width(64), GUILayout.Height(64));
				GUILayout.EndVertical();

                GUILayout.Space(12);
            }

            GUILayout.BeginVertical();
            EditorGUILayout.Space(10);

            GUILayout.Label($"<b>Package version:</b> {currentInstall.version}", richTextStyle);
            EditorGUILayout.Space(6);
            GUILayout.Label("MmoKitCE is an <i>opinonated</i> community edition distribution of MMORPG Kit.", richTextStyle);
            EditorGUILayout.Space(6);
            GUILayout.Label("What's New in CE", EditorStyles.boldLabel);
			DrawBullet("Addon Manager is an in-editor interface that allows the community and team to modularize functionality.");
			DrawBullet("Login Manager is a clean separation of login/authentication logic from the central game servers.");
			DrawBullet("Sharded DatabaseNetworkManager adds lanes, queueing, deferred/throttled saves, and a working in-memory cache.");
			DrawBullet("Cell-based position quantization dramatically improves network efficiency for entity movement.");
			DrawBullet("Jobs Movement Pipeline converted from monothreaded per-entity updates to Unity Jobs + Burst parallel processing.");

            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(20);
        }

		private void DrawBullet(string text)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(10);
			GUILayout.Label("•", bulletStyle, GUILayout.Width(15));
			GUILayout.Label(text, bulletStyle);
			GUILayout.EndHorizontal();
		}

        private void DrawStepsBar()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(40);

            DrawStepBox("1. Settings", currentStep >= 1, currentStep > 1);
            DrawStepBox("2. Package", currentStep >= 2, currentStep > 2);
            DrawStepBox("3. Customize", currentStep >= 3, currentStep > 3);

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawStepBox(string label, bool active, bool completed)
        {
            Color bgColor = completed ? new Color(0.2f, 0.6f, 0.2f) :
                        	active   ? 	new Color(0f, 0.5f, 0f) :
                                    	new Color(0.3f, 0.3f, 0.3f);

            // Use GUILayoutUtility instead of GetControlRect to avoid interaction
            Rect rect = GUILayoutUtility.GetRect(172, 32);
            EditorGUI.DrawRect(rect, bgColor);

            GUIStyle style = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };

            GUI.Label(rect, label, style);
        }

        private void DrawContent()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(40);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(8);

            switch (currentStep)
            {
                case 1:
                    GUILayout.Label("Import the recommended settings for Input, Physics2D, Tags/Layers, Quality, and Time.\n\nThis should only be done on install or if you wish to reset settings.", richTextStyle);
                    break;

                case 2:
                    GUILayout.Label("Import the latest MmoKitCE package. It will take a minute to gather content after clicking the Import button.", richTextStyle);
                    break;

                case 3:
                    GUILayout.Label("Setup complete!\n\nCustomize your version of MmoKitCE with addons found in <b>Tools > MmoKitCE > Develop > Addon Manager</b> menu.\n\nIf you are developing MmoKitCE, delete the MmoKitCE directory and git clone the repo. <i>All development should be on a feature branch upstreamed from develop.</i>", richTextStyle);
                    GUILayout.Space(8);

					GUILayout.BeginHorizontal();
					GUILayout.Space(20);
					GUILayout.Label("git clone https://github.com/denariigames/MmoKitCE.git", richTextStyle);
					GUILayout.EndHorizontal();
                    break;
            }

            GUILayout.Space(8);
            GUILayout.EndVertical();
            GUILayout.Space(40);
            GUILayout.EndHorizontal();
        }

        private void DrawFooter()
        {
            EditorGUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (currentStep == 1)
            {
                if (GUILayout.Button("Import Settings →", GUILayout.Width(160)))
                {
                    if (System.IO.File.Exists(SETTINGS_PACKAGE_PATH))
                    {
                        AssetDatabase.ImportPackage(SETTINGS_PACKAGE_PATH, true);
                        currentStep = 2;
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("File Missing",
                            $"Cannot find '{SETTINGS_PACKAGE_PATH}' in project root.",
                            "OK");
                    }
                }

                GUILayout.Space(10);

                if (GUILayout.Button("Skip", GUILayout.Width(120)))
                    currentStep = 2;
            }
            else if (currentStep == 2)
            {
                if (GUILayout.Button("Import MmoKitCE →", GUILayout.Width(160)))
                {
                    if (System.IO.File.Exists(MMOKITCE_PACKAGE_PATH))
                    {
                        AssetDatabase.ImportPackage(MMOKITCE_PACKAGE_PATH, true);
                        currentStep = 3;
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("File Missing",
                            $"Cannot find '{MMOKITCE_PACKAGE_PATH}' in project root.",
                            "OK");
                    }
                }
            }
            else // Step 3
            {
                if (GUILayout.Button("Finish & Close", GUILayout.Width(140)))
                    Close();
            }

            GUILayout.Space(40);
            EditorGUILayout.EndHorizontal();
        }

        private void UpdateInstalledVersion()
        {
            string packageJsonPath = $"Packages/{PACKAGE_NAME}/package.json";

            if (File.Exists(packageJsonPath))
            {
                try
                {
                    string json = File.ReadAllText(packageJsonPath);
                    currentInstall = JsonUtility.FromJson<PackageManifest>(json);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"Failed to read package.json: {e.Message}");
                }
            }
        }

        [MenuItem("Tools/MmoKitCE/Install/Show Setup Wizard", false, -1000)]
        private static void ManualOpen()
        {
            EditorPrefs.SetBool(PREF_KEY_SHOWN, false);
            ShowWizard();
        }
    }
}