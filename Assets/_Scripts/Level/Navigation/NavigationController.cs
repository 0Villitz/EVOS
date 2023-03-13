
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game2D
{
    public class NavigationController : MonoBehaviour
    {
        [SerializeField]
        private NavigationNode navigationNodePrefab;

        private Dictionary<int, NavigationNode> _mapNodes;

        [SerializeField] private List<NavigationNode> _navigationNodes;

        #region Monobehavior

        private void Awake()
        {
            _mapNodes = new Dictionary<int, NavigationNode>();
            
            foreach (NavigationNode navNode in _navigationNodes)
            {
                _mapNodes.Add(navNode.id, navNode);
            }
        }

        #endregion
    }
}