using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

namespace Defaulter
{
	public class DefaulterAssetPostProcessor : AssetPostprocessor
	{
		/*
		public MonoImporter MonoImporter { get { return this.assetImporter as MonoImporter; } }
		public MovieImporter MovieImporter { get { return this.assetImporter as MovieImporter; } }
		public PluginImporter PluginImporter { get { return this.assetImporter as PluginImporter; } }
		public ShaderImporter ShaderImporter { get { return this.assetImporter as ShaderImporter; } }
		public SketchUpImporter SketchUpImporter { get { return this.assetImporter as SketchUpImporter; } }
		public SpeedTreeImporter SpeedTreeImporter { get { return this.assetImporter as SpeedTreeImporter; } }
		public SubstanceImporter SubstanceImporter { get { return this.assetImporter as SubstanceImporter; } }
		public TrueTypeFontImporter TrueTypeFontImporter { get { return this.assetImporter as TrueTypeFontImporter; } }
		
		
		protected virtual void OnPostprocessAssetbundleNameChanged(string assetPath, string previousAssetBundleName, string newAssetBundleName) { }
		protected virtual void OnPostprocessGameObjectWithUserProperties(GameObject gameObject, string[] propNames, System.Object[] values) { }
		protected virtual void OnPostprocessSpeedTree(GameObject gameObject) { }
		protected virtual void OnPreprocessAnimation() { }
		protected virtual void OnPreprocessSpeedTree() { }
		*/

		public override int GetPostprocessOrder() { return int.MinValue; }
		public string AssetID { get { return AssetDatabase.AssetPathToGUID(assetImporter.assetPath); } }

		public AudioImporter AudioImporter { get { return this.assetImporter as AudioImporter; } }
		public ModelImporter ModelImporter { get { return this.assetImporter as ModelImporter; } }
		public TextureImporter TextureImporter { get { return this.assetImporter as TextureImporter; } }

		private static bool Ready { get { return DefaulterData.Instance.hasInitialized; } }

		private bool HasAssetBeenImported() { return HasAssetBeenImported(AssetID); }
		private static bool HasAssetBeenImported(string id)
		{
			bool output = DefaulterData.Instance.HasAssetBeenImported(id);
			//Log("Has asset been imported before?\n\r" + output + ": " + id);
			return output;
		}

		private void AddAssetToDatabase() { AddAssetToDatabase(AssetID); }
		private static void AddAssetToDatabase(string id)
		{
			Log("Adding asset to database:\r\n" + id);
			DefaulterData.Instance.AddAssetToDatabase(id);
		}

		private void LogRule(DefaulterData.AbstractData data) { LogRule(data, AssetID); }
		private static void LogRule(DefaulterData.AbstractData data, string guid)
		{
			Log("Applying " + data.Type.ToString() + " Settings: " + data.ruleName + " (ImportMode." + data.importMode + ")\n\r" + guid);
		}

		private static void Log(string log)
		{
#if DEFAULTER_DEV
			Debug.Log("[Defaulter] " + log + "\n\r", DefaulterData.Instance);
#endif
		}

#if UNITY_5_5_OR_NEWER
		protected static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			if (!Ready) { return; }
			foreach (var item in importedAssets)
			{
				// Scenes
				if (item.Contains(".unity")) { EditorApplication.delayCall += () => { OnPreprocessScene(item); }; continue; }

				// Blender backups
				if (DefaulterData.Instance.settingsData.blenderBackupHandleMode != DefaulterSettingsData.BlenderBackupHandleMode.Allow)
				{
					bool isBackup = IsBlenderBackup(item);
					// Log("Check for Blender backup: " + item + "\r\n" + isBackup.ToString() + ", " + (isBackup ? "DELETING!" : "ignoring"));
					if (isBackup)
					{
						if (DefaulterData.Instance.settingsData.blenderBackupHandleMode != DefaulterSettingsData.BlenderBackupHandleMode.DeleteSilently)
						{
							Debug.LogWarning("[Defaulter] File '<color=grey>" + item + "</color>' is a Blender backup. Deleting.\r\n<color=grey>This message can be disabled in Defaulter settings.</color>");
						}
						AssetDatabase.DeleteAsset(item);
						continue;
					}
				}
			}
		}
#endif

