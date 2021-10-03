using UnityEngine;

public interface IDrag
{
    bool OnTryDrag(DragTransform dragValue);
    DragTransform GetDragCoordinates();
    void OnEndDrag(DragTransform dragValue);
}
