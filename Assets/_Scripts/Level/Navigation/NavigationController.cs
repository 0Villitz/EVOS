
using System.Collections.Generic;
using UnityEngine;

namespace Game2D
{
    public class NavigationController : MonoBehaviour
    {
        [SerializeField]
        private NavigationNode navigationNodePrefab;

        private Dictionary<int, NavigationNode> _mapNodes;
        private Dictionary<int, List<int>> _mapConnections;

        [SerializeField] private List<NavigationNode> _navigationNodes;

        public void AddNode(Vector3 worldPosition, int id)
        {
            NavigationNode node = GameObject.Instantiate(navigationNodePrefab, worldPosition, Quaternion.identity);
            node.id = id;
            node.name = node.GetType().Name + "_" + id;
            node.transform.SetParent(transform);
            
            _navigationNodes.Add(node);
        }

        public void Discard()
        {
            _mapNodes = new Dictionary<int, NavigationNode>();
            _mapConnections = new Dictionary<int, List<int>>();
            _navigationNodes = new List<NavigationNode>();
            
            for (int idx = transform.childCount - 1; idx >= 0; idx--)
            {
                DestroyImmediate(transform.GetChild(idx).gameObject);
            }
        }
    }
}