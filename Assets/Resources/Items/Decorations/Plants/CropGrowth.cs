// File: CropGrowth.cs
// Last Modified: 07/14/2018, 1:02 PM
// Description: 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DecorationController))]
public class CropGrowth : MonoBehaviour
{
    private static int sunlightBlocksAbove = 8;
    private static int sunlightBlocksSide = 3;
    private static float sunlightDecreasePerBlock = 0.1f;

    public List<Sprite> StageSprites;
    public List<float> StageTimers;
    private int currentStage;
    private World currentWorld;
    private int maxStage;

    private Int2 position = Int2.zero;
    [SerializeField] private float sunlight = 1f;
    private float timer;

    public void SetCropInfo(List<Sprite> stageSpriteList, List<float> stageTimerList)
    {
        position = GetComponent<DecorationController>().position;
        StageSprites = stageSpriteList;
        StageTimers = stageTimerList;
        maxStage = stageSpriteList.Count - 1;
        GetComponent<SpriteRenderer>().sprite = stageSpriteList[0];
    }

    private void Update()
    {
        if (currentStage >= maxStage) return;
        if (timer >= StageTimers[currentStage + 1])
        {
            currentStage++;
            GetComponent<SpriteRenderer>().sprite = StageSprites[currentStage];
        }

        CalculateSunlight();
        timer += Time.deltaTime * sunlight;
    }

    private void CalculateSunlight()
    {
        currentWorld = GameManager.Get<WorldManager>().currentWorld;
        sunlight = 1f;
        for (int x = position.x - sunlightBlocksSide; x < position.x + sunlightBlocksSide; x++)
            for (int y = position.y; y < position.y + sunlightBlocksAbove; y++)
                if (currentWorld.HasTile(0, x, y))
                    sunlight -= sunlightDecreasePerBlock;
        if (sunlight < 0) sunlight = 0;
    }
}