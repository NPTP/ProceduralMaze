namespace Maze
{
    /// <summary>
    /// Class for abstract maze generation.
    /// </summary>
    public class MazeBlockAbstract
    {
        public bool Visited;
        public bool NorthWall = true;
        public bool SouthWall = true;
        public bool EastWall = true;
        public bool WestWall = true;
    }
}