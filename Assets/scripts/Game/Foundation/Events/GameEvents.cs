
using UnityEngine;

namespace Game.Foundation
{
    public struct VictoryEvent
    {
        public VictorySystem Source;
        public VictoryEvent(VictorySystem source)
        {
            Source = source;
        }   
    }

    public struct DefeatEvent
    {
        public GameObject Player;
        public DefeatEvent(GameObject player)
        {
            Player = player;
        }
    }

    public struct ProgressUpdateEvent
    {
        public VictoryConditionBase Condition;

        public ProgressUpdateEvent(VictoryConditionBase condition)
        {
            Condition = condition;
        }
    }
}
