using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;
using System;
using System.IO;
using System.Collections.Generic;

namespace Defaulter
{
	public class DefaulterData : ScriptableObject
	{
		#region Statics and Constants
		private const bool DEFAULT_TO_UNITY_IMPORT_SETTINGS = true;
		public const string API_IS_PRIVATE_MESSAGE = "Unity has not made this API public";

		/*
		#region Old "JSON" method of loading
		private static string DataPath { get { return Directory.GetParent(Application.dataPath).ToString() + "/ProjectSettings/Defaulter.asset"; } }

		private static DefaulterData _instance;
		public static DefaulterData Instance { get { if (_instance == null) { Load(); } _instance.Validate(); return _instance; } }

		private static void Load()
		{
			_instance = CreateInstance<DefaulterData>();

			if (File.Exists(DataPath))
			{
				JsonUtility.FromJsonOverwrite(File.ReadAllText(DataPath), _instance);
			}
			else
			{
				Debug.LogWarning("[Defaulter] Defaulter data does not exist. Creating new data file in ProjectSettings.");
				_instance.Save();
			}
		}

		internal void Save()
		{
			EditorUtility.SetDirty(this);
			File.WriteAllText(DataPath, JsonUtility.ToJson(Instance));
		}

		internal static void ResetAllData()
		{
			if (File.Exists(DataPath)) { File.Delete(DataPath); }
			_instance = null;
		}
		#endregion
		*/

		/*
		#region Old "Resources" method of loading
		private const string FILENAME = "defaulter_data";
		private const string EDITOR_PATH = "/DefaulterOutput/Resources/Defaulter/";
		private const string RESOURCES_PATH = "Defaulter/" + FILENAME;

		private static DefaulterData _instance;
		public static DefaulterData Instance { get { if (_instance == null) { Load(); } _instance.Validate(); return _instance; } }

		internal static void Load()
		{
			_instance = Resources.Load<DefaulterData>(RESOURCES_PATH);
			if (_instance == null) { _instance = CreateDataObject(); }
		}

		internal void Save()
		{
			EditorUtility.SetDirty(this);
		}

		private static DefaulterData CreateDataObject()
		{
			var data = ScriptableObject.CreateInstance<DefaulterData>();
			string path = Application.dataPath + EDITOR_PATH;
			Directory.CreateDirectory(path);
			path = "Assets" + EDITOR_PATH + FILENAME + ".asset";
			AssetDatabase.CreateAsset(data, path);
			AssetDatabase.SaveAssets();
			Debug.LogWarning("[Defaulter] Data not found, creating new data object at " + path);
			return data;
		}

		internal static void ResetAllData()
		{
			AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(_instance));
			_instance = null;
		}
		#endregion
		*/

		#region ScriptableSingleton method
		private static string DataPath { get { return Directory.GetParent(Application.dataPath).ToString() + "/ProjectSettings/Defaulter.asset"; } }
		private const string OLD_RESOURCES_PATH = "Defaulter/defaulter_data";

		private static DefaulterData _instance;
		public static DefaulterData Instance { get { if (_instance == null) { Load(); } _instance.Validate(); return _instance; } }

		internal static void Load()
		{
			var objects = UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(DataPath);
			for (int i = 0; i < objects.Length; i++) { if (objects[i] is DefaulterData) { _instance = Instantiate<DefaulterData>(objects[i] as DefaulterData); break; } }

			if (_instance == null)
			{

				var oldData = Resources.Load<DefaulterData>(OLD_RESOURCES_PATH);
				if (oldData != null)
				{
					_instance = Instantiate<DefaulterData>(oldData);
					_instance.Save();
					AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(oldData));
				}
				else
				{
					// old data does not exist, create completely fresh
					_instance = CreateDataObject();
				}
			}
		}

		internal void Save()
		{
			this.name = "Defaulter";
			EditorUtility.SetDirty(this);
			UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(new UnityEngine.Object[] { this }, DataPath, true);
		}

		private static DefaulterData CreateDataObject()
		{
			Debug.LogWarning("[Defaulter] Data not found, creating new data object at " + DataPath);

			var data = ScriptableObject.CreateInstance<DefaulterData>();
			data.Save();
			return data;
		}

		internal static void ResetAllData()
		{
			_instance = CreateDataObject();
		}
		#endregion

		#endregion

		#region Fields and Properties
		[Flags]
		public enum ImporterType
		{
			Model = 1,
			Texture = 2,
			Sprite = 4,
			Audio = 8,
			Scene = 16,
			All = Model | Texture | Sprite | Audio | Scene
		}
		public ImporterType currentType = ImporterType.Model;
		public ImporterType safeType = ImporterType.Model;

		public enum ImportMode
		{
			Once = 0,
			Always = 1,
		}

		public DefaulterSettingsData settingsData = new DefaulterSettingsData();
		public bool hasInitialized = false;

		public ModelData modelData = new ModelData();
		public TextureData textureData = new TextureData();
		public SpriteData spriteData = new SpriteData();
		public AudioData audioData = new AudioData();
		public SceneData sceneData = new SceneData();

		public List<ModelData> modelRules = new List<ModelData>();
		public List<TextureData> textureRules = new List<TextureData>();
		public List<SpriteData> spriteRules = new List<SpriteData>();
		public List<AudioData> audioRules = new List<AudioData>();
		public List<SceneData> sceneRules = new List<SceneData>();

		public HelpData helpData = new HelpData();

