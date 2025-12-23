using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CombatLogic : MonoBehaviour
{    
    [SerializeField] private CurrentRunData currentRunData;
    [SerializeField] private ExamSchedule examSchedule;
    [SerializeField] private ScheduledExam exam;
    [SerializeField] private GameObject playerHpBar;
    [SerializeField] private GameObject testHpBar;
    [SerializeField] private Transform damagePopUpPrefab;
    
    private int playerSpeed, playerWit, playerMemory, playerLuck;
    private int testSpeed, testWit, testMemory, testLuck;
    private int currentTurn;
    private float playerHitChance, testHitChance;
    private float playerCritChance, testCritChance;
    private String mood;




    void Start()
    {
        currentTurn = currentRunData.CurrentTurn;

        mood = currentRunData.GetMood();
        Debug.Log($"Current Mood: {mood}");

        playerSpeed = (int)((float)currentRunData.GetStatValue(StatType.spd) * GetMoodMultiplier());
        playerWit = (int)((float)currentRunData.GetStatValue(StatType.wit) * GetMoodMultiplier());
        playerMemory = (int)((float)currentRunData.GetStatValue(StatType.mem) * GetMoodMultiplier());
        playerLuck = (int)((float)currentRunData.GetStatValue(StatType.luk) * GetMoodMultiplier());

        playerHitChance = (float)playerLuck/testLuck;
        playerCritChance = (float)playerLuck/1000;
        
        exam = examSchedule.GetExamForTurn(currentTurn);

        testSpeed = exam.GetStatValue(StatType.spd);
        testWit = exam.GetStatValue(StatType.wit);
        testMemory = exam.GetStatValue(StatType.mem);
        testLuck = exam.GetStatValue(StatType.luk);

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

    public void SpawnDamagePopUp(int damageAmount, Transform spawnPosition, bool isCrit)
    {
        Transform damagePopUpTransform = Instantiate(damagePopUpPrefab, spawnPosition.position, Quaternion.identity);
        DamagePopUp damagePopUp = damagePopUpTransform.GetComponent<DamagePopUp>();
        damagePopUp.Setup(damageAmount, isCrit);
    }
}

