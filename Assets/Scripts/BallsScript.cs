using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallsScript : MonoBehaviour
{
    HandBallScript HBS;
    new Rigidbody2D rigidbody2D;
    private void Start()
    {
        HBS = FindObjectOfType<HandBallScript>();
        rigidbody2D = FindObjectOfType<Rigidbody2D>();
    }
    private void Update()
    {
        HBS.SetMoveBalls(ThisBallNumber(),IsBallSleeping());
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Hole"))
        {
            Debug.Log("HoleHit" + this.gameObject.name);
            HBS.SetMoveBalls(ThisBallNumber(), true);
            this.gameObject.SetActive(false);
        }
    }
    private bool IsBallSleeping()
    {
        return rigidbody2D.IsSleeping();
    }

    int ThisBallNumber()
    {
        string name = this.gameObject.name;
        if (name == "Ball0") return 0;
        else if (name == "Ball1") return 1;
        else if (name == "Ball2") return 2;
        else if (name == "Ball3") return 3;
        else if (name == "Ball4") return 4;
        else if (name == "Ball5") return 5;
        else if (name == "Ball6") return 6;
        else if (name == "Ball7") return 7;
        else if (name == "Ball8") return 8;
        else if (name == "Ball9") return 9;
        else if (name == "Ball10") return 10;
        else if (name == "Ball11") return 11;
        else if (name == "Ball12") return 12;
        else if (name == "Ball13") return 13;
        else return 14;
    }
}
