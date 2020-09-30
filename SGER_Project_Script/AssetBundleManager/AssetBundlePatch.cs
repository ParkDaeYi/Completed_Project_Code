using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;


public class AssetBundlePatch : MonoBehaviour
{
    static private AssetBundlePatch instance = null;
    string dir_path;

    //매니저를 만듬에 있어서 Class 자체를 static으로 지정하여 사용할 수도 있음
    //static으로 자신을 하나 물고 있고, AssetBundleManager.Instance.Anything()의 형식으로
    //접근하는 방식이 편해서
    //이렇게 작성! 클래스를 static으로 선언하고 생성자에서 초기화해도 상관 없음
    static public AssetBundlePatch Instance
    {
        get
        {
            //처음 호출시엔 인스턴스가 물려있을리 없으니
            //게임오브젝트에 에셋번들매니져 클래스를 붙이고 물려줌
            if (instance == null)
            {
                GameObject inst = new GameObject("_AssetBundlePatch");
                inst.isStatic = true;
                instance = inst.AddComponent<AssetBundlePatch>();

                GameObject inst_2 = new GameObject("_AssetBundleManager");
                inst_2.transform.parent = inst.transform;
                inst_2.transform.localPosition = Vector3.zero;
                abManager = inst_2.AddComponent<AssetBundleManager>();
            }
            return instance;
            //이 Property는 get만 존재하고 set이 없기 떄문에 읽기 전용!
        }
    }

    private static AssetBundleManager abManager;

    public class AssetBundleInfo
    {
        public string bundle { get; private set; }
        public Hash128 hash128 { get; private set; }

        public AssetBundleInfo(string bundle, Hash128 hash128)
        {
            this.bundle = bundle;
            this.hash128 = hash128;
        }
    }

    private string assetBundleBaseUrl = "";
    private int version;
    private List<AssetBundleInfo> oldAsset = new List<AssetBundleInfo>();
    private List<AssetBundleInfo> newAsset = new List<AssetBundleInfo>();

    public static void CleanAssetBundleCache()
    {
        Debug.Log(Caching.ClearCache() ? "Successfully removed Cache" : "Cache in use");
    }

    public IEnumerator Patch(string url)
    {
        assetBundleBaseUrl = url;
        //assetBundleBaseUrl = "https://168.115.119.95/svn/VRProject/AssetBundle_Test/";
        dir_path = Static.STATIC.dir_path;
        version = 3;

        ////1
        ///.meta 파일이 ...
        if (!FileCheck(dir_path + "/AssetBundles/", "StandaloneWindows.unity3d"))
        {
            yield return StartCoroutine(this.SaveAndDownload(assetBundleBaseUrl + "StandaloneWindows", dir_path + "/AssetBundles/", "StandaloneWindows.unity3d"));
            //Debug.Log("StandaloneWindows.unity3d 생성 완료");

        }
        if (!FileCheck(dir_path + "/AssetBundles/", "human.unity3d"))
        {
            yield return StartCoroutine(this.SaveAndDownload(assetBundleBaseUrl + "human", dir_path + "/AssetBundles/", "human.unity3d"));
            //Debug.Log("cube.unity3d 생성 완료");
        }
        if (!FileCheck(dir_path + "/AssetBundles/", "items.unity3d"))
        {
            yield return StartCoroutine(this.SaveAndDownload(assetBundleBaseUrl + "items", dir_path + "/AssetBundles/", "items.unity3d"));
            //Debug.Log("cube.unity3d 생성 완료");
        }
        if (!FileCheck(dir_path + "/AssetBundles/", "place.unity3d"))
        {
            yield return StartCoroutine(this.SaveAndDownload(assetBundleBaseUrl + "place", dir_path + "/AssetBundles/", "place.unity3d"));
            //Debug.Log("cube.unity3d 생성 완료");
        }
        if (!FileCheck(dir_path + "/AssetBundles/", "handitem.unity3d"))
        {
            yield return StartCoroutine(this.SaveAndDownload(assetBundleBaseUrl + "handitem", dir_path + "/AssetBundles/", "handitem.unity3d"));
            //Debug.Log("cube.unity3d 생성 완료");
        }
        if (!FileCheck(dir_path + "/AssetBundles/", "wall.unity3d"))
        {
            yield return StartCoroutine(this.SaveAndDownload(assetBundleBaseUrl + "wall", dir_path + "/AssetBundles/", "wall.unity3d"));
            //Debug.Log("cube.unity3d 생성 완료");
        }

        //로컬
        StartCoroutine(this.LoadFromLocal("StandaloneWindows.unity3d"));
        //서버
        StartCoroutine(LoadFromCacheOrDownload(assetBundleBaseUrl + "StandaloneWindows", (manifest) =>
        {

            int i = 0;
            foreach (var old in this.oldAsset)
            {

                if (old.hash128 != this.newAsset[i].hash128)
                {
                    Debug.LogFormat("<color=red>{0},{1}</color>", this.newAsset[i].bundle, this.newAsset[i].hash128);

                    StartCoroutine(this.SaveAndDownload(assetBundleBaseUrl + this.newAsset[i].bundle, dir_path + "/AssetBundles/", this.newAsset[i].bundle + ".unity3d"));

                    StartCoroutine(this.SaveAndDownload(assetBundleBaseUrl + "StandaloneWindows", dir_path + "/AssetBundles/", "StandaloneWindows.unity3d"));
                }

                i++;
            }

        }));

    }

