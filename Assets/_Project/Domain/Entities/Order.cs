using System.Collections.Generic;
using ObservableCollections;

namespace SolarPhobia.Domain
{
    public class Order
    {
        private readonly ObservableDictionary<OrderType, int> _itemsNeeded = new();

        public string Id { get; set; }
        public string GhostId { get; set; }
        public IEnumerable<KeyValuePair<OrderType, int>> ItemsNeeded => _itemsNeeded;
        public int RemainingItemTypeCount => _itemsNeeded.Count;
        public float TimeLimitSeconds { get; set; } = 30f;
        public bool IsComplete { get; private set; }

        public Order(string id, string ghostId)
        {
            Id = id;
            GhostId = ghostId;
        }

        public void SetRequiredItem(OrderType type, int count)
        {
            if (count <= 0)
            {
                _itemsNeeded.Remove(type);
                return;
            }

            _itemsNeeded[type] = count;
        }

        public bool TryConsumeItem(OrderType type)
        {
            if (!_itemsNeeded.ContainsKey(type))
            {
                return false;
            }

            _itemsNeeded[type] -= 1;
            if (_itemsNeeded[type] <= 0)
            {
                _itemsNeeded.Remove(type);
            }

            if (_itemsNeeded.Count == 0)
            {
                CompleteOrder();
            }

            return true;
        }

        public void AddItem(OrderType type)
        {
            TryConsumeItem(type);
        }

        private void CompleteOrder()
        {
            IsComplete = true;
        }

        public override string ToString()
        {
            return $"{Id}::{GhostId}::{IsComplete}";
        }
    }
}
