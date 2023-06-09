using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float _lifePoints;

    public void Attack(float hitForce)
    {
        if (_lifePoints - hitForce > 0)
            _lifePoints -= hitForce;
        else
        {
            Death();
        }
    }

    private void Death()
    {
        Debug.Log("Se murióooo");
    }
}
