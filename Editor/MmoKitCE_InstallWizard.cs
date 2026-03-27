using UnityEditor;
using UnityEngine;

namespace MmoKitCE
{
    public class MmoKitCE_InstallWizard : EditorWindow
    {
        private const string PREF_KEY_SHOWN = "MmoKitCE_WizardShown_v1";

        private Texture2D iconTexture;
        private int currentStep = 1; // 1 = Dependencies (done), 2 = Settings, 3 = Done/Manual

        private const string SETTINGS_PACKAGE_PATH = "Packages/com.mmokitce.dependency-installer/MmoKitCE_Settings.unitypackage";

        [InitializeOnLoadMethod]
        private static void InitOnLoad()
        {
            // Show only once per session/project open
            if (!SessionState.GetBool(PREF_KEY_SHOWN, false))
            {
                EditorApplication.delayCall += () =>
                {
                    ShowWizard();
                    SessionState.SetBool(PREF_KEY_SHOWN, true);
                };
            }
        }

        public static void ShowWizard()
        {
            var window = GetWindow<MmoKitCE_InstallWizard>(true, "MmoKitCE Setup Wizard");
            window.minSize = new Vector2(480, 320);
            window.maxSize = new Vector2(480, 320);
            window.Show();
        }

        private void OnEnable()
        {
            iconTexture = Resources.Load<Texture2D>("MmoKitCE");
            if (iconTexture == null)
            {
                Debug.LogWarning("MmoKitCE.png not found in Resources");
            }
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
                GUILayout.Box(iconTexture, GUILayout.Width(64), GUILayout.Height(64));
                GUILayout.Space(12);
            }

            GUILayout.Label("MmoKitCE is an opinonated community edition distribution of MMORPG Kit. It will continue to pull improvements and fixes from core repos into this distribution, but may also remove functionality if it improves scalability, stability or security.", EditorStyles.wordWrappedLabel);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(8);
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.5f, 0.5f, 0.5f, 0.3f));
            EditorGUILayout.Space(8);
        }

        private void DrawStepsBar()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);

            DrawStepBox("1. Dependencies", currentStep >= 1, currentStep > 1);
            DrawStepBox("2. Settings", currentStep >= 2, currentStep > 2);
            DrawStepBox("3. Finalize", currentStep >= 3, currentStep > 3);

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(12);
        }

        private void DrawStepBox(string label, bool active, bool completed)
        {
            Color bgColor = completed ? new Color(0.2f, 0.8f, 0.2f) :
                        active   ? new Color(0.9f, 0.6f, 0.1f) :
                                    new Color(0.7f, 0.7f, 0.7f);

            // Use GUILayoutUtility instead of GetControlRect to avoid interaction
            Rect rect = GUILayoutUtility.GetRect(110, 32);

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
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.Space(8);

            switch (currentStep)
            {
                case 1:
                    EditorGUILayout.LabelField("Step 1: Dependencies");
                    EditorGUILayout.LabelField("All required packages (Addressables, Burst, etc.) have been installed automatically via this package.", EditorStyles.wordWrappedLabel);
                    break;

                case 2:
                    EditorGUILayout.LabelField("Step 2: Project Settings");
                    EditorGUILayout.LabelField("Import the recommended settings for Input, Physics2D, Tags/Layers, Quality, and Time.\n\n" +
                                            "The file is in the repository root:", EditorStyles.wordWrappedLabel);
                    EditorGUILayout.LabelField("MmoKitCE_Settings.unitypackage", EditorStyles.boldLabel);
                    break;

                case 3:
                    EditorGUILayout.LabelField("Step 3: Finalize");
                    EditorGUILayout.LabelField("Setup almost complete!\n\n" +
                                            "Clone the MmoKitCE repository into your Assets folder. This will enable you to keep your project up-to-date with a simple git pull. After importing, check the MMORPG Kit > MmoKitCE menu to enable optional addons.", EditorStyles.wordWrappedLabel);
                    EditorGUILayout.Space(8);
                    EditorGUILayout.LabelField("git clone https://github.com/MMORPG-Kit/MmoKitCE.git", EditorStyles.boldLabel);
                    break;
            }

            EditorGUILayout.Space(8);
            EditorGUILayout.EndVertical();
        }

        private void DrawFooter()
        {
            EditorGUILayout.Space(12);
            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if (currentStep == 1)
            {
                if (GUILayout.Button("Settings →", GUILayout.Width(140)))
                    currentStep = 2;
            }
            else if (currentStep == 2)
            {
                if (GUILayout.Button("Import Settings →", GUILayout.Width(160)))
                {
                    if (System.IO.File.Exists(SETTINGS_PACKAGE_PATH))
                    {
                        AssetDatabase.ImportPackage(SETTINGS_PACKAGE_PATH, true);
                        currentStep = 3;
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("File Missing",
                            $"Cannot find '{SETTINGS_PACKAGE_PATH}' in project root.\n\nPlease check your clone and try again.",
                            "OK");
                    }
                }

                GUILayout.Space(10);

                if (GUILayout.Button("Skip", GUILayout.Width(120)))
                    currentStep = 3;
            }
            else // Step 3
            {
                if (GUILayout.Button("Finish & Close", GUILayout.Width(140)))
                    Close();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(8);
        }

        // Optional: Menu item to reopen wizard manually
        [MenuItem("MMORPG KIT/MmoKitCE/Show Setup Wizard", false, 10201)]
        private static void ManualOpen()
        {
            SessionState.SetBool(PREF_KEY_SHOWN, false);
            ShowWizard();
        }
    }
}