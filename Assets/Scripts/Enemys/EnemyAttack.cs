using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public PlayerMovement playerMovement;

    public float forceAttackX, forceAttackY;
    public void Attack(string type)
    {
        switch (type)
        {
            case "Bull":
                playerMovement.Atacada(forceAttackX, forceAttackY);
                break;
        }
    }
}
