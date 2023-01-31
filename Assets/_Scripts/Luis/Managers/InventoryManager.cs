
using System.Collections.Generic;
using System.Linq;
using Game2D.Inventory;
using UnityEngine;

namespace GameManagers
{
    public class InventoryManager
    {
        private Dictionary<string, IInventoryItem> _inventory = new Dictionary<string, IInventoryItem>();

        public void AddItem(IInventoryItem item)
        {
            if (_inventory.TryGetValue(item.Id, out IInventoryItem inventoryItem)
                && inventoryItem != null
               )
            {
                inventoryItem.Aggregate(item.Amount);
            }
            else
            {
                _inventory[item.Id] = item;
            }
        }

        public void ApplyItem(string id, int amount)
        {
            IInventoryItem inventoryItem = GetInventoryItem(id);
            if (inventoryItem.Amount < amount)
            {
                Debug.LogError("Not enough items in inventory item " + id);
            }
            else
            {
                inventoryItem.Apply(amount);
                if (inventoryItem.Amount <= 0)
                {
                    _inventory.Remove(inventoryItem.Id);
                }
            }
        }

        public List<IInventoryItem> GetInventoryItems()
        {
            return _inventory.Values.ToList();
        }

        private IInventoryItem GetInventoryItem(string id)
        {
            _inventory.TryGetValue(id, out IInventoryItem inventoryItem);
            return inventoryItem;
        }
    }
}