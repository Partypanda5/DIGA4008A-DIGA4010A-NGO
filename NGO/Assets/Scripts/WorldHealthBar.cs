using UnityEngine;
using UnityEngine.UI;

public class WorldHealthBar : MonoBehaviour
{
    [SerializeField] private Image fill;            // drag your Fill Image here
    [SerializeField] private NetworkHealth health;  // can auto-find if left empty

    private void OnEnable() //When the NetworkVariable changes, call my OnHealthChanged method.
    {
        if (health != null)
            health.Health.OnValueChanged += OnHealthChanged; //on value changed needs an old value and a new value

        UpdateFill();
    }

    private void OnDisable() //This unsubscribes from the event / Stop listening to health changes.
    {
        if (health != null)
            health.Health.OnValueChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(int oldValue, int newValue)
    {
        UpdateFill();
    }

    private void UpdateFill()
    {
        if (!fill || health == null) return;
        fill.fillAmount = health.Health01; // the Health01 return float in NetworkHealth
    }
}