    public IEnumerator LoadAssetBundle(string url, string assetName, bool removeAll)
    {
        yield return StartCoroutine(abManager.LoadFromFileAsync(url, assetName, removeAll));
    }

    //파일 유무 확인
    private bool FileCheck(string localPath, string fileName)
    {
        string path = Path.Combine(localPath, fileName);

        FileInfo fInfo = new FileInfo(path);

        if (fInfo.Exists)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator SaveAndDownload(string url, string localPath, string fileName)
    {
        WWW www = new WWW(url);

        Debug.Log(url);

        yield return www;

        byte[] bytes = www.bytes;

        Debug.Log("<color=red>" + bytes.Length + "</color>");

        if (!Directory.Exists(localPath))
        {
            Directory.CreateDirectory(localPath);
#if UNITY_EDITOR
            /* 동적으로 폴더에 있는 데이터 임포트! -> 유니티 상에서 바로 사용 할 수 있도록
             * 에디터 모드에만 적용되는 소스를 전처리기 적용 우회시켜 빌드 모드에서도 적용할 수 있도록 설정!
             */
            ImportAsset.NewImportAsset_Dic("Assets/AssetBundles");
#endif
        }

        Debug.Log(localPath + " + " + fileName);

        File.WriteAllBytes(localPath + fileName, bytes);

#if UNITY_EDITOR
        /* 동적으로 폴더에 있는 데이터 임포트! -> 유니티 상에서 바로 사용 할 수 있도록
         * 에디터 모드에만 적용되는 소스를 전처리기 적용 우회시켜 빌드 모드에서도 적용할 수 있도록 설정!
         */
        ImportAsset.NewImportAsset_File("Assets/AssetBundles/" + fileName);
#endif
    }

    IEnumerator LoadFromLocal(string assetName)
    {
        string path = Path.Combine(dir_path + "/AssetBundles/", assetName);
        Debug.Log(path);

        AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(path);
        yield return req;

        AssetBundle assetBundle = req.assetBundle;

        if (assetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle");
            yield break;
        }

        Debug.Log("assetBundle : " + assetBundle);

        AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

        Debug.Log("manifest: " + manifest);

        foreach (var bundle in manifest.GetAllAssetBundles())
        {
            this.oldAsset.Add(new AssetBundleInfo(bundle, manifest.GetAssetBundleHash(bundle)));

            Debug.LogFormat("{0},{1}", bundle, manifest.GetAssetBundleHash(bundle));
        }

        assetBundle.Unload(true);


    }

    public IEnumerator LoadFromCacheOrDownload(string url, System.Action<AssetBundleManifest> onComplete)
    {

        using (WWW www = WWW.LoadFromCacheOrDownload(url, version))
        {
            yield return www;

            AssetBundle assetBundle = www.assetBundle;

            Debug.Log("assetBundle : " + assetBundle);

            AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            Debug.Log("manifest : " + manifest);

            foreach (var bundle in manifest.GetAllAssetBundles())
            {
                this.newAsset.Add(new AssetBundleInfo(bundle, manifest.GetAssetBundleHash(bundle)));
                Debug.Log(manifest.GetAssetBundleHash(bundle));
            }

            assetBundle.Unload(false);

            onComplete(manifest);
        }

    }

    public AssetBundle GetAssetBundle(string url, string assetName)
    {
        return abManager.GetAssetBundle(url, assetName);
    }

    public void RemoveAllAssetBundles()
    {
        abManager.RemoveAllAssetBundles();
    }

    IEnumerator LoadAssetBundle(string url, System.Action onComplete)
    {

        UnityWebRequest req = UnityWebRequestAssetBundle.GetAssetBundle(url, 0);
        //var req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();

        AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(req);

        Debug.Log("assetBundle: " + assetBundle);

        AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

        foreach (var bundle in manifest.GetAllAssetBundles())
        {

            Debug.LogFormat("{0}", manifest.GetAssetBundleHash(bundle));

            this.newAsset.Add(new AssetBundleInfo(bundle, manifest.GetAssetBundleHash(bundle)));
        }

        assetBundle.Unload(false);

        onComplete();

    }

}
