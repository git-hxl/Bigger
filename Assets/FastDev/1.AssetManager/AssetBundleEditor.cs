#if UNITY_EDITOR
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FastDev
{
    public class AssetBundleEditor : EditorWindow
    {
        public enum CompressionType
        {
            LZMA = BuildAssetBundleOptions.None,
            UncompressedAssetBundle = BuildAssetBundleOptions.UncompressedAssetBundle,
            LZ4 = BuildAssetBundleOptions.ChunkBasedCompression
        }

        private AssetBundleEditorSetting assetBundleEditorSetting;

        [MenuItem("FastDev/AssetBundle����")]
        public static void OpenWindow()
        {
            AssetBundleEditor window = (AssetBundleEditor)EditorWindow.GetWindow(typeof(AssetBundleEditor), false, "AssetBundle");
            window.Show();
        }

        private void Awake()
        {
            InitWindowSetting();
        }

        private void InitWindowSetting()
        {
            assetBundleEditorSetting = new AssetBundleEditorSetting();
            if (File.Exists(AssetBundleEditorSetting.SettingPath))
            {
                string jsonTxt = File.ReadAllText(AssetBundleEditorSetting.SettingPath);
                assetBundleEditorSetting = JsonConvert.DeserializeObject<AssetBundleEditorSetting>(jsonTxt);
            }
            AssetDatabase.RemoveUnusedAssetBundleNames();
        }

        private void OnGUI()
        {
            GUISelectBuildFolder();
            GUIBuildSetting();
            GUISelectBundles();
            GUIBuild();
        }

        private void GUISelectBuildFolder()
        {
            GUILayout.Label("���·��");
            assetBundleEditorSetting.BuildPath = GUILayout.TextField(assetBundleEditorSetting.BuildPath);
            if (GUILayout.Button("ѡ���ļ���"))
            {
                string selectPath = EditorUtility.OpenFolderPanel("���Ŀ¼", Application.dataPath, "");
                if (!string.IsNullOrEmpty(selectPath))
                {
                    assetBundleEditorSetting.BuildPath = selectPath;
                }
            }
        }

        private void GUIBuildSetting()
        {
            GUILayout.Label("���ƽ̨");
            assetBundleEditorSetting.Platform = (BuildTarget)EditorGUILayout.EnumPopup(assetBundleEditorSetting.Platform);

            GUILayout.Label("ѹ����ʽ");
            assetBundleEditorSetting.CompressionType = (CompressionType)EditorGUILayout.EnumPopup(assetBundleEditorSetting.CompressionType);

            GUILayout.Label("��Դ�汾");
            assetBundleEditorSetting.AssetVersion = GUILayout.TextField(assetBundleEditorSetting.AssetVersion);
            GUILayout.Label("App�汾");
            assetBundleEditorSetting.AppVersion = GUILayout.TextField(assetBundleEditorSetting.AppVersion);
        }


        private void GUISelectBundles()
        {
            foreach (var item in AssetDatabase.GetAllAssetBundleNames())
            {
                bool value = assetBundleEditorSetting.SelectBundles.Contains(item);
                if (GUILayout.Toggle(value, item))
                {
                    if (!assetBundleEditorSetting.SelectBundles.Contains(item))
                    {
                        assetBundleEditorSetting.SelectBundles.Add(item);
                    }
                }
                else if (assetBundleEditorSetting.SelectBundles.Contains(item))
                {
                    assetBundleEditorSetting.SelectBundles.Remove(item);
                }
            }
        }

        private void GUIBuild()
        {
            if (GUILayout.Button("���"))
            {
                AssetBundleBuild[] builds = new AssetBundleBuild[assetBundleEditorSetting.SelectBundles.Count];

                for (int i = 0; i < builds.Length; i++)
                {
                    AssetBundleBuild build = new AssetBundleBuild();
                    build.assetBundleName = assetBundleEditorSetting.SelectBundles[i];
                    build.assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(build.assetBundleName);
                    builds[i] = build;
                }

                string outputPath = assetBundleEditorSetting.BuildPath + "/" + assetBundleEditorSetting.Platform;
                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }

                AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(outputPath, builds, (BuildAssetBundleOptions)assetBundleEditorSetting.CompressionType, assetBundleEditorSetting.Platform);
                Dictionary<string, string> config = new Dictionary<string, string>();

                foreach (var bundle in manifest.GetAllAssetBundles())
                {
                    config[bundle] = manifest.GetAssetBundleHash(bundle).ToString();
                }
                config["AssetVersion"] = assetBundleEditorSetting.AssetVersion;
                config["AppVersion"] = assetBundleEditorSetting.AppVersion;

                string configJson = JsonConvert.SerializeObject(config, Formatting.Indented);

                File.WriteAllText(outputPath + "/config.json", configJson);

                AssetDatabase.Refresh();
            }
        }
    }
}
#endif