		[SerializeField]
		internal List<string> _importedAssets = new List<string>();
		#endregion

		internal bool HasAssetBeenImported(string id) { return _importedAssets.Contains(id); }
		internal void AddAssetToDatabase(string id) { _importedAssets.Add(id); Save(); }

		internal void ResetImportedAssets() { _importedAssets.Clear(); }

		private void Validate()
		{
			if (settingsData == null) { settingsData = new DefaulterSettingsData(); }

			modelData.ValidateDefault("All Models");
			textureData.ValidateDefault("All Textures");
			spriteData.ValidateDefault("All Sprites");
			audioData.ValidateDefault("All Audio Clips");
			sceneData.ValidateDefault("All Scenes");

			foreach (var rule in modelRules) { rule.ValidateCustom(); }
			foreach (var rule in textureRules) { rule.ValidateCustom(); }
			foreach (var rule in spriteRules) { rule.ValidateCustom(); }
			foreach (var rule in audioRules) { rule.ValidateCustom(); }
			foreach (var rule in sceneRules) { rule.ValidateCustom(); }
		}

		#region Getters
		internal string GetImporterListName(ImporterType type)
		{
			switch (type)
			{
				case ImporterType.Model: return "modelRules";
				case ImporterType.Texture: return "textureRules";
				case ImporterType.Sprite: return "spriteRules";
				case ImporterType.Audio: return "audioRules";
				case ImporterType.Scene: return "sceneRules";
				case ImporterType.All: return GetImporterListName(ImporterType.Model);
				default: Debug.LogWarning("[Defaulter] Type of '" + type.ToString() + "' has not yet been set up!"); return "";
			}
		}

		internal AbstractData GetImporterDefault(ImporterType type)
		{
			switch (type)
			{
				case ImporterType.Model: return modelData;
				case ImporterType.Texture: return textureData;
				case ImporterType.Sprite: return spriteData;
				case ImporterType.Audio: return audioData;
				case ImporterType.Scene: return sceneData;
				case ImporterType.All: return GetImporterDefault(ImporterType.Model);
				default: Debug.LogWarning("[Defaulter] Type of '" + type.ToString() + "' has not yet been set up!"); return modelData;
			}
		}

		internal IList GetImporterRuleList(ImporterType type)
		{
			switch (type)
			{
				case ImporterType.Model: return modelRules;
				case ImporterType.Texture: return textureRules;
				case ImporterType.Sprite: return spriteRules;
				case ImporterType.Audio: return audioRules;
				case ImporterType.Scene: return sceneRules;
				case ImporterType.All: return GetImporterRuleList(ImporterType.Model);
				default: Debug.LogWarning("[Defaulter] Type of '" + type.ToString() + "' has not yet been set up!"); return modelRules;
			}
		}

		internal AbstractData NewImporterOfType(ImporterType type)
		{
			switch (type)
			{
				case ImporterType.Model: return new ModelData();
				case ImporterType.Texture: return new TextureData();
				case ImporterType.Sprite: return new SpriteData();
				case ImporterType.Audio: return new AudioData();
				case ImporterType.Scene: return new SceneData();
				case ImporterType.All: return NewImporterOfType(ImporterType.Model);
				default: Debug.LogWarning("[Defaulter] Type of '" + type.ToString() + "' has not yet been set up!"); return new ModelData();
			}
		}

		internal AbstractData GetImporter(ImporterType type, int index)
		{
			switch (type)
			{
				case ImporterType.Model: return GetImporterSafe<ModelData>(modelData, modelRules, index);
				case ImporterType.Texture: return GetImporterSafe<TextureData>(textureData, textureRules, index);
				case ImporterType.Sprite: return GetImporterSafe<SpriteData>(spriteData, spriteRules, index);
				case ImporterType.Audio: return GetImporterSafe<AudioData>(audioData, audioRules, index);
				case ImporterType.Scene: return GetImporterSafe<SceneData>(sceneData, sceneRules, index);
				case ImporterType.All: return GetImporter(ImporterType.Model, index);
				default: Debug.LogWarning("[Defaulter] Type of '" + type.ToString() + "' has not yet been set up!"); return GetImporterSafe<ModelData>(modelData, modelRules, index);
			}
		}

		private AbstractData GetImporterSafe<T>(T defaults, List<T> ruleList, int index) where T : AbstractData
		{
			if (index >= ruleList.Count) { index = ruleList.Count - 1; }
			if (index < 0) { return defaults; }
			return ruleList[index];
		}

		#endregion

		#region AbstractData
		[System.Serializable]
		public abstract class AbstractData
		{
			public string ruleName = "NEW RULE";
			public bool isDefault = false;
			public ImportMode importMode = ImportMode.Once;

			public NameContainsCondition nameContainsCondition = new NameContainsCondition();
			public PathContainsCondition pathContainsCondition = new PathContainsCondition();
			public FileExtensionCondition fileExtensionCondition = new FileExtensionCondition();
			public ImageSizeCondition imageSizeCondition = new ImageSizeCondition();

			protected AbstractCondition[] DefaultConditions { get { return new AbstractCondition[] { nameContainsCondition, pathContainsCondition, fileExtensionCondition, imageSizeCondition }; } }
			protected virtual AbstractCondition[] Conditions { get { return DefaultConditions; } }
			public abstract ImporterType Type { get; }

