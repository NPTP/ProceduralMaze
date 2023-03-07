using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using Cameras;
using ScriptableObjects;
using Tools;
using UI;
using UnityEngine;

namespace Maze
{
    /// <summary>
    /// In-scene maze generator that handles actually placing and animating
    /// objects in the scene to make the maze the player will navigate.
    /// </summary>
    public class MazeGenerator : MonoBehaviour
    {
        private const string NUM_ROWS_PLAYERPREFS_KEY = "NumRows";
        private const string NUM_COLS_PLAYERPREFS_KEY = "NumCols";
        
        private const float BLOCK_SIZE = 10f;
        private const float WALL_HEIGHT = 10f;
        private const float PLANE_SCALE_TIME = 1.5f;
        private const float WALL_SCALE_TIME = 1f;
        private const float LIGHT_INSTANTIATE_STAGGER_DELAY = 0.01f;
        private const float CAMERA_MOVE_DURATION = 0.5f;
        private const int NUM_PLANE_ROTATIONS = 2;
        
        public static event Action<MazeGenerator> OnMazeGenerationStarted;
        public static event Action<MazeGenerator> OnMazeGenerationCompleted;
        public static event Action<MazeGenerator> OnFocusedOnEndBlock;
        public static event Action<MazeGenerator> OnPlayerEnteredEndBlock;

        public static int NumRows => PlayerPrefs.GetInt(NUM_ROWS_PLAYERPREFS_KEY, 1);
        public static int NumCols => PlayerPrefs.GetInt(NUM_COLS_PLAYERPREFS_KEY, 1);

        private static readonly Color StartBlockLightColor = Color.blue;
        private static readonly Color EndBlockLightColor = Color.green;
        private static readonly Color DefaultBlockLightColor = Color.yellow;

        [SerializeField] private MazeBlock mazeBlockPrefab;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Material mazePlaneMaterial;
        [SerializeField] private Material startBlockMaterial;
        [SerializeField] private Material endBlockMaterial;

        private Renderer planeRenderer;
        private Transform mazeParent;
        private readonly List<Light> lights = new List<Light>();
        private MazeBlock[][] blocks;
        private GameObject player;
        private Coroutine generationCoroutine;

        private void Awake()
        {
            GenerationScreen.OnGenerateButtonClicked += HandleGenerationButtonClicked;
        }

        private void OnDestroy()
        {
            UIManager.OnMazeRestart -= HandleMazeRestart;
            GenerationScreen.OnGenerateButtonClicked -= HandleGenerationButtonClicked;
        }
        
        public static void SetNumRows(int numRows)
        {
            PlayerPrefs.SetInt(NUM_ROWS_PLAYERPREFS_KEY, numRows);
        }

        public static void SetNumCols(int numCols)
        {
            PlayerPrefs.SetInt(NUM_COLS_PLAYERPREFS_KEY, numCols);
        }

        private void HandleGenerationButtonClicked()
        {
            GenerationScreen.OnGenerateButtonClicked -= HandleGenerationButtonClicked;
            UIManager.OnMazeRestart += HandleMazeRestart;
            
            CoroutineTools.ReplaceAndStartCoroutine(ref generationCoroutine, GenerationRoutine(), this);
        }
        
        private void HandleMazeRestart()
        {
            UIManager.OnMazeRestart -= HandleMazeRestart;
            TearDown();
            GenerationScreen.OnGenerateButtonClicked += HandleGenerationButtonClicked;
        }

