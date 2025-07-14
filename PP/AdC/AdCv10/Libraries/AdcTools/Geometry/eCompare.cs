namespace AdcTools
{
    public enum eCompare
    {
        RectIsInside,
        RectIsOutside,
        RectIntersects,
        SegmentIsInside = RectIsInside,
        SegmentIsOutside = RectIsOutside,
        SegmentIntersects = RectIntersects,
        QuadIsInside = RectIsInside,
        QuadIsOutside = RectIsOutside,
        QuadIntersects = RectIntersects
    };
}
