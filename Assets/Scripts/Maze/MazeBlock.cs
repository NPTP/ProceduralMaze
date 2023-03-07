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

        /// <summary>
        /// Given an abstract maze block, match this game object maze block's wall configuration to it.
        /// </summary>
        /// <param name="mazeBlockAbstract">The abstract maze block to match</param>
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

        public Bounds GetRendererBounds()
        {
            Bounds bounds = new Bounds();
            
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            if (renderers.Length > 0)
            {
                bounds = renderers[0].bounds;
                for (int i = 1; i < renderers.Length; i++)
                {
                    bounds.Encapsulate(renderers[i].bounds);
                }
            }

            return bounds;
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