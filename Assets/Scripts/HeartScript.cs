using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartScript : MonoBehaviour
{
    public GameObject[] Hearts;
    public Sprite AfterSprite;
    int LifeCount = 0;
    public SpriteRenderer[] spriteRenderers;

    void Start()
    {
        SetMaxLife();
    }

    void SetMaxLife()
    {
        LifeCount = 2;
    }

    public void LifeSpriteChange()
    {
        spriteRenderers[LifeCount].sprite = AfterSprite;
        LifeDecrease();
    }

    void LifeDecrease()
    {
        LifeCount--;
    }
}
