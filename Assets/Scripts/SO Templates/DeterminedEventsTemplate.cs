using UnityEngine;
using System.Collections.Generic;

namespace VisualNovel
{
    [CreateAssetMenu(fileName = "DeterminedEvents", menuName = "Pre-determined Events", order = 0)]
    public class DeterminedEventsTemplate : ScriptableObject
    {
        public List<TurnEvent> events;

        public TextAsset GetEventForTurn(int turn)
        {
            foreach (TurnEvent e in events)
            {
                if (e.turnNumber == turn) return e.inkJSON;
            }
            return null; 
        }
    }
    
    [System.Serializable]
    public class TurnEvent
    {
        public int turnNumber;
        public TextAsset inkJSON;
    }
}

