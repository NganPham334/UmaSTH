using UnityEngine;
using System.Collections.Generic;

public class StatsManager : MonoBehaviour
{
	public static StatsManager Instance { get; private set; }
	
	[Header("Reference")]
	public CurrentRunData runData;
	public StudyProgressionHandler progressionHandler;
    public StatsProcessor statsProcessor;

	public void Awake()
	{
		// Singleton Pattern
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);

			// Justin Case, not actually needed
			if (progressionHandler == null) progressionHandler = GetComponent<StudyProgressionHandler>();
        	if (statsProcessor == null) statsProcessor = GetComponent<StatsProcessor>();
		}
		else Destroy(gameObject);
	}

	public StatType ExecuteStudyAction(StatType primaryStatType)
	{
		if (runData == null) return primaryStatType;

		// 0. Comsume Clarity (Whether fail or success)
		int clrCost = statsProcessor.GetClarityCost(primaryStatType);
		runData.Clarity = Mathf.Max(0, runData.Clarity - clrCost);
		runData.SetStatValue(StatType.clr, runData.Clarity);

		// 1. Check for Failure (Clarity System)
        bool success = statsProcessor.RollForSuccess(runData.Clarity);
        
        if (!success)
        {
            HandleStudyFailure(primaryStatType);
            return primaryStatType;
        }

		// 2. Moved the calculation to its separate function
		var expectedValue = GetExpectedGains(primaryStatType);
        
        ApplyStatGain(primaryStatType, expectedValue.pGain);
        ApplyStatGain(expectedValue.sType, expectedValue.sGain);

        // 3. Update Weight for next time
        progressionHandler.ProcessStudyWeight(primaryStatType);

		// 4. Display
		return expectedValue.sType;
	}
	
	// Calculator for Preview UI
	// returns: (Primary Gain, Secondary Gain, Which Secondary Stat is it?)
	public (int pGain, int sGain, StatType sType) GetExpectedGains(StatType primaryStatType)
	{
		// 1. Get Base Gains based on current study level
		int currentLevel = runData.GetStatLevel(primaryStatType);
		var baseGains = progressionHandler.GetGainsForLevel(currentLevel);

		// 2. Calculate Final Gains using the Mood System in StatsProcessor
		var (finalPrimary, finalSecondary) = statsProcessor.CalculateFinalGain(
			baseGains.p, 
			baseGains.s, 
			runData.Mood
		);

		// 3. Find which secondary stat is linked to this primary
		StatGain relationship = progressionHandler.studyGains.Find(g => g.primaryStat == primaryStatType);

		return (finalPrimary, finalSecondary, relationship.secondaryStat);
	}
	
	private void ApplyStatGain(StatType type, int amount)
	{
		int currentValue = runData.GetStatValue(type);
		const int maxValue = 1000;
		
		currentValue += amount;
		currentValue = Mathf.Min(currentValue, maxValue);
		
		runData.SetStatValue(type, currentValue);
	}

	private void HandleStudyFailure(StatType type)
    {
        Debug.Log($"Study Failed for {type}!");
        VisualNovelTransition(type, false);
    }
	
	// Since we don't have a Visual Novel dialogue for study success
	// this method can be obsolete
	private void VisualNovelTransition(StatType type, bool passed)
	// Created purely for convenience, I'm lazy to type out the whole thing
    {
        string suffix = passed ? "_pass" : "_fail";
        var vnParams = new Dictionary<string, object>
        {
            { "vn_type", "studying" },
            { "study_type", $"{type}{suffix}" }
        };
        GameStateMan.Instance.RequestState(GameStateMan.GameState.VisualNovel, vnParams);
    }
}