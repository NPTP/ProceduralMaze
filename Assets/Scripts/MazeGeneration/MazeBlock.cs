using UnityEngine;

namespace MazeGeneration
{
    public class MazeBlock : MonoBehaviour
    {
        [SerializeField] private GameObject northWall;
        [SerializeField] private GameObject eastWall;
        [SerializeField] private GameObject southWall;
        [SerializeField] private GameObject westWall;

        public void MatchAbstractBlock(MazeGenerator.MazeBlockAbstract mazeBlockAbstract)
        {
            if (!mazeBlockAbstract.NorthWall)
            {
                Destroy(northWall);
            }
            
            if (!mazeBlockAbstract.EastWall)
            {
                Destroy(eastWall);
            }
            
            if (!mazeBlockAbstract.SouthWall)
            {
                Destroy(southWall);
            }
            
            if (!mazeBlockAbstract.WestWall)
            {
                Destroy(westWall);
            }
        }
    }
}