		#region Models
		protected virtual void OnPreprocessModel()
		{
			if (!Ready) { return; }
			ApplyModelSettings(DefaulterData.Instance.modelData);
			foreach (var rule in DefaulterData.Instance.modelRules) { ApplyModelSettings(rule); }
		}

		protected virtual void OnPostprocessModel(GameObject gameObject)
		{
			if (!Ready) { return; }
			ApplyModelGameObjectSettings(gameObject, DefaulterData.Instance.modelData);
			foreach (var rule in DefaulterData.Instance.modelRules) { ApplyModelGameObjectSettings(gameObject, rule); }
			if (!HasAssetBeenImported()) { AddAssetToDatabase(); }
			Selection.activeObject = gameObject;
		}

		protected virtual Material OnAssignMaterialModel(Material previousMaterial, Renderer renderer)
		{
			if (!Ready) { return null; }

			Log("Previous Material: " + previousMaterial.name);

			Material output = null;
			GetModelMaterial(DefaulterData.Instance.modelData, ref output);
			foreach (var rule in DefaulterData.Instance.modelRules) { GetModelMaterial(rule, ref output); }

			if (output != null) { Log("Assigning Material: " + output.name + "\n\r" + AssetID); }
			return output;
		}

		private void GetModelMaterial(DefaulterData.ModelData modelData, ref Material output)
		{
			if (modelData.materialOptions.override_defaultMaterial) { output = modelData.materialOptions.defaultMaterial; }
		}

		private void ApplyModelGameObjectSettings(GameObject gameObject, DefaulterData.ModelData modelData)
		{
			if ((modelData.isDefault || modelData.importMode == DefaulterData.ImportMode.Once) && HasAssetBeenImported()) { return; }
			if (!modelData.Evaluate(ModelImporter)) { return; }
			LogRule(modelData);

			#region GameObject
			var go = modelData.gameObjectOptions;
			if (go.override_tag) { SetTagRecursively(gameObject, go.tag); }
			if (go.override_layer) { SetLayerRecursively(gameObject, go.layer); }
			if (go.override_flags) { SetFlagsRecursively(gameObject, (StaticEditorFlags)go.flags); }
			#endregion
		}

		private void SetFlagsRecursively(GameObject gameObject, StaticEditorFlags flags)
		{
			GameObjectUtility.SetStaticEditorFlags(gameObject, flags);
			foreach (Transform child in gameObject.transform) { SetFlagsRecursively(child.gameObject, flags); }
		}

		private void SetLayerRecursively(GameObject gameObject, int layer)
		{
			gameObject.layer = layer;
			foreach (Transform child in gameObject.transform) { SetLayerRecursively(child.gameObject, layer); }
		}

		private void SetTagRecursively(GameObject gameObject, string tag)
		{
			gameObject.tag = tag;
			foreach (Transform child in gameObject.transform) { SetTagRecursively(child.gameObject, tag); }
		}