        private IEnumerator GenerationRoutine()
        {
            OnMazeGenerationStarted?.Invoke(this);
            
            mazeParent = new GameObject("Maze").transform;
            planeRenderer = ConstructPlaneAndGetRenderer();
            Transform planeTransform = planeRenderer.transform;

            Vector3 goalScale = GetGoalPlaneScale(planeTransform);

            // Move the camera so it can see the full maze generation animations
            planeTransform.localScale = goalScale;
            planeTransform.rotation = Quaternion.identity;
            MazeCamera.Instance.FitBoundsInView(planeRenderer.bounds, CAMERA_MOVE_DURATION, true);
            
            yield return RotateAndScalePlane(planeTransform, goalScale);

            MazeBlockAbstract[][] maze = MazeGeneration.GenerateMaze(NumRows, NumCols);
            
            Vector3 topLeftBlockPosition = GetTopLeftBlockPosition();

            blocks = ConstructMazeBlocks(maze, topLeftBlockPosition);

            yield return ScaleUpAllWalls(topLeftBlockPosition);

            // Get the start and end blocks of the maze.
            MazeBlock playerStartBlock = blocks[NumRows - 1][0];
            MazeBlock playerEndBlock = blocks[0][NumCols - 1];
            
            yield return SetUpLights(playerStartBlock, playerEndBlock);

            // Look at the ending block so we know where the maze exit is
            yield return new WaitForSeconds(0.5f);
            Vector3 mapViewCameraPosition = MazeCamera.Instance.Position;
            FocusCameraOnBlock(playerEndBlock);
            yield return new WaitForSeconds(CAMERA_MOVE_DURATION);
            OnFocusedOnEndBlock?.Invoke(this);
            yield return new WaitForSeconds(1.5f);
            MazeCamera.Instance.MoveToPosition(mapViewCameraPosition, CAMERA_MOVE_DURATION);
            
            InstantiatePlayerAtBlock(playerStartBlock);

            yield return new WaitForSeconds(1);

            OnMazeGenerationCompleted?.Invoke(this);
        }

        /// <summary>
        /// Instantiate the player to drop into the maze at the given block.
        /// </summary>
        /// <param name="block">The maze block for the player to drop into</param>
        private void InstantiatePlayerAtBlock(MazeBlock block)
        {
            if (player != null)
            {
                Destroy(player);
            }

            Vector3 playerSpawnPosition = block.transform.position + Vector3.up * (WALL_HEIGHT / 2);
            player = Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);
        }

        /// <summary>
        /// Get the position to spawn the first block at the top left of the maze floor plane (ie row 0, column 0).
        /// </summary>
        /// <returns>The world space position where the top left block should be spawned.</returns>
        private Vector3 GetTopLeftBlockPosition()
        {
            Bounds planeRendererBounds = planeRenderer.bounds;
            Vector3 planeCenter = planeRendererBounds.center;
            Vector3 topLeftBlockPosition = new Vector3(planeCenter.x - planeRendererBounds.extents.x + BLOCK_SIZE * 0.5f,
                planeCenter.y, planeCenter.z + planeRendererBounds.extents.z - BLOCK_SIZE * 0.5f);
            return topLeftBlockPosition;
        }

        /// <summary>
        /// Set up lights in every other block, alternating start column on each row.
        /// The start and finish blocks gets special lights and materials. Lights turning
        /// on is staggered by a time delay.
        /// </summary>
        /// <param name="startBlock">The block the player begins the maze in.</param>
        /// <param name="endBlock">The block which, if reached by the player, completes the maze</param>
        private IEnumerator SetUpLights(MazeBlock startBlock, MazeBlock endBlock)
        {
            lights.Clear();
            
            for (int i = 0; i < blocks.Length; i++)
            {
                bool placeLight = i % 2 == 0;

                AudioPlayer.PlayOneShot(AudioPlayer.AudioContainer.SwitchOn);
                
                for (int j = 0; j < blocks[i].Length; j++)
                {
                    MazeBlock block = blocks[i][j];

                    if (!placeLight && block != startBlock && block != endBlock)
                    {
                        placeLight = true;
                        continue;
                    }

                    GameObject lightGameObject = new GameObject($"Light ({i})({j})");
                    Transform lightTransform = lightGameObject.transform;
                    Transform blockTransform = block.transform;
                    lightTransform.position = blockTransform.position;
                    lightTransform.parent = blockTransform;
                    Light addedLight = lightGameObject.AddComponent<Light>();
                    addedLight.type = LightType.Point;
                    lights.Add(addedLight);
                    
                    if (block == endBlock)
                    {
                        block.SetMaterial(endBlockMaterial);
                        block.CreateTriggerBox();
                        block.OnPlayerEnterBlock += HandlePlayerEnterEndBlock;
                        addedLight.color = EndBlockLightColor;
                        addedLight.intensity = 2;
                        addedLight.range = WALL_HEIGHT * 2;
                    }
                    else if (block == startBlock)
                    {
                        block.SetMaterial(startBlockMaterial);
                        addedLight.color = StartBlockLightColor;
                        addedLight.intensity = 2;
                        addedLight.range = WALL_HEIGHT * 2;
                    }
                    else
                    {
                        addedLight.color = DefaultBlockLightColor;
                        addedLight.intensity = 1f;
                        addedLight.range = WALL_HEIGHT;
                    }

                    placeLight = false;

                    // Creates staggered lighting turn-on effect
                    yield return new WaitForSeconds(LIGHT_INSTANTIATE_STAGGER_DELAY);
                }
            }
        }

