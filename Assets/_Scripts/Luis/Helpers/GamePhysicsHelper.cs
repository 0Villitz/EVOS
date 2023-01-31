
using UnityEngine;

namespace Game2D.GamePhysics
{
    public static class GamePhysicsHelper
    {
        public enum Layers : int
        {
            Player = 6,
            Ground = 7,
            Platform = 15
        }

        public static void IgnoreLayerCollision(
            Layers layer1,
            Layers layer2,
            bool ignore
        )
        {
            Physics.IgnoreLayerCollision((int)layer1, (int)layer2, ignore);
        }

        public static bool RayCast(
            Vector3 origin,
            Vector3 direction,
            float distance,
            Layers[] layers,
            out RaycastHit raycastHit
        )
        {
            int layerMask = 0;
            for (int i = 0; i < layers.Length; i++)
            {
                layerMask |= (1 << (int)layers[i]);
            }

            return Physics.Raycast(
                origin,
                direction,
                out raycastHit,
                distance,
                layerMask
            );
        }
    }
}