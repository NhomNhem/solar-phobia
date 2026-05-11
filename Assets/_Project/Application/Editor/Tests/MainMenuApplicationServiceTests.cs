using NUnit.Framework;
using R3;
using SolarPhobia.Application.Services;

namespace SolarPhobia.Application.Tests
{
    [TestFixture]
    public class MainMenuApplicationServiceTests
    {
        private MainMenuApplicationService _service;
        private MainMenuUiState _state;

        [SetUp]
        public void SetUp()
        {
            UnityEngine.PlayerPrefs.DeleteAll();
            _service = new MainMenuApplicationService();
            _service.CurrentState.Subscribe(next => _state = next);
        }

        [TearDown]
        public void TearDown()
        {
            _service.Dispose();
            UnityEngine.PlayerPrefs.DeleteAll();
        }

        [Test]
        public void RequestNewGame_WithExistingSave_OpensNewGameConfirm()
        {
            UnityEngine.PlayerPrefs.SetInt("SaveData.hasSave", 1);
            _service.Initialize();

            _service.RequestNewGame();

            Assert.That(_state.ScreenState, Is.EqualTo(MainMenuScreenState.NewGameConfirm));
        }

        [Test]
        public void ConfirmNewGameOverwrite_EmitsStartEvent()
        {
            var emitted = false;
            UnityEngine.PlayerPrefs.SetInt("SaveData.hasSave", 1);
            _service.Initialize();
            _service.OnStartNewGameRequested.Subscribe(_ => emitted = true);
            _service.RequestNewGame();

            _service.ConfirmNewGameOverwrite();

            Assert.That(emitted, Is.True);
            Assert.That(_state.ScreenState, Is.EqualTo(MainMenuScreenState.MainMenu));
        }

        [Test]
        public void SetSettingsTab_Accessibility_UpdatesState()
        {
            _service.Initialize();
            _service.OpenSettings();

            _service.SetSettingsTab(3);

            Assert.That(_state.ActiveTab, Is.EqualTo(MainMenuSettingsTab.Accessibility));
            Assert.That(_state.ScreenState, Is.EqualTo(MainMenuScreenState.Settings));
        }

        [Test]
        public void SetResolution_OpensVideoApplyConfirm_AndCancelReturnsToSettings()
        {
            _service.Initialize();
            _service.OpenSettings();

            _service.SetResolution("1920x1080");
            Assert.That(_state.ScreenState, Is.EqualTo(MainMenuScreenState.VideoApplyConfirm));

            _service.CancelVideoApply();
            Assert.That(_state.ScreenState, Is.EqualTo(MainMenuScreenState.Settings));
        }
    }
}