			public AbstractData(bool useUnityDefaults)
			{
				if (useUnityDefaults)
				{
					LoadUnityDefaults();
				}
				else
				{
					LoadRecommendedDefaults();
				}
			}

			public virtual void LoadUnityDefaults() { }
			public virtual void LoadRecommendedDefaults() { }

			public void ValidateDefault(string name)
			{
				isDefault = true;
				ruleName = name;
				importMode = ImportMode.Once;
			}

			public void ValidateCustom()
			{
				isDefault = false;
			}

			public bool Evaluate(AssetImporter importer)
			{
				if (isDefault) { return true; }
				bool allDisabled = true;
				foreach (var condition in Conditions) { if (condition.enabled == true) { allDisabled = false; } }
				if (allDisabled) { Debug.LogWarning("[Defaulter] Custom " + Type + " Rule (" + ruleName + "): \r\nNo conditions are enabled. Rule will not be applied!"); return false; }
				foreach (AbstractCondition condition in Conditions) { if (!condition.SupportsType(Type)) { continue; } if (!condition.Evaluate(importer, Type)) { return false; } }
				return true;
			}

			public void Draw()
			{
				foreach (AbstractCondition condition in Conditions)
				{
					if (!condition.SupportsType(Type)) { condition.enabled = false; continue; }
					condition.DrawAll(); EditorGUILayoutPlus.LineBreak(10);
				}
			}
		}

		#endregion
		#region Conditions
		[System.Serializable]
		public abstract class AbstractCondition
		{
			public abstract string Name { get; }
			public abstract ImporterType SupportedTypes { get; }

			public bool enabled = false;

			internal bool Evaluate(AssetImporter importer, ImporterType type)
			{
				if (!SupportsType(type)) { return false; }
				if (!enabled) { return true; }
				return MeetsCondition(importer, type);
			}

			public bool SupportsType(ImporterType type)
			{
				return (type & SupportedTypes) != 0;
			}

			public void DrawAll()
			{
				GUILayout.BeginHorizontal();
				enabled = GUILayout.Toggle(enabled, GUIContent.none, DefaulterWindow.RadioStyle, GUILayout.Width(12));
				EditorGUI.BeginDisabledGroup(!enabled);
				GUILayout.Label(Name);
				GUILayout.EndHorizontal();
				Draw();
				Validate();
				EditorGUI.EndDisabledGroup();
			}

			internal abstract void Draw();
			protected abstract bool MeetsCondition(AssetImporter importer, ImporterType type);
			internal virtual void Validate() { }
		}

		[System.Serializable]
		public class NameContainsCondition : AbstractCondition
		{
			public override string Name { get { return "Name Contains"; } }
			public override ImporterType SupportedTypes { get { return ImporterType.All; } }

			public string check = "";

			protected override bool MeetsCondition(AssetImporter importer, ImporterType type)
			{
				string[] searchStrings = check.Split(',');
				foreach (var s in searchStrings)
				{
					if (Path.GetFileNameWithoutExtension(importer.assetPath).Contains(s)) { return true; }
				}
				return false;
			}

			internal override void Draw()
			{
				check = EditorGUILayout.TextField(check);
				DefaulterWindow.InfoLabel("separate search strings with a comma");
			}
		}

		[System.Serializable]
		public class PathContainsCondition : AbstractCondition
		{
			public override string Name { get { return "Path Contains"; } }
			public override ImporterType SupportedTypes { get { return ImporterType.All; } }

			public string check = "";

			protected override bool MeetsCondition(AssetImporter importer, ImporterType type)
			{
				string path = importer.assetPath.Replace(Path.GetFileName(importer.assetPath), "");
				string[] searchStrings = check.Split(',');
				foreach (var s in searchStrings)
				{
					if (path.Contains(s)) { return true; }
				}
				return false;
			}

			internal override void Draw()
			{
				check = EditorGUILayout.TextField(check);
				DefaulterWindow.InfoLabel("separate search strings with a comma");
			}
		}

		[System.Serializable]
		public class FileExtensionCondition : AbstractCondition
		{
			public override string Name { get { return "File Extension"; } }
			public override ImporterType SupportedTypes { get { return ImporterType.All; } }

			public string check = "";

			protected override bool MeetsCondition(AssetImporter importer, ImporterType type)
			{
				string extension = Path.GetExtension(importer.assetPath);
				string[] searchStrings = check.Split(',');
				foreach (var s in searchStrings)
				{
					if (extension.Contains(s)) { return true; }
				}
				return false;
			}

			internal override void Draw()
			{
				check = EditorGUILayout.TextField(check);
				DefaulterWindow.InfoLabel("separate search strings with a comma");
			}
		}

		[System.Serializable]
		public class ImageSizeCondition : AbstractCondition
		{
			public override string Name { get { return "Image Size"; } }
			public override ImporterType SupportedTypes { get { return ImporterType.Texture | ImporterType.Sprite; } }

			public bool enableWidthLessThan = false;
			public bool enableHeightLessThan = false;
			public bool enableWidthGreaterThan = false;
			public bool enableHeightGreaterThan = false;

			public int widthLessThan = 1;
			public int heightLessThan = 1;
			public int widthGreaterThan = 2;
			public int heightGreaterThan = 2;

