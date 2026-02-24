using UnityEngine;

namespace BFME2.Core
{
    public interface ISelectable
    {
        SelectableType Type { get; }
        FactionId OwnerFaction { get; }
        int OwnerPlayerId { get; }
        Transform Transform { get; }
        Bounds SelectionBounds { get; }
        bool IsSelectable { get; }
        void OnSelected();
        void OnDeselected();
    }
}
