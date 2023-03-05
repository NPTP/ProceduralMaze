using System;
using System.Collections;
using System.Collections.Generic;
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
        private const float BLOCK_SIZE = 10f;
        private const float WALL_HEIGHT = 10f;
        private const float PLANE_SCALE_TIME = 1.5f;
        private const float WALL_SCALE_TIME = 1f;
        private const int PLANE_NUM_ROTATIONS = 2;
        private const string NUM_ROWS_PLAYERPREFS_KEY = "NumRows";
        private const string NUM_COLS_PLAYERPREFS_KEY = "NumCols";
        public static event Action OnMazeGenerationCompleted;
        public static event Action OnPlayerEnteredEndBlock;

        public static int NumRows => PlayerPrefs.GetInt(NUM_ROWS_PLAYERPREFS_KEY, 1);
        public static int NumCols => PlayerPrefs.GetInt(NUM_COLS_PLAYERPREFS_KEY, 1);

        private static readonly Color startBlockLightColor = Color.blue;
        private static readonly Color endBlockLightColor = Color.green;
        private static readonly Color defaultBlockLightColor = Color.yellow;

        [SerializeField] private MazeBlock mazeBlockPrefab;
        [SerializeField] private Material mazePlaneMaterial;
        [SerializeField] private GameObject playerPrefab;

        private Transform mazeParent;
        private readonly List<Light> lights = new List<Light>();
        private MazeBlock[][] blocks;
        private Renderer planeRenderer;
        private GameObject player;

        private void Awake()
        {
            GenerationScreen.OnGenerateButtonClicked += HandleGenerationButtonClicked;
        }

        private void OnDestroy()
        {
            UIManager.OnMazeRestart -= HandleMazeRestart;
            GenerationScreen.OnGenerateButtonClicked -= HandleGenerationButtonClicked;
        }

        private void HandleGenerationButtonClicked()
        {
            GenerationScreen.OnGenerateButtonClicked -= HandleGenerationButtonClicked;
            UIManager.OnMazeRestart += HandleMazeRestart;
            Generate();
        }
        
        private void HandleMazeRestart()
        {
            UIManager.OnMazeRestart -= HandleMazeRestart;
            TearDown();
            GenerationScreen.OnGenerateButtonClicked += HandleGenerationButtonClicked;
        }

        public static void SetNumRows(int numRows)
        {
            PlayerPrefs.SetInt(NUM_ROWS_PLAYERPREFS_KEY, numRows);
        }

        public static void SetNumCols(int numCols)
        {
            PlayerPrefs.SetInt(NUM_COLS_PLAYERPREFS_KEY, numCols);
        }

        private void Generate()
        {
            mazeParent = new GameObject("Maze").transform;

            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.name = "MazeFloorPlane";
            plane.transform.parent = mazeParent;
            planeRenderer = plane.GetComponent<Renderer>();
            planeRenderer.sharedMaterial = mazePlaneMaterial;
            
            Vector3 planeSize = planeRenderer.bounds.size;
            float startXSize = planeSize.x;
            float startZSize = planeSize.z;
            
            float goalXSize = NumCols * BLOCK_SIZE;
            float goalZSize = NumRows * BLOCK_SIZE;
            
            Transform planeTransform = plane.transform;
            Vector3 planeScale = planeTransform.localScale;
            Vector3 goalScale = new Vector3(
                planeScale.x * (goalXSize / startXSize),
                planeScale.y,
                planeScale.z * (goalZSize / startZSize)
            );
            
            CoroutineHost.StartHostedCoroutine(GenerationRoutine(goalScale));
        }

        private IEnumerator GenerationRoutine(Vector3 goalScale)
        {
            int numRows = NumRows;
            int numCols = NumCols;
            
            Transform planeTransform = planeRenderer.transform;
            planeTransform.localScale = Vector3.zero;
            float scaleMultiplier = 0;
            Vector3 eulerRotation = planeTransform.rotation.eulerAngles;
            Tween scaleMultiplierTween = new Tween(() => scaleMultiplier, x => scaleMultiplier = x, Tween.Curve.EaseOutBack);
            scaleMultiplierTween.Start(1, PLANE_SCALE_TIME);

            while (scaleMultiplier < 1)
            {
                planeTransform.localScale = goalScale * scaleMultiplier;
                planeTransform.rotation = Quaternion.Euler(eulerRotation.x, (PLANE_NUM_ROTATIONS * 360f * scaleMultiplier) % 360f, eulerRotation.z);
                yield return null;
            }

            planeTransform.localScale = goalScale;
            planeTransform.rotation = Quaternion.identity;
            
            MazeGeneration.GenerateMaze(numRows, numCols, out MazeBlockAbstract[][] maze);

            Bounds planeRendererBounds = planeRenderer.bounds;
            Vector3 planeCenter = planeRendererBounds.center;
            
            // Start the block spawn position at the top left corner position on the plane.
            Vector3 blockStartPos = new Vector3(
                planeCenter.x - planeRendererBounds.extents.x + BLOCK_SIZE * 0.5f,
                planeCenter.y,
                planeCenter.z + planeRendererBounds.extents.z - BLOCK_SIZE * 0.5f);

            blocks = new MazeBlock[numRows][];
            
            for (int i = 0; i < numRows; i++)
            {
                blocks[i] = new MazeBlock[numCols];
                for (int j = 0; j < numCols; j++)
                {
                    MazeBlock block = GameObject.Instantiate(mazeBlockPrefab);
                    block.name = $"Block ({i})({j})";
                    Transform blockTransform = block.transform;
                    blockTransform.parent = mazeParent;
                    blockTransform.position = new Vector3(
                        blockStartPos.x + BLOCK_SIZE * j,
                        blockStartPos.y,
                        blockStartPos.z - BLOCK_SIZE * i);

                    block.MatchAbstractBlock(maze[i][j]);
                    blocks[i][j] = block;
                }
            }
            
            // Scale up all the walls simultaneously
            scaleMultiplier = 0;
            scaleMultiplierTween.Start(1, WALL_SCALE_TIME);
            Vector3 goalWallScale = new Vector3(BLOCK_SIZE, WALL_HEIGHT, BLOCK_SIZE);
            
            // Tween loop
            while (scaleMultiplier < 1)
            {
                for (int i = 0; i < numRows; i++)
                {
                    for (int j = 0; j < numCols; j++)
                    {
                        Transform blockTransform = blocks[i][j].transform;
                        blockTransform.localScale = new Vector3(
                            goalWallScale.x, 
                            goalWallScale.y * scaleMultiplier, 
                            goalWallScale.z);
                        blockTransform.position = new Vector3(
                            blockTransform.position.x, 
                            blockStartPos.y + goalWallScale.y * scaleMultiplier * 0.5f, 
                            blockTransform.position.z);
                    }
                }
                
                yield return null;
            }
            
            // Set final values loop
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    Transform blockTransform = blocks[i][j].transform;
                    blockTransform.localScale = goalWallScale;
                    blockTransform.position = new Vector3(
                        blockTransform.position.x,
                        blockStartPos.y + goalWallScale.y * 0.5f,
                        blockTransform.position.z);
                }
            }
            
            // Set up lights in every other block, alternating start column on each row.
            // The start and finish blocks gets special lights regardless.
            lights.Clear();
            for (int i = 0; i < numRows; i++)
            {
                bool placeLight = i % 2 == 0 ? true : false;
                
                for (int j = 0; j < numCols; j++)
                {
                    bool isEndBlock = i == 0 && j == numCols - 1;
                    bool isStartBlock = i == numRows - 1 && j == 0;
                    
                    if (!placeLight && !isStartBlock && !isEndBlock)
                    {
                        placeLight = true;
                        continue;
                    }

                    MazeBlock block = blocks[i][j];
                    GameObject lightGameObject = new GameObject("Light ({i})({j})");
                    Transform lightTransform = lightGameObject.transform;
                    Transform blockTransform = block.transform;
                    lightTransform.position = blockTransform.position;
                    lightTransform.parent = blockTransform;

                    Light addedLight = lightGameObject.AddComponent<Light>();
                    lights.Add(addedLight);
                    addedLight.type = LightType.Point;
                    if (isEndBlock)
                    {
                        // TODO: add trigger box on ending block
                        block.CreateTriggerBox();
                        block.OnPlayerEnterBlock += HandlePlayerEnterEndBlock;
                        addedLight.color = endBlockLightColor;
                        addedLight.intensity = 2f;
                        addedLight.range = WALL_HEIGHT * 2;
                        
                    }
                    else if (isStartBlock)
                    {
                        addedLight.color = startBlockLightColor;
                        addedLight.intensity = 2f;
                        addedLight.range = WALL_HEIGHT * 2;
                    }
                    else
                    {
                        addedLight.color = defaultBlockLightColor;
                        addedLight.intensity = 1f;
                        addedLight.range = WALL_HEIGHT;
                    }

                    placeLight = false;

                    yield return new WaitForSecondsRealtime(0.01f);
                }
            }
            
            // Instantiate the player at the starting block
            // (if there is only one block, this is also the ending block)
            if (player == null)
            {
                Vector3 startBlockPosition = blocks[numRows - 1][0].transform.position;
                player = GameObject.Instantiate(playerPrefab,
                    startBlockPosition + Vector3.up * 5,
                    Quaternion.identity);
            }
            
            OnMazeGenerationCompleted?.Invoke();
        }

        private static void HandlePlayerEnterEndBlock(MazeBlock endBlock)
        {
            endBlock.OnPlayerEnterBlock -= HandlePlayerEnterEndBlock;
            OnPlayerEnteredEndBlock?.Invoke();
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
            for (int i = 0; i < lights.Count; i++)
            {
                Destroy(lights[i].gameObject);
            }
            lights.Clear();

            // Tear down the plane
            Destroy(planeRenderer.gameObject);
            
            Destroy(mazeParent.gameObject);
            
            Resources.UnloadUnusedAssets();
        }
    }
}