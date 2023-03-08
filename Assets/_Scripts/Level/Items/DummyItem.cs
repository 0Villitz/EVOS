
using Game2D.Inventory;
using UnityEngine;

namespace Game2D
{
    public class DummyItem : MonoBehaviour, IInventoryItem
    {
        #region IInventoryItem

        public string Id { get; } = "DummyItem_1";
        public int Amount { get; private set; } = 1;

        public void Aggregate(int amount)
        {
            Amount += amount;
        }

        public void Apply(int amount)
        {
            Debug.LogError(nameof(DummyItem) + " cannot be applied");
        }

        public string DisplayName { get; } = "DummyItem";
        public string DisplayDescription { get; } = "Does Nothing";
        public string DisplaySpriteName { get; } = "DummyItemSprite";

        public void Interact(ICharacterController characterController)
        {
            characterController.AddToInventory(this);
        }

        #endregion
    }
}