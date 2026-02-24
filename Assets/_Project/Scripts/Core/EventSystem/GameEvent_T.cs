using System.Collections.Generic;
using UnityEngine;

namespace BFME2.Core
{
    public class GameEvent<T> : ScriptableObject
    {
        private readonly List<IGameEventListener<T>> _listeners = new();

        public void Raise(T value)
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                _listeners[i].OnEventRaised(value);
            }
        }

        public void Register(IGameEventListener<T> listener)
        {
            if (!_listeners.Contains(listener))
            {
                _listeners.Add(listener);
            }
        }

        public void Unregister(IGameEventListener<T> listener)
        {
            _listeners.Remove(listener);
        }
    }

    public interface IGameEventListener<T>
    {
        void OnEventRaised(T value);
    }

    // Concrete typed events for use as ScriptableObject assets
    [CreateAssetMenu(menuName = "BFME2/Events/Int Event")]
    public class IntGameEvent : GameEvent<int> { }

    [CreateAssetMenu(menuName = "BFME2/Events/Float Event")]
    public class FloatGameEvent : GameEvent<float> { }

    [CreateAssetMenu(menuName = "BFME2/Events/Vector3 Event")]
    public class Vector3GameEvent : GameEvent<Vector3> { }

    [CreateAssetMenu(menuName = "BFME2/Events/GameObject Event")]
    public class GameObjectGameEvent : GameEvent<GameObject> { }
}