		private void ApplyModelSettings(DefaulterData.ModelData modelData)
		{
			if ((modelData.isDefault || modelData.importMode == DefaulterData.ImportMode.Once) && HasAssetBeenImported()) { return; }
			if (!modelData.Evaluate(ModelImporter)) { return; }
			LogRule(modelData);


			#region Mesh Options
			var mesh = modelData.meshOptions;
			if (mesh.override_globalScale) { ModelImporter.globalScale = mesh.globalScale; }
			if (mesh.override_meshCompression) { ModelImporter.meshCompression = mesh.meshCompression; }
			if (mesh.override_isReadable) { ModelImporter.isReadable = mesh.isReadable; }
			if (mesh.override_optimizeMesh) { ModelImporter.optimizeMesh = mesh.optimizeMesh; }
			if (mesh.override_importBlendShapes) { ModelImporter.importBlendShapes = mesh.importBlendShapes; }
			if (mesh.override_addCollider) { ModelImporter.addCollider = mesh.addCollider; }
			if (mesh.override_keepQuads) { /* No public APIs */ }
			if (mesh.override_swapUVChannels) { ModelImporter.swapUVChannels = mesh.swapUVChannels; }
			#endregion

			#region Normals and Tangent Options
			var normals = modelData.normalAndTangentOptions;
			if (normals.override_importNormals) { ModelImporter.importNormals = normals.importNormals; }
			if (normals.override_normalSmoothingAngle) { ModelImporter.normalSmoothingAngle = normals.normalSmoothingAngle; }
			if (normals.override_importTangents) { ModelImporter.importTangents = (ModelImporterTangents)((int)normals.importTangents); }
			#endregion

			#region Lightmap Options
			var lightmap = modelData.lightmapOptions;
			if (lightmap.override_generateSecondaryUV) { ModelImporter.generateSecondaryUV = lightmap.generateSecondaryUV; }
			if (lightmap.override_secondaryUVHardAngle) { ModelImporter.secondaryUVHardAngle = lightmap.secondaryUVHardAngle; }
			if (lightmap.override_secondaryUVPackMargin) { ModelImporter.secondaryUVPackMargin = lightmap.secondaryUVPackMargin; }
			if (lightmap.override_secondaryUVAngleDistortion) { ModelImporter.secondaryUVAngleDistortion = lightmap.secondaryUVAngleDistortion; }
			if (lightmap.override_secondaryUVAreaDistortion) { ModelImporter.secondaryUVAreaDistortion = lightmap.secondaryUVAreaDistortion; }
			#endregion

			#region Material Options
			var materials = modelData.materialOptions;
			if (materials.override_defaultMaterial)
			{
				ModelImporter.importMaterials = false;
			}
			else
			{
				if (materials.override_importMaterials) { ModelImporter.importMaterials = materials.importMaterials; }
				if (materials.override_materialName) { ModelImporter.materialName = materials.materialName; }
				if (materials.override_materialSearch) { ModelImporter.materialSearch = materials.materialSearch; }
			}
			#endregion

			#region Rig Options
			var rig = modelData.rigOptions;
			if (rig.override_animationType) { ModelImporter.animationType = rig.animationType; }
			if (rig.override_optimizeGameObjects) { ModelImporter.optimizeGameObjects = rig.optimizeGameObjects; }
			#endregion

			#region Animation Options
			var anim = modelData.animationOptions;
			if (anim.override_importAnimation) { ModelImporter.importAnimation = anim.importAnimation; }
			#endregion
		}

		private static bool IsBlenderBackup(string item)
		{
			const int BACKUP_CHECKS = 9;
			for (int i = 0; i < BACKUP_CHECKS; i++)
			{
				if (item.Substring(item.Length - 7) == ".blend" + i) { return true; }
			}
			return false;
		}
		#endregion

		#region Textures
		protected virtual void OnPreprocessTexture()
		{
			if (!Ready) { return; }
			if (TextureImporter.textureType != TextureImporterType.Sprite && TextureImporter.assetPath.ToLower().Contains(SPRITE_PATH_CHECK) && DefaulterData.Instance.settingsData.automaticSpriteConversion) { TextureImporter.textureType = TextureImporterType.Sprite; }
			if (TextureImporter.textureType == TextureImporterType.Sprite) { OnPreprocessSprite(); return; }

			ApplyTextureSettings(DefaulterData.Instance.textureData);
			foreach (var rule in DefaulterData.Instance.textureRules) { ApplyTextureSettings(rule); }
		}

		protected virtual void OnPostprocessTexture(Texture2D texture)
		{
			if (!Ready) { return; }
			if (!HasAssetBeenImported()) { AddAssetToDatabase(); }
		}

		private void ApplyTextureSettings(DefaulterData.TextureData textureData)
		{
			if ((textureData.isDefault || textureData.importMode == DefaulterData.ImportMode.Once) && HasAssetBeenImported()) { return; }
			if (!textureData.Evaluate(TextureImporter)) { return; }
			LogRule(textureData);

			#region General Options
			ApplyTextureGeneralSettings(textureData.general);
			#endregion

			#region Mip Map Options
			ApplyTextureMipMapSettings(textureData.mipMaps);
			#endregion

			#region Alpha Options
			ApplyTextureAlphaSettings(textureData.alpha);
			#endregion

			#region Normal Map Options
			var normal = textureData.normalMap;
			if (normal.override_convertToNormalmap) { TextureImporter.convertToNormalmap = normal.convertToNormalmap; }
			if (normal.override_heightmapScale) { TextureImporter.heightmapScale = normal.heightmapScale; }
			if (normal.override_normalmapFilter) { TextureImporter.normalmapFilter = normal.normalmapFilter; }
			#endregion

			#region Quality Options
			ApplyTextureQualitySettings(textureData.quality);
			#endregion
		}

