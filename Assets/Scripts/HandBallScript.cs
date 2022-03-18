using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandBallScript : MonoBehaviour
{
    new Rigidbody2D rigidbody2D;
    Vector2 DebugForce = new Vector2(10.0f, 0.0f);
    bool stop = false;

    void Start()
    {
        rigidbody2D = this.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space) && !stop)
        {
            Debug.Log("SpaceKey->Debug");
            DebugAddForce();
            stop = true;
            Debug.Log(ZtoX());
        }
    }

    void DebugAddForce()
    {
        //rigidbody2D.AddForce(DebugForce, ForceMode2D.Impulse);
        rigidbody2D.AddForce(this.transform.forward, ForceMode2D.Impulse);
    }

    private float ZtoX()
    {
        return (this.gameObject.transform.localEulerAngles.z % 90);
    }
}
