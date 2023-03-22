
using UnityEngine;

namespace Game2D
{

    public interface IAttackerObject
    {
        Transform GetTransform();
        void ProcessAttack();
    }
}