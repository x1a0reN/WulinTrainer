using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Build.Reporting;

public class BuildAB
{
    [MenuItem("Tools/BuildAB")]

    public static void Build() 
    {
        string dir = "D:\\开发\\大侠立志传\\HaxxToyBox-master\\Assets";

        if (!Directory.Exists(dir))
        { 
            Directory.CreateDirectory(dir);
        }
        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);


    }



}