			protected override bool MeetsCondition(AssetImporter importer, ImporterType type)
			{
				Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(importer.assetPath);

				if (!enableWidthLessThan && !enableHeightLessThan && !enableWidthGreaterThan && !enableHeightGreaterThan) { return true; }
				if (enableWidthLessThan) { if (texture.width >= widthLessThan) { return false; } }
				if (enableHeightLessThan) { if (texture.height >= heightLessThan) { return false; } }
				if (enableWidthGreaterThan) { if (texture.width <= widthGreaterThan) { return false; } }
				if (enableHeightGreaterThan) { if (texture.height <= heightGreaterThan) { return false; } }

				return true;
			}

			internal override void Draw()
			{
				DrawRow("Width  <", ref enableWidthLessThan, ref widthLessThan);
				DrawRow("Height <", ref enableHeightLessThan, ref heightLessThan);
				DrawRow("Width  >", ref enableWidthGreaterThan, ref widthGreaterThan);
				DrawRow("Height >", ref enableHeightGreaterThan, ref heightGreaterThan);
			}

			private void DrawRow(string name, ref bool enabled, ref int size)
			{
				GUILayout.BeginHorizontal();
				enabled = EditorGUILayout.Toggle(GUIContent.none, enabled, GUILayout.Width(12));
				EditorGUI.BeginDisabledGroup(!enabled);
				size = EditorGUILayout.IntField(name, size);
				EditorGUI.EndDisabledGroup();
				GUILayout.EndHorizontal();
			}

			internal override void Validate()
			{
				base.Validate();
				if (widthLessThan < 1) { widthLessThan = 1; }
				if (heightLessThan < 1) { heightLessThan = 1; }
				if (widthGreaterThan < 1) { widthGreaterThan = 1; }
				if (heightGreaterThan < 1) { heightGreaterThan = 1; }
			}
		}
		#endregion

		#region Help Data
		[System.Serializable]
		public class HelpData
		{
			public bool override_example = false;
			public bool example = true;
		}
		#endregion

		#region Model Data
		[System.Serializable]
		public class ModelData : AbstractData
		{
			public GameObjectOptions gameObjectOptions;
			public MeshOptions meshOptions;
			public NormalAndTangentOptions normalAndTangentOptions;
			public LightmapOptions lightmapOptions;
			public MaterialOptions materialOptions;
			public RigOptions rigOptions;
			public AnimationOptions animationOptions;

			public override ImporterType Type { get { return ImporterType.Model; } }

			public ModelData() : base(DEFAULT_TO_UNITY_IMPORT_SETTINGS) { }

			public override void LoadUnityDefaults()
			{
				gameObjectOptions = new GameObjectOptions();
				meshOptions = new MeshOptions();
				normalAndTangentOptions = new NormalAndTangentOptions();
				lightmapOptions = new LightmapOptions();
				materialOptions = new MaterialOptions();
				rigOptions = new RigOptions();
				animationOptions = new AnimationOptions();
			}

			public override void LoadRecommendedDefaults()
			{
				LoadUnityDefaults();

				meshOptions.override_globalScale = true;
				meshOptions.globalScale = 1;

				materialOptions.override_importMaterials = true;
				materialOptions.importMaterials = false;

				rigOptions.override_animationType = true;
				rigOptions.animationType = ModelImporterAnimationType.None;

				animationOptions.override_importAnimation = true;
				animationOptions.importAnimation = false;
			}

			[System.Serializable]
			public class GameObjectOptions
			{
				public bool override_tag = false;
				public string tag = "Untagged";

				public bool override_layer = false;
				public int layer = 0;

				public bool override_flags = false;
				public StaticFlags flags = (StaticFlags)0;


				[Flags]
				public enum StaticFlags
				{
					LightmapStatic = 1,
					OccluderStatic = 2,
					BatchingStatic = 4,
					NavigationStatic = 8,
					OccludeeStatic = 16,
					OffMeshLinkGeneration = 32,
					ReflectionProbeStatic = 64
				}
			}

			[System.Serializable]
			public class MeshOptions
			{
				public bool override_globalScale = false;
				public float globalScale = 1;

				public bool override_meshCompression = false;
				public ModelImporterMeshCompression meshCompression = ModelImporterMeshCompression.Off;

				public bool override_isReadable = false;
				public bool isReadable = true;

				public bool override_optimizeMesh = false;
				public bool optimizeMesh = true;

				public bool override_importBlendShapes = false;
				public bool importBlendShapes = true;

				public bool override_addCollider = false;
				public bool addCollider = false;

				[Tooltip(API_IS_PRIVATE_MESSAGE)]
				public bool override_keepQuads = false;
				[Tooltip(API_IS_PRIVATE_MESSAGE)]
				public bool keepQuads = false;

				public bool override_swapUVChannels = false;
				public bool swapUVChannels = false;
			}

			[System.Serializable]
			public class NormalAndTangentOptions
			{
				public bool override_importNormals = false;
				public ModelImporterNormals importNormals = ModelImporterNormals.Import;

				public bool override_normalSmoothingAngle = false;
				[Range(0, 180)]
				public float normalSmoothingAngle = 60;

				public bool override_importTangents = false;
				public ModelImporterTangents importTangents = ModelImporterTangents.CalculateTangentSpace;

				public enum ModelImporterTangents
				{
					//Import = 0,
					CalculateTangentSpace = 3,
					CalculateLegacy = 1,
					CalculateLegacyWithSplitTangents = 4,
					None = 2,
				}
			}

			[System.Serializable]
			public class LightmapOptions
			{
				public bool override_generateSecondaryUV = false;
				public bool generateSecondaryUV = false;