        /// <summary>
        /// Scale up all maze block walls concurrently.
        /// </summary>
        /// <param name="topLeftBlockPosition">The world space position of the top left block of the maze</param>
        private IEnumerator ScaleUpAllWalls(Vector3 topLeftBlockPosition)
        {
            Vector3 goalWallScale = new Vector3(BLOCK_SIZE, WALL_HEIGHT, BLOCK_SIZE);
            
            float scaleMultiplier = 0;
            Tween tween = new Tween(() => scaleMultiplier, x => scaleMultiplier = x, Tween.Curve.EaseOutBack);
            tween.Start(1, WALL_SCALE_TIME);

            AudioPlayer.PlayOneShot(AudioPlayer.AudioContainer.WallsUp);
            
            while (scaleMultiplier < 1)
            {
                setBlockScaleAndYPosition(
                    new Vector3(goalWallScale.x, goalWallScale.y * scaleMultiplier, goalWallScale.z),
                    topLeftBlockPosition.y + goalWallScale.y * scaleMultiplier * 0.5f);
                yield return null;
            }

            // Ensure all final values are set correctly after the scaling tween
            setBlockScaleAndYPosition(goalWallScale, topLeftBlockPosition.y + goalWallScale.y * 0.5f);

            void setBlockScaleAndYPosition(Vector3 scale, float yPosition)
            {
                for (int i = 0; i < blocks.Length; i++)
                {
                    for (int j = 0; j < blocks[i].Length; j++)
                    {
                        Transform blockTransform = blocks[i][j].transform;
                        blockTransform.localScale = scale;
                        Vector3 blockPosition = blockTransform.position;
                        blockTransform.position = new Vector3(blockPosition.x, yPosition, blockPosition.z);
                    }
                }
            }
        }

        /// <summary>
        /// Start building the real maze in the scene from the abstract data
        /// </summary>
        /// <param name="mazeAbstract">The abstract representation of the maze</param>
        /// <param name="topLeftBlockPosition">The top left block position, matching the first entry of
        /// the abstract maze, where maze[i][j] gives row i and column j from the top left down</param>
        /// <returns>The 2D array containing the in-scene maze blocks</returns>
        private MazeBlock[][] ConstructMazeBlocks(MazeBlockAbstract[][] mazeAbstract, Vector3 topLeftBlockPosition)
        {
            blocks = new MazeBlock[mazeAbstract.Length][];

            for (int i = 0; i < mazeAbstract.Length; i++)
            {
                blocks[i] = new MazeBlock[mazeAbstract[i].Length];
                for (int j = 0; j < mazeAbstract[i].Length; j++)
                {
                    MazeBlock block = Instantiate(mazeBlockPrefab);
                    block.name = $"Block ({i})({j})";
                    Transform blockTransform = block.transform;
                    blockTransform.parent = mazeParent;
                    blockTransform.position = new Vector3(
                        topLeftBlockPosition.x + BLOCK_SIZE * j,
                        topLeftBlockPosition.y,
                        topLeftBlockPosition.z - BLOCK_SIZE * i);

                    block.MatchAbstractBlock(mazeAbstract[i][j]);
                    blocks[i][j] = block;
                }
            }
            
            return blocks;
        }

