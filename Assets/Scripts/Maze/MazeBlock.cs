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

        private BoxCollider boxCollider;
        public BoxCollider BoxCollider => boxCollider;

        public void MatchAbstractBlock(MazeBlockAbstract mazeBlockAbstract)
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
            boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
        }

        public void SetMaterial(Material material)
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer rdr in renderers)
            {
                rdr.sharedMaterial = material;
            }
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