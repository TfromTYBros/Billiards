using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallsScript : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Hole"))
        {
            Debug.Log("HoleHit" + this.gameObject.name);
            this.gameObject.SetActive(false);
        }
    }
}
