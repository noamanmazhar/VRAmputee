using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EMGInteraction))]
public class EMGInteractionGUI : Editor
{
    public override void OnInspectorGUI()
    {
        EMGInteraction myScript = (EMGInteraction)target;

        DrawDefaultInspector();

        EditorGUILayout.Space(70);

        GUIStyle style = new GUIStyle(GUI.skin.box);
        Color blueColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);
        Texture2D blueTexture = new Texture2D(1, 1);
        blueTexture.SetPixel(0, 0, blueColor);
        blueTexture.Apply();
        style.normal.background = blueTexture;
        style.normal.textColor = Color.white;
        style.margin = new RectOffset(0, 0, 0, 0);

        GUILayout.BeginVertical(style);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Set Right Hand"))
        {
            myScript.SetRightHand();
        }

        if (GUILayout.Button("Set Left Hand"))
        {
            myScript.SetLeftHand();
        }


        GUILayout.EndHorizontal();
        GUILayout.EndVertical();


        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Current Hand: ------------   " + myScript.CurrentHand.ToString() + "   ------------", EditorStyles.boldLabel);



    }
}