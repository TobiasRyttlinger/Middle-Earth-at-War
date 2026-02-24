using System;
using UnityEngine;

namespace BFME2.Core
{
    public interface IInputManager
    {
        event Action<Vector2> OnCameraMoveInput;
        event Action<float> OnCameraZoomInput;
        event Action<float> OnCameraRotateInput;
        event Action<Vector2> OnSelectInput;
        event Action<Vector2> OnBoxSelectStart;
        event Action<Vector2> OnBoxSelectEnd;
        event Action<Vector2> OnActionCommand;
        event Action OnDoubleClick;
        event Action OnCancelAction;
        event Action<Vector2> OnBuildPlaceInput;
        event Action<float> OnBuildRotateInput;
        event Action OnBuildCancelInput;

        Vector2 MouseScreenPosition { get; }
        bool IsShiftHeld { get; }
        bool IsCtrlHeld { get; }

        void SwitchToGameplayMap();
        void SwitchToBuildMap();
        void SwitchToUIMap();
    }
}
