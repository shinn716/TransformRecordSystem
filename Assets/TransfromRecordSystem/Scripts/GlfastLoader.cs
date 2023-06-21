using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using GLTFast;
using System;

[RequireComponent(typeof(RecordManager))]
public class GlfastLoader : MonoBehaviour
{
    public PathType pathType = PathType.StreamingAssets;
    public string url = string.Empty;

    string fullUrl = string.Empty;
    string folderName = string.Empty;

    public enum PathType
    {
        StreamingAssets,
        URL,
    }

    void Awake()
    {
        if (pathType.Equals(PathType.StreamingAssets))
        {
            fullUrl = Path.Combine(Application.streamingAssetsPath, url) + "/";
            folderName = url;
        }
        else
        {
            fullUrl = url + "/";
            folderName = Path.GetFileName(Path.GetDirectoryName(fullUrl));
        }
    }

    void Start()
    {
        LoadGltfBinaryFromMemory(Path.Combine(fullUrl, folderName + ".glb"), transform, folderName, (go) =>
        {
            try
            {
                var jsonUrl = Path.Combine(fullUrl, folderName + ".json");
                var jsonContent = File.ReadAllText(jsonUrl);
                var jsonData = JsonUtility.FromJson<RecordManager.ExportJson>(jsonContent);
                RecordManager.instance.GetAndSetRecordUnits = jsonData.ObjectList;
            }
            catch (Exception) { }
            RecordManager.instance.target = go.transform;
            StartCoroutine(RecordManager.instance.InitRoot());
        });
    }

    async void LoadGltfBinaryFromMemory(string filePath, Transform obj, string name, Action<GameObject> action)
    {
        byte[] data = File.ReadAllBytes(filePath);
        var gltf = new GltfImport();
        bool success = await gltf.LoadGltfBinary(data, new System.Uri(filePath));
        if (success)
        {
            await gltf.InstantiateMainSceneAsync(obj);
            // var scene = GameObject.Find(gltf.GetSceneName(0));
            // scene.name = name;
            // action.Invoke(scene);

            var scene = transform.GetChild(0).gameObject;
            scene.transform.parent = null;
            scene.name = name;
            action.Invoke(scene);
        }
    }


}
