using System.Collections.Generic;
using UnityEngine;

namespace BFME2.Core
{
    [CreateAssetMenu(menuName = "BFME2/Events/Game Event")]
    public class GameEvent : ScriptableObject
    {
        private readonly List<IGameEventListener> _listeners = new();

        public void Raise()
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                _listeners[i].OnEventRaised();
            }
        }

        public void Register(IGameEventListener listener)
        {
            if (!_listeners.Contains(listener))
            {
                _listeners.Add(listener);
            }
        }

        public void Unregister(IGameEventListener listener)
        {
            _listeners.Remove(listener);
        }
    }

    public interface IGameEventListener
    {
        void OnEventRaised();
    }
}
