using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallsScript : MonoBehaviour
{
    HandBallScript HBS;

    private void Start()
    {
        HBS = FindObjectOfType<HandBallScript>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Hole"))
        {
            Debug.Log("HoleHit" + this.gameObject.name);
            HBS.PocketCountPlus();
            this.gameObject.SetActive(false);
        }

        if (collision.gameObject.CompareTag("Cushion") && this.gameObject == HBS.GetCurrMinimumBall())
        {
            //Debug.Log("CushionHit" + this.gameObject.name);
            HBS.TrueClear_Cushion_CurrBall();
            HBS.CushionHitCountUp();
        }
    }

}
