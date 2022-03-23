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
    public GameObject CantTouchAreaBox;
    public GameObject Que;

    [SerializeField] public List<bool> MoveBalls;

    void Start()
    {
        rigidbody2D = this.GetComponent<Rigidbody2D>();
        MakeMoveBalls();
        CantTouchAreaBox.SetActive(true);
        SetSpeed(20.0f);
        SetBreakShotTrue();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && IsAllSleeping()) StepMove();
        if (BreakShot && StepHeadArea) MouseFollowHeadSpotArea();
        else if (!BreakShot && StepHeadArea) FreeBall();
        if (StepRotation) MouseFollowRotation();
        if (StepSpeed) MouseFollowSpeed();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hole"))
        {
            SetBreakShotFalse();
            SetFirstStep();
        }
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
            CantTouchAreaBox.SetActive(false);
        }
        else if (StepRotation)
        {
            StepHeadArea = false;
            StepRotation = false;
            StepSpeed = true;
            Que.SetActive(false);
        }
        else if (StepSpeed)
        {
            SetFirstStep();
            SpeedChangeOBJ.SetActive(false);
            AddForce();
        }
        else
        {
            StepRotation = true;
        }
    }

    private void SetFirstStep()
    {
        StepHeadArea = false;
        StepRotation = false;
        StepSpeed = false;
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
        rigidbody2D.velocity = Vector2.zero;
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
        Que.SetActive(true);
        Vector2 mouse = this.gameObject.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        this.transform.rotation = Quaternion.FromToRotation(Vector2.right, mouse);
    }

    private void MouseFollowSpeed()
    {
        //Debug.Log("MouseFollowSpeed");
        SetSpeedFieldXY();
        Vector3 mouse = this.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = -3.0f;
        mouse.x = SpeedChangeOBJ.transform.position.x - 0.1f;
        mouse.y = mouse.y * -1;
        if(mouse.y <= -1.7f)
        {
            mouse.y = SpeedChangeOBJ.transform.position.y - 1.7f;
        }
        if(1.7f <= mouse.y)
        {
            //43
            mouse.y = SpeedChangeOBJ.transform.position.y + 1.7f;
        }
        SpeedArrow.transform.position = mouse;
        SetSpeed((2.0f + mouse.y) * 10);
    }

    void SetSpeedFieldXY()
    {
        SpeedChangeOBJ.SetActive(true);
        SpeedChangeOBJ.transform.rotation = Quaternion.identity;
        Vector3 SpeedPos = SpeedChangeOBJ.transform.position;
        SpeedPos.z = -2.0f;
        if (this.transform.position.y <= -1.5f)
        {
            SpeedPos.y = 0.0f;
        }
        if(1.5f <= this.transform.position.y)
        {
            SpeedPos.y = 0.0f;
        }
        SpeedChangeOBJ.transform.position = SpeedPos;
    }

    void MakeMoveBalls()
    {
        for (int i = 0; i < 15; i++) MoveBalls.Add(false);
    }

    public void SetMoveBalls(int index,bool isSleep)
    {
        MoveBalls[index] = isSleep;
    }

    bool IsAllSleeping()
    {
        foreach (bool ball in MoveBalls) if (!ball) return false;
        return true;
    }
}
