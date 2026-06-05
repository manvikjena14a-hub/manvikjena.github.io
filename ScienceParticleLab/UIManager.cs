using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private ParticleManager particleManager;
    [SerializeField] private TextMeshProUGUI temperatureText;
    [SerializeField] private TextMeshProUGUI pressureText;
    [SerializeField] private TextMeshProUGUI elementText;
    [SerializeField] private Slider temperatureSlider;
    [SerializeField] private Slider pressureSlider;
    [SerializeField] private Button spawnButton;
    [SerializeField] private Button clearButton;

    void Start()
    {
        if (spawnButton != null)
            spawnButton.onClick.AddListener(() => particleManager.SpawnParticle(particleManager.GetSelectedElement()));
        
        if (clearButton != null)
            clearButton.onClick.AddListener(() => ClearAllParticles());

        if (temperatureSlider != null)
            temperatureSlider.onValueChanged.AddListener(OnTemperatureChanged);

        if (pressureSlider != null)
            pressureSlider.onValueChanged.AddListener(OnPressureChanged);
    }

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (temperatureText != null)
            temperatureText.text = $"Temperature: {particleManager.GetTemperature():F1}°C";

        if (pressureText != null)
            pressureText.text = $"Pressure: {particleManager.GetPressure():F2} atm";

        if (elementText != null)
            elementText.text = $"Selected: {particleManager.GetSelectedElement()}";

        // Update sliders
        if (temperatureSlider != null)
            temperatureSlider.value = particleManager.GetTemperature();

        if (pressureSlider != null)
            pressureSlider.value = particleManager.GetPressure();
    }

    void OnTemperatureChanged(float value)
    {
        // Temperature adjustment logic
    }

    void OnPressureChanged(float value)
    {
        // Pressure adjustment logic
    }

    void ClearAllParticles()
    {
        ParticleBehaviour[] particles = FindObjectsOfType<ParticleBehaviour>();
        foreach (ParticleBehaviour particle in particles)
        {
            Destroy(particle.gameObject);
        }
    }

    public void ShowReactionInfo(string reactionName, string description)
    {
        Debug.Log($"Reaction: {reactionName} - {description}");
    }
}