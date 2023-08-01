using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace HomaGames.HomaBelly.DataPrivacy
{
    public class DataPrivacySceneSetter : IActiveBuildTargetChanged
    {
        private const string INIT_SCENE_PATH = "Assets/Homa Games/Homa Belly/Core/DataPrivacy/Runtime/Scenes/Homa Games DataPrivacy Init Scene.unity";
        private const string GDPR_SCENE_PATH = "Assets/Homa Games/Homa Belly/Core/DataPrivacy/Runtime/Scenes/Homa Games DataPrivacy GDPR Scene.unity";
        private const string IDFA_SCENE_PATH = "Assets/Homa Games/Homa Belly/Core/DataPrivacy/Runtime/Scenes/Homa Games DataPrivacy IDFA Scene.unity";

        private static readonly string[] ScenePaths = {INIT_SCENE_PATH, GDPR_SCENE_PATH, IDFA_SCENE_PATH};
        
        public int callbackOrder => 0;
        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            Configure();
        }


        [InitializeOnLoadMethod]
        private static void OnProjectLoaded()
        {
            BuildPlayerHandlerWrapper.AddBuildFilter(OnBuild);
            
            Configure();
        }

        private static bool OnBuild(BuildReport options)
        {
            Configure();
            return true;
        }

        private static void Configure()
        {
            PluginManifest pluginManifest = PluginManifest.LoadFromLocalFile();
            Settings settings = Settings.EditorCreateOrLoadDataPrivacySettings();

            if (pluginManifest != null)
            {
                PackageComponent packageComponent = pluginManifest.Packages
                    .GetPackageComponent(HomaBellyEditorConstants.PACKAGE_ID, HomaBellyEditorConstants.PACKAGE_TYPE);
                if (packageComponent != null)
                {
                    Dictionary<string, string> configurationData = packageComponent.Data;

                    UpdateSettings(settings, configurationData);
                }
            }
            
            EditorBuildSettingsScene[] buildSettingsScenes = EditorBuildSettings.scenes;
            AddAllDataPrivacyScenes(ref buildSettingsScenes);

            try
            {
                buildSettingsScenes.First(scene => scene.path == INIT_SCENE_PATH).enabled = true;
                buildSettingsScenes.First(scene => scene.path == GDPR_SCENE_PATH).enabled =
                    IsGdprSceneEnabled(settings);
                buildSettingsScenes.First(scene => scene.path == IDFA_SCENE_PATH).enabled =
                    IsIdfaSceneEnabled(settings);
            }
            catch (InvalidOperationException)
            {
                Debug.LogError("Error when setting up DataPrivacy scenes.");
            }

            EditorBuildSettings.scenes = buildSettingsScenes;
        }

        private static void UpdateSettings(Settings settings, Dictionary<string, string> configurationData)
        {
            string[] dataKeys =
            {
                "b_enable_gdpr_android",
                "b_enable_gdpr_ios",
                "b_enable_idfa",
                "b_enable_idfa_prepopup"
            };

            Action<bool>[] dataSetters =
            {
                b => settings.GdprEnabledForAndroid = b,
                b => settings.GdprEnabledForIos = b,
                b => settings.ShowIdfa = b,
                b => settings.ShowIdfaPrePopup = b,
            };

            for (int i = 0; i < dataKeys.Length; i++)
            {
                if (configurationData.TryGetValue(dataKeys[i], out var value))
                {
                    dataSetters[i].Invoke(Convert.ToBoolean(value));
                }
            }
        }

        private static bool IsGdprSceneEnabled(Settings settings)
        {
#if !UNITY_ANDROID && !UNITY_IOS
            return false;
#else
            return !settings.ForceDisableGdpr && settings.GdprEnabled;
#endif
        }

        private static bool IsIdfaSceneEnabled(Settings settings)
        {
#if !UNITY_IOS
            return false;
#else
            return settings.ShowIdfa;
#endif
        }

        private static void AddAllDataPrivacyScenes(ref EditorBuildSettingsScene[] sceneList)
        {
            if (StartsWith(sceneList, ScenePaths))
                return;

            IEnumerable<EditorBuildSettingsScene> nonDataPrivacyScenes =
                sceneList.Where(scene => !ScenePaths.Contains(scene.path));

            IEnumerable<EditorBuildSettingsScene> DataPrivacyScenes =
                ScenePaths.Select(path => new EditorBuildSettingsScene(path, true));

            sceneList = DataPrivacyScenes.Concat(nonDataPrivacyScenes).ToArray();
        }

        private static bool StartsWith(EditorBuildSettingsScene[] sceneList, string[] scenePaths)
        {
            if (sceneList.Length < scenePaths.Length)
                return false;
            
            for (int i = 0; i < scenePaths.Length; i++)
            {
                if (sceneList[i].path != scenePaths[i])
                    return false;
            }

            return true;
        }
    }
}