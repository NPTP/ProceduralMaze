using System.Collections;
using UnityEngine;

namespace Tools
{
    /// <summary>
    /// Static utility methods for oft-repeated code around coroutine use.
    /// </summary>
    public static class CoroutineTools
    {
        /// <summary>
        /// Stop the given ref coroutine and set its reference to null.
        /// </summary>
        /// <param name="coroutine">The coroutine to stop and set to null</param>
        /// <param name="owningMonoBehaviour">The MonoBehaviour that owned the coroutine</param>
        public static void StopAndNullCoroutine(ref Coroutine coroutine, MonoBehaviour owningMonoBehaviour)
        {
            if (coroutine != null)
            {
                owningMonoBehaviour.StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        /// <summary>
        /// Stops the coroutine at the given ref if it exists, and replaces it by starting a new
        /// coroutine using the given IEnumerator
        /// </summary>
        /// <param name="routine">The IEnumerator to use for the new coroutine</param>
        /// <param name="coroutine">The updated coroutine after starting the new routine</param>
        /// <param name="owningMonoBehaviour">The MonoBehaviour that owned the coroutine</param>
        public static void ReplaceAndStartCoroutine(ref Coroutine coroutine, IEnumerator routine,
            MonoBehaviour owningMonoBehaviour)
        {
            StopAndNullCoroutine(ref coroutine, owningMonoBehaviour);
            coroutine = owningMonoBehaviour.StartCoroutine(routine);
        }
    }
}