		private void ApplyTextureGeneralSettings(DefaulterData.TextureData.GeneralOptions general)
		{
			if (general.override_textureType) { TextureImporter.textureType = (TextureImporterType)((int)general.textureType); }
			if (general.override_npotScale) { TextureImporter.npotScale = general.npotScale; }
			if (general.override_isReadable) { TextureImporter.isReadable = general.isReadable; }
			if (general.override_lightType) { /* No public APIs */ }

#if UNITY_5_5_OR_NEWER
			if (general.override_textureShape) { TextureImporter.textureShape = (TextureImporterShape)((int)general.textureShape); }
			if (general.override_sRGBTexture) { TextureImporter.sRGBTexture = general.sRGBTexture; }
#endif
		}

		private void ApplyTextureQualitySettings(DefaulterData.TextureData.QualityOptions quality)
		{
			if (quality.override_wrapMode) { TextureImporter.wrapMode = quality.wrapMode; }
			if (quality.override_filterMode) { TextureImporter.filterMode = quality.filterMode; }
			if (quality.override_anisoLevel) { TextureImporter.anisoLevel = quality.anisoLevel; }
			if (quality.override_maxTextureSize) { TextureImporter.maxTextureSize = (int)quality.maxTextureSize; }
			if (quality.override_compressionQuality) { TextureImporter.compressionQuality = quality.compressionQuality; }
#if UNITY_5_5_OR_NEWER
			if (quality.override_crunchedCompression) { TextureImporter.crunchedCompression = quality.crunchedCompression; }
			if (quality.override_textureCompression) { TextureImporter.textureCompression = (TextureImporterCompression)((int)quality.textureCompression); }

			// Sort this out
			if (quality.override_textureFormat) { TextureImporter.textureFormat = quality.textureFormat; }
#else
			if (quality.override_textureFormat) { TextureImporter.textureFormat = quality.textureFormat; }
#endif

		}

		private void ApplyTextureMipMapSettings(DefaulterData.TextureData.MipMapOptions mipmaps)
		{
			if (mipmaps.override_mipmapEnabled) { TextureImporter.mipmapEnabled = mipmaps.mipmapEnabled; }
			if (mipmaps.override_borderMipmap) { TextureImporter.borderMipmap = mipmaps.borderMipmap; }
			if (mipmaps.override_mipmapFilter) { TextureImporter.mipmapFilter = mipmaps.mipmapFilter; }
			if (mipmaps.override_fadeout) { TextureImporter.fadeout = mipmaps.fadeout; }
			if (mipmaps.override_mipmapFadeDistance)
			{
				TextureImporter.mipmapFadeDistanceStart = mipmaps.mipmapFadeDistanceStart;
				TextureImporter.mipmapFadeDistanceEnd = mipmaps.mipmapFadeDistanceEnd;
			}
		}

		private void ApplyTextureAlphaSettings(DefaulterData.TextureData.AlphaOptions alpha)
		{
			if (alpha.override_alphaIsTransparency) { TextureImporter.alphaIsTransparency = alpha.alphaIsTransparency; }
#if UNITY_5_5_OR_NEWER
			if (alpha.override_alphaSource) { TextureImporter.alphaSource = (TextureImporterAlphaSource)((int)alpha.alphaSource); }
#else
			if (alpha.override_alphaSource)
			{
				if (alpha.alphaSource == DefaulterData.TextureData.AlphaOptions.TextureImporterAlphaSource.FromGrayScale) { TextureImporter.alphaIsTransparency = false; TextureImporter.grayscaleToAlpha = true; }
				if (alpha.alphaSource == DefaulterData.TextureData.AlphaOptions.TextureImporterAlphaSource.FromInput) { TextureImporter.alphaIsTransparency = true; TextureImporter.grayscaleToAlpha = false; }
				if (alpha.alphaSource == DefaulterData.TextureData.AlphaOptions.TextureImporterAlphaSource.None) { TextureImporter.alphaIsTransparency = false; TextureImporter.grayscaleToAlpha = false; }
			}
#endif
		}
		#endregion

		#region Sprites
		private const string SPRITE_PATH_CHECK = "/sprites/";
		private void OnPreprocessSprite()
		{
			if (!Ready) { return; }
			ApplySpriteSettings(DefaulterData.Instance.spriteData);
			foreach (var rule in DefaulterData.Instance.spriteRules) { ApplySpriteSettings(rule); }
		}

