namespace MechWars.Utils
{
    public interface IRectangleBounds
    {
        IVector2 Location { get; }
        IVector2 Size { get; }

        int X0 { get; }
        int X1 { get; }
        int Y0 { get; }
        int Y1 { get; }

        bool ContainsPoint(IVector2 point);
        bool IntersectsOther(IRectangleBounds bounds);
    }
}
