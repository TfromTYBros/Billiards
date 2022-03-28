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

    private readonly int DEGREE_90 = 90;
    private readonly int DEGREE_180 = 180;
    private readonly int DEGREE_270 = 270;
    private readonly int DEGREE_360 = 360;
    private readonly float DEGREE_360f = 360.0f;

    private readonly int FIRST = 0;
    private readonly int SECOND = 1;
    private readonly int THIRD = 2;
    private readonly int INT_NOTHING = 0;

    private readonly float SAFE_ZONE_POS_ON_BOARD_X = 4.3f;
    private readonly float SAFE_ZONE_POS_ON_BOARD_Y = 1.826f;

    private readonly float BREAKSHOT_AREA = 2.6f;
    private readonly float BACK_POS = 1.0f;

    private readonly float SAFEZONE_POS = 1.5f;
    private readonly float SCREEN_NEAR = -2.0f;
    private readonly float NOTHING_F = 0.0f;

    private readonly float SPEEDDIFF_POS = 1.7f;
    private readonly float FOREMOST_POS = 3.0f;
    private readonly float ADJUSTMENT = 0.1f;
    private readonly float REVERSE_NUM = -1.0f;
    private readonly float BASE_SPEED = 1.8f;
    private readonly float MAGNIFICATION = 20.0f;

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
        rigidbody2D.AddForce(new Vector3(ZtoX(), ZtoY(), NOTHING_F), ForceMode2D.Impulse);
    }

    public void SetSpeed(float speedValue)
    {
        speed = speedValue;
    }

    private float ZtoX()
    {
        float ballz = this.gameObject.transform.localEulerAngles.z % DEGREE_360f;
        if (DEGREE_90 <= ballz && ballz <= DEGREE_270)
        {
            return speed * (Mathf.Abs((ballz % DEGREE_180) - DEGREE_90) / DEGREE_90) * REVERSE_NUM;
        }
        else
        {
            return speed * (Mathf.Abs((ballz % DEGREE_180) - DEGREE_90) / DEGREE_90);
        }
    }

    private float ZtoY()
    {
        float ballz = this.gameObject.transform.localEulerAngles.z % DEGREE_360f;
        //Debug.Log("ballz: " + ballz);
        if (INT_NOTHING <= ballz && ballz <= DEGREE_180)
        {
            //Debug.Log("0 <= 180");
            return speed - (speed * (Mathf.Abs((ballz % DEGREE_180) - DEGREE_90) / DEGREE_90));
        }
        else
        {
            //Debug.Log("180 <= 360");
            return (speed - (speed * (Mathf.Abs((ballz % DEGREE_180) - DEGREE_90) / DEGREE_90))) * REVERSE_NUM;
        }
    }

    private void StepMove()
    {
        if (currStep == FIRST)
        {
            StopAllCoroutines();

            FalseStepHeadArea();
            TrueStepRotation();
            FalseStepSpeed();

            FalseCantTouchAreaBox();
            StepPlus();
        }
        else if (currStep == SECOND)
        {
            DecompressionBalls();

            FalseStepHeadArea();
            FalseStepRotation();
            TrueStepSpeed();

            FalseQue();
            SetSpeedField();
            StepPlus();
        }
        else if (currStep == THIRD)
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
        if (THIRD <= currStep || (!BreakShot && SECOND <= currStep)) StepDown();
        if (!BreakShot && currStep == FIRST)
        {
            FreezeBalls();

            TrueStepHeadArea();
            FalseStepRotation();
            FalseStepSpeed();

            FalseQue();
        }
        else if (currStep == SECOND)
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
        currStep = SECOND;
    }

    private IEnumerator RagGoFirstStep()
    {
        yield return MoveStopTime;
        SetTrueIsAllBallsStop();
        FreezeBalls();
        TrueStepHeadArea();
        FalseStepRotation();
        FalseStepSpeed();
        currStep = INT_NOTHING;
    }

    private void MouseFollowHeadSpotArea()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mouse.x <= -SAFE_ZONE_POS_ON_BOARD_X)
        {
            mouse.x = -SAFE_ZONE_POS_ON_BOARD_X;
        }
        if (-BREAKSHOT_AREA <= mouse.x)
        {
            mouse.x = -BREAKSHOT_AREA;
        }
        if (mouse.y <= -SAFE_ZONE_POS_ON_BOARD_Y)
        {
            mouse.y = -SAFE_ZONE_POS_ON_BOARD_Y;
        }
        if (SAFE_ZONE_POS_ON_BOARD_Y <= mouse.y)
        {
            mouse.y = SAFE_ZONE_POS_ON_BOARD_Y;
        }
        mouse.z = BACK_POS;
        this.gameObject.transform.position = mouse;
    }

    private void FreeBall()
    {
        spriteRenderer.enabled = true;
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        this.gameObject.transform.position = mouse;
        if (mouse.x <= -SAFE_ZONE_POS_ON_BOARD_X)
        {
            mouse.x = -SAFE_ZONE_POS_ON_BOARD_X;
        }
        if (SAFE_ZONE_POS_ON_BOARD_X <= mouse.x)
        {
            mouse.x = SAFE_ZONE_POS_ON_BOARD_X;
        }
        if (mouse.y <= -SAFE_ZONE_POS_ON_BOARD_Y)
        {
            mouse.y = -SAFE_ZONE_POS_ON_BOARD_Y;
        }
        if (SAFE_ZONE_POS_ON_BOARD_Y <= mouse.y)
        {
            mouse.y = SAFE_ZONE_POS_ON_BOARD_Y;
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
        mouse.z = FOREMOST_POS * REVERSE_NUM;
        mouse.x = speedFieldVec.x - ADJUSTMENT;
        if(mouse.y <= speedFieldVec.y - SPEEDDIFF_POS)
        {
            mouse.y = speedFieldVec.y - SPEEDDIFF_POS;
        }
        if(speedFieldVec.y + SPEEDDIFF_POS <= mouse.y)
        {
            mouse.y = speedFieldVec.y + SPEEDDIFF_POS;
        }
        SpeedArrow.transform.position = mouse;
        SetSpeed((BASE_SPEED + mouse.y) * MAGNIFICATION);
    }

    private void SetSpeedField()
    {
        TrueSpeedFieldBox();
        SpeedFieldBox.transform.rotation = Quaternion.identity;
        Vector3 SpeedPos = SpeedFieldBox.transform.position;
        SpeedPos.z = SCREEN_NEAR;
        if (this.transform.position.y <= -SAFEZONE_POS)
        {
            SpeedPos.y = NOTHING_F;
        }
        if(SAFEZONE_POS <= this.transform.position.y)
        {
            SpeedPos.y = NOTHING_F;
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
