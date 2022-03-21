using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandBallScript : MonoBehaviour
{
    new Rigidbody2D rigidbody2D;
    [SerializeField] float speed = 0.0f;
    bool BreakShot = false;
    bool StepHeadArea = true;
    bool StepRotation = false;
    bool StepSpeed = false;

    public GameObject SpeedChangeOBJ;
    public GameObject SpeedArrow;

    void Start()
    {
        rigidbody2D = this.GetComponent<Rigidbody2D>();
        SetSpeed(20.0f);
        SetBreakShotTrue();
    }

    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("SpaceKey->Debug");
            AddForce();
            //Debug.Log("ZtoX(): " + ZtoX());
            //Debug.Log("ZtoY(): " + ZtoY());
        }*/
        if (Input.GetMouseButtonDown(0)) StepMove();
        if (BreakShot && StepHeadArea) MouseFollowHeadSpotArea();
        else if (!BreakShot && StepHeadArea) FreeBall();
        if (StepRotation) MouseFollowRotation();
        if (StepSpeed) MouseFollowSpeed();
    }

    void AddForce()
    {
        rigidbody2D.AddForce(new Vector3(ZtoX(), ZtoY(), 0.0f), ForceMode2D.Impulse);
    }

    public void SetSpeed(float speedValue)
    {
        speed = speedValue;
    }

    void SetBreakShotTrue()
    {
        BreakShot = true;
    }

    void SetBreakShotFalse()
    {
        BreakShot = false;
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
        if (0 <= ballz && ballz <= 180)
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

    private void StepMove()
    {
        if (StepHeadArea)
        {
            StepHeadArea = false;
            StepRotation = true;
            StepSpeed = false;
        }
        else if (StepRotation)
        {
            StepHeadArea = false;
            StepRotation = false;
            StepSpeed = true;
        }
        else if (StepSpeed)
        {
            //AllFalse
            StepHeadArea = false;
            StepRotation = false;
            StepSpeed = false;
            SpeedChangeOBJ.SetActive(false);
            AddForce();
        }
        else
        {
            StepRotation = true;
        }
    }

    private void MouseFollowHeadSpotArea()
    {
        rigidbody2D.velocity = Vector2.zero;
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mouse.x <= -4.3f)
        {
            mouse.x = -4.3f;
        }
        if (-2.6f <= mouse.x)
        {
            mouse.x = -2.6f;
        }
        if (mouse.y <= -1.825f)
        {
            mouse.y = -1.826f;
        }
        if (1.825f <= mouse.y)
        {
            mouse.y = 1.825f;
        }
        mouse.z = 1.0f;
        this.gameObject.transform.position = mouse;
    }

    private void FreeBall()
    {
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        this.gameObject.transform.position = mouse;
        if (mouse.x <= -4.3f)
        {
            mouse.x = -4.3f;
        }
        if (4.3f <= mouse.x)
        {
            mouse.x = 4.3f;
        }
        if (mouse.y <= -1.825f)
        {
            mouse.y = -1.826f;
        }
        if (1.825f <= mouse.y)
        {
            mouse.y = 1.825f;
        }
        this.gameObject.transform.position = mouse;

    }

    private void MouseFollowRotation()
    {
        Vector2 mouse = this.gameObject.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        this.transform.rotation = Quaternion.FromToRotation(Vector2.right, mouse);
    }

    private void MouseFollowSpeed()
    {
        //Debug.Log("MouseFollowSpeed");
        SpeedChangeOBJ.SetActive(true);
        SpeedChangeOBJ.transform.rotation = Quaternion.identity;
        Vector3 mouse = this.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = -2.0f;
        mouse.x = SpeedChangeOBJ.transform.position.x - 0.1f;
        mouse.y = mouse.y * -1;
        if(mouse.y <= -1.7f)
        {
            mouse.y = SpeedChangeOBJ.transform.position.y - 1.7f;
        }
        if(1.743 <= mouse.y)
        {
            mouse.y = SpeedChangeOBJ.transform.position.y + 1.743f;
        }
        SpeedArrow.transform.position = mouse;
        SetSpeed((2.0f + mouse.y) * 10);
    }
}
