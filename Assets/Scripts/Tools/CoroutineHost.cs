using System.Collections;
using UnityEngine;

namespace Tools
{
    public class CoroutineHost : Singleton<CoroutineHost>
    {
        public static Coroutine StartHostedCoroutine(IEnumerator routine)
        {
            return Instance.StartCoroutine(routine);
        }

        public static void StopHostedCoroutine(ref Coroutine coroutine)
        {
            if (coroutine != null)
            {
                Instance.StopCoroutine(coroutine);
            }
        }

        public static void InterruptAndStartCoroutine(IEnumerator routine, ref Coroutine coroutine)
        {
            Instance.InterruptAndStartCoroutineInternal(routine, ref coroutine);
        }

        private void InterruptAndStartCoroutineInternal(IEnumerator routine, ref Coroutine coroutine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            coroutine = StartCoroutine(routine);
        }
    }
}