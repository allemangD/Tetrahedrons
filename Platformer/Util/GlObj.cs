namespace Platformer.Util
{
    public abstract class GlObj
    {
        public readonly int Id;

        public GlObj(int id)
        {
            Id = id;
        }

        public static implicit operator int(GlObj o) => o.Id;
    }
}