
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

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(Color.red.r, Color.red.g, Color.red.b, 0.25f);
            Gizmos.DrawSphere(transform.position, 0.5f);

            foreach (NavigationConnectionData connectionData in connections)
            {
                Gizmos.DrawLine(transform.position, connectionData.node.transform.position);
            }
        }
    }
}