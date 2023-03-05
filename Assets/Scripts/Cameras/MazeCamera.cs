using System.Collections;
using Tools;
using UI;
using UnityEngine;

namespace Cameras
{
    public class MazeCamera : Singleton<MazeCamera>
    {
        private Camera mainCamera;
        private Coroutine cameraMoveRoutine;
        private (Vector3, Quaternion) startPositionRotation;

        private void Awake()
        {
            mainCamera = Camera.main;
            startPositionRotation = (mainCamera.transform.position, mainCamera.transform.rotation);
            UIManager.OnMazeRestart += HandleMazeRestart;
        }

        private void OnDestroy()
        {
            UIManager.OnMazeRestart -= HandleMazeRestart;
        }

        private void HandleMazeRestart()
        {
            if (cameraMoveRoutine != null)
            {
                StopCoroutine(cameraMoveRoutine);
            }
            
            mainCamera.transform.position = startPositionRotation.Item1;
            mainCamera.transform.rotation = startPositionRotation.Item2;
        }

        /// <summary>
        /// Move and rotate the camera to fit the given bounds into its view.
        /// <param name="bounds">The bounds to fit in the camera view.</param>
        /// <param name="time">How long the transition to fit the bounds in view should be.</param>
        /// </summary>
        public void FitBoundsInView(Bounds bounds, float time)
        {
            // Cannot fit zero bounds into camera view.
            if (bounds.size == Vector3.zero)
            {
                return;
            }

            if (cameraMoveRoutine != null)
            {
                StopCoroutine(cameraMoveRoutine);
            }
            cameraMoveRoutine = StartCoroutine(FitBoundsInViewRoutine(bounds, time));
        }
        
        private IEnumerator FitBoundsInViewRoutine(Bounds bounds, float time)
        {
            time = Mathf.Max(0, time);

            Vector3 boundsSize = bounds.size;
            float maxSize = Mathf.Max(boundsSize.x, Mathf.Max(boundsSize.y, boundsSize.z));
            Transform mainCameraTransform = mainCamera.transform;
            Vector3 centerToCameraDirection = (mainCameraTransform.position - bounds.center).normalized;
            Vector3 destinationPosition = bounds.center + maxSize * centerToCameraDirection;

            if (time == 0)
            {
                mainCameraTransform.position = destinationPosition;
                mainCameraTransform.forward = -centerToCameraDirection;
                yield break;
            }

            float elapsedTime = 0;
            Vector3 startPos = mainCameraTransform.position;
            Quaternion startRotation = mainCameraTransform.rotation;
            
            // TODO: move up?
            Quaternion destinationLocalRotation = Quaternion.LookRotation(-centerToCameraDirection);
            
            while (elapsedTime < time)
            {
                float percentage = elapsedTime / time;
                mainCameraTransform.position = Vector3.Lerp(startPos, destinationPosition, percentage);
                mainCameraTransform.localRotation = Quaternion.Slerp(startRotation, destinationLocalRotation, percentage);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            mainCameraTransform.position = destinationPosition;
            mainCameraTransform.localRotation = destinationLocalRotation;
        }
    }
}