using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/VictoryConfig", fileName = "VictoryConfig")]
public class VictoryConfig : ScriptableObject
{
    public enum Logic { All, Any }

    public Logic logic = Logic.All;

    // 多态序列化列表（可扩展）
    [SerializeReference] public List<VictoryConditionBase> conditions = new();

    // 在 Inspector 右键组件标题或点齿轮菜单可调用
    [ContextMenu("Add Condition/Kill Count")]
    private void AddKillCountCondition()
    {
        conditions ??= new List<VictoryConditionBase>();
        conditions.Add(new KillCountCondition());
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    [ContextMenu("Add Condition/Survive Time")]
    private void AddSurviveTimeCondition()
    {
        conditions ??= new List<VictoryConditionBase>();
        conditions.Add(new SurviveTimeCondition());
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    [ContextMenu("Clear Conditions")]
    private void ClearConditions()
    {
        conditions?.Clear();
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}