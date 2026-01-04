using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Linq;

[CustomEditor(typeof(PrefabsTools))]
public class PrefabsTools : ScriptableWizard
{
    [UnityEditor.MenuItem("Tools/Apply Selected Prefabs")]
    static void ApplySelectedPrefabs()
    {
        //获取选中的gameobject对象
        GameObject[] selectedsGameobject = Selection.gameObjects;

        for (int i = 0; i < selectedsGameobject.Length; i++)
        {
            GameObject obj = selectedsGameobject[i];

            UnityEngine.Object newsPref = PrefabUtility.GetPrefabObject(obj);

            //判断选择的物体，是否为预设
            if (PrefabUtility.GetPrefabType(obj) == PrefabType.PrefabInstance)
            {

                UnityEngine.Object parentObject = PrefabUtility.GetPrefabParent(obj);
                //获取路径
                //string path = AssetDatabase.GetAssetPath(parentObject);
                //Debug.Log("path:"+path);
                //替换预设
                PrefabUtility.ReplacePrefab(obj, parentObject, ReplacePrefabOptions.ConnectToPrefab);
                //刷新
                AssetDatabase.Refresh();
            }
        }
    }
    [UnityEditor.MenuItem("Tools/SetAllChild")]
    static void SetAllChild()
    {
        GameObject terrain = GameObject.Find("StaticEnvironment");
        SetAllChild(terrain.transform);
    }

    static void SetAllChild(Transform tf)
    {
        List<string> ls = new List<string>() { "Building", "road", "ISLAND", "Island", "dirt", "SeaBottom", "pavement", "GrassTerrain", "BigMountain", "tunnel", "skaly", "t20", "pesok", "beach", "Borders", "t33", "MilitaryBase", "Pavements", "t30", "t32", "t17", "Steps", "Roof", "Obelisk", "piramid", "SphereHolder","WireObject", "Concrete_Roadblock","Pavement", "otboy","BridgeSideBlock", /*"road","road", "road","road", "road","road", "road","road", "road","road", "road","road", "road","road", "road","road", "road","road", "road","road", "road","road", "road","road", "road","road", "road","road", "road",*/ };
        foreach (var mc in tf.GetComponentsInChildren<MeshCollider>(true))
        {
            MeshFilter mf = mc.GetComponent<MeshFilter>();
            if (mf != null)
            {
                bool b = true;
                foreach (var lss in ls)
                {
                    if (mf.name.Contains(lss))
                    {
                        b = false;
                    }
                }
                if (b)
                {
                    continue;
                }
            }
            else
            {
                continue;
            }
            if (mc.transform.localScale.x < 0 && mc.transform.localScale.y < 0 && mc.transform.localScale.z < 0)
            {
                mc.transform.localScale = new Vector3(-mc.transform.localScale.x, -mc.transform.localScale.y, -mc.transform.localScale.z);

                GameObject go = new GameObject();
                go.transform.parent = mc.transform;
                go.transform.localPosition = Vector3.zero;
                go.transform.localEulerAngles = Vector3.zero;
                go.transform.localScale = new Vector3(-1, -1, -1);
                go.layer = mc.gameObject.layer;
                UnityEditorInternal.ComponentUtility.CopyComponent(mc);
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(go);
                GameObject.DestroyImmediate(mc);
            }
        }
    }


    [UnityEditor.MenuItem("Tools/SetAllChild222222")]
    static void SetAllChild2222222222()
    {
        GameObject terrain = GameObject.Find("StaticEnvironment");
        SetAllChild2222222222(terrain.transform);
    }

    static void SetAllChild2222222222(Transform tf)
    {
        foreach (var mc in tf.GetComponentsInChildren<MeshCollider>(true))
        {
            MeshFilter mf = mc.GetComponent<MeshFilter>();
            if (mf == null)
            {
                continue;
            }
            if (mc.transform.localScale.x < 0 && mc.transform.localScale.y < 0 && mc.transform.localScale.z < 0)
            {
                Debug.LogError( mc.name);
            }
        }
    }
}
