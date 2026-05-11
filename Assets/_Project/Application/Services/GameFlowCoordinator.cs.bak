using SolarPhobia.Application.Messages;
using SolarPhobia.Application.UseCases;
using SolarPhobia.Domain;

namespace SolarPhobia.Application.Services
{
    public class GameFlowCoordinator
    {
        private readonly StartDayUseCase _startDayUseCase;
        private readonly SubmitDialogueChoiceUseCase _submitDialogueChoiceUseCase;
        private readonly CompleteOrderUseCase _completeOrderUseCase;
        private readonly TransitionToNightUseCase _transitionToNightUseCase;
        private readonly TravelToNextShrineUseCase _travelToNextShrineUseCase;
        private readonly EvaluateEndingUseCase _evaluateEndingUseCase;

        public GameSessionState State { get; }

        public GameFlowCoordinator(
            StartDayUseCase startDayUseCase,
            SubmitDialogueChoiceUseCase submitDialogueChoiceUseCase,
            CompleteOrderUseCase completeOrderUseCase,
            TransitionToNightUseCase transitionToNightUseCase,
            TravelToNextShrineUseCase travelToNextShrineUseCase,
            EvaluateEndingUseCase evaluateEndingUseCase,
            GameSessionState state)
        {
            _startDayUseCase = startDayUseCase;
            _submitDialogueChoiceUseCase = submitDialogueChoiceUseCase;
            _completeOrderUseCase = completeOrderUseCase;
            _transitionToNightUseCase = transitionToNightUseCase;
            _travelToNextShrineUseCase = travelToNextShrineUseCase;
            _evaluateEndingUseCase = evaluateEndingUseCase;
            State = state;
        }

        public void StartDay()
        {
            _startDayUseCase.Execute(new StartDayCommand(State));
        }

        public void SubmitChoice(Choice choice)
        {
            _submitDialogueChoiceUseCase.Execute(new SubmitDialogueChoiceCommand(State, choice));
        }

        public void CompleteOrder(Order order)
        {
            _completeOrderUseCase.Execute(new CompleteOrderCommand(State, order));
        }

        public void GoNight()
        {
            _transitionToNightUseCase.Execute(new TransitionToNightCommand(State));
        }

        public void TravelTo(string shrineId)
        {
            _travelToNextShrineUseCase.Execute(new TravelToNextShrineCommand(State, shrineId));
        }

        public EndingType EvaluateEnding()
        {
            return _evaluateEndingUseCase.Execute(new EvaluateEndingQuery(State));
        }
    }
}

