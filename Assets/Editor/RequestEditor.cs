using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Request))]
public class RequestEditor : Editor
{
    const int reqSize = 10;
    Request[] request = new Request[reqSize];
    /*
    private void OnEnable()
    {
        for (int i = 0; i < reqSize; i++)
        {
            request[i] = target as Request;
            request[i].availableCard = new List<string>();
        }
    }

    public override void OnInspectorGUI()
    {
        for (int i = 0; i < reqSize; i++)
        {
            request[i].requestType = EditorGUILayout.TextField("Type", request[i].requestType);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Content");
            request[i].requestContent = EditorGUILayout.TextArea(request[i].requestContent);
            EditorGUILayout.EndHorizontal();
            
        }
    }
    */
}
