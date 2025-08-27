namespace RegionVisualiser;

public class Region
{
    public string Name;
    public int Start;
    public int Size;
    public int End => this.Start + this.Size -1;
    public string Info;
}