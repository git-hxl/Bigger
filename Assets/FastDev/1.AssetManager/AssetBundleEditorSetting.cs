#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FastDev
{
    public class AssetBundleEditorSetting
    {
        public static string SettingPath { get; } = "./abbuildsetting.json";
        //Ab���������Ŀ¼
        public string BuildPath { get; set; }
        //���ƽ̨
        public BuildTarget Platform { get; set; }
        //ѹ����ʽ
        public AssetBundleEditor.CompressionType CompressionType { get; set; }
        //��Ҫ�����bundles
        public List<string> SelectBundles { get; set; } = new List<string>();
        public string AssetVersion { get; set; }
        public string AppVersion { get; set; }
        public AssetBundleEditorSetting()
        {
            BuildPath = Application.streamingAssetsPath;
            Platform = BuildTarget.StandaloneWindows;
            CompressionType = AssetBundleEditor.CompressionType.LZ4;
            AssetVersion = "1.0.0";
            AppVersion = "1.0.0";
        }
    }
}
#endif