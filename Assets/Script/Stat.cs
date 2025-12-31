using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField] private float baseValue;
    private List<float> modifiers = new();

    public float BaseValue
    {
        get => baseValue;
        set => baseValue = value;
    }

    public float Value
    {
        get
        {
            float v = baseValue;
            foreach (var m in modifiers)
                v += m;

            return v;
        }
    }

    public void AddModifier(float value) => modifiers.Add(value);
    public void RemoveModifier(float value) => modifiers.Remove(value);
}