        /// <summary>
        /// Perform scaling and rotating of the maze floor plane
        /// </summary>
        /// <param name="planeTransform">The transform of the maze floor plane</param>
        /// <param name="goalScale">The goal scale to reach when scaling up the plane</param>
        private IEnumerator RotateAndScalePlane(Transform planeTransform, Vector3 goalScale)
        {
            planeTransform.localScale = Vector3.zero;
            Vector3 eulerRotation = planeTransform.rotation.eulerAngles;
            
            float scaleMultiplier = 0;
            Tween tween = new Tween(() => scaleMultiplier, x => scaleMultiplier = x, Tween.Curve.EaseOutBack);
            tween.Start(1, PLANE_SCALE_TIME);
            
            AudioPlayer.PlayOneShot(AudioPlayer.AudioContainer.SpinUp);

            while (scaleMultiplier < 1)
            {
                planeTransform.localScale = goalScale * scaleMultiplier;
                planeTransform.rotation = Quaternion.Euler(eulerRotation.x,
                    (NUM_PLANE_ROTATIONS * 360f * scaleMultiplier) % 360f, eulerRotation.z);
                yield return null;
            }
            
            planeTransform.localScale = goalScale;
            planeTransform.rotation = Quaternion.identity;
        }

        /// <summary>
        /// Build the maze floor plane and return its renderer for usage
        /// </summary>
        /// <returns>The maze floor plane renderer</returns>
        private Renderer ConstructPlaneAndGetRenderer()
        {
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.name = "MazeFloorPlane";
            Transform planeTransform = plane.transform;
            planeTransform.parent = mazeParent;
            Renderer rdr = plane.GetComponent<Renderer>();
            rdr.sharedMaterial = mazePlaneMaterial;
            return rdr;
        }

        /// <summary>
        /// Get the desired scale for the maze floor plane based on the
        /// user's chosen maze sizing.
        /// </summary>
        /// <param name="planeTransform">Transform of the maze floor plane</param>
        /// <returns>A vector representing the goal end scale of the plane</returns>
        private Vector3 GetGoalPlaneScale(Transform planeTransform)
        {
            Vector3 planeSize = planeRenderer.bounds.size;
            float startXSize = planeSize.x;
            float startZSize = planeSize.z;

            float goalXSize = NumCols * BLOCK_SIZE;
            float goalZSize = NumRows * BLOCK_SIZE;

            Vector3 planeScale = planeTransform.localScale;
            Vector3 goalScale = new Vector3(
                planeScale.x * (goalXSize / startXSize),
                planeScale.y,
                planeScale.z * (goalZSize / startZSize)
            );
            
            return goalScale;
        }

        private void HandlePlayerEnterEndBlock(MazeBlock endBlock)
        {
            FocusCameraOnBlock(endBlock);
            endBlock.OnPlayerEnterBlock -= HandlePlayerEnterEndBlock;
            OnPlayerEnteredEndBlock?.Invoke(this);
        }

        /// <summary>
        /// Focus the camera on the given block.
        /// </summary>
        /// <param name="block">The block to focus on</param>
        private void FocusCameraOnBlock(MazeBlock block)
        {
            Bounds blockRendererBounds = block.GetRendererBounds();
            if (blockRendererBounds.size != Vector3.zero)
            {
                MazeCamera.Instance.FitBoundsInView(blockRendererBounds, CAMERA_MOVE_DURATION, false);
            }
        }

        private void TearDown()
        {
            // Remove the player
            Destroy(player);
            player = null;

            // Tear down the maze
            for (int i = 0; i < blocks.Length; i++)
            {
                for (int j = 0; j < blocks[i].Length; j++)
                {
                    Destroy(blocks[i][j].gameObject);
                }

                blocks[i] = null;
            }
            blocks = null;

            // Tear down the lights
            foreach (Light l in lights)
            {
                Destroy(l.gameObject);
            }
            lights.Clear();

            // Tear down the plane
            Destroy(planeRenderer.gameObject);
            planeRenderer = null;

            // Tear down the parent object
            Destroy(mazeParent.gameObject);
            mazeParent = null;

            // Clear memory
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }
    }
}