				public bool override_secondaryUVHardAngle = false;
				[Range(0, 180)]
				public int secondaryUVHardAngle = 88;

				public bool override_secondaryUVPackMargin = false;
				[Range(1, 64)]
				public int secondaryUVPackMargin = 4;

				public bool override_secondaryUVAngleDistortion = false;
				[Range(1, 75)]
				public int secondaryUVAngleDistortion = 8;

				public bool override_secondaryUVAreaDistortion = false;
				[Range(1, 75)]
				public int secondaryUVAreaDistortion = 15;
			}

			[System.Serializable]
			public class MaterialOptions
			{
				public bool override_importMaterials = false;
				public bool importMaterials = true;

				public bool override_materialName = false;
				public ModelImporterMaterialName materialName = ModelImporterMaterialName.BasedOnTextureName;

				public bool override_materialSearch = false;
				public ModelImporterMaterialSearch materialSearch = ModelImporterMaterialSearch.RecursiveUp;

				public bool override_defaultMaterial = false;
				public Material defaultMaterial = null;
			}

			[System.Serializable]
			public class RigOptions
			{
				public bool override_animationType = false;
				public ModelImporterAnimationType animationType = ModelImporterAnimationType.Generic;

				public bool override_optimizeGameObjects = false;
				public bool optimizeGameObjects = false;
			}

			[System.Serializable]
			public class AnimationOptions
			{
				public bool override_importAnimation = false;
				public bool importAnimation = true;
			}
		}
		#endregion

		#region Texture Data
		[System.Serializable]
		public class TextureData : AbstractData
		{
			public GeneralOptions general;
			public AlphaOptions alpha;
			public MipMapOptions mipMaps;
			public NormalMapOptions normalMap;
			public QualityOptions quality;

			public override ImporterType Type { get { return ImporterType.Texture; } }

			public TextureData() : base(DEFAULT_TO_UNITY_IMPORT_SETTINGS) { }

			public override void LoadUnityDefaults()
			{
				general = new GeneralOptions();
				alpha = new AlphaOptions();
				mipMaps = new MipMapOptions();
				normalMap = new NormalMapOptions();
				quality = new QualityOptions();
			}

			public override void LoadRecommendedDefaults()
			{
				LoadUnityDefaults();
			}

			[System.Serializable]
			public class GeneralOptions
			{
				public bool override_textureType = false;
				public TextureImporterType textureType = TextureImporterType.Default;

				public bool override_textureShape = false;
				public TextureImporterShape textureShape = TextureImporterShape._2D;

				public bool override_npotScale = false;
				public TextureImporterNPOTScale npotScale = TextureImporterNPOTScale.ToNearest;

				// Read/Write Enabled
				public bool override_isReadable = false;
				public bool isReadable = true;

				// sRGB (Color Texture)
				public bool override_sRGBTexture = false;
				public bool sRGBTexture = true;

				// NOT PUBLIC
				[Tooltip(API_IS_PRIVATE_MESSAGE)]
				public bool override_lightType = false;
				[Tooltip(API_IS_PRIVATE_MESSAGE)]
				public LightType lightType = LightType.Spot;

				public enum TextureImporterType
				{
					Default = 0,
					NormalMap = 1,
					GUI = 2,
					Cubemap = 3,
					Cookie = 4,
					//Advanced = 5,
					Lightmap = 6,
					Cursor = 7,
					Sprite = 8,
					HDRI = 9,
					SingleChannel = 10
				}

				public enum TextureImporterShape
				{
					_2D = 1,
					_3D = 2
				}
			}

			[System.Serializable]
			public class MipMapOptions
			{
				public bool override_mipmapEnabled = false;
				public bool mipmapEnabled = true;

				public bool override_borderMipmap = false;
				public bool borderMipmap = false;

				public bool override_mipmapFilter = false;
				public TextureImporterMipFilter mipmapFilter = TextureImporterMipFilter.BoxFilter;

				public bool override_fadeout = false;
				public bool fadeout = false;

				public bool override_mipmapFadeDistance = false;
				[Range(0, 10)]
				public int mipmapFadeDistanceStart = 1;
				[Range(0, 10)]
				public int mipmapFadeDistanceEnd = 3;
			}

			[System.Serializable]
			public class AlphaOptions
			{
				public bool override_alphaSource = false;
				public TextureImporterAlphaSource alphaSource = TextureImporterAlphaSource.FromInput;

				public bool override_alphaIsTransparency = false;
				public bool alphaIsTransparency = false;

				public enum TextureImporterAlphaSource
				{
					None = 0,
					FromInput = 1,
					FromGrayScale = 2
				}
			}

			[System.Serializable]
			public class NormalMapOptions
			{
				// Create From Grayscale
				public bool override_convertToNormalmap = false;
				public bool convertToNormalmap = false;

				// Bumpiness
				public bool override_heightmapScale = false;
				[Range(0f, 0.3f)]
				public float heightmapScale = 0.25f;

				public bool override_normalmapFilter = false;
				public TextureImporterNormalFilter normalmapFilter = TextureImporterNormalFilter.Standard;
			}

			[System.Serializable]
			public class QualityOptions
			{
				public bool override_wrapMode = false;
				public TextureWrapMode wrapMode = TextureWrapMode.Clamp;

				public bool override_filterMode = false;
				public FilterMode filterMode = FilterMode.Bilinear;

				public bool override_anisoLevel = false;
				[Range(0, 16)]
				public int anisoLevel = 1;

