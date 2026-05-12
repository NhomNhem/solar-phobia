using UnityEngine;

namespace SolarPhobia.Infrastructure.VFX
{
    /// <summary>
    /// Centralized director for managing global shader parameters and VFX states.
    /// Acts as the single source of truth for environmental data in shaders.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class SolarPhobiaVFXDirector : MonoBehaviour
    {
        public static SolarPhobiaVFXDirector Instance { get; private set; }

        [Header("Live Global Parameters")]
        [SerializeField, Range(0, 1)] private float _phase01;
        [SerializeField, Range(0, 1)] private float _dayPressure01;
        [SerializeField, Range(0, 1)] private float _nightDanger01;
        [SerializeField, Range(0, 1)] private float _sensoryDecay01;

        [Header("Tracked Objects")]
        [SerializeField] private Transform _playerTransform;

        public float Phase01 { get => _phase01; set => _phase01 = Mathf.Clamp01(value); }
        public float DayPressure01 { get => _dayPressure01; set => _dayPressure01 = Mathf.Clamp01(value); }
        public float NightDanger01 { get => _nightDanger01; set => _nightDanger01 = Mathf.Clamp01(value); }
        public float SensoryDecay01 { get => _sensoryDecay01; set => _sensoryDecay01 = Mathf.Clamp01(value); }

        public Transform PlayerTransform { get => _playerTransform; set => _playerTransform = value; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            if (transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }

            // Attempt to find player if not set
            if (_playerTransform == null)
            {
                var player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    _playerTransform = player.transform;
                }
            }
        }

        private void Update()
        {
            UpdateGlobalShaderParameters();
        }

        private void UpdateGlobalShaderParameters()
        {
            Shader.SetGlobalFloat(ShaderPropertyIds.Phase01, _phase01);
            Shader.SetGlobalFloat(ShaderPropertyIds.DayPressure01, _dayPressure01);
            Shader.SetGlobalFloat(ShaderPropertyIds.NightDanger01, _nightDanger01);
            Shader.SetGlobalFloat(ShaderPropertyIds.SensoryDecay01, _sensoryDecay01);

            if (_playerTransform != null)
            {
                Shader.SetGlobalVector(ShaderPropertyIds.PlayerWorldPos, _playerTransform.position);
            }
        }
    }
}
