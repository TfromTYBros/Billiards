using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandBallScript : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    new Rigidbody2D rigidbody2D;
    [SerializeField] float speed = 0.0f;
    bool BreakShot = false;
    bool StepHeadArea = true;
    bool StepRotation = false;
    bool StepSpeed = false;
    [SerializeField] int currStep = 0;

    public GameObject SpeedChangeOBJ;
    public GameObject SpeedArrow;
    public GameObject CantTouchAreaBox;
    public GameObject Que;
    public GameObject[] BallsOBJ;

    [SerializeField] public List<bool> MoveBalls;

    WaitForSeconds MoveStopTime = new WaitForSeconds(9.0f);

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
        if (Input.GetMouseButtonDown(1) && IsAllSleeping()) StepReMove();
        if (BreakShot && StepHeadArea) MouseFollowHeadSpotArea();
        else if (!BreakShot && StepHeadArea) FreeBall();
        if (StepRotation) MouseFollowRotation();
        if (StepSpeed) MouseFollowSpeed();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hole"))
        {
            Debug.Log("HoleHitByHandBall");
            HandBallDisappear();
            SetBreakShotFalse();
            StartCoroutine(RagGoFirstStep());
        }
    }

    void HandBallDisappear()
    {
        FreezeBalls();
        spriteRenderer.enabled = false;
    }

    void FreezeBalls()
    {
        Debug.Log("FreezeBalls");
        foreach(GameObject ball in BallsOBJ)
        {
            Rigidbody2D rigidbody2D = ball.GetComponent<Rigidbody2D>();
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezePosition;
        }
    }

    void DecompressionBalls()
    {
        Debug.Log("DecompressionBalls");
        foreach(GameObject ball in BallsOBJ)
        {
            Rigidbody2D rigidbody2D = ball.GetComponent<Rigidbody2D>();
            rigidbody2D.constraints = RigidbodyConstraints2D.None;
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
        if (currStep == 0)
        {
            StopAllCoroutines();
            StepHeadArea = false;
            StepRotation = true;
            StepSpeed = false;
            CantTouchAreaBox.SetActive(false);
            currStep++;
        }
        else if (currStep == 1)
        {
            DecompressionBalls();
            StepHeadArea = false;
            StepRotation = false;
            StepSpeed = true;
            Que.SetActive(false);
            currStep++;
        }
        else if (currStep == 2)
        {
            StepHeadArea = false;
            StepRotation = false;
            StepSpeed = false;
            SpeedChangeOBJ.SetActive(false);
            AddForce();
            StartCoroutine(AllBallStop());
            currStep = 0;
        }
    }

    void StepReMove()
    {
        if (2 <= currStep || (!BreakShot && 1 <= currStep)) currStep--;
        if (!BreakShot && currStep == 0)
        {
            DecompressionBalls();
            StepHeadArea = true;
            StepRotation = false;
            StepSpeed = false;
            Que.SetActive(false);
        }
        else if (currStep == 1)
        {
            StepHeadArea = false;
            StepRotation = true;
            StepSpeed = false;
            SpeedChangeOBJ.SetActive(false);
            Que.SetActive(true);
        }
    }

    private void GoFirstStep()
    {
        StepHeadArea = true;
        StepRotation = false;
        StepSpeed = false;
        currStep = 0;
    }

    private IEnumerator RagGoFirstStep()
    {
        yield return MoveStopTime;
        StepHeadArea = true;
        StepRotation = false;
        StepSpeed = false;
        currStep = 0;
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
        spriteRenderer.enabled = true;
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
        SetSpeedField();
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
        SetSpeed((2.0f + mouse.y) * 20);
    }

    void SetSpeedField()
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
        Debug.Log("index" + index + "IsSleep" + isSleep);
        MoveBalls[index] = isSleep;
    }

    bool IsAllSleeping()
    {
        foreach (bool ball in MoveBalls) if (!ball) return false;
        return true;
    }

    IEnumerator AllBallStop()
    {
        //Debug.Log("AllBallStop");
        yield return MoveStopTime;
        rigidbody2D.velocity = Vector2.zero;
        foreach(GameObject ball in BallsOBJ)
        {
            Rigidbody2D rigidbody2D = ball.GetComponent<Rigidbody2D>();
            rigidbody2D.velocity = Vector2.zero;
        }
    }
}