				public bool override_maxTextureSize = false;
				public TextureSize maxTextureSize = TextureSize._2048;

				public bool override_textureCompression = false;
				public Compression textureCompression = Compression.NormalQuality;

				public bool override_textureFormat = false;
				public TextureImporterFormat textureFormat = (TextureImporterFormat)(-1); // Automatic, across Unity versions

				/*
				public bool override_platformSettings = false;
				public TextureImporterPlatformSettings platformSettings = new TextureImporterPlatformSettings();
				*/

				public bool override_crunchedCompression = false;
				public bool crunchedCompression = false;

				public bool override_compressionQuality = false;
				[Range(0, 100)]
				public int compressionQuality = 50;

				public enum TextureSize
				{
					//_16 = 16,
					_32 = 32,
					_64 = 64,
					_128 = 128,
					_256 = 256,
					_512 = 512,
					_1024 = 1024,
					_2048 = 2048,
					_4096 = 4096,
					_8192 = 8192
				}

				public enum Compression
				{
					None = 0,
					NormalQuality = 1,
					HighQuality = 2,
					LowQuality = 3
				}
			}
		}
		#endregion

		#region Sprite Data
		[System.Serializable]
		public class SpriteData : AbstractData
		{
			public SpriteOptions spriteOptions;
			public TextureData.GeneralOptions general;
			public TextureData.AlphaOptions alpha;
			public TextureData.MipMapOptions mipMaps;
			public TextureData.QualityOptions quality;

			public override ImporterType Type { get { return ImporterType.Sprite; } }

			public SpriteData() : base(DEFAULT_TO_UNITY_IMPORT_SETTINGS) { }

			public override void LoadUnityDefaults()
			{
				spriteOptions = new SpriteOptions();
				general = new TextureData.GeneralOptions();
				alpha = new TextureData.AlphaOptions();
				mipMaps = new TextureData.MipMapOptions();
				quality = new TextureData.QualityOptions();

				general.textureType = TextureData.GeneralOptions.TextureImporterType.Sprite;
				quality.anisoLevel = 16;
			}

			public override void LoadRecommendedDefaults()
			{
				LoadUnityDefaults();
			}

			[System.Serializable]
			public class SpriteOptions
			{
				public bool override_spriteImportMode = false;
				public SpriteImportMode spriteImportMode = SpriteImportMode.Single;

				public bool override_spritePackingTag = false;
				public string spritePackingTag = "";

				public bool override_spritePixelsPerUnit = false;
				public float spritePixelsPerUnit = 100;

				// Private
				[Tooltip(API_IS_PRIVATE_MESSAGE)]
				public bool override_spriteMeshType = false;
				[Tooltip(API_IS_PRIVATE_MESSAGE)]
				public SpriteMeshType spriteMeshType = SpriteMeshType.Tight;

				// Private
				[Tooltip(API_IS_PRIVATE_MESSAGE)]
				public bool override_spriteExtrude = false;
				[Range(0, 32), Tooltip(API_IS_PRIVATE_MESSAGE)]
				public uint spriteExtrude = 1;

				public bool override_single = false;
				public SpriteMetaData single = SpriteMetaData.DefaultSingle();

				public bool override_spritesheet = false;
				public SpriteMetaData[] spritesheet;

				public enum SpriteImportMode
				{
					Single = 1,
					Multiple = 2,
					Polygon = 3
				}

				[System.Serializable]
				public class SpriteMetaData
				{
					public string name;
					public Vector2 pivot;
					public Rect rect;
					public Vector4 border;
					[HideInInspector]
					public int alignment;

					public static SpriteMetaData DefaultSingle()
					{
						SpriteMetaData data = new SpriteMetaData();
						data.pivot = new Vector2(0.5f, 0.5f);
						data.border = new Vector4(0, 0, 0, 0);
						return data;
					}
				}
			}
		}
		#endregion

		#region Audio Data
		[System.Serializable]
		public class AudioData : AbstractData
		{
			public GeneralOptions generalOptions;
			public SampleSettingsOptions sampleSettingsOptions;

			public override ImporterType Type { get { return ImporterType.Audio; } }

			public AudioData() : base(DEFAULT_TO_UNITY_IMPORT_SETTINGS) { }

			public override void LoadUnityDefaults()
			{
				generalOptions = new GeneralOptions();
				sampleSettingsOptions = new SampleSettingsOptions();
			}

			public override void LoadRecommendedDefaults()
			{
				LoadUnityDefaults();
			}

			[System.Serializable]
			public class GeneralOptions
			{
				public bool override_forceToMono = false;
				public bool forceToMono = false;

				[Tooltip(API_IS_PRIVATE_MESSAGE)]
				public bool override_normalize = false;
				[Tooltip(API_IS_PRIVATE_MESSAGE)]
				public bool normalize = false;

				public bool override_loadInBackground = false;
				public bool loadInBackground = false;

				public bool override_preloadAudioData = false;
				public bool preloadAudioData = true;
			}

			[System.Serializable]
			public class SampleSettingsOptions
			{
				public bool override_compressionFormat = false;
				public AudioCompressionFormat compressionFormat = AudioCompressionFormat.Vorbis;

				[Tooltip(API_IS_PRIVATE_MESSAGE)]
				public bool override_conversionMode = false;
				[Tooltip(API_IS_PRIVATE_MESSAGE)]
				public int conversionMode;

				public bool override_loadType = false;
				public AudioClipLoadType loadType = AudioClipLoadType.DecompressOnLoad;