		//protected virtual void OnPostprocessSprites(Texture2D texture, Sprite[] sprites) { }

		private void ApplySpriteSettings(DefaulterData.SpriteData spriteData)
		{
			if ((spriteData.isDefault || spriteData.importMode == DefaulterData.ImportMode.Once) && HasAssetBeenImported()) { return; }
			if (!spriteData.Evaluate(TextureImporter)) { return; }
			LogRule(spriteData);

			#region General Options
			ApplyTextureGeneralSettings(spriteData.general);
			#endregion

			#region Mip Map Options
			ApplyTextureMipMapSettings(spriteData.mipMaps);
			#endregion

			#region Alpha Options
			ApplyTextureAlphaSettings(spriteData.alpha);
			#endregion

			#region Quality Options
			ApplyTextureQualitySettings(spriteData.quality);
			#endregion

			#region Sprite Options
			var sprite = spriteData.spriteOptions;
			if (sprite.override_spriteImportMode) { TextureImporter.spriteImportMode = (SpriteImportMode)((int)sprite.spriteImportMode); }
			if (sprite.override_spritePackingTag) { TextureImporter.spritePackingTag = sprite.spritePackingTag; }
			if (sprite.override_spritePixelsPerUnit) { TextureImporter.spritePixelsPerUnit = sprite.spritePixelsPerUnit; }
			if (sprite.override_spriteMeshType) { /* No public APIs */ }
			if (sprite.override_spriteExtrude) { /* No public APIs */ }
			if (sprite.override_single)
			{
				TextureImporter.spriteBorder = sprite.single.border;
				TextureImporter.spritePivot = sprite.single.pivot;
			}
			if (sprite.override_spritesheet)
			{
				var sprites = new SpriteMetaData[sprite.spritesheet.Length];
				for (int i = 0; i < sprite.spritesheet.Length; i++)
				{
					sprites[i].name = Path.GetFileNameWithoutExtension(TextureImporter.assetPath) + ":" + sprite.spritesheet[i].name;
					sprites[i].rect = sprite.spritesheet[i].rect;
					sprites[i].border = sprite.spritesheet[i].border;
					sprites[i].pivot = sprite.spritesheet[i].pivot;
					// Not sure what this does
					//sprites[i].alignment = sprite.spritesheet[i].alignment;
				}

				TextureImporter.spritesheet = sprites;
			}
			#endregion
		}
		#endregion

		#region Audio
		protected virtual void OnPreprocessAudio()
		{
			if (!Ready) { return; }
			ApplyAudioSettings(DefaulterData.Instance.audioData);
			foreach (var rule in DefaulterData.Instance.audioRules) { ApplyAudioSettings(rule); }
		}

		protected virtual void OnPostprocessAudio(AudioClip audioClip)
		{
			if (!Ready) { return; }
			if (!HasAssetBeenImported()) { AddAssetToDatabase(); }
		}

		private void ApplyAudioSettings(DefaulterData.AudioData audioData)
		{
			if ((audioData.isDefault || audioData.importMode == DefaulterData.ImportMode.Once) && HasAssetBeenImported()) { return; }
			if (!audioData.Evaluate(AudioImporter)) { return; }
			LogRule(audioData);

			#region General Options
			var general = audioData.generalOptions;
			if (general.override_forceToMono) { AudioImporter.forceToMono = general.forceToMono; }
			if (general.override_normalize) { /* No public APIs */ }
			if (general.override_loadInBackground) { AudioImporter.loadInBackground = general.loadInBackground; }
			if (general.override_preloadAudioData) { AudioImporter.preloadAudioData = general.preloadAudioData; }
			#endregion

			#region Sample Options
			var sample = audioData.sampleSettingsOptions;
			AudioImporterSampleSettings sampleSettings = AudioImporter.defaultSampleSettings;

			if (sample.override_compressionFormat) { sampleSettings.compressionFormat = sample.compressionFormat; }
			if (sample.override_conversionMode) { /* No public APIs */ }
			if (sample.override_loadType) { sampleSettings.loadType = sample.loadType; }
			if (sample.override_quality) { sampleSettings.quality = sample.quality; }
			if (sample.override_sampleRateSetting) { sampleSettings.sampleRateSetting = sample.sampleRateSetting; }
			if (sample.override_sampleRateOverride) { sampleSettings.sampleRateOverride = (uint)sample.sampleRateOverride; }

			AudioImporter.defaultSampleSettings = sampleSettings;
			#endregion
		}
		#endregion

