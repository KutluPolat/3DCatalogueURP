using UnityEngine;

public struct Msg_DeltaPos
{
    public Vector2 DeltaPos;
    public float x => DeltaPos.x;
    public float y => DeltaPos.y;
    public bool IsVerticalMovement { get; private set; }
    public bool IsHorizontalMovement { get; private set; }

    //public bool IsInputStartedOnTopSide => Input.mousePosition.y > Screen.height * TouchSwipeHandler.TopSideStartPercent;

    public Msg_DeltaPos(Vector2 deltaPos, bool isVerticalMovement, bool isHorizontalMovement)
    {
        DeltaPos = deltaPos;
        IsVerticalMovement = isVerticalMovement;
        IsHorizontalMovement = isHorizontalMovement;
    }
}