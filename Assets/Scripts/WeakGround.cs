using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakGround : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && other.GetComponent<PlayerMovement>()._pisoton == true)
        {
            //gameObject.SetActive(false);
            GetComponent<Animator>().enabled = true;
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
