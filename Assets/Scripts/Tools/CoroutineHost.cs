using System.Collections;
using UnityEngine;

namespace Tools
{
    /// <summary>
    /// Singleton class for hosting coroutines from other classes so as not to rely
    /// on MonoBehaviours being active for them to run.
    /// </summary>
    public class CoroutineHost : Singleton<CoroutineHost>
    {
        /// <summary>
        /// Start the given routine hosted by the coroutine hosted.
        /// </summary>
        /// <param name="routine">IEnumerator routine to be started and hosted by the coroutine host</param>
        /// <returns>The created Coroutine from starting the given routine</returns>
        public static Coroutine StartHostedCoroutine(IEnumerator routine)
        {
            return Instance.StartCoroutine(routine);
        }

        /// <summary>
        /// Stops the coroutine at the given reference.
        /// </summary>
        /// <param name="coroutine">Ref parameter to the coroutine to be stopped</param>
        public static void StopHostedCoroutine(ref Coroutine coroutine)
        {
            if (coroutine != null)
            {
                Instance.StopCoroutine(coroutine);
            }
        }

        /// <summary>
        /// Stops the coroutine at the given ref if it exists, and replaces it by starting a new
        /// coroutine using the given IEnumerator
        /// </summary>
        /// <param name="routine">The IEnumerator to use for the new coroutine</param>
        /// <param name="coroutine">The updated coroutine after starting the new routine</param>
        public static void InterruptAndStartCoroutine(IEnumerator routine, ref Coroutine coroutine)
        {
            if (coroutine != null)
            {
                Instance.StopCoroutine(coroutine);
            }

            coroutine = Instance.StartCoroutine(routine);
        }
    }
}