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

    bool IsAllBallsStop = true;

    WaitForSeconds MoveStopTime = new WaitForSeconds(10.0f);
    float SpeedDiffPos = 1.7f;
    float ForemostPos = 3.0f;

    void Start()
    {
        rigidbody2D = this.GetComponent<Rigidbody2D>();
        CantTouchAreaBox.SetActive(true);
        SetBreakShotTrue();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && IsAllBallsStop) StepMove();
        if (Input.GetMouseButtonDown(1) && IsAllBallsStop) StepReMove();
        if (BreakShot && StepHeadArea) MouseFollowHeadSpotArea();
        else if (!BreakShot && StepHeadArea) FreeBall();
        if (StepRotation) MouseFollowRotation();
        if (StepSpeed) MouseFollowSpeed();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hole"))
        {
            //Debug.Log("HoleHitByHandBall");
            StopAllCoroutines();
            HandBallDisappear();
            SetBreakShotFalse();
            StartCoroutine(RagGoFirstStep());
        }
    }

    void HandBallDisappear()
    {
        rigidbody2D.velocity = Vector2.zero;
        spriteRenderer.enabled = false;
    }

    void FreezeBalls()
    {
        //Debug.Log("FreezeBalls");
        foreach(GameObject ball in BallsOBJ)
        {
            Rigidbody2D rigidbody2D = ball.GetComponent<Rigidbody2D>();
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezePosition;
        }
    }
    
    void DecompressionBalls()
    {
        //Debug.Log("DecompressionBalls");
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
            SetSpeedField();
            currStep++;
        }
        else if (currStep == 2)
        {
            StepHeadArea = false;
            StepRotation = false;
            StepSpeed = false;
            //SpeedArrow.transform.position = new Vector3(SpeedChangeOBJ.transform.position.x, 0.0f, SpeedChangeOBJ.transform.position.z);
            SpeedChangeOBJ.SetActive(false);
            AddForce();
            SetFalseIsAllBallsStop();
            StartCoroutine(AllBallStop());
        }
    }

    void StepReMove()
    {
        if (2 <= currStep || (!BreakShot && 1 <= currStep)) currStep--;
        if (!BreakShot && currStep == 0)
        {
            FreezeBalls();
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

    private void GoSecondStep()
    {
        //IsAllBallsStop‚ÍAllBallStop()‚ÅŽÀ‘•Ï‚ÝB
        spriteRenderer.enabled = true;
        StepHeadArea = false;
        StepRotation = true;
        StepSpeed = false;
        currStep = 1;
    }

    private IEnumerator RagGoFirstStep()
    {
        yield return MoveStopTime;
        SetTrueIsAllBallsStop();
        FreezeBalls();
        StepHeadArea = true;
        StepRotation = false;
        StepSpeed = false;
        currStep = 0;
    }

    void SetTrueIsAllBallsStop()
    {
        IsAllBallsStop = true;
    }

    void SetFalseIsAllBallsStop()
    {
        IsAllBallsStop = false;
    }

    private void MouseFollowHeadSpotArea()
    {
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
        Vector3 speedFieldVec = GetSpeedFieldPos();
        Vector3 mouse = speedFieldVec - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = ForemostPos*-1;
        mouse.x = speedFieldVec.x - 0.1f;
        mouse.y *= -1;
        if(mouse.y <= speedFieldVec.y - SpeedDiffPos)
        {
            mouse.y = speedFieldVec.y - SpeedDiffPos;
        }
        if(speedFieldVec.y + SpeedDiffPos <= mouse.y)
        {
            mouse.y = speedFieldVec.y + SpeedDiffPos;
        }
        SpeedArrow.transform.position = mouse;
        SetSpeed((2.0f + mouse.y) * 20.0f);
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

    Vector3 GetSpeedFieldPos()
    {
        return SpeedChangeOBJ.transform.position;
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
        SetTrueIsAllBallsStop();
        GoSecondStep();
    }
}
