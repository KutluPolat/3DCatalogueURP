using KPFramework;
using UnityEngine;

[CreateAssetMenu(fileName = "NewClass", menuName = "Kutlu/NewClass")]
public class SO_Class : ScriptableObject
{
    public Class Class => charClass;
    public int GetMultiplier(Stat stat)
    {
        switch (stat)
        {
            case Stat.Vitality:
                return Vitality;
            case Stat.Stamina:
                return Stamina;
            case Stat.Mental:
                return Mental;

            default:
                DebugUtility.LogError(ErrorType.SwitchCaseNotFound, stat.ToString());
                return -1;
        }
    }

    [SerializeField] private Class charClass;
    [SerializeField, Range(0, 5)] private int Vitality;
    [SerializeField, Range(0, 5)] private int Stamina;
    [SerializeField, Range(0, 5)] private int Mental;
}