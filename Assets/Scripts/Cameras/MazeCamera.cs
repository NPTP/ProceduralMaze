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
        private Vector3 sceneStartPosition;
        private Quaternion sceneStartRotation;

        public Vector3 Position => mainCamera.transform.position;

        private void Awake()
        {
            mainCamera = Camera.main;
            sceneStartPosition = mainCamera.transform.position;
            sceneStartRotation = mainCamera.transform.rotation;

            UIManager.OnMazeRestart += HandleMazeRestart;
        }

        private void OnDestroy()
        {
            UIManager.OnMazeRestart -= HandleMazeRestart;
        }

        private void HandleMazeRestart()
        {
            CoroutineTools.StopAndNullCoroutine(ref cameraMoveRoutine, this);
            mainCamera.transform.position = sceneStartPosition;
            mainCamera.transform.rotation = sceneStartRotation;
        }

        /// <summary>
        /// Move the camera to the given position over the course of a given duration.
        /// </summary>
        /// <param name="position">The destination position for the camera.</param>
        /// <param name="duration">The duration of the transition from the camera's current position to the destination.</param>
        public void MoveToPosition(Vector3 position, float duration)
        {
            CoroutineTools.ReplaceAndStartCoroutine(ref cameraMoveRoutine,
                MoveCameraRoutine(position, mainCamera.transform.localRotation, duration, false), this);
        }

        /// <summary>
        /// Move and rotate the camera to fit the given bounds into its view.
        /// <param name="bounds">The bounds to fit in the camera view.</param>
        /// <param name="duration">How long the transition to fit the bounds in view should be.</param>
        /// </summary>
        public void FitBoundsInView(Bounds bounds, float duration, bool useRotation)
        {
            // Cannot fit zero bounds into camera view.
            if (bounds.size == Vector3.zero)
            {
                return;
            }

            Vector3 boundsSize = bounds.size;
            float maxSize = Mathf.Max(boundsSize.x, Mathf.Max(boundsSize.y, boundsSize.z));
            Transform mainCameraTransform = mainCamera.transform;
            Vector3 centerToCameraDirection = (mainCameraTransform.position - bounds.center).normalized;
            Vector3 destinationPosition = bounds.center + maxSize * centerToCameraDirection;
            Quaternion destinationLocalRotation = Quaternion.LookRotation(-centerToCameraDirection);

            CoroutineTools.ReplaceAndStartCoroutine(ref cameraMoveRoutine,
                MoveCameraRoutine(destinationPosition, destinationLocalRotation, duration, useRotation), this);
        }

        private IEnumerator MoveCameraRoutine(Vector3 position, Quaternion localRotation, float duration, bool useRotation)
        {
            Transform mainCameraTransform = mainCamera.transform;
            
            duration = Mathf.Max(0, duration);

            if (duration == 0)
            {
                mainCameraTransform.position = position;
                if (useRotation) mainCameraTransform.localRotation = localRotation;
                yield break;
            }

            Vector3 startPos = mainCameraTransform.position;
            Quaternion startRotation = mainCameraTransform.localRotation;
            float elapsedTime = 0;
            
            while (elapsedTime < duration)
            {
                float percentage = elapsedTime / duration;
                percentage *= percentage; // Quadratic curve
                mainCameraTransform.position = Vector3.Lerp(startPos, position, percentage);
                if (useRotation) mainCameraTransform.localRotation = Quaternion.Slerp(startRotation, localRotation, percentage);
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            mainCameraTransform.position = position;
            if (useRotation) mainCameraTransform.localRotation = localRotation;
        }
    }
}