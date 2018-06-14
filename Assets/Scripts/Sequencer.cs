using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

namespace Zyron
{
    [System.Serializable]
    public class SequenceEvent
    {
        public System.Action _onDone;
        [SerializeField]
        private UnityEvent Response;
        public float time;

        public SequenceEvent()
        {

        }

        public SequenceEvent(System.Action onDone)
        {
            _onDone = onDone;
        }

        public void Doit(MonoBehaviour mb)
        {
            mb.StartCoroutine(Invoke());
        }

        public IEnumerator Invoke()
        {
            if (time > 0)
                yield return new WaitForSeconds(time);
            else
                yield return null;

            Response.Invoke();

            var targetType = Response.GetPersistentTarget(0).GetType();

            if(targetType == typeof(SequencerMono))
            {
                if (Response.GetPersistentMethodName(0) == "BeginSequence")
                {
                    float seqtime = 0;

                    var seq = Response.GetPersistentTarget(0) as SequencerMono;
                    foreach (SequenceEvent s in seq.Sequence)
                        seqtime += s.time * seq.loops;

                    yield return new WaitForSeconds(seqtime);
                }
            }

            _onDone.Invoke();
        }
    }
}