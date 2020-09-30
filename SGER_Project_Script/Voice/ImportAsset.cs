#if UNITY_EDITOR
using UnityEditor;
#endif

class ImportAsset
{
    public static void NewImportAsset_Dic(string path)
    {
#if UNITY_EDITOR
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ImportRecursive);
#endif
    }

    public static void NewImportAsset_File(string path)
    {
#if UNITY_EDITOR
        AssetDatabase.ImportAsset(path, ImportAssetOptions.Default);
#endif
    }
}
