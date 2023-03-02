
using System.Collections.Generic;
using Game2D.GamePhysics;
using Game2D.Inventory;
using UnityEngine;

namespace Game2D
{
    public interface ICharacterController
    {
        void GrabClimbObject(IClimbObject climbObject);
        void AddToInventory(IInventoryItem inventoryObject);
    }
}