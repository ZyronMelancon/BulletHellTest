using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

namespace Zyron
{

    [CreateAssetMenu(menuName = "Scriptables/Sequencer")]
    public class Sequencer : ScriptableObject
    {
        public List<SequenceEvent> Sequence;

        public SequenceEvent Current;
        public int start = 0;

        public void OnEnable()
        {
            if (Sequence == null)
                Sequence = new List<SequenceEvent>();
            Sequence.ForEach(s => s._onDone = MoveNext);
            Current = Sequence[start];
        }

        public void BeginSequence(MonoBehaviour mb)
        {
            Current.Doit(mb);
        }

        public void MoveNext()
        {
            start++;
            Current = Sequence[start];
        }
    }

    [System.Serializable]
    public class SequenceEvent
    {
        public SequenceEvent(System.Action onDone)
        {
            _onDone = onDone;
        }

        public System.Action _onDone;
        [SerializeField]
        private UnityEvent Response;
        public float time = 2;

        public void Doit(MonoBehaviour mb)
        {
            mb.StartCoroutine(Invoke());
        }

        public IEnumerator Invoke()
        {
            var ct = time;
            while (ct > 0)
            {
                ct -= Time.deltaTime;
                yield return null;
            }

            Response.Invoke();
            time = 2;
            _onDone.Invoke();
        }
    }


    [CustomEditor(typeof(Sequencer))]
    public class SequenceEditor : Editor
    {
        private ReorderableList list;
        private SerializedProperty spEvent;

        private void OnEnable()
        {

            // Create the list
            list = new ReorderableList(serializedObject, serializedObject.FindProperty("Sequence"), true, true, true, true)
            {
                drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Sequence"); }
            };

            // Draw header label

            // Draw each element in the list
            list.drawElementCallback =
                (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = list.serializedProperty.GetArrayElementAtIndex(index);
                    //rect.y += 40;

                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, rect.width - 30, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("Response"), GUIContent.none);

                    EditorGUI.PropertyField(
                        new Rect(rect.x + rect.width - 25, rect.y, 25, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("time"), GUIContent.none);
                    GUILayout.Space(25);        
                };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            GUILayout.Space(25);
            list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }


}