using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandBallScript : MonoBehaviour
{
    new Rigidbody2D rigidbody2D;
    Vector2 impulsForce = new Vector2(10.0f, 0.0f);
    bool stop = false;

    void Start()
    {
        rigidbody2D = this.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space) && !stop)
        {
            Debug.Log("SpaceKey");
            TestAddForce();
            stop = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Cushion"))
        {
            Debug.Log("CushionHit");
            //TestReverse();
        }
    }

    void TestAddForce()
    {
        rigidbody2D.AddForce(impulsForce, ForceMode2D.Impulse);
    }

    void TestReverse()
    {
        //rigidbody2D.AddRelativeForce();
    }
}
