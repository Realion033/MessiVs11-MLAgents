using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Entity Stats")]
    public float maxHealth;
    public string entityName;

    protected float currentHealth;

    void Awake()
    {
        Initialize();
    }
    
    public virtual void Initialize()
    {
        currentHealth = maxHealth;
    }
}
