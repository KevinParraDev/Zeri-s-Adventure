using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundManager : MonoBehaviour
{
    [SerializeField] private string map = "Cave";
    [SerializeField] private GameObject bgCave;
    [SerializeField] private GameObject bgSky;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (map == "Sky")
            {
                bgCave.SetActive(false);
                bgSky.SetActive(true);
            }
            else if (map == "Cave")
            {
                bgCave.SetActive(true);
                bgSky.SetActive(false);
            }
        }
    }
}
