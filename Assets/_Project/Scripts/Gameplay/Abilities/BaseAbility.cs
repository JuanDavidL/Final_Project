using UnityEngine;

public abstract class BaseAbility : MonoBehaviour
{
    [Header("Habilidad")]
    public string abilityName;
    public Sprite abilityIcon;
    public float cooldown;
    public float manaCost;
    public float damage;

    [Header("Quantity")]
    public int quantity = 1;

    [Header("Chager")]
    public int maxCharge = 1;
    private int _currentCharge;
    private float[] _chargeTimers;

    protected float isIndicatorA = -10f;
    public bool isIndicatorActive = false;
    private bool _initialized = false;

    void Awake()
    {
        InitCharge();
    }


    private void InitCharge()
    {
        _currentCharge = maxCharge;
        _chargeTimers = new float[maxCharge];
        for (int i = 0; i < maxCharge; i++)
            _chargeTimers[i] = -cooldown;

        _initialized = true;
    }

    protected virtual void Update()
    {
        if (!_initialized) InitCharge();
        RechargeUpdate();
    }

    private void RechargeUpdate()
    {
        if (_currentCharge >= maxCharge) return;

        for (int i = 0; i < maxCharge; i++)
        {
        if (_chargeTimers[i] > 0)
            {
                float elapsed = Time.time - _chargeTimers[i];
            
                if (elapsed >= cooldown)
                {
                _currentCharge++;
                _chargeTimers[i] = 0f;
                break;
                }
            }
        }
    }

    public bool TryConsumeCharge()
    {
        if (_currentCharge <= 0) 
        {
            //Debug.Log($"{abilityName}: Sin cargas disponibles");
            return false;
        }

        _currentCharge--;
        //Debug.Log($"{abilityName}: Carga consumida. Quedan: {_currentCharge}/{maxCharge}");

        // Busca el primer timer libre y lo activa
        for (int i = 0; i < maxCharge; i++)
        {
            if (_chargeTimers[i] <= 0f)
            {
                _chargeTimers[i] = Time.time;
                 //Debug.Log($"{abilityName}: Timer[{i}] activado en {Time.time}");
                break;
            }
        }

        return true;
    }

    public bool IsOnCooldown()
    {
        return _currentCharge <= 0;
    }

    public float GetCooldownRemaining()
    {
        // Devuelve el tiempo restante de la carga que termina primero
        float shortest = float.MaxValue;

        for (int i = 0; i < maxCharge; i++)
        {
            if (_chargeTimers[i] > 0f)
            {
                float remaining = cooldown - (Time.time - _chargeTimers[i]);
                if (remaining < shortest)
                    shortest = remaining;
            }
        }

        return shortest == float.MaxValue ? 0f : Mathf.Max(0f, shortest);
    }

    public int GetCurrentCharges() => _currentCharge;
    public int GetMaxCharges() => maxCharge;

    public virtual void ShowIndicator()
    {
        isIndicatorActive = true;
    }

    public virtual void HideIndicator()
    {
        isIndicatorActive = false;
    }

    public abstract void Use();

}
