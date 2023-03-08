
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game2D
{
    [Serializable]
    public class NavigationConnectionData
    {
        public UnitMovement action;
        public NavigationNode node;
    }
    public class NavigationNode : MonoBehaviour
    {
        public int id;

        public List<NavigationConnectionData> connections;
        
        // public UnitMovement GetPossibleMovements()
        // {
        //     
        // }
    }
}