		#region Scenes
		private static void OnPreprocessScene(string scenePath)
		{
			for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
			{
				var openedScene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

				if ((scenePath == openedScene.path))
				{
					string guid = AssetDatabase.AssetPathToGUID(scenePath);
					var sceneImporter = AssetImporter.GetAtPath(scenePath);

					ApplySceneSettings(DefaulterData.Instance.sceneData, sceneImporter, guid);
					foreach (var rule in DefaulterData.Instance.sceneRules) { ApplySceneSettings(rule, sceneImporter, guid); }

					if (!HasAssetBeenImported(guid)) { AddAssetToDatabase(guid); }
				}
			}
		}

		private static void ApplySceneSettings(DefaulterData.SceneData sceneData, AssetImporter sceneImporter, string guid)
		{
			if ((sceneData.isDefault || sceneData.importMode == DefaulterData.ImportMode.Once) && HasAssetBeenImported(guid)) { return; }
			if (!sceneData.Evaluate(sceneImporter)) { return; }
			LogRule(sceneData, guid);

			Debug.LogWarning("[Defaulter] Applying default Scene settings to new Scene \r\n'<color=grey>" + sceneImporter.assetPath + "</color>'");

			#region Environmental Lighting
			var env = sceneData.environmentLightingOptions;
			if (env.override_skybox) { RenderSettings.skybox = env.skybox; }
			if (env.override_sun) { /* No public APIs */ }
			if (env.override_ambientMode) { RenderSettings.ambientMode = (UnityEngine.Rendering.AmbientMode)((int)env.ambientMode); }

			if (env.override_ambientLight) { RenderSettings.ambientLight = env.ambientLight; }

			if (env.override_ambientSkyColor) { RenderSettings.ambientSkyColor = env.ambientSkyColor; }
			if (env.override_ambientEquatorColor) { RenderSettings.ambientEquatorColor = env.ambientEquatorColor; }
			if (env.override_ambientGroundColor) { RenderSettings.ambientGroundColor = env.ambientGroundColor; }

			if (env.override_ambientIntensity) { RenderSettings.ambientIntensity = env.ambientIntensity; }

			if (env.override_ambientGI) { /* No public APIs */ }

			if (env.override_defaultReflectionMode) { RenderSettings.defaultReflectionMode = (UnityEngine.Rendering.DefaultReflectionMode)((int)env.defaultReflectionMode); }
			if (env.override_defaultReflectionResolution) { RenderSettings.defaultReflectionResolution = (int)env.defaultReflectionResolution; }
			if (env.override_reflectionCompression) { /* No public APIs */ }
			if (env.override_reflectionIntensity) { RenderSettings.reflectionIntensity = env.reflectionIntensity; }
			if (env.override_reflectionBounces) { RenderSettings.reflectionBounces = env.reflectionBounces; }
			if (env.override_customReflection) { RenderSettings.customReflection = env.customReflection; }
			#endregion

			#region Fog
			var fog = sceneData.fogOptions;
			if (fog.override_fog) { RenderSettings.fog = fog.fog; }
			if (fog.override_fogColor) { RenderSettings.fogColor = fog.fogColor; }
			if (fog.override_fogMode) { RenderSettings.fogMode = fog.fogMode; }
			if (fog.override_fogStartDistance) { RenderSettings.fogStartDistance = fog.fogStartDistance; }
			if (fog.override_fogEndDistance) { RenderSettings.fogEndDistance = fog.fogEndDistance; }
			if (fog.override_fogDensity) { RenderSettings.fogDensity = fog.fogDensity; }
			#endregion

			#region Other
			var other = sceneData.otherOptions;
			if (other.override_haloTexture) { /* No public APIs */ }
			if (other.override_haloStrength) { RenderSettings.haloStrength = other.haloStrength; }

			if (other.override_flareFadeSpeed) { RenderSettings.flareFadeSpeed = other.flareFadeSpeed; }
			if (other.override_flareStrength) { RenderSettings.flareStrength = other.flareStrength; }

			if (other.override_spotCookie) { /* No public APIs */ }
			#endregion

			#region General GI
			var lightmap = sceneData.generalGlobalIlluminationOptions;
			if (lightmap.override_automaticBaking)
			{
				Lightmapping.giWorkflowMode = sceneData.generalGlobalIlluminationOptions.automaticBaking ? Lightmapping.GIWorkflowMode.Iterative : Lightmapping.GIWorkflowMode.OnDemand;
			}
			#endregion

			#region Realtime GI
			var realtime = sceneData.precomputedRealtimeGlobalIlluminationOptions;
			if (realtime.override_lightmappingRealtimeGI) { Lightmapping.realtimeGI = realtime.lightmappingRealtimeGI; }

			#endregion

			#region Baked GI
			var baked = sceneData.bakedGlobalIlluminationOptions;
			if (baked.override_lightmappingBakedGI) { Lightmapping.bakedGI = baked.lightmappingBakedGI; }

			#endregion

			#region NavMesh
			var navMesh = sceneData.navMeshOptions;

#if UNITY_5_5_OR_NEWER
			var navMeshSettings = new SerializedObject(UnityEditor.AI.NavMeshBuilder.navMeshSettingsObject);
#else
			var _settingsObject = new SerializedObject(NavMeshBuilder.navMeshSettingsObject);
#endif

			const string AGENT_RADIUS = "m_BuildSettings.agentRadius";
			const string AGENT_HEIGHT = "m_BuildSettings.agentHeight";
			const string MAX_SLOPE = "m_BuildSettings.agentSlope";
			const string STEP_HEIGHT = "m_BuildSettings.agentClimb";
			const string DROP_HEIGHT = "m_BuildSettings.ledgeDropHeight";
			const string JUMP_DISTANCE = "m_BuildSettings.maxJumpAcrossDistance";
			const string MANUAL_VOXEL_SIZE = "m_BuildSettings.manualCellSize";
			const string VOXEL_SIZE = "m_BuildSettings.cellSize";
			const string MIN_REGION_AREA = "m_BuildSettings.minRegionArea";
			const string HEIGHT_MESH = "m_BuildSettings.accuratePlacement";

			SerializedProperty agentRadius = navMeshSettings.FindProperty(AGENT_RADIUS);
			SerializedProperty agentHeight = navMeshSettings.FindProperty(AGENT_HEIGHT);
			SerializedProperty maxSlope = navMeshSettings.FindProperty(MAX_SLOPE);
			SerializedProperty stepHeight = navMeshSettings.FindProperty(STEP_HEIGHT);
			SerializedProperty dropHeight = navMeshSettings.FindProperty(DROP_HEIGHT);
			SerializedProperty jumpDistance = navMeshSettings.FindProperty(JUMP_DISTANCE);
			SerializedProperty manualVoxelSize = navMeshSettings.FindProperty(MANUAL_VOXEL_SIZE);
			SerializedProperty voxelSize = navMeshSettings.FindProperty(VOXEL_SIZE);
			SerializedProperty minRegionArea = navMeshSettings.FindProperty(MIN_REGION_AREA);
			SerializedProperty heightMesh = navMeshSettings.FindProperty(HEIGHT_MESH);

			if (navMesh.override_agentRadius) { agentRadius.floatValue = navMesh.agentRadius; }
			if (navMesh.override_agentHeight) { agentHeight.floatValue = navMesh.agentHeight; }
			if (navMesh.override_agentSlope) { maxSlope.floatValue = navMesh.agentSlope; }
			if (navMesh.override_agentClimb) { stepHeight.floatValue = navMesh.agentClimb; }
			if (navMesh.override_ledgeDropHeight) { dropHeight.floatValue = navMesh.ledgeDropHeight; }
			if (navMesh.override_maxJumpAcrossDistance) { jumpDistance.floatValue = navMesh.maxJumpAcrossDistance; }
			if (navMesh.override_manualCellSize) { manualVoxelSize.boolValue = navMesh.manualCellSize; }
			if (navMesh.override_cellSize) { voxelSize.floatValue = navMesh.cellSize; }
			if (navMesh.override_minRegionArea) { minRegionArea.floatValue = navMesh.minRegionArea; }
			if (navMesh.override_accuratePlacement) { heightMesh.boolValue = navMesh.accuratePlacement; }

			navMeshSettings.ApplyModifiedProperties();
			#endregion
		}
		#endregion
	}
}