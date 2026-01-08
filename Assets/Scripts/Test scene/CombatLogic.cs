using System;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;

public class CombatLogic : MonoBehaviour
{    
    [SerializeField] private CurrentRunData currentRunData;
    [SerializeField] private ExamSchedule examSchedule;
    [SerializeField] private ScheduledExam exam;
    [SerializeField] private GameObject playerHpBar;
    [SerializeField] private GameObject testHpBar;
    [SerializeField] private ActionBarController playerActionBar, testActionBar;
    [SerializeField] private Transform damagePopUpPrefab;
    [SerializeField] private RectTransform playerHpRect, TestHpRect;
    [SerializeField] private Transform canvasTransform;
    [SerializeField] private Transform playerTransform, testTransform;
    
    private int playerSpeed, playerWit, playerMemory, playerLuck;
    private int testSpeed, testWit, testMemory, testLuck;
    private int currentTurn;
    private double playerHitChance, testHitChance;
    private double playerCritChance, testCritChance;
    private String mood;




    void Start()
    {
        currentTurn = currentRunData.CurrentTurn;

        mood = currentRunData.GetMood();
        Debug.Log($"Current Mood: {mood}");

        playerSpeed = (int)((double)currentRunData.GetStatValue(StatType.spd) * GetMoodMultiplier());
        playerWit = (int)((double)currentRunData.GetStatValue(StatType.wit) * GetMoodMultiplier());
        playerMemory = (int)((double)currentRunData.GetStatValue(StatType.mem) * GetMoodMultiplier());
        playerLuck = (int)((double)currentRunData.GetStatValue(StatType.luk) * GetMoodMultiplier());

        exam = examSchedule.GetExamForTurn(currentTurn);

        testSpeed = exam.GetStatValue(StatType.spd);
        testWit = exam.GetStatValue(StatType.wit);
        testMemory = exam.GetStatValue(StatType.mem);
        testLuck = exam.GetStatValue(StatType.luk);

        playerHitChance = (double)playerLuck/testLuck;
        playerCritChance = (double)playerLuck/1000;
        testHitChance = (double)testLuck/playerLuck;
        testCritChance = (double)testLuck/1000;

        playerHpBar.GetComponent<HpBarController>().SetMaxHp(playerMemory * 7);
        testHpBar.GetComponent<HpBarController>().SetMaxHp(testMemory * 7);
        playerActionBar.setSpeed(playerSpeed);
        testActionBar.setSpeed(testSpeed);

        Debug.Log($"Player Stats - Speed: {playerSpeed}, Wit: {playerWit}, Memory: {playerMemory}, Luck: {playerLuck}, testHitChance: {testHitChance},testCritChance: {testCritChance}");
        Debug.Log($"Test Stats - Speed: {testSpeed}, Wit: {testWit}, Memory: {testMemory}, Luck: {testLuck}, playerHitChance: {playerHitChance},playerCritChance: {playerCritChance}");

    }

    public void PlayerTakeDamage()
    {
        float height = playerHpRect.rect.height;
        float xPosition = -playerHpRect.rect.width;
        float yPosition = (height * playerHpBar.GetComponent<HpBarController>().GetValue()) - (height / 2);
        testTransform.DOMoveX(- 640f, 0.2f).SetRelative().SetEase(Ease.OutQuad).OnComplete(
            () => testTransform.DOMoveX(640f, 0.2f).SetRelative().SetEase(Ease.OutQuad)
            );

        if (Random.value >= (testHitChance))
        {
            Debug.Log("Test's attack missed!");
            SpawnMissPopUp(playerHpBar.transform.position + new Vector3(xPosition, yPosition, 0));
            return;
        }
        if (Random.value <= (testCritChance))
        {
            playerHpBar.GetComponent<HpBarController>().TakeDamage(testWit * 2);
            Debug.Log($"Test critical attacked for {testWit * 2}!");
            SpawnDamagePopUp(testWit * 2, playerHpBar.transform.position + new Vector3(xPosition, yPosition, 0), true);
            return;
        }
        playerHpBar.GetComponent<HpBarController>().TakeDamage(testWit);
        SpawnDamagePopUp(testWit, playerHpBar.transform.position + new Vector3(xPosition, yPosition, 0), false);
        Debug.Log($"Player takes {testWit} damage.");
    }

    public void TestTakeDamage()
    {
        float height = TestHpRect.rect.height;
        float xPosition = TestHpRect.rect.width;
        float yPosition = (height * testHpBar.GetComponent<HpBarController>().GetValue()) - (height / 2);
        playerTransform.DOMoveX(640f, 0.2f).SetRelative().SetEase(Ease.OutQuad).OnComplete(
            () => playerTransform.DOMoveX(-640f, 0.2f).SetRelative().SetEase(Ease.OutQuad)
        );
        if (Random.value >= (playerHitChance))
        {
            Debug.Log("Player's attack missed!");
            SpawnMissPopUp(testHpBar.transform.position + new Vector3(xPosition, yPosition, 0));
            return;
        }
        if (Random.value <= (playerCritChance))
        {
            testHpBar.GetComponent<HpBarController>().TakeDamage(playerWit * 2);
            Debug.Log($"Player critical attacked for {playerWit * 2}!");
            SpawnDamagePopUp(playerWit * 2, testHpBar.transform.position + new Vector3(xPosition, yPosition, 0), true);
            return;
        }
        testHpBar.GetComponent<HpBarController>().TakeDamage(playerWit);
        Debug.Log($"Test takes {playerWit} damage.");
        SpawnDamagePopUp(playerWit, testHpBar.transform.position + new Vector3(xPosition, yPosition, 0), false);
    }

    public double GetMoodMultiplier()
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

    public void SpawnDamagePopUp(int damageAmount, Vector3 spawnPosition, bool isCrit)
    {
        Transform damagePopUpTransform = Instantiate(damagePopUpPrefab, spawnPosition, Quaternion.identity, canvasTransform);
        DamagePopUp damagePopUp = damagePopUpTransform.GetComponent<DamagePopUp>();
        damagePopUp.SetupDamage(damageAmount, isCrit);
    }

    public void SpawnMissPopUp(Vector3 spawnPosition)
    {
        Transform damagePopUpTransform = Instantiate(damagePopUpPrefab, spawnPosition, Quaternion.identity, canvasTransform);
        DamagePopUp damagePopUp = damagePopUpTransform.GetComponent<DamagePopUp>();
        damagePopUp.SetupMiss();
    }

    public void FailNextScene()
    {
        GameStateMan.Instance.RequestState(GameStateMan.GameState.VisualNovel, new() 
            {{"vn_type", "post_test"}, {"post_test_node", exam.nodeNameFail}});
    }

    public void PassNextScene()
    {
        GameStateMan.Instance.RequestState(GameStateMan.GameState.VisualNovel, new() 
            {{"vn_type", "post_test"}, {"post_test_node", exam.nodeNamePass}});
    }

    public void OnDestroy()
    {
        Time.timeScale = 1.0f;
    }
}

