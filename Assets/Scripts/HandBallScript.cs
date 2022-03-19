using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandBallScript : MonoBehaviour
{
    new Rigidbody2D rigidbody2D;
    //Vector2 DebugForce = new Vector2(10.0f, 0.0f);
    bool stop = false;
    float speed = 0.0f;

    void Start()
    {
        rigidbody2D = this.GetComponent<Rigidbody2D>();
        SetSpeed(10.0f);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space) && !stop)
        {
            Debug.Log("SpaceKey->Debug");
            DebugAddForce();
            stop = true;
            Debug.Log("ZtoX(): " + ZtoX());
            Debug.Log("ZtoY(): " + ZtoY());
        }
    }

    void DebugAddForce()
    {
        //rigidbody2D.AddForce(DebugForce, ForceMode2D.Impulse);
        rigidbody2D.AddForce(new Vector3(ZtoX(), ZtoY(), 0.0f), ForceMode2D.Impulse);
    }

    public void SetSpeed(float speedValue)
    {
        speed = speedValue;
    }

    private float ZtoX()
    {
        float ballz = this.gameObject.transform.localEulerAngles.z % 360.0f;
        if (90 <= ballz && ballz <= 270)
        {
            return speed * (Mathf.Abs((ballz % 180) - 90) / 90) * -1;
        }
        else
        {
            return speed * (Mathf.Abs((ballz % 180) - 90) / 90);
        }
    }

    private float ZtoY()
    {
        float ballz = this.gameObject.transform.localEulerAngles.z % 360.0f;
        //Debug.Log("ballz: " + ballz);
        if(0 <= ballz && ballz <= 180)
        {
            //Debug.Log("0 <= 180");
            return speed - (speed * (Mathf.Abs((ballz % 180) - 90) / 90));
        }
        else
        {
            //Debug.Log("180 <= 360");
            return (speed - (speed * (Mathf.Abs((ballz % 180) - 90) / 90))) * -1;
        }
    }
}
