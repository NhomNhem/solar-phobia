using System.Collections.Generic;
using SolarPhobia.Domain;
using SolarPhobia.Domain.ValueObjects;
using UnityEngine.UIElements;

namespace SolarPhobia.Presentation.HUD
{
    public sealed class OrderQueueView
    {
        private VisualElement _root;
        private VisualElement _itemsContainer;

        public void Bind(VisualElement root)
        {
            _root = root;
            _itemsContainer = _root.Q<VisualElement>("orders-container");
        }

        public void Render(IReadOnlyDictionary<OrderType, int> itemsNeeded)
        {
            if (_itemsContainer == null)
            {
                return;
            }

            _itemsContainer.Clear();

            if (itemsNeeded == null)
            {
                return;
            }

            foreach (var pair in itemsNeeded)
            {
                _itemsContainer.Add(new Label($"{pair.Key}: {pair.Value}"));
            }
        }
    }
}