				public bool override_quality = false;
				[Range(0, 1)]
				public float quality = 1;

				public bool override_sampleRateSetting = false;
				public AudioSampleRateSetting sampleRateSetting = AudioSampleRateSetting.PreserveSampleRate;

				public bool override_sampleRateOverride = false;
				public SampleRate sampleRateOverride = SampleRate._44100Hz;

				public enum SampleRate
				{
					_8000Hz = 8000,
					_11025Hz = 11025,
					_22050Hz = 22050,
					_44100Hz = 44100,
					_48000Hz = 48000,
					_96000Hz = 96000,
					_192000Hz = 192000
				}
			}
		}
		#endregion

		#region Scene Data
		[System.Serializable]
		public class SceneData : AbstractData
		{
			public EnvironmentLightingOptions environmentLightingOptions;
			public GeneralGIOptions generalGlobalIlluminationOptions;
			public PrecomputedRealtimeGIOptions precomputedRealtimeGlobalIlluminationOptions;
			public BakedGIOptions bakedGlobalIlluminationOptions;
			public FogOptions fogOptions;
			public OtherOptions otherOptions;
			public NavMeshOptions navMeshOptions;

			public override ImporterType Type { get { return ImporterType.Scene; } }

			public SceneData() : base(DEFAULT_TO_UNITY_IMPORT_SETTINGS) { }

			public override void LoadUnityDefaults()
			{
				environmentLightingOptions = new EnvironmentLightingOptions();
				generalGlobalIlluminationOptions = new GeneralGIOptions();
				precomputedRealtimeGlobalIlluminationOptions = new PrecomputedRealtimeGIOptions();
				bakedGlobalIlluminationOptions = new BakedGIOptions();
				fogOptions = new FogOptions();
				otherOptions = new OtherOptions();
				navMeshOptions = new NavMeshOptions();
			}

			public override void LoadRecommendedDefaults()
			{
				LoadUnityDefaults();

				environmentLightingOptions.override_skybox = true;
				environmentLightingOptions.skybox = null;

				environmentLightingOptions.override_ambientMode = true;
				environmentLightingOptions.ambientMode = EnvironmentLightingOptions.AmbientMode.Color;

				generalGlobalIlluminationOptions.override_automaticBaking = true;
				generalGlobalIlluminationOptions.automaticBaking = false;

				precomputedRealtimeGlobalIlluminationOptions.override_lightmappingRealtimeGI = true;
				precomputedRealtimeGlobalIlluminationOptions.lightmappingRealtimeGI = false;

				bakedGlobalIlluminationOptions.override_lightmappingBakedGI = true;
				bakedGlobalIlluminationOptions.lightmappingBakedGI = false;
			}

			[System.Serializable]
			public class EnvironmentLightingOptions
			{
				public bool override_skybox = false;
				public Material skybox = null;

				[Tooltip(API_IS_PRIVATE_MESSAGE)]
				public bool override_sun = false;
				[Tooltip(API_IS_PRIVATE_MESSAGE)]
				public Light sun = null;

				public bool override_ambientMode = false;
				public AmbientMode ambientMode = AmbientMode.Skybox;

				public bool override_ambientLight = false;
				public Color ambientLight = new Color(0.212f, 0.227f, 0.259f, 1);

				public bool override_ambientSkyColor = false;
				public Color ambientSkyColor = new Color(0.212f, 0.227f, 0.259f, 1);

				public bool override_ambientEquatorColor = false;
				public Color ambientEquatorColor = new Color(0.114f, 0.125f, 0.133f, 1);

				public bool override_ambientGroundColor = false;
				public Color ambientGroundColor = new Color(0.047f, 0.043f, 0.035f, 1);

				public bool override_ambientIntensity = false;
				public float ambientIntensity = 1;

				[Tooltip(API_IS_PRIVATE_MESSAGE)]
				public bool override_ambientGI = false;
				[Tooltip(API_IS_PRIVATE_MESSAGE)]
				public AmbientGlobalIllumination ambientGI = AmbientGlobalIllumination.Realtime;

				public bool override_defaultReflectionMode = false;
				public ReflectionMode defaultReflectionMode = ReflectionMode.Skybox;

				public bool override_defaultReflectionResolution = false;
				public ReflectionResolution defaultReflectionResolution = ReflectionResolution._128;

				[Tooltip(API_IS_PRIVATE_MESSAGE)]
				public bool override_reflectionCompression = false;
				[Tooltip(API_IS_PRIVATE_MESSAGE)]
				public ReflectionCompression reflectionCompression = ReflectionCompression.Auto;

				public bool override_reflectionIntensity = false;
				[Range(0f, 1f)]
				public float reflectionIntensity = 1;

				public bool override_reflectionBounces = false;
				[Range(1, 5)]
				public int reflectionBounces = 1;

				public bool override_customReflection = false;
				public Cubemap customReflection = null;

				public enum AmbientMode
				{
					Skybox = 0,
					Gradient = 1,
					Color = 3,
					//Custom = 4
				}

				public enum AmbientGlobalIllumination
				{
					Realtime = 0,
					Baked = 1,
				}

				public enum ReflectionMode
				{
					Skybox = 0,
					Custom = 1
				}

				public enum ReflectionResolution
				{
					_128 = 128,
					_256 = 256,
					_512 = 512,
					_1024 = 1024
				}

