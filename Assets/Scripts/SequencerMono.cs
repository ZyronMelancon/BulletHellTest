using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Zyron;

public class SequencerMono : MonoBehaviour
{
    public bool startOnAwake = false;
    public List<SequenceEvent> Sequence;
    public int loops = 1;

    public SequenceEvent Current;
    public int index = 0;
    public bool going = false;
    public int loopsleft;

    public void OnEnable()
    {
        if (Sequence == null)
            Sequence = new List<SequenceEvent>();
    }

    public void BeginSequence()
    {
        index = 0;
        Sequence[index].Doit(this);
        going = true;
    }

    public void StopSequence()
    {
        going = false;
        StopAllCoroutines();
    }

    public void MoveNext()
    {
        if (index != Sequence.Count - 1)
            index++;
        else
        {
            index = 0;
            loopsleft--;
        }
        if(loopsleft == 0)
        {
            going = false;
            return;
        }

        Sequence[index].Doit(this);
    }

    private void Start()
    {
        loopsleft = loops;
        if (Sequence == null)
            Sequence = new List<SequenceEvent>();
        Sequence.ForEach(s => s._onDone = MoveNext);

        if (startOnAwake)
            BeginSequence();
    }
}

[CustomEditor(typeof(SequencerMono))]
public class SequenceEditor : Editor
{
    private ReorderableList list;
    private bool loop;

    protected virtual void OnEnable()
    {
        // Create the list
        list = new ReorderableList(serializedObject, serializedObject.FindProperty("Sequence"), true, true, true, true)
        {
            drawHeaderCallback = DrawHeaderCallback,
            elementHeightCallback = ElementHeightCallback,
            drawElementCallback = DrawElementCallback
        };
    }

    public void DrawHeaderCallback(Rect rect)
    {
        EditorGUI.LabelField(rect, "Sequence");
    }
    
    public void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        var element = list.serializedProperty.GetArrayElementAtIndex(index);
        rect.y += 2;

        EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width - 50, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("Response"), GUIContent.none);

        EditorGUI.PropertyField(
            new Rect(rect.x + rect.width - 25, rect.y, 25, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("time"), GUIContent.none);
    }

    public float ElementHeightCallback(int index)
    {
        var element = list.serializedProperty.GetArrayElementAtIndex(index);
        var elementHeight = EditorGUI.GetPropertyHeight(element.FindPropertyRelative("Response"));
        // optional, depending on the situation in question and the defaults you like
        // you may want to subtract the margin out in the drawElementCallback before drawing
        var margin = EditorGUIUtility.standardVerticalSpacing + 15;
        return elementHeight + margin;
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        var toggle = serializedObject.FindProperty("startOnAwake");
        EditorGUILayout.PropertyField(toggle);
        var loopamt = serializedObject.FindProperty("loops");
        EditorGUILayout.PropertyField(loopamt);
        GUILayout.Space(10);

        list.DoLayoutList();
        GUILayout.Space(10);
        
        if (EditorGUI.EndChangeCheck())
        {
            Debug.Log("changes made");

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }


    }
}