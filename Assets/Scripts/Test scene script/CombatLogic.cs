using UnityEngine;

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


    void Start()
    {
        currentTurn = currentRunData.CurrentTurn;
        
        playerSpeed = currentRunData.GetStatValue(StatType.SPD);
        playerWit = currentRunData.GetStatValue(StatType.WIT);
        playerMemory = currentRunData.GetStatValue(StatType.MEM);
        playerLuck = currentRunData.GetStatValue(StatType.LUK);

        exam = examSchedule.GetExamForTurn(currentTurn);

        testSpeed = exam.GetStatValue(StatType.SPD);
        testWit = exam.GetStatValue(StatType.WIT);
        testMemory = exam.GetStatValue(StatType.MEM);
        testLuck = exam.GetStatValue(StatType.LUK);

        playerHitChance = (float)playerLuck/testLuck;
        testHitChance = (float)testLuck/playerLuck;

        playerCritChance = (float)playerLuck/1000;
        testCritChance = (float)testLuck/1000;

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
}
