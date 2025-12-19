using UnityEngine;

public class CombatLogic : MonoBehaviour
{    
    public CurrentRunData currentRunData;
    public int playerSpeed, playerWit, playerMemory, playerLuck;
    public int testSpeed, testWit, testMemory, testLuck;
    public GameObject playerHpBar;
    public GameObject testHpBar;

    void Start()
    {
        playerSpeed = currentRunData.GetStatValue(StatType.SPD);
        playerWit = currentRunData.GetStatValue(StatType.WIT);
        playerMemory = currentRunData.GetStatValue(StatType.MEM);
        playerLuck = currentRunData.GetStatValue(StatType.LUK);

        testSpeed = 20;
        testWit = 10;
        testMemory = 5;
        testLuck = 5;

        Debug.Log($"Player Stats - Speed: {playerSpeed}, Wit: {playerWit}, Memory: {playerMemory}, Luck: {playerLuck}");
    }

    public void PlayerTakeDamage()
    {
        playerHpBar.GetComponent<HpBarController>().TakeDamage(testWit);
        Debug.Log($"Player takes {testWit} damage.");
    }

    public void TestTakeDamage()
    {
        testHpBar.GetComponent<HpBarController>().TakeDamage(playerWit);
        Debug.Log($"Test takes {playerWit} damage.");
    }
}
