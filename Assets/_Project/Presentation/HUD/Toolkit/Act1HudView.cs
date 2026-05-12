using System;
using System.Collections.Generic;
using SolarPhobia.Application.Messages;
using SolarPhobia.Domain;
using SolarPhobia.Domain.ValueObjects;
using UnityEngine;
using UnityEngine.UIElements;

namespace SolarPhobia.Presentation.HUD
{
    [RequireComponent(typeof(UIDocument))]
    public class Act1HudView : MonoBehaviour
    {
        private VisualElement _root;
        private Label _phaseLabel;
        private Label _kindnessLabel;
        private Label _spiritLabel;
        private DialogueView _dialogueView;
        private OrderQueueView _orderQueueView;

        public GameSessionState State { get; private set; }
        public event Action<Choice> ChoiceSelected;

        private void Awake()
        {
            var document = GetComponent<UIDocument>();
            _root = document.rootVisualElement;
            BuildUi();
        }

        public void Bind(GameSessionState state)
        {
            State = state;
            RefreshState();
        }

        public void ShowDialogue(DialogueNode node)
        {
            _dialogueView?.Show(node);
        }

        public void RenderOrders(IReadOnlyDictionary<OrderType, int> itemsNeeded)
        {
            _orderQueueView?.Render(itemsNeeded);
        }

        public void RefreshState()
        {
            if (State == null)
            {
                return;
            }

            _phaseLabel.text = $"Phase: {State.CurrentPhase}";
            _kindnessLabel.text = $"Kindness: {State.KindnessScore.Value}";
            _spiritLabel.text = $"Spirit: {State.SpiritEssence}";
        }

        private void BuildUi()
        {
            _phaseLabel = _root.Q<Label>("phase-label");
            _kindnessLabel = _root.Q<Label>("kindness-label");
            _spiritLabel = _root.Q<Label>("spirit-label");

            var dialogueRoot = _root.Q<VisualElement>("dialogue-view");
            var orderQueueRoot = _root.Q<VisualElement>("order-queue-view");

            _dialogueView = new DialogueView();
            _dialogueView.Bind(dialogueRoot);
            _dialogueView.ChoiceSelected += choice => ChoiceSelected?.Invoke(choice);

            _orderQueueView = new OrderQueueView();
            _orderQueueView.Bind(orderQueueRoot);
        }
    }
}
