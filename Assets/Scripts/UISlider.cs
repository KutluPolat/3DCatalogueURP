using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UISlider : MonoBehaviour
{
    public bool isMaxed => slider.value == slider.maxValue;
    public float maxValue => slider.maxValue;
    public float value => slider.value;

    [SerializeField] private Slider slider;
    [SerializeField] private float incrementAmount = 1f;
    [SerializeField] private Button decrease, increase;
    private float initCooldown = 0.4f, minCooldown = 0.1f;
    private bool isDecreasePointerDown, isIncreasePointerDown;
    private float currentDuration = 0f;
    private float currentCooldown = 0f;

    public void OnDecreasePointerDown()
    {
        isDecreasePointerDown = true;
        PointerDown();
    }

    public void OnDecreasePointerUp()
    {
        isDecreasePointerDown = false;
    }

    public void OnIncreasePointerDown()
    {
        isIncreasePointerDown = true;
        PointerDown();
    }

    public void OnIncreasePointerUp()
    {
        isIncreasePointerDown = false;
    }

    private void PointerDown()
    {
        currentDuration = 0f;
        currentCooldown = initCooldown;
    }

    private void Increment(float amount)
    {
        slider.value += amount;
        currentCooldown = Mathf.Max(minCooldown, currentCooldown - 0.2f);
        currentDuration = currentCooldown;
    }

    private void Update()
    {
        if (isDecreasePointerDown || isIncreasePointerDown)
        {
            currentDuration -= Time.deltaTime;
            if (currentDuration < 0f)
            {
                Increment(isIncreasePointerDown ? incrementAmount : -incrementAmount);
            }
        }
    }

    public void OnValueChanged(UnityAction<float> action)
    {
        slider.onValueChanged.AddListener(action);
    }
    public void SetValue(int newValue)
    {
        slider.value = newValue;
    }

    public void SetMin(int min)
    {
        slider.minValue = min;
    }

    public void SetMax(int max)
    {
        slider.maxValue = max;
    }
}