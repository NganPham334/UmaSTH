using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CombatLogic : MonoBehaviour
{    
    public CurrentRunData currentRunData;
    public ExamSchedule examSchedule;
    public ScheduledExam exam;
    public int playerSpeed, playerWit, playerMemory, playerLuck;
    public int testSpeed, testWit, testMemory, testLuck;
    public int currentTurn;
    public GameObject playerHpBar;
    public GameObject testHpBar;
    public float playerHitChance, testHitChance;
    public float playerCritChance, testCritChance;
    public String mood;


    void Start()
    {
        currentTurn = currentRunData.CurrentTurn;

        mood = currentRunData.GetMood();
        Debug.Log($"Current Mood: {mood}");

        playerSpeed = (int)((float)currentRunData.GetStatValue(StatType.SPD) * GetMoodMultiplier());
        playerWit = (int)((float)currentRunData.GetStatValue(StatType.WIT) * GetMoodMultiplier());
        playerMemory = (int)((float)currentRunData.GetStatValue(StatType.MEM) * GetMoodMultiplier());
        playerLuck = (int)((float)currentRunData.GetStatValue(StatType.LUK) * GetMoodMultiplier());

        playerHitChance = (float)playerLuck/testLuck;
        playerCritChance = (float)playerLuck/1000;
        
        exam = examSchedule.GetExamForTurn(currentTurn);

        testSpeed = exam.GetStatValue(StatType.SPD);
        testWit = exam.GetStatValue(StatType.WIT);
        testMemory = exam.GetStatValue(StatType.MEM);
        testLuck = exam.GetStatValue(StatType.LUK);

        testHitChance = (float)testLuck/playerLuck;
        testCritChance = (float)testLuck/1000;

        playerHpBar.GetComponent<HpBarController>().SetMaxHp(playerMemory * 15);
        testHpBar.GetComponent<HpBarController>().SetMaxHp(testMemory * 15);

        Debug.Log($"Player Stats - Speed: {playerSpeed}, Wit: {playerWit}, Memory: {playerMemory}, Luck: {playerLuck}, testHitChance: {testHitChance},testCritChance: {testCritChance}");
        Debug.Log($"Test Stats - Speed: {testSpeed}, Wit: {testWit}, Memory: {testMemory}, Luck: {testLuck}, playerHitChance: {playerHitChance},playerCritChance: {playerCritChance}");
    }

    public void PlayerTakeDamage()
    {
        if (Random.value >= (testHitChance))
        {
            Debug.Log("Test's attack missed!");
            return;
        }
        if (Random.value <= (testCritChance))
        {
            playerHpBar.GetComponent<HpBarController>().TakeDamage(testWit * 2);
            Debug.Log($"Test critical attacked for {testWit * 2}!");
            return;
        }
        playerHpBar.GetComponent<HpBarController>().TakeDamage(testWit);
        Debug.Log($"Player takes {testWit} damage.");
    }

    public void TestTakeDamage()
    {
        if (Random.value >= (playerHitChance))
        {
            Debug.Log("Player's attack missed!");
            return;
        }
        if (Random.value <= (playerCritChance))
        {
            testHpBar.GetComponent<HpBarController>().TakeDamage(playerWit * 2);
            Debug.Log($"Player critical attacked for {playerWit * 2}!");
            return;
        }
        testHpBar.GetComponent<HpBarController>().TakeDamage(playerWit);
        Debug.Log($"Test takes {playerWit} damage.");
    }

    public float GetMoodMultiplier()
{
    // Return a different multiplier based on Mood
    return mood switch
    {
        "Depressed" => 0.9f, 
        "Bad" => 0.95f, 
        "Normal" => 1.0f, 
        "Good" => 1.05f, 
        "Umazing" => 1.1f, 
        _ => 1.0f  // Default
    };
}
}

