using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditorInternal;
using System.Linq;
using System.IO;

namespace Defaulter
{
	[InitializeOnLoad]
	public class DefaulterWindow : EditorWindow
	{
		#region Statics
		private const string VERSION = "0.6.5 (BETA)";

		private const int RULES_WIDTH = 300;
		private const int CANVAS_WIDTH = 425;
		private const int HEIGHT_MIN = 600;
		private const int HEIGHT_MAX = 1000;

		public static void Open()
		{
			DefaulterWindow window = CreateInstance<DefaulterWindow>();
			window.CheckRulesPanel();
			window.titleContent = new GUIContent("Defaulter - Default Importer Settings");
			//window.ShowAuxWindow();
			window.ShowUtility();
		}
		#endregion

		private GUIContent[] TYPE_ICONS = null;
		private GUIContent HELP_ICON = null;
		private DefaulterData.ImporterType[] TABS;

		private SerializedObject _serializedObject;
		private SerializedProperty _serializedProperty;

		private Dictionary<DefaulterData.ImporterType, ReorderableList> _ruleLists;
		private string _propertyGroupName;
		private string _propertyArrayName;

		private static GUIStyle _radioStyle;
		private static GUIStyle _titleStyle;
		private static GUIStyle _toolbarStyle;
		private static GUIStyle _toolbarHelpStyle;

		private Texture2D _icon;
		private Texture2D Icon { get { if (_icon == null) { _icon = Resources.Load<Texture2D>("Defaulter/logo_defaulter"); } return _icon; } }

		public DefaulterData Data { get { return DefaulterData.Instance; } }
		public DefaulterData.AbstractData CurrentImporter { get { return Data.GetImporter(Data.safeType, CurrentRulesList.index); } }

		public float WindowWidth { get { return this.position.width; } }
		public float WindowHeight { get { return this.position.height; } }

		private ReorderableList CurrentRulesList
		{
			get
			{
				SetupRuleLists();
				if (_ruleLists.ContainsKey(Data.currentType))
				{
					return _ruleLists[Data.currentType];
				}

				return _ruleLists[Data.safeType];
			}
		}

		private void SetWindowSizeOpen() { minSize = new Vector2(RULES_WIDTH + CANVAS_WIDTH, HEIGHT_MIN); maxSize = new Vector2(RULES_WIDTH + CANVAS_WIDTH, HEIGHT_MAX); }
		private void SetWindowSizeClosed() { minSize = new Vector2(CANVAS_WIDTH, HEIGHT_MIN); maxSize = new Vector2(CANVAS_WIDTH, HEIGHT_MAX); }
		void OnDestroy() { if (Data != null) { Data.Save(); } }

		#region Setup
		private void Setup()
		{
			if (_serializedObject == null)
			{
				_serializedObject = new SerializedObject(DefaulterData.Instance);
			}

			if (TYPE_ICONS == null)
			{
				TYPE_ICONS = new GUIContent[]
				{
					EditorGUIUtility.IconContent("Mesh Icon"),
					EditorGUIUtility.IconContent("Texture Icon"),
					EditorGUIUtility.IconContent("Sprite Icon"),
					EditorGUIUtility.IconContent("AudioClip Icon"),
					EditorGUIUtility.IconContent("SceneAsset Icon"),
				};
			}

			if (TABS == null)
			{
				TABS = Enum.GetValues(typeof(DefaulterData.ImporterType)).Cast<DefaulterData.ImporterType>().ToArray();
			}

			if (HELP_ICON == null)
			{
				HELP_ICON = EditorGUIUtility.IconContent("console.infoicon");
			}

			SetupStyles();

			SetupRuleLists();
		}
		#endregion

		protected virtual void OnGUI()
		{
			if (!DefaulterData.Instance.hasInitialized)
			{
				DrawSetup();
				return;
			}

			if (!Application.isPlaying)
			{
				Setup();

				_serializedObject.Update();
				DrawCanvas();
				if (Data.settingsData.showRules) { DrawRules(); }
				_serializedObject.ApplyModifiedProperties();

				if (GUI.changed)
				{
					Data.Save();
				}
			}
		}


		#region Setup
		private const float SETUP_BAR_WIDTH = CANVAS_WIDTH - 50;
		private const float SETUP_BAR_HEIGHT = 30;


		private enum SetupState
		{
			Ready,
			Working,
			Complete
		}

		private SetupState _setupState = SetupState.Ready;
		private FileInfo[] _projectFiles;
		private int _setupIndex = 0;
		private Rect _setupLogoRect;
		private Rect _setupProgressRect;
		private Rect _setupInfoRect;
		private Rect _setupContinueRect;

		private void DrawSetup()
		{
			_setupLogoRect = new Rect(0, 0, Icon.width, Icon.height);
			_setupLogoRect.x = (WindowWidth / 2) - (_setupLogoRect.width / 2);
			_setupLogoRect.y = 50;
			GUI.DrawTexture(_setupLogoRect, Icon);

			_setupInfoRect = new Rect(0, _setupLogoRect.yMax + 40, WindowWidth, 20);

			_setupProgressRect = new Rect((WindowWidth / 2) - (SETUP_BAR_WIDTH / 2), _setupInfoRect.yMax + 50, SETUP_BAR_WIDTH, SETUP_BAR_HEIGHT);

			_setupContinueRect = _setupProgressRect;
			_setupContinueRect.y = WindowHeight - _setupContinueRect.height - 50;
			_setupContinueRect.xMin += 50;
			_setupContinueRect.xMax -= 50;

			switch (_setupState)
			{
				case SetupState.Ready: DrawSetupReady(); break;
				case SetupState.Working: DrawSetupWorking(); break;
				case SetupState.Complete: DrawSetupComplete(); break;
			}

			EditorGUI.BeginDisabledGroup(_setupState != SetupState.Complete);
			if (GUI.Button(_setupContinueRect, "Continue"))
			{
				DefaulterData.Instance.hasInitialized = true;
				DefaulterData.Instance.Save();
			}
			EditorGUI.EndDisabledGroup();
		}

		private void DrawSetupReady()
		{
			GUI.Label(_setupInfoRect, "First, Defaulter will need to whitelist all your current assets.", EditorStyles.centeredGreyMiniLabel);
			if (GUI.Button(_setupProgressRect, "Begin Setup"))
			{
				DefaulterData.Instance._importedAssets.Clear();

				DirectoryInfo projectDirectory = new DirectoryInfo(Application.dataPath);
				_projectFiles = projectDirectory.GetFiles("*", SearchOption.AllDirectories);

				_setupState = SetupState.Working;
				EditorApplication.update += OnSetupUpdate;
			}
		}

