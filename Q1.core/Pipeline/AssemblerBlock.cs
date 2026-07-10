namespace Q1.core.Pipeline;

public class AssemblerBlock
{
    private readonly MemoryStream _stream = new();
    public readonly Dictionary<i64, (bool word, string constant)> DeferredConstants = new();
    
    public long Size => this._stream.Length;
    
    public void WriteByte(u8 data)
    {
        this._stream.WriteByte(data);
    }

    public void WriteWord(u16 data)
    {
        u8 high = (u8) (data       & 0xFF);
        u8 low = (u8) ((data >> 8) & 0xFF);
        this._stream.WriteByte(low);
        this._stream.WriteByte(high);
    }

    public void DeferWordConstant(string constant)
    {
        this.DeferredConstants.Add(this._stream.Length - 1, (true, constant));
        this.WriteWord(0);
    }
    
    public void DeferByteConstant(string constant)
    {
        this.DeferredConstants.Add(this._stream.Length - 1, (false, constant));
        this.WriteByte(0);
    }
    
    public void ResolveWordConstant(i32 offset, u16 value)
    {
        long currentPos = this._stream.Position;
        this._stream.Position = offset;
        
        this.WriteWord(value);
        
        this._stream.Position = currentPos;
    }
    
    public void ResolveByteConstant(i32 offset, u8 value)
    {
        long currentPos = this._stream.Position;
        this._stream.Position = offset;
        
        this.WriteByte(value);
        
        this._stream.Position = currentPos;
    }
    
    public void WriteToStream(MemoryStream target)
    {
        this._stream.Position = 0;
        this._stream.CopyTo(target);
    }
}