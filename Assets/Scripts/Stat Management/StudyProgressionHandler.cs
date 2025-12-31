using UnityEngine;
using System;
using System.Collections.Generic;

public class StudyProgressionHandler : MonoBehaviour
{
    public CurrentRunData runData;

    // The Reward Table (Level 1: 10/2, Level 2: 14/4, etc.)
    private Dictionary<int, (int primary, int secondary)> levelRewards = new Dictionary<int, (int, int)>
    {
        { 1, (10, 2) },
        { 2, (14, 4) },
        { 3, (18, 6) },
        { 4, (22, 8) },
        { 5, (26, 10) }
    };

    [Header("Study Gains Relationship")]
	public List<StatGain> StudyGains = new List<StatGain>()
	{
		new StatGain {PrimaryStat = StatType.spd, SecondaryStat = StatType.wit},
		new StatGain {PrimaryStat = StatType.mem, SecondaryStat = StatType.spd},
		new StatGain {PrimaryStat = StatType.wit, SecondaryStat = StatType.luk},
		new StatGain {PrimaryStat = StatType.luk, SecondaryStat = StatType.mem}
	};

    public (int p, int s) GetGainsForLevel(int level)
    {
        if (levelRewards.TryGetValue(level, out var gains))
            return gains;
        return (10, 2); // Fallback
    }

    public void ProcessStudyWeight(StatType type)
    {
        // If Study Level is maxed (5), no Weight will be added to that 
        if (runData.GetStatLevel(type) >= 5) return;
        AddWeight(type);
    }

    public List<string> TriggerUpgradeEvent(int points)
    {
        List<string> results = new List<string>();

        for (int i = 0; i < points; i++)
        {
            StatType winner = RollLottery();
            
            // If winner is -1, all stats are maxed or weights are 0
            if (winner == (StatType)(-1)) break;

            LevelUp(winner);
            results.Add(winner.ToString().ToUpper());
        }

        ResetWeights();
        return results;
    }

    private StatType RollLottery()
    {
        int TotalWeight = runData.spdWeight + runData.witWeight + runData.memWeight + runData.lukWeight;
        if (TotalWeight <= 0) return (StatType)(-1);

        int roll = UnityEngine.Random.Range(0, TotalWeight);
        int current = 0;

        foreach (StatType type in Enum.GetValues(typeof(StatType)))
        {
            current += runData.GetStatWeight(type); // Lấy Weight của từng Stat
            if (roll < current) return type;
            
            // Đầu tiên, mình sẽ vẽ ra một cái bảng, lần lượt ghi ra weight của từng stat
            // Ví dụ: | 3 | 2 | 1 | 1 |
            // Rồi mình sẽ tung bừa một số (gọi là roll) để quyết định stat nào được level up
            // Bằng cách so sánh current và roll (nếu roll < current)
            // VD: Roll ra 5
            // Current += Weight của Speed = 0 + 3 = 3
            // roll = 5 < current = 3 ? False, vậy Speed không được level up
            // Current += Weight của Wit = 3 + 2 = 5
            // roll = 5 < current = 5 ? False, vậy Wit không được level up
            // Current += Weight của Memory = 5 + 1 = 6
            // roll = 5 < current = 6 ? True, vậy Memory leveled up
        }
        return (StatType)(-1);
    }

    private void LevelUp(StatType type)
    {
        switch (type)
        {
            case StatType.spd: if (runData.spdLevel < 5) runData.spdLevel++; break;
            case StatType.wit: if (runData.witLevel < 5) runData.witLevel++; break;
            case StatType.mem: if (runData.memLevel < 5) runData.memLevel++; break;
            case StatType.luk: if (runData.lukLevel < 5) runData.lukLevel++; break;
        }
    }

    private void ResetWeights()
    {
        // If max level, weight locks to 0. Otherwise resets to 1
        runData.spdWeight = runData.spdLevel >= 5 ? 0 : 1;
        runData.witWeight = runData.witLevel >= 5 ? 0 : 1;
        runData.memWeight = runData.memLevel >= 5 ? 0 : 1;
        runData.lukWeight = runData.lukLevel >= 5 ? 0 : 1;
    }

    private void AddWeight(StatType type)
    {
        switch (type)
        {
            case StatType.spd: runData.spdWeight++; break;
            case StatType.wit: runData.witWeight++; break;
            case StatType.mem: runData.memWeight++; break;
            case StatType.luk: runData.lukWeight++; break;
        }
    }
}