public interface IDrag
{
    void OnDrag(DragTransform dragValue, out DragTransform dragResult);
    DragTransform? GetDragCoordinates();
    void OnEndDrag(DragTransform dragResult);
}
