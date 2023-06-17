using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpMushroom : MonoBehaviour
{
    public float _jumpForce;
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            GetComponent<Animator>().SetTrigger("rebotar");
            //other.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, _jumpForce));
            other.gameObject.GetComponent<PlayerMovement>().Atacada(0, _jumpForce);
        }
    }
}
