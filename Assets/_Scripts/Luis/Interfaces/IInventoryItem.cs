
namespace GameManagers.Inventory
{
    public interface IInventoryItem
    {
        string Id { get; }
        int Amount { get; }
        void Aggregate(int amount);
        void Apply(int amount);

        string DisplayName { get; }
        string DisplayDescription { get; }
        string DisplaySpriteName { get; }
    }
}