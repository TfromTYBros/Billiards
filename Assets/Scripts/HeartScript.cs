using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartScript : MonoBehaviour
{
    public GameObject[] Hearts;
    public Sprite AfterSprite;
    public Sprite BeforeSprite;
    int LifeCount = 0;
    public SpriteRenderer[] spriteRenderers;

    void Start()
    {
        SetMaxLife();
    }

    void SetMaxLife()
    {
        LifeCount = 3;
    }

    public void LifeSpriteChange()
    {
        spriteRenderers[LifeCount-1].sprite = AfterSprite;
        LifeDecrease();
    }

    void LifeDecrease()
    {
        LifeCount--;
    }

    public int GetLifeCount()
    {
        return LifeCount;
    }

    public void ResetLife()
    {
        SetMaxLife();
        for (int i = 0; i < 3; i++) spriteRenderers[i].sprite = BeforeSprite;
    }
}
