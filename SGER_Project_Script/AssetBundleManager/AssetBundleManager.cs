using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AssetBundleManager : MonoBehaviour
{

    // 에셋 번들을 저장할 노드임. 에셋번들, url, 버전, Unload 시 bool 조건값을 저장함
    public class AssetBundleNode
    {
        public AssetBundle assetBundle;
        public string url;
        public string assetName;
        public bool removeAll;

        //constructor
        //생성자. url, version, removeAll, assetbundle을 초기화 시 불러와 대입함
        public AssetBundleNode(string urlIn, string nameIn, bool removeAllIn, AssetBundle bundleIn)
        {
            this.url = urlIn;
            this.assetName = nameIn;
            this.removeAll = removeAllIn;
            this.assetBundle = bundleIn;
        }

        //클래스 함수임. 물려 있는 에셋 번들의 Unload() 함수를 실행해줌
        public void UnloadAssetBundle()
        {
            this.assetBundle.Unload(this.removeAll);
        }
    }

    static private Dictionary<string, AssetBundleNode> dicAssetBundle;

    //save keyNames for remove All ABs
    static private List<string> IstKeyName;

    private void Awake()
    {
        //에셋번들을 저장할 Dictionary와 Dictionary<key, value>의
        //key 부분을 저장할 리스트 하나를 동적 할당하여 초기화 해줌
        dicAssetBundle = new Dictionary<string, AssetBundleNode>();
        IstKeyName = new List<string>();
    }

    //코루틴 함수. 에셋 번들을 로드하여 Dictionary에 저장하는 역할을 함 + 생성
    public IEnumerator LoadFromFileAsync(string url, string assetName, bool removeAll)
    {
        if (this.IsVersionAdded(url, assetName))
        {
            yield return null;
        }
        else
        {
            string keyName = this.MakeKeyName(url, assetName);
            Debug.Log(keyName);

            AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(keyName);
            yield return req;

            //AssetBundle ab = req.assetBundle;

            //노드를 만들어서 데이터를 대입하여 초기화하고 dictionary에 저장함
            AssetBundleNode node = new AssetBundleNode(url, assetName, removeAll, req.assetBundle);
            dicAssetBundle.Add(keyName, node);
            IstKeyName.Add(keyName);

            ////아마 assetName 마다 차별화 해둬야 할 거임. Ex) Item, Human, etc.
            //string[] bundleNames = ab.GetAllAssetNames();
            //foreach (string path in bundleNames)
            //{
            //    StartCoroutine(this.LoadFromABasync_Item(path, ab));
            //}
            //this.RemoveAllAssetBundles();
        }
    }

    //public IEnumerator LoadFromABasync_Item(string path, AssetBundle ab)
    //{
    //    string[] name = path.Split('/');
    //    Debug.Log(name[0] + " + " + name[1] + " + " + name[2]);

    //    string[] asset = name[2].Split('.');
    //    Debug.Log(asset[0] + " + " + asset[1]);

    //    var obj = ab.LoadAsset<GameObject>(asset[0]);
    //    yield return Instantiate(obj);

    //    AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(dir_path + "/AssetBundles/StandaloneWindows.unity3d");
    //    yield return req;

    //    //AssetBundle을 사용할 시 GetAllAssetNames()를 실행하면 assetbundlemanifest만 저장됨
    //    //그래서 아예 AssetBundleManifest를 사용해서 GetAllAssetBundles() 를 실행하면
    //    //StandaloneWindows 에 내가 빌드한 에셋번들들의 이름이 string에 저장됨.
    //    AssetBundleManifest m = req.assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

    //    string[] s = m.GetAllAssetBundles();

    //    Debug.Log(s[0] + "   stand");


    //    obj.transform.localPosition = Vector3.zero;

    //}

    //위의 LoadAssetBundle() 함수를 통하여 불러온 에셋 번들을 return 시키는 함수

    public AssetBundle GetAssetBundle(string url, string assetName)
    {
        string keyName = this.MakeKeyName(url, assetName);
        AssetBundle ab = dicAssetBundle[keyName].assetBundle;

        return ab;
    }

    //url, version 정보를 받아와 keyname을 return 하는 함수
    public string MakeKeyName(string url, string assetName)
    {
        string keyName = Path.Combine(url, assetName);
        return keyName;
    }

    //받아온 url,version 정보로 저장된 에셋 번들이 있는지 확인함
    public bool IsVersionAdded(string url, string assetName)
    {
        string keyName = this.MakeKeyName(url, assetName);
        if (dicAssetBundle.ContainsKey(keyName))
            return true;
        else
            return false;
    }

    //에셋 번들을 Unload 시키는 함수. 반드시 Dictionary에서 Remove시키기 전에 에셋 번들을 Unload 시킵시다
    public void RemoveAssetBundle(string url, string assetName)
    {
        string keyName = this.MakeKeyName(url, assetName);

        if (dicAssetBundle.ContainsKey(keyName))
        {
            dicAssetBundle[keyName].UnloadAssetBundle();
            dicAssetBundle.Remove(keyName);
            IstKeyName.Remove(keyName);
        }
    }
    public void RemoveAssetBundle(string keyName)
    {
        if (dicAssetBundle.ContainsKey(keyName))
        {
            dicAssetBundle[keyName].UnloadAssetBundle();
            dicAssetBundle.Remove(keyName);
            IstKeyName.Remove(keyName);
        }
    }

    //저장되어 있는 모든 에셋번들을 Unload 시킴
    public void RemoveAllAssetBundles()
    {
        for (int i = 0; i < dicAssetBundle.Count; i++)
        {
            dicAssetBundle[IstKeyName[i]].UnloadAssetBundle();
        }
        dicAssetBundle.Clear();
        IstKeyName.Clear();
    }

}
