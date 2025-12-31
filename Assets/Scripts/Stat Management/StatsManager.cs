using UnityEngine;
using System;
using System.Collections.Generic;

public class StatsManager : MonoBehaviour
{
	public static StatsManager Instance { get; private set; }
	
	[Header("Reference")]
	public CurrentRunData runData;
    public StudyProgressionHandler progressionHandler;
    public StatsProcessor statProcessor;
	public StatType StatType;

	public void Awake()
	{
		// Singleton Pattern
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else Destroy(gameObject);
	}

	public StatType ExecuteStudyAction(StatType primaryStatType)
	{
		if (runData == null) return;

		// 1. Check for Failure (Clarity System)
        bool success = statProcessor.RollForSuccess(runData.Clarity);
        
        if (!success)
        {
            HandleStudyFailure(primaryStatType);
            return;
        }

        // 2. Get Base Gains
        int currentLevel = progressionHandler.GetLevel(primaryStatType);
        var baseGains = progressionHandler.GetGainsForLevel(currentLevel);

        // 3. Bonus Gain (Mood System)
        int finalPrimary = statProcessor.CalculateFinalGain(baseGains.p, runData.Mood);
        int finalSecondary = statProcessor.CalculateFinalGain(baseGains.s, runData.Mood);

        // 4. Find Relationship and Apply
        StatGain relationship = progressionHandler.StudyGains.Find(g => g.PrimaryStat == primaryStatType);
        
        ApplyStatGain(relationship.PrimaryStat, finalPrimary);
        ApplyStatGain(relationship.SecondaryStat, finalSecondary);

        // 5. Update Weight for next time
        progressionHandler.AddWeight(primaryStatType);

        // 6. Transition to VN
        VisualNovelTransition(primaryStatType, true);
	}
	
	private void ApplyStatGain(StatType type, int amount)
	{
		int currentValue = runData.GetStatValue(statType);
		const int MaxValue = 1000;
		
		currentValue += amount;
		currentValue = Mathf.Min(currentValue, MaxValue);
		
		runData.SetStatValue(type, currentValue);
	}

	private void HandleStudyFailure(StatType type)
    {
        Debug.Log($"Study Failed for {type}!");
        VisualNovelTransition(type, false);
    }
	
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

	// Getter function for StudyScreenController
	public int GetStatValue(StatType type)
	{
		if (runData == null) return 0;
		return runData.GetStatValue(type); // Move reading value to CurrentRunData object
	}
	
}