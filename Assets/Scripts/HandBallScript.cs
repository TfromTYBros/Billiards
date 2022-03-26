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

    public GameObject SpeedFieldBox;
    public GameObject SpeedArrow;
    public GameObject CantTouchAreaBox;
    public GameObject Que;
    public GameObject[] BallsOBJ;

    bool IsAllBallsStop = true;

    private readonly WaitForSeconds MoveStopTime = new WaitForSeconds(10.0f);
    private readonly int First = 0;
    private readonly int Second = 1;
    private readonly int Third = 2;
    private readonly int IntNothing = 0;

    private readonly float SafeZonePosOnBoardX = 4.3f;
    private readonly float SafeZonePosOnBoardY = 1.826f;

    private readonly float BreakShotArea = 2.6f;
    private readonly float BackPos = 1.0f;

    private readonly float SafeZonePos = 1.5f;
    private readonly float ScreenNear = -2.0f;
    private readonly float Nothing = 0.0f;

    private readonly float SpeedDiffPos = 1.7f;
    private readonly float ForemostPos = 3.0f;
    private readonly float AdJustMent = 0.1f;
    private readonly float ReverseNum = -1.0f;
    private readonly float BaseSpeed = 1.8f;
    private readonly float Magnification = 20.0f;

    void Start()
    {
        rigidbody2D = this.GetComponent<Rigidbody2D>();
        TrueCantTouchAreaBox();
        TrueBreakShot();
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

    private void StepPlus()
    {
        currStep++;
    }

    private void StepDown()
    {
        currStep--;
    }

    private void TrueBreakShot()
    {
        BreakShot = true;
    }

    private void FalseBreakShot()
    {
        BreakShot = false;
    }

    private void TrueStepHeadArea()
    {
        StepHeadArea = true;
    }

    private void FalseStepHeadArea()
    {
        StepHeadArea = false;
    }

    private void TrueStepRotation()
    {
        StepRotation = true;
    }

    private void FalseStepRotation()
    {
        StepRotation = false;
    }

    private void TrueStepSpeed()
    {
        StepSpeed = true;
    }

    private void FalseStepSpeed()
    {
        StepSpeed = false;
    }

    private void TrueSpriteRenderer()
    {
        spriteRenderer.enabled = true;
    }

    private void FalseSpriteRenderer()
    {
        spriteRenderer.enabled = false;
    }

    private void SetTrueIsAllBallsStop()
    {
        IsAllBallsStop = true;
    }

    private void SetFalseIsAllBallsStop()
    {
        IsAllBallsStop = false;
    }

    private void TrueCantTouchAreaBox()
    {
        CantTouchAreaBox.SetActive(true);
    }

    private void FalseCantTouchAreaBox()
    {
        CantTouchAreaBox.SetActive(false);
    }

    private void TrueQue()
    {
        Que.SetActive(true);
    }

    private void FalseQue()
    {
        Que.SetActive(false);
    }

    private void TrueSpeedFieldBox()
    {
        SpeedFieldBox.SetActive(true);
    }

    private void FalseSpeedFieldBox()
    {
        SpeedFieldBox.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hole"))
        {
            //Debug.Log("HoleHitByHandBall");
            StopAllCoroutines();
            HandBallDisappear();
            FalseBreakShot();
            StartCoroutine(RagGoFirstStep());
        }
    }

    void HandBallDisappear()
    {
        rigidbody2D.velocity = Vector2.zero;
        FalseSpriteRenderer();
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
        rigidbody2D.AddForce(new Vector3(ZtoX(), ZtoY(), Nothing), ForceMode2D.Impulse);
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
        if (currStep == First)
        {
            StopAllCoroutines();

            FalseStepHeadArea();
            TrueStepRotation();
            FalseStepSpeed();

            FalseCantTouchAreaBox();
            StepPlus();
        }
        else if (currStep == Second)
        {
            DecompressionBalls();

            FalseStepHeadArea();
            FalseStepRotation();
            TrueStepSpeed();

            FalseQue();
            SetSpeedField();
            StepPlus();
        }
        else if (currStep == Third)
        {
            FalseStepHeadArea();
            FalseStepRotation();
            FalseStepSpeed();

            FalseSpeedFieldBox();
            AddForce();
            SetFalseIsAllBallsStop();
            StartCoroutine(AllBallStop());
        }
    }

    void StepReMove()
    {
        if (Third <= currStep || (!BreakShot && Second <= currStep)) StepDown();
        if (!BreakShot && currStep == First)
        {
            FreezeBalls();

            TrueStepHeadArea();
            FalseStepRotation();
            FalseStepSpeed();

            FalseQue();
        }
        else if (currStep == Second)
        {
            FalseStepHeadArea();
            TrueStepRotation();
            FalseStepSpeed();
            FalseSpeedFieldBox();
            TrueQue();
        }
    }

    private void GoSecondStep()
    {
        //IsAllBallsStop‚ÍAllBallStop()‚ÅŽÀ‘•Ï‚ÝB
        TrueSpriteRenderer();
        FalseStepHeadArea();
        TrueStepRotation();
        FalseStepSpeed();
        currStep = Second;
    }

    private IEnumerator RagGoFirstStep()
    {
        yield return MoveStopTime;
        SetTrueIsAllBallsStop();
        FreezeBalls();
        TrueStepHeadArea();
        FalseStepRotation();
        FalseStepSpeed();
        currStep = IntNothing;
    }

    private void MouseFollowHeadSpotArea()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mouse.x <= -SafeZonePosOnBoardX)
        {
            mouse.x = -SafeZonePosOnBoardX;
        }
        if (-BreakShotArea <= mouse.x)
        {
            mouse.x = -BreakShotArea;
        }
        if (mouse.y <= -SafeZonePosOnBoardY)
        {
            mouse.y = -SafeZonePosOnBoardY;
        }
        if (SafeZonePosOnBoardY <= mouse.y)
        {
            mouse.y = SafeZonePosOnBoardY;
        }
        mouse.z = BackPos;
        this.gameObject.transform.position = mouse;
    }

    private void FreeBall()
    {
        spriteRenderer.enabled = true;
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        this.gameObject.transform.position = mouse;
        if (mouse.x <= -SafeZonePosOnBoardX)
        {
            mouse.x = -SafeZonePosOnBoardX;
        }
        if (SafeZonePosOnBoardX <= mouse.x)
        {
            mouse.x = SafeZonePosOnBoardX;
        }
        if (mouse.y <= -SafeZonePosOnBoardY)
        {
            mouse.y = -SafeZonePosOnBoardY;
        }
        if (SafeZonePosOnBoardY <= mouse.y)
        {
            mouse.y = SafeZonePosOnBoardY;
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
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = ForemostPos * ReverseNum;
        mouse.x = speedFieldVec.x - AdJustMent;
        if(mouse.y <= speedFieldVec.y - SpeedDiffPos)
        {
            mouse.y = speedFieldVec.y - SpeedDiffPos;
        }
        if(speedFieldVec.y + SpeedDiffPos <= mouse.y)
        {
            mouse.y = speedFieldVec.y + SpeedDiffPos;
        }
        SpeedArrow.transform.position = mouse;
        SetSpeed((BaseSpeed + mouse.y) * Magnification);
    }

    private void SetSpeedField()
    {
        TrueSpeedFieldBox();
        SpeedFieldBox.transform.rotation = Quaternion.identity;
        Vector3 SpeedPos = SpeedFieldBox.transform.position;
        SpeedPos.z = ScreenNear;
        if (this.transform.position.y <= -SafeZonePos)
        {
            SpeedPos.y = Nothing;
        }
        if(SafeZonePos <= this.transform.position.y)
        {
            SpeedPos.y = Nothing;
        }
        SpeedFieldBox.transform.position = SpeedPos;
    }

    private Vector3 GetSpeedFieldPos()
    {
        return SpeedFieldBox.transform.position;
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
