using System.Collections;
using Tools;
using UnityEngine;

namespace MazeGeneration
{
    public static class MazeGenerator
    {
        public const string NUM_ROWS_PLAYERPREFS_KEY = "NumRows";
        public const string NUM_COLS_PLAYERPREFS_KEY = "NumCols";
        
        private const float BLOCK_SIZE = 10f;
        private const float WALL_HEIGHT = 10f;
        private const float PLANE_SCALE_TIME = 3f;
        private const float WALL_SCALE_TIME = 0.1f;
        private const int PLANE_NUM_ROTATIONS = 3;

        public static void GenerateMaze()
        {
            int numRows = PlayerPrefs.GetInt(NUM_ROWS_PLAYERPREFS_KEY, 1);
            int numCols = PlayerPrefs.GetInt(NUM_COLS_PLAYERPREFS_KEY, 1);
            
            Debug.Log($"Generating maze with {numRows} rows and {numCols} columns.");
            
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);

            Renderer planeRenderer = plane.GetComponent<Renderer>();
            Vector3 planeSize = planeRenderer.bounds.size;
            float startXSize = planeSize.x;
            float startZSize = planeSize.z;
            
            float goalXSize = numCols * BLOCK_SIZE;
            float goalZSize = numRows * BLOCK_SIZE;
            
            Transform planeTransform = plane.transform;
            Vector3 planeScale = planeTransform.localScale;
            Vector3 goalScale = new Vector3(
                planeScale.x * (goalXSize / startXSize),
                planeScale.y,
                planeScale.z * (goalZSize / startZSize)
            );
            
            CoroutineHost.StartHostedCoroutine(GenerationRoutine(numRows, numCols, planeTransform, goalScale, planeRenderer));
        }

        private static IEnumerator GenerationRoutine(int numRows, int numCols, Transform planeTransform, Vector3 goalScale, Renderer planeRenderer)
        {
            planeTransform.localScale = Vector3.zero;
            float scaleMultiplier = 0;
            Vector3 eulerRotation = planeTransform.rotation.eulerAngles;
            Tween scaleMultiplierTween = new Tween(() => scaleMultiplier, x => scaleMultiplier = x);
            scaleMultiplierTween.Start(1, PLANE_SCALE_TIME);

            while (scaleMultiplier < 1)
            {
                planeTransform.localScale = goalScale * scaleMultiplier;
                planeTransform.rotation = Quaternion.Euler(eulerRotation.x, (PLANE_NUM_ROTATIONS * 360f * scaleMultiplier) % 360f, eulerRotation.z);
                yield return null;
            }

            planeTransform.localScale = goalScale;
            planeTransform.rotation = Quaternion.identity;
            
            Debug.Log("Plane scaling completed.");

            // TODO: actually generate the maze here

            Bounds planeRendererBounds = planeRenderer.bounds;
            Vector3 planeCenter = planeRendererBounds.center;
            
            // Start the block spawn position at the bottom left corner position on the plane.
            Vector3 blockStartPos = new Vector3(
                planeCenter.x - planeRendererBounds.extents.x + BLOCK_SIZE * 0.5f,
                planeCenter.y,
                planeCenter.z - planeRendererBounds.extents.z + BLOCK_SIZE * 0.5f);

            Transform[][] cubeTransforms = new Transform[numRows][];
            Vector3 goalWallScale = new Vector3(BLOCK_SIZE, WALL_HEIGHT, BLOCK_SIZE);

            for (int i = 0; i < numRows; i++)
            {
                cubeTransforms[i] = new Transform[numCols];
                for (int j = 0; j < numCols; j++)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.name = $"Cube ({i})({j})";
                    Transform cubeTransform = cube.transform;
                    cubeTransform.position = new Vector3(
                        blockStartPos.x + BLOCK_SIZE * j,
                        blockStartPos.y,
                        blockStartPos.z + BLOCK_SIZE * i);

                    scaleMultiplier = 0;
                    scaleMultiplierTween.Start(1, WALL_SCALE_TIME);

                    while (scaleMultiplier < 1)
                    {
                        cubeTransform.localScale = new Vector3(goalWallScale.x, goalWallScale.y * scaleMultiplier,
                            goalWallScale.z);
                        cubeTransform.position = new Vector3(cubeTransform.position.x,
                            blockStartPos.y + goalWallScale.y * scaleMultiplier * 0.5f, cubeTransform.position.z);
                        yield return null;
                    }

                    cubeTransform.localScale = goalWallScale;
                    cubeTransform.position = new Vector3(cubeTransform.position.x,
                        blockStartPos.y + goalWallScale.y * 0.5f, cubeTransform.position.z);

                    cubeTransforms[i][j] = cubeTransform;
                }
            }
        }
    }
}