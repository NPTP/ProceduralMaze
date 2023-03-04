using System;
using System.Collections;
using Tools;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MazeGeneration
{
    // TODO: separate static abstract maze generation from a class that actually hosts resources, keeps track of instances etc
    public static class MazeGenerator
    {
        /// <summary>
        /// Called when the maze generation begins. Sends the goal position and scale.
        /// </summary>
        public static event Action<Vector3, Vector3> OnMazeGenerationBegun;
        
        /// <summary>
        /// Clockwise cardinal directions
        /// </summary>
        public enum Direction
        {
            North = 0,
            East,
            South,
            West,
        }
        
        public const string NUM_ROWS_PLAYERPREFS_KEY = "NumRows";
        public const string NUM_COLS_PLAYERPREFS_KEY = "NumCols";
        
        public class MazeBlockAbstract
        {
            public bool Visited;
            public bool NorthWall = true;
            public bool SouthWall = true;
            public bool EastWall = true;
            public bool WestWall = true;
        }
        
        private const float BLOCK_SIZE = 10f;
        private const float WALL_HEIGHT = 10f;
        private const float PLANE_SCALE_TIME = 1.5f;
        private const float WALL_SCALE_TIME = 1f;
        private const int PLANE_NUM_ROTATIONS = 2;

        private static readonly Color startBlockLightColor = Color.white;
        private static readonly Color endBlockLightColor = Color.green;
        private static readonly Color defaultBlockLightColor = Color.yellow;

        public static void Generate()
        {
            int numRows = PlayerPrefs.GetInt(NUM_ROWS_PLAYERPREFS_KEY, 1);
            int numCols = PlayerPrefs.GetInt(NUM_COLS_PLAYERPREFS_KEY, 1);

            Debug.Log($"Generating maze with {numRows} rows and {numCols} columns.");
            
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.name = "MazeFloorPlane";

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
            
            OnMazeGenerationBegun?.Invoke(planeTransform.position, goalScale);
            CoroutineHost.StartHostedCoroutine(GenerationRoutine(numRows, numCols, planeTransform, goalScale, planeRenderer));
        }

        /// <summary>
        /// Generate the maze in a 2D array top-down in row-column orientation such
        /// that maze[0][0] is the top left corner and maze[numRows - 1][numCols - 1]
        /// is the bottom right corner.
        /// </summary>
        /// <param name="numRows">Number of rows the maze will have</param>
        /// <param name="numCols">Number of columns the maze will have</param>
        /// <param name="maze">The output 2D array representing the generated maze</param>
        private static void GenerateMaze(int numRows, int numCols, out MazeBlockAbstract[][] maze)
        {
            // Populate the maze block array with new blocks
            maze = new MazeBlockAbstract[numRows][];
            for (int i = 0; i < numRows; i++)
            {
                maze[i] = new MazeBlockAbstract[numCols];
                for (int j = 0; j < numCols; j++)
                {
                    maze[i][j] = new MazeBlockAbstract();
                }
            }

            GenerateMazeRecursive(0, 0, maze);
        }

        /// <summary>
        /// Generate the maze recursively from a given maze block position.
        /// </summary>
        /// <param name="row">The row of the maze block currently being set up.</param>
        /// <param name="col">The column of the maze block currently being set up.</param>
        /// <param name="mazeBlocks">The 2D maze blocks array in row-column orientation.</param>
        private static void GenerateMazeRecursive(int row, int col, MazeBlockAbstract[][] mazeBlocks)
        {
            Debug.Log($"GenerateMazeRecursive: row {row}, col {col}");
            MazeBlockAbstract thisBlockAbstract = mazeBlocks[row][col];
            thisBlockAbstract.Visited = true;

            Direction[] directions = new[]
            {
                MazeGenerator.Direction.North,
                MazeGenerator.Direction.East,
                MazeGenerator.Direction.South,
                MazeGenerator.Direction.West
            };

            // Shuffle the order of directions to choose a random maze block.
            for (int i = directions.Length - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                (directions[i], directions[randomIndex]) = (directions[randomIndex], directions[i]);
            }

            for (int i = 0; i < directions.Length; i++)
            {
                Direction direction = directions[i];
                switch (direction)
                {
                    case Direction.North when row - 1 >= 0 && !mazeBlocks[row - 1][col].Visited:
                        thisBlockAbstract.NorthWall = false;
                        mazeBlocks[row - 1][col].SouthWall = false;
                        GenerateMazeRecursive(row - 1, col, mazeBlocks);
                        break;
                    case Direction.East when col + 1 < mazeBlocks[row].Length && !mazeBlocks[row][col + 1].Visited:
                        thisBlockAbstract.EastWall = false;
                        mazeBlocks[row][col + 1].WestWall = false;
                        GenerateMazeRecursive(row, col + 1, mazeBlocks);
                        break;
                    case Direction.South when row + 1 < mazeBlocks.Length && !mazeBlocks[row + 1][col].Visited:
                        thisBlockAbstract.SouthWall = false;
                        mazeBlocks[row + 1][col].NorthWall = false;
                        GenerateMazeRecursive(row + 1, col, mazeBlocks);
                        break;
                    case Direction.West when col - 1 >= 0 && !mazeBlocks[row][col - 1].Visited:
                        thisBlockAbstract.WestWall = false;
                        mazeBlocks[row][col - 1].EastWall = false;
                        GenerateMazeRecursive(row, col - 1, mazeBlocks);
                        break;
                }
            }
        }

        private static IEnumerator GenerationRoutine(int numRows, int numCols, Transform planeTransform, Vector3 goalScale, Renderer planeRenderer)
        {
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
            
            GenerateMaze(numRows, numCols, out MazeBlockAbstract[][] maze);

            Bounds planeRendererBounds = planeRenderer.bounds;
            Vector3 planeCenter = planeRendererBounds.center;
            
            // Start the block spawn position at the top left corner position on the plane.
            Vector3 blockStartPos = new Vector3(
                planeCenter.x - planeRendererBounds.extents.x + BLOCK_SIZE * 0.5f,
                planeCenter.y,
                planeCenter.z + planeRendererBounds.extents.z - BLOCK_SIZE * 0.5f);

            Transform[][] cubeTransforms = new Transform[numRows][];
            Vector3 goalWallScale = new Vector3(BLOCK_SIZE, WALL_HEIGHT, BLOCK_SIZE);
            
            // LOAD CALL!!!!!
            MazeBlock mazeBlockPrefab = Resources.Load<MazeBlock>("MazeBlockPrefab");
            Transform mazeParent = new GameObject("Maze").transform;

            for (int i = 0; i < numRows; i++)
            {
                cubeTransforms[i] = new Transform[numCols];
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
                    cubeTransforms[i][j] = blockTransform;
                }
            }
            
            // Scale up all the walls simultaneously
            scaleMultiplier = 0;
            scaleMultiplierTween.Start(1, WALL_SCALE_TIME);
            
            // Tween loop
            while (scaleMultiplier < 1)
            {
                for (int i = 0; i < numRows; i++)
                {
                    for (int j = 0; j < numCols; j++)
                    {
                        Transform blockTransform = cubeTransforms[i][j];
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
                    Transform blockTransform = cubeTransforms[i][j];
                    blockTransform.localScale = goalWallScale;
                    blockTransform.position = new Vector3(
                        blockTransform.position.x,
                        blockStartPos.y + goalWallScale.y * 0.5f,
                        blockTransform.position.z);
                }
            }
            
            // Set up lights in every other block, alternating start column on each row.
            // The start and finish blocks gets special lights regardless.
            for (int i = 0; i < numRows; i++)
            {
                bool placeLight = i % 2 == 0 ? true : false;
                
                for (int j = 0; j < numCols; j++)
                {
                    bool isStartBlock = i == numRows - 1 && j == 0;
                    bool isEndBlock = i == 0 && j == numCols - 1;
                    
                    if (!placeLight && !isStartBlock && !isEndBlock)
                    {
                        placeLight = true;
                        continue;
                    }
                    
                    GameObject lightGameObject = new GameObject("Light ({i})({j})");
                    Transform lightTransform = lightGameObject.transform;
                    Transform blockTransform = cubeTransforms[i][j];
                    lightTransform.position = blockTransform.position;
                    lightTransform.parent = blockTransform;

                    Light light = lightGameObject.AddComponent<Light>();
                    light.type = LightType.Point;
                    if (isStartBlock)
                    {
                        light.color = startBlockLightColor;
                        light.intensity = 0.5f;
                        light.range = WALL_HEIGHT * 2;
                    }
                    else if (isEndBlock)
                    {
                        light.color = endBlockLightColor;
                        light.intensity = 0.5f;
                        light.range = WALL_HEIGHT * 2;
                    }
                    else
                    {
                        light.color = defaultBlockLightColor;
                        light.intensity = 1f;
                        light.range = WALL_HEIGHT;
                    }

                    placeLight = false;

                    yield return new WaitForSecondsRealtime(0.01f);
                }
            }
        }
    }
}