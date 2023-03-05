using System;
using UnityEngine;

namespace Maze
{
    public class MazeBlock : MonoBehaviour
    {
        private const string PLAYER_TAG = "Player";

        public event Action<MazeBlock> OnPlayerEnterBlock; 

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

        public void CreateTriggerBox()
        {
            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(PLAYER_TAG))
            {
                OnPlayerEnterBlock?.Invoke(this);
            }
        }
    }
}