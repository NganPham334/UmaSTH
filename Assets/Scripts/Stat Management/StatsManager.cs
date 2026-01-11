using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatsManager : MonoBehaviour
{
	public static StatsManager Instance { get; private set; }
	
	[Header("Reference")]
	public CurrentRunData runData;
	public StudyProgressionHandler progressionHandler;
    public StatsProcessor statsProcessor;
    
    // An Event, this tell StudyButton.cs that the wait is over and it can continue
    public static System.Action OnStudyActionFinished;

	public void Awake()
	{
		// Singleton Pattern
		if (Instance == null)
		{
			Instance = this;

			// Justin Case, not actually needed
			if (progressionHandler == null) progressionHandler = GetComponent<StudyProgressionHandler>();
        	if (statsProcessor == null) statsProcessor = GetComponent<StatsProcessor>();
		}
		else Destroy(gameObject);
	}

	// Ensure the old reference is gone
	private void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}

	public void ExecuteStudyAction(StatType primaryStatType)
	{
		StartCoroutine(StudyRoutine(primaryStatType));
	}

	private IEnumerator StudyRoutine(StatType primaryStatType)
	{
		if (runData == null) yield break;

		// 0. Consume Clarity (Whether fail or success)
		int clrCost = statsProcessor.GetClarityCost(primaryStatType);
		runData.Clarity = Mathf.Max(0, runData.Clarity - clrCost);
		if (ClarityBar.Instance != null)
		{
			ClarityBar.UpdateClarity(-clrCost);
		}
		
		// In preparation for StatsBox Tweening, the calculation
		// have to be done beforehand
		// 1. Calculate and apply stats
		var expectedValue = GetExpectedGains(primaryStatType);
        ApplyStatGain(primaryStatType, expectedValue.pGain);
        ApplyStatGain(expectedValue.sType, expectedValue.sGain);
        
        // 2. Update Weight for Upgrade Event
        progressionHandler.ProcessStudyWeight(primaryStatType);
        
		// 3. Delay: 1s to finish DOTween animation
		yield return new WaitForSeconds(1.0f);
		
		// 4. Check for Failure (Clarity check)
        bool success = statsProcessor.RollForSuccess(runData.Clarity);
        if (!success)
        {
            HandleStudyFailure(primaryStatType);
            yield break;
        }
        
		// 5. Notify StudyButton that all tasks are finished
		OnStudyActionFinished?.Invoke();
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