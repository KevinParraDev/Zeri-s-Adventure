using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public GameObject _playerGO;

    public float forceAttackX, forceAttackY;
    public void Attack(string type)
    {
        switch (type)
        {
            case "Bull":
                _playerGO.GetComponent<Rigidbody2D>().AddForce(new Vector2(forceAttackX, forceAttackY));
                break;
        }
    }
}