		private void DrawSetupWorking()
		{
			if (_projectFiles == null)
			{
				_setupState = SetupState.Ready;
				return;
			}

			GUI.Label(_setupInfoRect, "Setup in progress", EditorStyles.centeredGreyMiniLabel);

			float value = (float)_setupIndex / (float)_projectFiles.Length;
			string progressText = "Complete!";
			if (_setupIndex < _projectFiles.Length) { progressText = _projectFiles[_setupIndex].Name; }

			EditorGUI.ProgressBar(_setupProgressRect, value, progressText);
		}

		private void DrawSetupComplete()
		{
			GUI.Label(_setupInfoRect, "Done! Thank you for using Defaulter!", EditorStyles.centeredGreyMiniLabel);

			EditorGUI.ProgressBar(_setupProgressRect, 1, "Complete!");
		}

		private void OnSetupUpdate()
		{
			if (_setupIndex >= _projectFiles.Length)
			{
				EditorApplication.update -= OnSetupUpdate;
				_setupState = SetupState.Complete;
				Repaint();
				return;
			}

			string filePath = _projectFiles[_setupIndex].FullName.Replace("\\", "/").Replace(Application.dataPath, "Assets");
			string guid = AssetDatabase.AssetPathToGUID(filePath);

			if (!string.IsNullOrEmpty(guid))
			{
				if (DefaulterData.Instance._importedAssets.Contains(guid))
				{
					DefaulterData.Instance.AddAssetToDatabase(guid);
				}
			}

			_setupIndex += 1;
			Repaint();
		}

		#endregion

		#region Rules
		private const int RULES_PADDING = 10;
		private const int RULES_CANVAS_HEIGHT = 400;
		private const int TOOLBAR_HEIGHT = 18;

		private Vector2 _ruleSelectorScroll = Vector2.zero;
		private Vector2 _rulesScroll = Vector2.zero;

		#region List
		private void SetupRuleLists()
		{
			if (_ruleLists == null)
			{
				_ruleLists = new Dictionary<DefaulterData.ImporterType, ReorderableList>();
				foreach (var type in Enum.GetValues(typeof(DefaulterData.ImporterType)).Cast<DefaulterData.ImporterType>())
				{
					if (!_ruleLists.ContainsKey(type))
					{
						ReorderableList list = new ReorderableList(_serializedObject, _serializedObject.FindProperty(Data.GetImporterListName(type)), true, false, false, false);

						list.headerHeight = 0;
						list.footerHeight = 0;
						list.showDefaultBackground = false;
						list.drawElementBackgroundCallback += OnDrawElementBackground;
						list.drawElementCallback += OnDrawElement;

						_ruleLists.Add(type, list);
					}
				}
			}
		}

		private void AddCurrentListElement()
		{
			var ruleList = Data.GetImporterRuleList(Data.currentType);
			ruleList.Add(Data.NewImporterOfType(Data.currentType));
			CurrentRulesList.index = ruleList.Count - 1;
			_serializedObject.ApplyModifiedProperties();
			_serializedObject.Update();
		}

		private void RemoveCurrentListElement()
		{
			CurrentRulesList.serializedProperty.DeleteArrayElementAtIndex(CurrentRulesList.index);
			if (CurrentRulesList.index >= CurrentRulesList.count)
			{
				CurrentRulesList.index = CurrentRulesList.serializedProperty.arraySize - 1;
			}
			_serializedObject.ApplyModifiedProperties();
			_serializedObject.Update();
		}

		private void OnDrawElementBackground(Rect rect, int index, bool isActive, bool isFocused)
		{
			rect.xMin += 1;

			if (index < 0) { return; }

			Color c = (index % 2 > 0) ? EditorColors.CHECKER_LIGHT : EditorColors.CHECKER_DARK;
			if (isFocused || index == CurrentRulesList.index) { c = EditorColors.SELECTION; }
			Defaulter.EditorGUIPlus.Solid(rect, c);
		}

		private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			if (index < 0) { return; }

			rect.yMin += 2;
			rect.xMin += 1;

			var property = CurrentRulesList.serializedProperty.GetArrayElementAtIndex(index);
			GUI.Label(rect, property.FindPropertyRelative("ruleName").stringValue);
		}
		#endregion

		private void DrawRules()
		{
			EditorGUI.BeginDisabledGroup(Data.currentType != Data.safeType);

			SetPropertyChild(null);
			Rect r = new Rect(CANVAS_WIDTH, 0, RULES_WIDTH, WindowHeight - RULES_CANVAS_HEIGHT);
			r.xMax -= RULES_PADDING;
			r.yMin += RULES_PADDING;
			r.yMax -= RULES_PADDING;

			GUILayout.BeginArea(r);

			Rect listRect = new Rect(0, 0, RULES_WIDTH - RULES_PADDING - 1, 0);
			listRect.height = CurrentRulesList.GetHeight() + TOOLBAR_HEIGHT + CurrentRulesList.elementHeight - 3;

			// REMOVE THIS
			//EditorGUI.BeginDisabledGroup(true);

			Rect toolbarRect = new Rect(1, 1, listRect.width - 3, TOOLBAR_HEIGHT);
			GUILayout.BeginArea(toolbarRect, EditorStyles.toolbar);
			GUILayout.BeginHorizontal();
			GUILayout.Space(-6);
			GUILayout.Label(Data.safeType.ToString() + " Import Rules");
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(new GUIContent("Add"), EditorStyles.toolbarButton)) { AddCurrentListElement(); }
			EditorGUI.BeginDisabledGroup(CurrentRulesList.index < 0);
			if (GUILayout.Button("Remove", EditorStyles.toolbarButton)) { RemoveCurrentListElement(); }
			EditorGUI.EndDisabledGroup();
			GUILayout.Space(-6);
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
			Defaulter.EditorGUIPlus.Line(new Vector2(toolbarRect.x, toolbarRect.yMax), new Vector2(toolbarRect.xMax, toolbarRect.yMax), EditorColors.SHADOW);

			// THIS TOO
			//EditorGUI.EndDisabledGroup();

			GUILayout.Space(TOOLBAR_HEIGHT + 1);

			_ruleSelectorScroll = GUILayout.BeginScrollView(_ruleSelectorScroll, false, true, GUILayout.Width(toolbarRect.width + 2));

