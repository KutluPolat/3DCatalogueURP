using KPFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config
{
    /// <summary>
    /// Duration of default UI Action.
    /// </summary>
    public const float UI_ACT_DUR = 0.25f;
    public const float DelayForRepeatingSounds = 0.05f;

    public static int GetExpToReach(int level)
    {
        int cache = level - 1;
        return 300 * cache + (300 * cache * cache * cache / level);
    }
    /*
     * 300 * 0 + 300 * 0 / 2 = 0
     * 300 * 1 + 300 * 1 / 2 = 450
     * 300 * 2 + 300 * 2 / 2 = 900
     * 300 * 3 + 300 * 3 / 2 = 1350
     * 300 * 4 + 300 * 4 / 2 = 1800
     * 
     */

    private const float BASE_UNIT_SPEED = 3f;
    private readonly static (int, float)[] STAT_CAPS = new (int, float)[]
    {
        (40, 100f),
        (50, 50f),
        (60, 35f),
        (70, 25f),
        (80, 10f)
    };
    private static float GetCappedValue(int level, float valuePerLevel, params (int levelLimit, float capPercent)[] caps)
    {
        if (caps == null || caps.Length == 0)
        {
            DebugUtility.LogError(ErrorType.MethodParameterIsNull, "CAPPED VALUE CALCULATION");
            return level * valuePerLevel;
        }

        float rawValue = 0f;
        for (int i = 1; i <= level; i++)
        {
            bool capApplied = false;
            foreach (var tuple in caps)
            {
                if (i <= tuple.levelLimit)
                {
                    rawValue += valuePerLevel * tuple.capPercent * 0.01f;
                    capApplied = true;
                    break;
                }
            }

            if (!capApplied)
            {
                rawValue += valuePerLevel * caps[^1].capPercent * 0.01f;
            }
        }

        return rawValue;
    }

    public static float GetMaxSpeed(int charLevel, int vitaLevel, int staLevel)
    {
        return BASE_UNIT_SPEED
            + GetCappedValue(charLevel, 0.05f, STAT_CAPS)
            + GetCappedValue(vitaLevel, 0.1f, STAT_CAPS)
            + GetCappedValue(staLevel, 0.5f, STAT_CAPS);
    }


    public static CharGrade GetGrade(int level)
    {
        if (level.IsInRange(0, 15))
        {
            return CharGrade.Peasant;
        }
        else if (level.IsInRange(16, 30))
        {
            return CharGrade.Adventurer;
        }
        else if (level.IsInRange(31, 45))
        {
            return CharGrade.Hero;
        }
        else if (level.IsInRange(46, 60))
        {
            return CharGrade.Myth;
        }
        else if (level.IsInRange(61, 75))
        {
            return CharGrade.Legend;
        }
        else if (level.IsInRange(76, 90))
        {
            return CharGrade.Demigod;
        }
        else if (level.IsInRange(91, int.MaxValue))
        {
            return CharGrade.God;
        }

        DebugUtility.LogError(ErrorType.SwitchCaseNotFound, "Level:"+level.ToString());
        return CharGrade.Peasant;
    }
}