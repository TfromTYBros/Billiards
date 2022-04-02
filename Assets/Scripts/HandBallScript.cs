using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandBallScript : MonoBehaviour
{
    HeartScript heartScript;
    TimerScript timerScript;
    public SpriteRenderer spriteRenderer;
    new Rigidbody2D rigidbody2D;
    [SerializeField] float speed = 0.0f;
    [SerializeField] bool BreakShot = false;
    bool StepHeadArea = true;
    bool StepRotation = false;
    bool StepSpeed = false;
    [SerializeField] int currStep = 0;
    [SerializeField] int CushionHitCount = 0;

    public GameObject SpeedFieldBox;
    public GameObject SpeedField;
    public GameObject SpeedArrow;
    public GameObject CantTouchAreaBox;
    public GameObject Que;
    public GameObject QueBoard;
    public GameObject[] BallsOBJ = new GameObject[15];
    public Transform[] BallsPos = new Transform[15];

    bool IsAllBallsStop = true;
    bool FoulChecked = false;
    [SerializeField] bool Clear_Minimum = false;
    [SerializeField] bool Clear_Cushion_HandBall = false;
    [SerializeField] bool Clear_Cushion_CurrBall = false;
    [SerializeField] bool Clear_AnyBallPocket = false;
    [SerializeField] bool HandBallPocket = false;

    private readonly WaitForSeconds MoveStopTime = new WaitForSeconds(10.0f);
    bool CoroutineNow = false;
    [SerializeField] bool Damaged = false;
    [SerializeField] bool BoolFreeBalled = false;

    public GameObject GameOverPanel;
    public GameObject GameClearPanel;
    int PocketCount = 0;
    Vector3[] BallsPosOnStart = new Vector3[15];
    Vector3 StartHandBallPos = new Vector3(-2.7f,0.0f,4.0f);
    Vector3 ScreenOutPos = new Vector3(10.0f, 0.0f, 4.0f);
    Vector3 HandBallPosOnPanel = new Vector3(-2.7f, 0.0f, 5.0f);

    private readonly int DEGREE_90 = 90;
    private readonly int DEGREE_180 = 180;
    private readonly int DEGREE_270 = 270;
    private readonly float DEGREE_360f = 360.0f;

    private readonly int FIRST = 0;
    private readonly int SECOND = 1;
    private readonly int THIRD = 2;
    private readonly int INT_NOTHING = 0;

    private readonly float SAFE_ZONE_POS_ON_BOARD_X = 4.3f;
    private readonly float SAFE_ZONE_POS_ON_BOARD_Y = 1.826f;

    private readonly float BREAKSHOT_AREA = 2.6f;
    private readonly float HANDBALL_POS = 4.0f;

    private readonly float SAFEZONE_POS = 0.7f;
    private readonly float SCREEN_NEAR = 3.0f;
    private readonly float NOTHING_F = 0.0f;
    private readonly float AVOID_DISTANCE = 1.5f;
    private readonly float HANDBALL_DISTANCE = 2.0f;

    private readonly float SPEEDDIFF_POS = 1.7f;
    private readonly float FOREMOST_POS = 2.0f;
    private readonly float ADJUSTMENT = 0.1f;
    private readonly float REVERSE_NUM = -1.0f;
    private readonly float BASE_SPEED = 2.0f;
    private readonly float MAGNIFICATION = 20.0f;

    void Start()
    {
        rigidbody2D = this.GetComponent<Rigidbody2D>();
        heartScript = FindObjectOfType<HeartScript>();
        timerScript = FindObjectOfType<TimerScript>();
        TrueCantTouchAreaBox();
        TrueBreakShot();
        SaveBallsPosOnStart();
        TrueBoolFreeBall();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && IsAllBallsStop && (!heartScript.LifeSafe() || IsGameClear() )) ReStartGame();
        else if (Input.GetMouseButtonDown(0) && IsAllBallsStop && heartScript.LifeSafe()) StepMove();
        if (Input.GetMouseButtonDown(1) && IsAllBallsStop && heartScript.LifeSafe()) StepReMove();
        if (BreakShot && StepHeadArea) MouseFollowHeadSpotArea();
        else if (!BreakShot && StepHeadArea) FreeBall();
        if (StepRotation) MouseFollowRotation();
        if (StepSpeed) MouseFollowSpeed();
    }

    private void StepPlus()
    {
        if (currStep < THIRD)currStep++;
    }

    private void StepDown()
    {
        if (!GetFreeBall() && currStep == SECOND) return;
        else if (FIRST < currStep) currStep--;
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

    private void TrueFoulChecked()
    {
        FoulChecked = true;
    }

    private void FalseFoulChecked()
    {
        FoulChecked = false;
    }

    private void TrueClear_Minimum()
    {
        Clear_Minimum = true;
    }

    private void FalseClear_Minimum()
    {
        Clear_Minimum = false;
    }

    private void TrueClear_Cushion_HandBall()
    {
        Clear_Cushion_HandBall = true;
    }

    private void FalseClear_Cushion_HandBall()
    {
        Clear_Cushion_HandBall = false;
    }

    public void TrueClear_Cushion_CurrBall()
    {
        Clear_Cushion_CurrBall = true;
    }

    public void FalseClear_Cushion_CurrBall()
    {
        Clear_Cushion_CurrBall = false;
    }

    public void TrueClear_AnyBallPocket()
    {
        Clear_AnyBallPocket = true;
    }

    private void FalseClear_AnyBallPocket()
    {
        Clear_AnyBallPocket = false;
    }

    private void TrueHandBallPocket()
    {
        HandBallPocket = true;
    }

    private void FalseHandBallPocket()
    {
        HandBallPocket = false;
    }

    private void TrueCoroutineNow()
    {
        CoroutineNow = true;
    }

    private void FalseCoroutineNow()
    {
        CoroutineNow = false;
    }

    public bool GetCoroutineNow()
    {
        return CoroutineNow;
    }

    private void TrueDamaged()
    {
        Damaged = true;
    }
    
    private void FalseDamaged()
    {
        Damaged = false;
    }

    private void TrueBoolFreeBall()
    {
        //Debug.Log("TrueBoolFreeBall");
        BoolFreeBalled = true;
    }

    private void FalseBoolFreeBall()
    {
        Debug.Log("FalseBoolFreeBall");
        BoolFreeBalled = false;
    }

    private bool GetFreeBall()
    {
        return BoolFreeBalled;
    }

    public void PocketCountPlus()
    {
        PocketCount++;
    }

    private void PocketCountReset()
    {
        PocketCount = 0;
    }

    private int GetPocketCount()
    {
        return PocketCount;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hole"))
        {
            //Debug.Log("HoleHitByHandBall");
            HandBallDisappear();
            if (!BreakShot && !Damaged) DamageMethod();
            TrueHandBallPocket();
        }

        //BreakShotˆÈŠO‚ÌŽž
        if (!FoulChecked && currStep == THIRD && !collision.gameObject.CompareTag("Cushion") && !collision.gameObject.CompareTag("Hole") && !BreakShot && collision.gameObject != GetCurrMinimumBall())
        {
            TrueFoulChecked();
            //Debug.Log("FoulTouch");
        }
        else if (!FoulChecked && currStep == THIRD && !collision.gameObject.CompareTag("Cushion") && !collision.gameObject.CompareTag("Hole") && !BreakShot && collision.gameObject == GetCurrMinimumBall())
        {
            //Debug.Log("NoFoul");
            TrueClear_Minimum();
            TrueFoulChecked();
        }

        if (Clear_Minimum && collision.gameObject.CompareTag("Cushion"))
        {
            TrueClear_Cushion_HandBall();
        }
    }

    private void HandBallDisappear()
    {
        rigidbody2D.velocity = Vector2.zero;
        this.transform.position = ScreenOutPos;
        FalseSpriteRenderer();
    }

    private void FreezeBalls()
    {
        //Debug.Log("FreezeBalls");
        foreach(GameObject ball in BallsOBJ)
        {
            Rigidbody2D rigidbody2D = ball.GetComponent<Rigidbody2D>();
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezePosition;
        }
    }
    
    private void DecompressionBalls()
    {
        //Debug.Log("DecompressionBalls");
        foreach(GameObject ball in BallsOBJ)
        {
            Rigidbody2D rigidbody2D = ball.GetComponent<Rigidbody2D>();
            rigidbody2D.constraints = RigidbodyConstraints2D.None;
        }
    }

    private void AddForce()
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
            TrueCoroutineNow();
            FalseBoolFreeBall();
        }
    }

    void StepReMove()
    {
        StepDown();
        if (currStep == FIRST)
        {
            GoFirstStep();
            FalseQue();
            if (BreakShot) CantTouchAreaBox.SetActive(true);
        }
        else if (currStep == SECOND)
        {
            GoSecondStep();
            FalseSpeedFieldBox();
            TrueQue();
        }
    }

    private void GoFirstStep()
    {
        TrueBoolFreeBall();
        SetTrueIsAllBallsStop();
        FreezeBalls();
        TrueStepHeadArea();
        FalseStepRotation();
        FalseStepSpeed();
        currStep = FIRST;
        timerScript.ResetTimer();
    }

    private void GoSecondStep()
    {
        //IsAllBallsStop‚ÍAllBallStop()‚ÅŽÀ‘•Ï‚ÝB
        TrueSpriteRenderer();
        FalseStepHeadArea();
        TrueStepRotation();
        FalseStepSpeed();
        currStep = SECOND;
        timerScript.ResetTimer();
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
        mouse.z = HANDBALL_POS;
        this.gameObject.transform.position = mouse;
    }

    private void FreeBall()
    {
        spriteRenderer.enabled = true;
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = HANDBALL_POS;
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
        mouse.z = FOREMOST_POS;
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
        /*
        Debug.Log(speedFieldVec);
        Debug.Log(mouse);
        Debug.Log(BASE_SPEED + mouse.y - speedFieldVec.y);*/
        SetSpeed((BASE_SPEED + mouse.y - speedFieldVec.y) * MAGNIFICATION);
    }

    private void SetSpeedField()
    {
        TrueSpeedFieldBox();
        SpeedFieldBox.transform.rotation = Quaternion.identity;
        Vector3 SpeedPos = QueBoard.transform.position;
        SpeedPos.z = SCREEN_NEAR;
        if (this.transform.position.y <= -SAFEZONE_POS)
        {
            SpeedPos.y = NOTHING_F;
        }
        if (SAFEZONE_POS <= this.transform.position.y)
        {
            SpeedPos.y = NOTHING_F;
        }
        if ( -HANDBALL_DISTANCE <= (this.transform.position.x - SpeedPos.x) && (this.transform.position.x - SpeedPos.x) <= HANDBALL_DISTANCE)
        {
            SpeedPos.x = this.transform.position.x - SpeedPos.x <= 0.0f ? this.transform.position.x + AVOID_DISTANCE : this.transform.position.x - AVOID_DISTANCE;
        }
        //Debug.Log(SpeedPos);
        SpeedFieldBox.transform.position = SpeedPos;
        SpeedField.transform.position = SpeedPos;
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
        CleanUp();
    }

    private void CleanUp()
    {
        FalseCoroutineNow();
        if (BreakShot)
        {
            FalseBreakShot();
            if (IsSafeOnBreakShot())
            {
                GoSecondStep();
            }
            else
            {
                DamageMethod();
                GoFirstStep();
            }
        }
        else if ((Clear_Cushion_HandBall || Clear_Cushion_CurrBall || Clear_AnyBallPocket) && Clear_Minimum && !HandBallPocket)
        {
            //Debug.Log("NoFoul");
            IsGameOver(SECOND);
        }
        else
        {
            //Debug.Log("Foul");
            if (!Damaged) DamageMethod();
            IsGameOver(FIRST);
        }
        FalseFoulChecked();
        FalseClear_Cushion_HandBall();
        FalseClear_Cushion_CurrBall();
        FalseClear_Minimum();
        FalseClear_AnyBallPocket();
        FalseHandBallPocket();
        FalseDamaged();
    }

    public GameObject GetCurrMinimumBall()
    {
        foreach(GameObject ball in BallsOBJ)
        {
            if(ball.activeSelf == true)
            {
                return ball;
            }
        }
        return null;
    }

    public void CushionHitCountUp()
    {
        Debug.Log("CushionHitCountUp");
        CushionHitCount++;
    }

    private void CushionHitCountReset()
    {
        CushionHitCount = 0;
    }

    bool IsSafeOnBreakShot()
    {
        if ((CushionHitCount < 3 && !Clear_AnyBallPocket) || HandBallPocket) return false;
        return true;
    }

    void DamageMethod()
    {
        //Debug.Log("Damaged");
        heartScript.LifeSpriteChange();
        TrueDamaged();
    }

    void IsGameOver(int Step)
    {
        if (IsGameClear()) GameClear();
        else if (!heartScript.LifeSafe()) GameOver();
        else
        {
            if (Step == FIRST) GoFirstStep();
            else if (Step == SECOND) GoSecondStep();
        }
    }

    void GameOver()
    {
        //Debug.Log("GameOver");
        HandBallPosChangeOnPanel();
        GameOverPanel.SetActive(true);
    }

    public void ReStartGame()
    {
        TrueBreakShot();
        CantTouchAreaBox.SetActive(true);
        RackMold();
        TrueSpriteRenderer();
        heartScript.ResetLife();
        GoFirstStep();
        CushionHitCountReset();
        PocketCountReset();
        GameOverPanel.SetActive(false);
        GameClearPanel.SetActive(false);
    }

    private void SaveBallsPosOnStart()
    {
        int index = 0;
        foreach(Transform pos in BallsPos)
        {
            BallsPosOnStart[index] = pos.position;
            index++;
        }
    }

    private void RackMold()
    {
        //Debug.Log("RackMold");
        this.gameObject.transform.position = StartHandBallPos;
        int index = 0;
        foreach (GameObject ball in BallsOBJ)
        {
            ball.SetActive(true);
            ball.transform.position = BallsPosOnStart[index];
            ball.transform.rotation = Quaternion.identity;
            index++;
        }
    }

    private bool IsGameClear()
    {
        return heartScript.LifeSafe() && (15 <= GetPocketCount());
    }

    private void GameClear()
    {
        //Debug.Log("GameClear");
        HandBallPosChangeOnPanel();
        GameClearPanel.SetActive(true);
    }

    private void SetHandBallPosOnPanel()
    {
        HandBallPosOnPanel = new Vector3(this.transform.position.x, this.transform.position.y, HandBallPosOnPanel.z);
    }

    private void HandBallPosChangeOnPanel()
    {
        SetHandBallPosOnPanel();
        this.gameObject.transform.position = HandBallPosOnPanel;
    }
}