			Rect defaultRect = GUILayoutUtility.GetRect(1, CurrentRulesList.elementHeight);
			bool isDefault = CurrentRulesList.index < 0;
			Color defaultColor = EditorColors.CHECKER_LIGHT;
			if (isDefault) { defaultColor = EditorColors.SELECTION; }
			Defaulter.EditorGUIPlus.Solid(defaultRect, defaultColor);

			defaultRect.yMin += 2;
			defaultRect.xMin += 5;
			GUI.Label(defaultRect, Data.GetImporterDefault(Data.safeType).ruleName);

			if (Event.current.type == EventType.MouseDown && defaultRect.Contains(Event.current.mousePosition))
			{
				CurrentRulesList.index = -1;
				Repaint();
			}
			GUILayout.Space(-2);

			if (CurrentRulesList.count > 0) { CurrentRulesList.DoLayoutList(); }

			GUILayout.EndScrollView();
			listRect.yMax = GUILayoutUtility.GetLastRect().yMax + 1;
			Defaulter.EditorGUIPlus.InsetLineBox(listRect);
			GUILayout.Space(2);
			GUILayout.EndArea();

			r.yMin = r.yMax + RULES_PADDING;
			r.yMax = WindowHeight - RULES_PADDING;
			EditorGUIPlus.OutsetBox(r);

