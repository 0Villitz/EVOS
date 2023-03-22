
using Game2D.Inventory;

namespace Game2D
{
    public interface ICharacterController
    {
        void GrabClimbObject(IClimbObject climbObject);
        void AddToInventory(IInventoryItem inventoryObject);
    }
}