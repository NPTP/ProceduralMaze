using System.Collections;
using UnityEngine;

namespace Tools
{
    public static class CoroutineTools
    {
        public static void StopAndNullCoroutine(ref Coroutine coroutine, MonoBehaviour owningMonoBehaviour)
        {
            if (coroutine != null)
            {
                owningMonoBehaviour.StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        public static void ReplaceAndStartCoroutine(ref Coroutine coroutine, IEnumerator iEnumerator,
            MonoBehaviour owningMonoBehaviour)
        {
            StopAndNullCoroutine(ref coroutine, owningMonoBehaviour);
            coroutine = owningMonoBehaviour.StartCoroutine(iEnumerator);
        }
    }
}