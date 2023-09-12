using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameFramework
{
    public class AssetUpdater : MonoBehaviour
    {
        public string RemoteAssetUrl;

        private string RemoteAssetConfigUrl;

        private string LocalAssetPath;
        private string LocalAssetConfigPath;


        private string remoteConfig;
        private List<string> updateFiles = new List<string>();

        // Start is called before the first frame update
        void Awake()
        {
            RemoteAssetUrl = RemoteAssetUrl + "/" + PlatformUtil.GetPlatformName();

            RemoteAssetConfigUrl = RemoteAssetUrl + "/AssetConfig.json";

            LocalAssetPath = Application.persistentDataPath + "/" + PlatformUtil.GetPlatformName();

            LocalAssetConfigPath = LocalAssetPath + "/AssetConfig.json";
        }

        private async void Start()
        {
            bool result = await CheckConfig();

            if (result == true)
                result = await UpdateAsset();

            if(result == true)
                result = await LoadAssetBundle();
        }

        private async UniTask<bool> CheckConfig()
        {
            try
            {
                updateFiles.Clear();

                remoteConfig = await HttpManager.Instance.GetTxt(RemoteAssetConfigUrl).Timeout(TimeSpan.FromSeconds(5));

                if (string.IsNullOrEmpty(remoteConfig))
                {
                    throw new Exception("��ȡ�����ļ�ʧ��!");
                }

                AssetConfig remoteAssetConfig = JsonConvert.DeserializeObject<AssetConfig>(remoteConfig);

                AssetConfig localAssetConfig = new AssetConfig();

                if (File.Exists(LocalAssetConfigPath))
                {
                    localAssetConfig = JsonConvert.DeserializeObject<AssetConfig>(File.ReadAllText(LocalAssetConfigPath));
                }

                if (!string.IsNullOrEmpty(localAssetConfig.AppVersion) && localAssetConfig.AppVersion != remoteAssetConfig.AppVersion)
                {
                    Debug.LogError("��Ҫ����APP");

                    return false;
                }

                foreach (var bundle in remoteAssetConfig.Bundles)
                {
                    if (!localAssetConfig.Bundles.ContainsKey(bundle.Key) || localAssetConfig.Bundles[bundle.Key] != bundle.Value)
                    {

                        updateFiles.Add(bundle.Key);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                Debug.LogError("�����ļ���ȡʧ�ܣ�");

                return false;
            }

            return true;
        }

        private async UniTask<bool> UpdateAsset()
        {

            if (updateFiles == null || updateFiles.Count == 0)
            {
                Debug.Log("û����Ҫ���µ���Դ");
                return true;
            }

            Debug.Log("��ʼ������Դ");

            for (int i = 0; i < updateFiles.Count; i++)
            {
                bool result = await HttpManager.Instance.Download(RemoteAssetUrl + "/" + updateFiles[i], LocalAssetPath, DownloadCallback);

                if (result == false)
                {
                    Debug.LogError("����ʧ�ܣ���������");

                    return false;
                }
            }

            File.WriteAllText(LocalAssetConfigPath, remoteConfig);

            Debug.Log("�������");

            return true;
        }


        private void DownloadCallback(string fileName, float progress)
        {
            Debug.Log($"{fileName} ���½��� {progress}");
        }


        private async UniTask<bool> LoadAssetBundle()
        {
            try
            {
                AssetConfig remoteAssetConfig = JsonConvert.DeserializeObject<AssetConfig>(remoteConfig);

                foreach (var bundle in remoteAssetConfig.Bundles)
                {
                    await AssetManager.Instance.LoadAssetBundleAsync(LocalAssetPath + "/" + bundle.Key, (progress) => LoadAssetBundleCallback(bundle.Key, progress));
                }
            }
            catch(Exception e)
            {
                Debug.LogError(e.ToString());

                return false;
            }
            Debug.Log("�������");
            return true;
        }

        private void LoadAssetBundleCallback(string fileName, float progress)
        {
            Debug.Log($"{fileName} ���ؽ��� {progress}");
        }
    }
}
