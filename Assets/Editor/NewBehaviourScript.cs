using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MyEditorUtils
{
    [MenuItem("GameTools/遍历Hierarchy")]
    static void GetAllSceneObjectsWithInactive()
    {
        var allGos = Resources.FindObjectsOfTypeAll(typeof(GameObject));
        var previousSelection = Selection.objects;
        Selection.objects = allGos;
        var selectedTransforms = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);
        Selection.objects = previousSelection;
        foreach (var trans in selectedTransforms)
        {
            if (trans.gameObject.layer == LayerMask.NameToLayer("MiniMap"))
            {
                Debug.Log(trans.name);
            }
        }
    }
}
