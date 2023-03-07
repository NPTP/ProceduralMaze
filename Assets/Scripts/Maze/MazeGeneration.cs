using UnityEngine;

namespace Maze
{
    /// <summary>
    /// Static class containing the logic for abstract procedural maze generation.
    /// </summary>
    public static class MazeGeneration
    {
        /// <summary>
        /// Clockwise cardinal directions
        /// </summary>
        private enum Direction
        {
            North = 0,
            East,
            South,
            West,
        }
        
        /// <summary>
        /// Generate the maze in a 2D array top-down in row-column orientation such
        /// that maze[0][0] is the top left corner and maze[numRows - 1][numCols - 1]
        /// is the bottom right corner.
        /// </summary>
        /// <param name="numRows">Number of rows the maze will have</param>
        /// <param name="numCols">Number of columns the maze will have</param>
        /// <returns>The output 2D array representing the generated maze</returns>
        public static MazeBlockAbstract[][] GenerateMaze(int numRows, int numCols)
        {
            // Populate the maze block array with new blocks
            MazeBlockAbstract[][] maze = new MazeBlockAbstract[numRows][];
            for (int i = 0; i < numRows; i++)
            {
                maze[i] = new MazeBlockAbstract[numCols];
                for (int j = 0; j < numCols; j++)
                {
                    maze[i][j] = new MazeBlockAbstract();
                }
            }

            GenerateMazeRecursive(0, 0, maze);

            return maze;
        }

        /// <summary>
        /// Generate the maze recursively from a given maze block position.
        /// </summary>
        /// <param name="row">The row of the maze block currently being set up.</param>
        /// <param name="col">The column of the maze block currently being set up.</param>
        /// <param name="mazeBlocks">The 2D maze blocks array in row-column orientation.</param>
        private static void GenerateMazeRecursive(int row, int col, MazeBlockAbstract[][] mazeBlocks)
        {
            MazeBlockAbstract thisBlockAbstract = mazeBlocks[row][col];
            thisBlockAbstract.Visited = true;

            Direction[] directions = new[]
            {
                Direction.North,
                Direction.East,
                Direction.South,
                Direction.West
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

            // Clean up any doubled-up walls around this block
            if (thisBlockAbstract.NorthWall && row - 1 >= 0 && mazeBlocks[row - 1][col].SouthWall)
            {
                thisBlockAbstract.NorthWall = false;
            }
            
            if (thisBlockAbstract.EastWall && col + 1 < mazeBlocks[row].Length && mazeBlocks[row][col + 1].WestWall)
            {
                thisBlockAbstract.EastWall = false;
            }
            
            if (thisBlockAbstract.SouthWall && row + 1 < mazeBlocks.Length && mazeBlocks[row + 1][col].NorthWall)
            {
                thisBlockAbstract.SouthWall = false;
            }
            
            if (thisBlockAbstract.WestWall && col - 1 >= 0 && mazeBlocks[row][col - 1].EastWall)
            {
                thisBlockAbstract.WestWall = false;
            }
        }
    }
}