				public enum ReflectionCompression
				{
					Uncompressed = 0,
					Compressed = 1,
					Auto = 10,
				}
			}

			[System.Serializable]
			public class GeneralGIOptions
			{
				public bool override_automaticBaking = false;
				public bool automaticBaking = true;

				public bool override_lightmappingIndirectIntensity = false;
				[Range(0f, 5f)]
				public float lightmappingIndirectIntensity = 1;

				public bool override_lightmappingBouceBoost = false;
				[Range(1f, 10f)]
				public float lightmappingBouceBoost = 1;

				public bool override_lightingDataAsset = false;
				public LightingDataQuality lightingDataQuality = LightingDataQuality.Medium;
				public LightingDataAsset customLightingDataAsset = null;

				public enum LightingDataQuality
				{
					Very_Low = 0,
					Low = 1,
					Medium = 2,
					High = 3,
					____ = 4,
					Custom = 5,
				}
			}

			[System.Serializable]
			public class PrecomputedRealtimeGIOptions
			{
				public bool override_lightmappingRealtimeGI = false;
				public bool lightmappingRealtimeGI = true;

				public bool override_texelsPerUnit = false;
				public int texelsPerUnit = 2;

				public bool override_cpuUsage = false;
				public CPUUsage cpuUsage = CPUUsage.Low;

				public enum CPUUsage
				{
					Low = 0,
					Medium = 1,
					High = 2,
					Unlimited = 3
				}
			}

			[System.Serializable]
			public class BakedGIOptions
			{
				public bool override_lightmappingBakedGI = false;
				public bool lightmappingBakedGI = true;

				public bool override_texelsPerUnit = false;
				public int texelsPerUnit = 40;

				public bool override_texelPadding = false;
				public int texelPadding = 2;

				public bool override_compressed = false;
				public bool compressed = true;

				public bool override_ambientOcclusion = false;
				public bool ambientOcclusion = false;

				public bool override_ambientOcclusionMaxDistance = false;
				public int ambientOcclusionMaxDistance = 1;

				public bool override_ambientOcclusionIndirect = false;
				[Range(0f, 10f)]
				public float ambientOcclusionIndirect = 1;

				public bool override_ambientOcclusionDirect = false;
				[Range(0f, 10f)]
				public float ambientOcclusionDirect = 0;

				public bool override_finalGather = false;
				public bool finalGather = false;

				public bool override_finalGatherRayCount = false;
				public int finalGatherRayCount = 256;

				public bool override_finalGatherDenoising = false;
				public bool finalGatherDenoising = true;

				public bool override_atlasSize = false;
				public TextureData.QualityOptions.TextureSize atlasSize = TextureData.QualityOptions.TextureSize._1024;

				public bool override_addDirectLight = false;
				public bool addDirectLight = true;
			}

			[System.Serializable]
			public class FogOptions
			{
				public bool override_fog = false;
				public bool fog = true;

				public bool override_fogColor = false;
				public Color fogColor = new Color32(128, 128, 128, 255);

				public bool override_fogMode = false;
				public FogMode fogMode = FogMode.ExponentialSquared;

				public bool override_fogStartDistance = false;
				public float fogStartDistance = 0f;

				public bool override_fogEndDistance = false;
				public float fogEndDistance = 300f;

				public bool override_fogDensity = false;
				public float fogDensity = 0.01f;
			}

			[System.Serializable]
			public class OtherOptions
			{
				[Tooltip(API_IS_PRIVATE_MESSAGE)]
				public bool override_haloTexture = false;
				[Tooltip(API_IS_PRIVATE_MESSAGE)]
				public Texture2D haloTexture = null;

				public bool override_haloStrength = false;
				[Range(0f, 1f)]
				public float haloStrength = 0.5f;

				public bool override_flareFadeSpeed = false;
				public float flareFadeSpeed = 3f;

				public bool override_flareStrength = false;
				[Range(0f, 1f)]
				public float flareStrength = 1f;

				[Tooltip(API_IS_PRIVATE_MESSAGE)]
				public bool override_spotCookie = false;
				[Tooltip(API_IS_PRIVATE_MESSAGE)]
				public Texture2D spotCookie = null;
			}

			[System.Serializable]
			public class NavMeshOptions
			{
				public bool override_agentRadius = false;
				public float agentRadius = 0.5f;

				public bool override_agentHeight = false;
				public float agentHeight = 2;

				public bool override_agentSlope = false;
				[Range(0, 60)]
				public float agentSlope = 45;

				public bool override_agentClimb = false;
				public float agentClimb = 0.4f;

				public bool override_ledgeDropHeight = false;
				public float ledgeDropHeight = 0;

				public bool override_maxJumpAcrossDistance = false;
				public float maxJumpAcrossDistance = 0;

				public bool override_manualCellSize = false;
				public bool manualCellSize = false;

				public bool override_cellSize = false;
				public float cellSize = 0.1666667f;

				public bool override_minRegionArea = false;
				public float minRegionArea = 2;

				public bool override_accuratePlacement = false;
				public bool accuratePlacement = false;
			}
		}
		#endregion
	}

	[System.Serializable]
	public class DefaulterSettingsData
	{
		public enum BlenderBackupHandleMode { Delete = 0, DeleteSilently = 10, Allow = 10000 }

		public bool showRules = false;

		public bool automaticSpriteConversion = true;

		public BlenderBackupHandleMode blenderBackupHandleMode = BlenderBackupHandleMode.Delete;
	}

}