			GUILayout.BeginArea(EditorGUIPlus.InsetRect(r, RULES_PADDING));
			DrawRuleCanvas();
			GUILayout.EndArea();
			EditorGUI.EndDisabledGroup();
		}

		private void DrawRuleCanvas()
		{
			SetPropertyGroup(Data.safeType);
			SetPropertyChild(null);
			EditorGUI.BeginDisabledGroup(CurrentImporter.isDefault);

			var ruleName = _serializedProperty.FindPropertyRelative("ruleName");
			if (ruleName != null) { ruleName.stringValue = EditorGUILayoutPlus.OutlinedTextField(ruleName.stringValue, 16); }
			GUILayout.Space(10);

			GUILayout.BeginHorizontal(EditorStyles.toolbar);
			GUILayout.Space(-3);
			GUILayout.Label("Conditions");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			Rect outline = GUILayoutUtility.GetLastRect();
			_rulesScroll = GUILayout.BeginScrollView(_rulesScroll, false, true);


			CurrentImporter.Draw();

			/*
			float y = outline.y + 20;
			for (int i = 0; i < CurrentImporter.conditions.Count; i++)
			{
				var condition = CurrentImporter.conditions[i];
				Rect r = GUILayoutUtility.GetRect(outline.x, y, outline.width, condition.Height);
				Color c = i % 2 != 0 ? EditorColors.CHECKER_DARK : EditorColors.CHECKER_LIGHT;
				EditorGUIPlus.Solid(r, c);
				condition.Draw(r);
				condition.Validate();
				y += condition.Height;
			}
			*/

			GUILayout.EndScrollView();
			outline.yMax = GUILayoutUtility.GetLastRect().yMax;
			EditorGUIPlus.InsetBox(outline);
			GUILayout.Space(1);

			EditorGUI.EndDisabledGroup();
		}

		private void ToggleRules()
		{
			minSize = new Vector2(0, 0);
			maxSize = new Vector2(2000, 2000);

			bool target = !Data.settingsData.showRules;
			Rect pos = position;
			if (target) { pos.xMax -= RULES_WIDTH; }
			else { pos.xMax += RULES_WIDTH; }
			position = pos;

			Data.settingsData.showRules = target;
			CheckRulesPanel();
			Repaint();

			foreach (var list in _ruleLists.Values) { list.index = -1; }

			Data.Save();

		}

		private void CheckRulesPanel()
		{
			if (Data.settingsData.showRules)
			{
				SetWindowSizeOpen();
			}
			else
			{
				SetWindowSizeClosed();
			}
		}
		#endregion

		#region Canvas
		private const int CANVAS_PADDING = 10;
		private Vector2 _canvasScroll = Vector2.zero;

		private void DrawCanvas()
		{
			Rect r = EditorGUIPlus.InsetRect(new Rect(0, 0, CANVAS_WIDTH, WindowHeight), CANVAS_PADDING);
			GUILayout.BeginArea(r);

			GUILayout.BeginHorizontal(GUILayout.Height(34));
			int currentTab = -1;
			for (int i = 0; i < TABS.Length; i++) { if (i >= TABS.Length) { continue; } if (Data.currentType == TABS[i]) { currentTab = i; break; } }
			int newTab = GUILayout.Toolbar(currentTab, TYPE_ICONS, _toolbarStyle, GUILayout.Width(r.width - 50), GUILayout.Height(_toolbarStyle.fixedHeight));
			if (newTab != currentTab) { Data.currentType = TABS[newTab]; }

			bool showHelp = (int)Data.currentType < 0;
			bool lastShowHelp = showHelp;
			showHelp = GUILayout.Toggle(showHelp, HELP_ICON, _toolbarHelpStyle);
			if (lastShowHelp != showHelp) { Data.currentType = (DefaulterData.ImporterType)(-1); }
			GUILayout.EndHorizontal();

			DrawTabTitle();

			Rect bgRect = new Rect(0, 0, r.width - 1, r.height - 1);
			bgRect.yMin = GUILayoutUtility.GetLastRect().yMax + 1;
			//Defaulter.EditorGUIPlus.BorderlessShadowBox(bgRect);
			Defaulter.EditorGUIPlus.InsetBox(bgRect);

			_canvasScroll = GUILayout.BeginScrollView(_canvasScroll, false, true, GUILayout.Width(r.width - 3));
			switch (Data.currentType)
			{
				default: DrawInstructions(); break;
				case DefaulterData.ImporterType.Model: DrawModelInspector(); break;
				case DefaulterData.ImporterType.Texture: DrawTextureInspector(); break;
				case DefaulterData.ImporterType.Sprite: DrawSpriteInspector(); break;
				case DefaulterData.ImporterType.Audio: DrawAudioInspector(); break;
				case DefaulterData.ImporterType.Scene: DrawSceneInspector(); break;
			}
			GUILayout.EndScrollView();
			GUILayout.Space(3);

			GUILayout.EndArea();
		}

		#region Tab Title
		private void DrawTabTitle()
		{
			string text = "Default ";
			if (CurrentRulesList.index >= 0) { text = ""; }
			switch (Data.currentType)
			{
				case DefaulterData.ImporterType.Model: text += "Model Importer"; break;
				case DefaulterData.ImporterType.Texture: text += "Texture Importer"; break;
				case DefaulterData.ImporterType.Sprite: text += "Sprite Importer"; break;
				case DefaulterData.ImporterType.Audio: text += "Audio Importer"; break;
				case DefaulterData.ImporterType.Scene: text += "Scene Settings"; break;
				default: text = "Defaulter"; break;
			}

			if (CurrentRulesList.index >= 0)
			{
				text += " Rule: " + CurrentImporter.ruleName;
			}

			GUILayout.Space(10);
			GUILayout.Label(text, _titleStyle);

			Rect buttonRect = GUILayoutUtility.GetLastRect();
			buttonRect.height = 25;
			buttonRect.y += 6;
			buttonRect.xMin = buttonRect.xMax - buttonRect.height;
			buttonRect.height -= 5;
			//if (GUI.Button(buttonRect, EditorGUIUtility.IconContent("SettingsIcon"))) { OnSettingsButton(); }
			if (GUI.Button(buttonRect, GUIContent.none, "PaneOptions")) { OnSettingsButton(); }

			//Defaulter.EditorGUILayoutPlus.LineBreak(5);
			GUILayout.Space(10);
		}

		private void OnSettingsButton()
		{
			GenericMenu menu = new GenericMenu();
			menu.AddItem(new GUIContent("Show Custom Importer Rules"), Data.settingsData.showRules, ToggleRules);
			menu.AddSeparator("");
			menu.AddItem(new GUIContent("Load Recommended Defaults"), false, OnSettingsLoadRecommended);
			menu.AddItem(new GUIContent("Load Unity Defaults"), false, OnSettingsLoadUnity);
			menu.AddSeparator("");
			menu.AddItem(new GUIContent("Run Defaulter Setup"), false, () => { Data.hasInitialized = false; });
#if DEFAULTER_DEV
			menu.AddSeparator("");
			menu.AddItem(new GUIContent("Reset Recorded Assets"), false, Data.ResetImportedAssets);
			menu.AddItem(new GUIContent("Reset ALL"), false, DefaulterData.ResetAllData);
#endif
			menu.ShowAsContext();
		}

		private void OnSettingsLoadRecommended()
		{
			Data.GetImporter(Data.safeType, CurrentRulesList.index).LoadRecommendedDefaults();
			Data.Save();
		}

		private void OnSettingsLoadUnity()
		{
			Data.GetImporter(Data.safeType, CurrentRulesList.index).LoadUnityDefaults();
			Data.Save();
		}
		#endregion

		#region Property Drawer
		private void SetPropertyGroup(DefaulterData.ImporterType objectType)
		{
			Data.safeType = objectType;

			switch (objectType)
			{
				case DefaulterData.ImporterType.Model:
					_propertyGroupName = "modelData";
					_propertyArrayName = "modelRules";
					break;

				case DefaulterData.ImporterType.Texture:
					_propertyGroupName = "textureData";
					_propertyArrayName = "textureRules";
					break;

				case DefaulterData.ImporterType.Sprite:
					_propertyGroupName = "spriteData";
					_propertyArrayName = "spriteRules";
					break;

				case DefaulterData.ImporterType.Audio:
					_propertyGroupName = "audioData";
					_propertyArrayName = "audioRules";
					break;

				case DefaulterData.ImporterType.Scene:
					_propertyGroupName = "sceneData";
					_propertyArrayName = "sceneRules";
					break;

				default:
					Debug.LogWarning("[Defaulter] Property group has not been set up for type '" + objectType.ToString() + "'");
					SetPropertyGroup(DefaulterData.ImporterType.Model);
					break;
			}
		}

		private void SetPropertyChild(string childName)
		{
			if (CurrentRulesList.index >= 0)
			{
				_serializedProperty = _serializedObject.FindProperty(_propertyArrayName).GetArrayElementAtIndex((int)Mathf.Clamp(CurrentRulesList.index, 0, _serializedObject.FindProperty(_propertyArrayName).arraySize - 1));
			}
			else
			{
				_serializedProperty = _serializedObject.FindProperty(_propertyGroupName);
			}

			if (!string.IsNullOrEmpty(childName)) { _serializedProperty = _serializedProperty.FindPropertyRelative(childName); }
		}

		private void PropertyField(string name) { PropertyField(name, null, false); }
		private void PropertyField(string name, bool disabled) { PropertyField(name, null, disabled); }
		private void PropertyField(string name, string label) { PropertyField(name, label, false); }
		private void PropertyField(string name, string label, bool disabled)
		{
			BeginOverrideGroup(name, disabled);
			SerializedProperty property = _serializedProperty.FindPropertyRelative(name);
			if (string.IsNullOrEmpty(label))
			{
				EditorGUILayout.PropertyField(property, true);
			}
			else
			{
				EditorGUILayout.PropertyField(property, new GUIContent(label), true);
			}
			EndOverrideGroup();
		}

		private void BeginOverrideGroup(string name) { BeginOverrideGroup(name, false); }
		private void BeginOverrideGroup(string name, bool disabled)
		{
			EditorGUI.BeginDisabledGroup(disabled);

			SerializedProperty overrider = _serializedProperty.FindPropertyRelative("override_" + name);

			GUILayout.BeginHorizontal();

			overrider.boolValue = GUILayout.Toggle(overrider.boolValue, "", _radioStyle, GUILayout.Width(16));
			GUILayout.Space(-6);

			EditorGUI.BeginDisabledGroup(!overrider.boolValue);
		}

		private void EndOverrideGroup()
		{
			EditorGUI.EndDisabledGroup();
			EditorGUI.EndDisabledGroup();
			GUILayout.EndHorizontal();
		}

		private void BeginSimpleOverrideGroup(string name, string label) { BeginSimpleOverrideGroup(name, label, false); }
		private void BeginSimpleOverrideGroup(string name, string label, bool disabled)
		{
			EditorGUI.BeginDisabledGroup(disabled);

			SerializedProperty overrider = _serializedProperty.FindPropertyRelative("override_" + name);

			GUILayout.BeginHorizontal();
			overrider.boolValue = GUILayout.Toggle(overrider.boolValue, "", _radioStyle, GUILayout.Width(16));
			EditorGUI.BeginDisabledGroup(!overrider.boolValue);
			GUILayout.Space(-6);
			GUILayout.Label(label);
			GUILayout.EndHorizontal();
		}

		private void EndSimpleOverrideGroup()
		{
			EditorGUI.EndDisabledGroup();
			EditorGUI.EndDisabledGroup();
		}

		private void DrawImportMode()
		{
			SetPropertyChild(null);
			EditorGUI.BeginDisabledGroup(CurrentImporter.isDefault);
			EditorGUILayout.PropertyField(_serializedProperty.FindPropertyRelative("importMode"));
			GUILayout.Space(-6);
			switch (CurrentImporter.importMode)
			{
				default:
				case DefaulterData.ImportMode.Once:
					InfoLabel("The following settings will only be applied to each new asset of this type once, during the first time they're imported.");
					break;
				case DefaulterData.ImportMode.Always:
					InfoLabel("The following settings will be applied to each assets of this type, every time they're imported, if they meet this rule's conditions.");
					break;
			}
			EditorGUI.EndDisabledGroup();
		}
		#endregion

		#region Instructions
		private void DrawInstructions()
		{
			GroupHeader("How To");
			GUILayout.Space(-6);
			GUI.enabled = false;
			GUILayout.Label(
				@"To override an importer property, toggle the property's corresponding radial button, to the ON state.

This will force all newly imported objects of that type to inherit the enabled properties, instead of Unity's default import settings.

Note:
These settings will only be overridden during the first import of each newly imported asset. Subsequent imports will not override the current import settings, allowing you to change the settings on all imported objects as per usual.

This tool is meant to change only the default import settings of objects.",
				EditorStyles.wordWrappedMiniLabel
			);
			GUI.enabled = true;

			GroupHeader("Example Usage");
			_propertyGroupName = "helpData";
			CurrentRulesList.index = -1;
			SetPropertyChild(null);
			PropertyField("example", "Import Materials");
			GUI.enabled = false;
			bool example = _serializedProperty.FindPropertyRelative("override_example").boolValue;
			bool value = _serializedProperty.FindPropertyRelative("example").boolValue;
			GUILayout.Label("Override is " + (example ? "ON" : "OFF"), EditorStyles.miniLabel);
			GUILayout.Space(-6);
			if (example)
			{
				GUILayout.Label("Default value of property 'Import Materials' will be set to " + value, EditorStyles.miniLabel);
			}
			else
			{
				GUILayout.Label("Default value of property 'Import Materials' will use Unity's default value", EditorStyles.miniLabel);
			}
			GUI.enabled = true;

			GUILayout.FlexibleSpace();
			Rect flexRect = GUILayoutUtility.GetLastRect();
			flexRect.width = CANVAS_WIDTH - (CANVAS_PADDING * 2);

			GUI.enabled = false;
			GUILayout.Space(-6);
			GUILayout.Label("Defaulter v" + VERSION, EditorStyles.miniLabel);
			GUILayout.Space(-6);
			GUILayout.Label("Created by Tony Coculuzzi, 2016", EditorStyles.miniLabel);
			GUI.enabled = true;

			string[] options = new string[] { "Website", "Forum Post", "Support Email" };
			int selection = GUILayout.Toolbar(-1, options, GUILayout.Width(CANVAS_WIDTH - 50));
			switch (selection)
			{
				default: /* do nothing */ break;
				case 0: DefaulterMenuItems.OpenWebsiteLink(); break;
				case 1: DefaulterMenuItems.OpenForumLink(); break;
				case 2: DefaulterMenuItems.OpenEmailLink(); break;
			}
			GUILayout.Space(10);
		}
		#endregion

		#region Types
		#region Model Inspector
		private void DrawModelInspector()
		{
			SetPropertyGroup(DefaulterData.ImporterType.Model);

			EditorGUI.BeginDisabledGroup(!CurrentImporter.isDefault);
			GroupHeader("Destroy Blender Backups");
			GUILayout.Space(-5);
			InfoLabel("Automatically destroy Blender backup files? (.blend1, .blend2, etc)");
			EditorGUILayout.PropertyField(_serializedObject.FindProperty("settingsData.blenderBackupHandleMode"), new GUIContent("Backup Handling Method"), true);
			EditorGUI.EndDisabledGroup();

			EditorGUILayoutPlus.LineBreak(10);

			DrawImportMode();

			DrawModelStaticFlagsOptions();
			DrawModelMeshOptions();
			DrawModelNormalAndTangentOptions();
			DrawModelLightmapOptions();
			DrawModelMaterialOptions();
			DrawModelRigOptions();
			DrawModelAnimationOptions();
		}

		private void DrawModelStaticFlagsOptions()
		{
			SetPropertyChild("gameObjectOptions");

			GroupHeader("Game Object");

			BeginOverrideGroup("tag");
			var tagProperty = _serializedProperty.FindPropertyRelative("tag");
			string[] tags = InternalEditorUtility.tags;
			int tag = 0;
			for (int i = 0; i < tags.Length; i++) { if (tags[i] == tagProperty.stringValue) { tag = i; } }
			tagProperty.stringValue = tags[EditorGUILayout.Popup("Tag", tag, tags)];
			EndOverrideGroup();


			BeginOverrideGroup("layer");
			var layerProperty = _serializedProperty.FindPropertyRelative("layer");
			string[] layerNames = InternalEditorUtility.layers;
			int[] layerValues = layerNames.Select(s => LayerMask.NameToLayer(s)).ToArray();
			layerProperty.intValue = EditorGUILayout.IntPopup("Layer", layerProperty.intValue, layerNames, layerValues);
			GUILayout.Label(layerProperty.intValue.ToString(), GUILayout.Width(20));
			EndOverrideGroup();

			BeginOverrideGroup("flags");
			var flagsProperty = _serializedProperty.FindPropertyRelative("flags");
			DefaulterData.ModelData.GameObjectOptions.StaticFlags flags = (DefaulterData.ModelData.GameObjectOptions.StaticFlags)flagsProperty.intValue;
			EditorGUI.BeginChangeCheck();
			flags = (DefaulterData.ModelData.GameObjectOptions.StaticFlags)EditorGUILayout.EnumMaskPopup("Static Flags", flags);
			if (EditorGUI.EndChangeCheck()) { flagsProperty.intValue = (int)flags; }
			EndOverrideGroup();
		}

		private void DrawModelMeshOptions()
		{
			SetPropertyChild("meshOptions");

			GroupHeader("Meshes");
			PropertyField("globalScale", "Scale Factor");
			PropertyField("meshCompression");
			PropertyField("isReadable", "Read/Write Enabled");
			PropertyField("optimizeMesh");
			PropertyField("importBlendShapes");
			PropertyField("addCollider", "Generate Colliders");
			PropertyField("keepQuads", true);
			PropertyField("swapUVChannels");
		}

		private void DrawModelNormalAndTangentOptions()
		{
			SetPropertyChild("normalAndTangentOptions");
			GroupHeader("Normals & Tangents");

			PropertyField("importNormals", "Normals");
			PropertyField("normalSmoothingAngle", "Smoothing Angle");
			PropertyField("importTangents", "Tangents");
		}

		private void DrawModelLightmapOptions()
		{
			SetPropertyChild("lightmapOptions");
			GroupHeader("Lightmaps");

			PropertyField("generateSecondaryUV", "Generate Lightmap UVs");
			PropertyField("secondaryUVHardAngle", "Hard Angle");
			PropertyField("secondaryUVPackMargin", "Pack Margin");
			PropertyField("secondaryUVAngleDistortion", "Angle Error");
			PropertyField("secondaryUVAreaDistortion", "Area Error");
		}

		private void DrawModelMaterialOptions()
		{
			SetPropertyChild("materialOptions");
			GroupHeader("Materials");

			GUI.enabled = !_serializedProperty.FindPropertyRelative("override_defaultMaterial").boolValue;
			PropertyField("importMaterials");
			GUI.enabled = GUI.enabled && _serializedProperty.FindPropertyRelative("importMaterials").boolValue;
			PropertyField("materialName");
			PropertyField("materialSearch");
			GUI.enabled = true;

			GUILayout.Space(10);
			PropertyField("defaultMaterial");
			GUI.enabled = _serializedProperty.FindPropertyRelative("override_defaultMaterial").boolValue;
			string text = "This value will override the default material on all imported objects";
			SetPropertyChild(null);
			if (!_serializedProperty.FindPropertyRelative("isDefault").boolValue) { text += " that are effected by this rule set."; }
			InfoLabel(text);
			GUI.enabled = true;

			/*
			Material mat = (CurrentImporter as DefaulterData.ModelData).materialOptions.defaultMaterial;
			if (mat != null)
			{
				string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(mat));

				GUILayout.Label(_serializedProperty.FindPropertyRelative("materialOptions.defaultMaterial").objectReferenceInstanceIDValue.ToString());
				GUILayout.Label("GUID: " + guid);
				GUILayout.Label(mat.name);

				GUILayout.Space(10);
				var obj = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(UnityEngine.Object));
				GUILayout.Label("References Object: " + obj.name); 
			}
			else
			{
				GUILayout.Label("null");
			}
			*/
		}

		private void DrawModelRigOptions()
		{
			SetPropertyChild("rigOptions");
			GroupHeader("Rig");

			PropertyField("animationType");
			PropertyField("optimizeGameObjects");
		}


		private void DrawModelAnimationOptions()
		{
			SetPropertyChild("animationOptions");
			GroupHeader("Animations");

			PropertyField("importAnimation");
		}
		#endregion

		#region Texture Inspector
		private void DrawTextureInspector()
		{
			SetPropertyGroup(DefaulterData.ImporterType.Texture);
			DrawImportMode();

			DrawTextureGeneralOptions();
			DrawTextureMipMapOptionsOptions();
			DrawTextureAlphaOptions();
			DrawTextureNormalMapOptionsOptions();
			DrawTextureQualityOptions();
		}

		private void DrawTextureGeneralOptions()
		{
			SetPropertyChild("general");
			GroupHeader("General");

			PropertyField("textureType", "Type");
			PropertyField("textureShape", "Shape");
			PropertyField("npotScale", "Non Power Of Two Scale");
			PropertyField("isReadable", "Read/Write Enabled");
			PropertyField("sRGBTexture", "sRGB Texture (Color)");
			PropertyField("lightType", "Cookie Light Type", true);
		}

		private void DrawTextureMipMapOptionsOptions()
		{
			SetPropertyChild("mipMaps");
			GroupHeader("Mip Maps");

			PropertyField("mipmapEnabled", "Generate Mip Maps");
			PropertyField("borderMipmap", "Border Mip Maps");
			PropertyField("mipmapFilter", "Filter");
			PropertyField("fadeout");

			BeginOverrideGroup("mipmapFadeDistance");
			EditorGUILayout.PrefixLabel("Fade Distance");
			EditorGUI.BeginChangeCheck();
			float min = _serializedProperty.FindPropertyRelative("mipmapFadeDistanceStart").intValue;
			float max = _serializedProperty.FindPropertyRelative("mipmapFadeDistanceEnd").intValue;
			EditorGUILayout.MinMaxSlider(ref min, ref max, 0, 10);
			if (EditorGUI.EndChangeCheck())
			{
				_serializedProperty.FindPropertyRelative("mipmapFadeDistanceStart").intValue = (int)min;
				_serializedProperty.FindPropertyRelative("mipmapFadeDistanceEnd").intValue = (int)max;
			}
			EndOverrideGroup();
		}

		private void DrawTextureAlphaOptions()
		{
			SetPropertyChild("alpha");
			GroupHeader("Alpha");

			PropertyField("alphaSource", "Source");
			PropertyField("alphaIsTransparency");
		}

		private void DrawTextureNormalMapOptionsOptions()
		{
			SetPropertyChild("normalMap");
			GroupHeader("Normal Maps");

			PropertyField("convertToNormalmap", "Create from Grayscale");
			PropertyField("heightmapScale", "Bumpiness");
			PropertyField("normalmapFilter", "Filter");
		}

		private void DrawTextureQualityOptions()
		{
			SetPropertyChild("quality");
			GroupHeader("Quality");

			PropertyField("maxTextureSize");
			PropertyField("wrapMode");
			PropertyField("filterMode");
			PropertyField("anisoLevel");

			GroupHeader("Compression");
			PropertyField("textureCompression", "Compression");
			PropertyField("textureFormat", "Format");
			PropertyField("crunchedCompression", "Use Crunch Compression");
			PropertyField("compressionQuality", "Compressor Quality");
		}
		#endregion

		#region Sprite Inspector
		private void DrawSpriteInspector()
		{
			SetPropertyGroup(DefaulterData.ImporterType.Sprite);

			EditorGUI.BeginDisabledGroup(!CurrentImporter.isDefault);
			GroupHeader("Automatic Sprite Conversion");
			GUILayout.Space(-5);
			InfoLabel("Converts Textures to Sprites if they reside in a folder called \"Sprites\"");
			EditorGUILayout.PropertyField(_serializedObject.FindProperty("settingsData.automaticSpriteConversion"), new GUIContent("Enabled"), true);
			EditorGUI.EndDisabledGroup();

			EditorGUILayoutPlus.LineBreak(10);

			DrawImportMode();

			DrawSpriteOptions();
			DrawSpriteSheetOptions();
			DrawSpriteGeneralOptions();
			DrawTextureAlphaOptions();
			DrawTextureMipMapOptionsOptions();
			DrawTextureQualityOptions();
		}

		private void DrawSpriteOptions()
		{
			SetPropertyChild("spriteOptions");
			GroupHeader("Sprite Options");

			PropertyField("spriteImportMode", "Sprite Mode");
			PropertyField("spritePackingTag", "Packing Tag");
			PropertyField("spritePixelsPerUnit", "Pixels Per Unit");
			PropertyField("spriteMeshType", "Mesh Type", true);
			PropertyField("spriteExtrude", "Extrude Edges", true);
		}

		private void DrawSpriteSheetOptions()
		{
			SetPropertyChild("spriteOptions");
			GroupHeader("Spritesheet");

			bool oldWide = EditorGUIUtility.wideMode;
			//EditorGUIUtility.wideMode = true;

			BeginSimpleOverrideGroup("single", "Single Sprite");
			EditorGUI.indentLevel += 1;
			EditorGUILayout.PropertyField(_serializedProperty.FindPropertyRelative("single.pivot"), true);
			GUILayout.Space(6);
			GUILayout.Label("    Border");
			GUILayout.BeginHorizontal();
			GUILayout.Space(36);
			DrawSpriteBorderProperty("T", "w");
			DrawSpriteBorderProperty("B", "y");
			DrawSpriteBorderProperty("L", "x");
			DrawSpriteBorderProperty("R", "z");
			GUILayout.EndHorizontal();
			EditorGUI.indentLevel -= 1;
			EndSimpleOverrideGroup();

			GUILayout.Space(10);
			PropertyField("spritesheet", "Multiple Sprites");
			EditorGUIUtility.wideMode = oldWide;
		}

		private void DrawSpriteBorderProperty(string label, string propertyName)
		{
			var property = _serializedProperty.FindPropertyRelative("single.border." + propertyName);
			GUILayout.Label(label, GUILayout.ExpandWidth(false));
			GUILayout.Space(-16);
			property.floatValue = EditorGUILayout.IntField((int)property.floatValue);
		}

		private void DrawSpriteGeneralOptions()
		{
			SetPropertyChild("general");
			GroupHeader("General");

			PropertyField("npotScale", "Non Power Of Two Scale");
			PropertyField("isReadable", "Read/Write Enabled");
			PropertyField("sRGBTexture", "sRGB Texture (Color)");
		}
		#endregion

		#region Audio Inspector
		private void DrawAudioInspector()
		{
			SetPropertyGroup(DefaulterData.ImporterType.Audio);
			DrawImportMode();

			SetPropertyChild("generalOptions");
			GroupHeader("General");
			PropertyField("forceToMono");
			PropertyField("normalize", "Normalize Mono", true);
			PropertyField("loadInBackground");
			PropertyField("preloadAudioData");

			SetPropertyChild("sampleSettingsOptions");
			GroupHeader("Sample Settings");
			PropertyField("compressionFormat");
			PropertyField("conversionMode", true);
			PropertyField("loadType");
			PropertyField("quality");
			PropertyField("sampleRateSetting");
			PropertyField("sampleRateOverride");

			GUILayout.Space(10);

			DrawAudioCompressionFormatHelp();
		}

		private void DrawAudioCompressionFormatHelp()
		{
			if (!_serializedProperty.FindPropertyRelative("override_compressionFormat").boolValue) { return; }

			string help = "";
			MessageType type = MessageType.Info;
			AudioCompressionFormat format = (AudioCompressionFormat)_serializedProperty.FindPropertyRelative("compressionFormat").intValue;
			switch (format)
			{
				default:
				case AudioCompressionFormat.Vorbis:
					help = "Raw vorbis format, without Ogg headers. This format is an optimised version of Ogg Vorbis that is more performant.";
					break;

				case AudioCompressionFormat.PCM:
					help = "Uncompressed pulse-code modulation. PCM is uncompressed raw audio data.";
					break;

				case AudioCompressionFormat.ADPCM:
					help = "Adaptive differential pulse-code modulation. This compression format is cheap to decode but contains additional noise artifacts over other compression types.";
					break;

				case AudioCompressionFormat.MP3:
					help = "MPEG Audio Layer III. This codec has poor looping characteristics.";
					break;

				case AudioCompressionFormat.AAC:
					help = "AAC Audio Compression.";
					break;

				case AudioCompressionFormat.VAG:
				case AudioCompressionFormat.HEVAG:
#if UNITY_5_5_OR_NEWER
				case AudioCompressionFormat.ATRAC9:
#endif
					help = "Sony proprietary hardware format. WARNING: This format will not work properly on non-Sony platforms and will CRASH the Unity importer.";
					type = MessageType.Warning;
					break;

				case AudioCompressionFormat.XMA:
					help = "Xbox One proprietary hardware format. WARNING: This format will not work properly on non-Xboxs platforms and will CRASH the Unity importer";
					type = MessageType.Warning;
					break;

				case AudioCompressionFormat.GCADPCM:
					help = "Nintendo ADPCM audio compression format. WARNING: This format will not work properly on non-Nintendo platforms and will CRASH the Unity importer";
					type = MessageType.Warning;
					break;
			}

			EditorGUILayout.HelpBox("Compression Format: \n\n\r\r" + help, type);
		}
		#endregion

		#region Scene Inspector
		private void DrawSceneInspector()
		{
			SetPropertyGroup(DefaulterData.ImporterType.Scene);
			DrawImportMode();

			GroupHeader("Info");
			GUILayout.Space(-6);
			InfoLabel("Scenes act a bit differently than most other assets. They have no defined import pipeline, so these settings will only be applied to a scene that is saved for the first time, while it is the current Active Scene.");

			DrawSceneEnvironmentLightingOptions();
			DrawSceneFogOptions();
			DrawSceneOtherOptions();
			DrawSceneGeneralGIOptions();
			DrawSceneRealtimeGIOptions();
			DrawSceneBakedGIOptions();
			DrawSceneNavMeshOptions();
		}

		private void DrawSceneEnvironmentLightingOptions()
		{
			SetPropertyChild("environmentLightingOptions");
			var sceneImporter = CurrentImporter as DefaulterData.SceneData;

			GroupHeader("Environment Lighting");
			PropertyField("skybox");
			PropertyField("sun", true);

			GroupHeader("Ambient Light");
			PropertyField("ambientGI", true);
			PropertyField("ambientMode");
			EditorGUI.indentLevel += 1;
			if (sceneImporter.environmentLightingOptions.ambientMode == DefaulterData.SceneData.EnvironmentLightingOptions.AmbientMode.Color)
			{
				PropertyField("ambientLight", "Color");
			}
			else if (sceneImporter.environmentLightingOptions.ambientMode == DefaulterData.SceneData.EnvironmentLightingOptions.AmbientMode.Gradient)
			{
				PropertyField("ambientSkyColor", "Sky");
				PropertyField("ambientEquatorColor", "Equator");
				PropertyField("ambientGroundColor", "Ground");
			}
			else
			{
				PropertyField("ambientIntensity", "Intensity");
			}
			EditorGUI.indentLevel -= 1;

			GroupHeader("Reflections");
			PropertyField("defaultReflectionMode", "Reflection Source");
			EditorGUI.indentLevel += 1;
			if (sceneImporter.environmentLightingOptions.defaultReflectionMode == DefaulterData.SceneData.EnvironmentLightingOptions.ReflectionMode.Skybox)
			{
				PropertyField("defaultReflectionResolution", "Resolution");
			}
			else
			{
				PropertyField("customReflection", "Custom");
			}
			PropertyField("reflectionCompression", "Compression");
			EditorGUI.indentLevel -= 1;
			PropertyField("reflectionIntensity", "Intensity");
			PropertyField("reflectionBounces", "Bounces");

		}

		private void DrawSceneGeneralGIOptions()
		{
			SetPropertyChild("generalGlobalIlluminationOptions");
			GroupHeader("Lightmaps");
			PropertyField("automaticBaking");
		}

		private void DrawSceneRealtimeGIOptions()
		{
			SetPropertyChild("precomputedRealtimeGlobalIlluminationOptions");
			GroupHeader("Realtime Global Illumination");

			PropertyField("lightmappingRealtimeGI", "Enabled");
		}

		private void DrawSceneBakedGIOptions()
		{
			SetPropertyChild("bakedGlobalIlluminationOptions");
			GroupHeader("Baked Global Illumination");

			PropertyField("lightmappingBakedGI", "Enabled");
		}

		private void DrawSceneFogOptions()
		{
			var sceneImporter = CurrentImporter as DefaulterData.SceneData;

			SetPropertyChild("fogOptions");
			GroupHeader("Fog");
			PropertyField("fog", "Enabled");
			PropertyField("fogColor", "Color");
			PropertyField("fogMode", "Mode");
			EditorGUI.indentLevel += 1;
			if (sceneImporter.fogOptions.fogMode == FogMode.Linear)
			{
				PropertyField("fogStartDistance");
				PropertyField("fogEndDistance");
			}
			else
			{
				PropertyField("fogDensity");
			}
			EditorGUI.indentLevel -= 1;
		}

		private void DrawSceneOtherOptions()
		{
			SetPropertyChild("otherOptions");
			GroupHeader("Other Lighting Settings");

			InfoLabel("Halos");
			PropertyField("haloTexture", "Texture", true);
			PropertyField("haloStrength", "Strength");

			GUILayout.Space(10);
			InfoLabel("Flares");
			PropertyField("flareFadeSpeed", "Fade Speed");
			PropertyField("flareStrength", "Strength");

			GUILayout.Space(10);
			InfoLabel("Cookies");
			PropertyField("spotCookie", "Spotlight Cookie", true);
		}

		private void DrawSceneNavMeshOptions()
		{
			SetPropertyChild("navMeshOptions");

			GroupHeader("Nav Mesh");
			PropertyField("agentRadius");
			PropertyField("agentHeight");
			PropertyField("agentSlope", "Max Slope");
			PropertyField("agentClimb", "Step Height");
			GUILayout.Space(10);
			InfoLabel("Generated Off Mesh Links");
			PropertyField("ledgeDropHeight", "Drop Height");
			PropertyField("maxJumpAcrossDistance", "Jump Distance");
			GUILayout.Space(10);
			InfoLabel("Advanced");
			PropertyField("manualCellSize", "Manual Voxel Size");
			EditorGUI.indentLevel += 1;
			PropertyField("cellSize", "Voxel Size");
			EditorGUI.indentLevel -= 1;
			PropertyField("minRegionArea");
			PropertyField("accuratePlacement", "Height Mesh");
		}
		#endregion
		#endregion
		#endregion

		#region GUI
		public static GUIStyle RadioStyle { get { SetupStyles(); return _radioStyle; } }

		private static void SetupStyles()
		{
			if (_titleStyle == null)
			{
				_titleStyle = new GUIStyle("label");
				_titleStyle.fontSize = 16;
				_titleStyle.fontStyle = FontStyle.Normal;
			}

			if (_radioStyle == null)
			{
				_radioStyle = new GUIStyle(EditorStyles.radioButton);
				RectOffset margin = _radioStyle.margin;
				margin.top -= 2;
				_radioStyle.margin = margin;
			}

			if (_toolbarStyle == null)
			{
				_toolbarStyle = new GUIStyle("button");
				_toolbarStyle.padding = new RectOffset(0, 0, 0, 0);
				_toolbarStyle.border = new RectOffset(0, 0, 0, 0);
				_toolbarStyle.margin = new RectOffset(0, 0, 0, 0);
				_toolbarStyle.fixedHeight = 34;
				_toolbarStyle.stretchHeight = false;
			}

			if (_toolbarHelpStyle == null)
			{
				_toolbarHelpStyle = new GUIStyle("button");
				_toolbarHelpStyle.padding = new RectOffset(0, 0, 0, 0);
				_toolbarHelpStyle.margin = new RectOffset(5, 0, 0, -5);
				_toolbarHelpStyle.fixedHeight = _toolbarStyle.fixedHeight;
				_toolbarHelpStyle.stretchHeight = _toolbarStyle.stretchHeight;
			}
		}

		internal static void GroupHeader(string text)
		{
			SetupStyles();
			GUILayout.Space(5);
			GUILayout.Label(text, EditorStyles.boldLabel);
		}

		internal static void InfoLabel(string text)
		{
			SetupStyles();
			Color lastColor = GUI.color;
			GUI.color = lastColor * new Color(1, 1, 1, 0.5f);
			GUILayout.Label(text, EditorStyles.wordWrappedMiniLabel);
			GUI.color = lastColor;
		}
		#endregion
	}
}