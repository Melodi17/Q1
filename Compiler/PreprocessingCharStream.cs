namespace Q1.Compiler;

using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

public class PreprocessingCharStream : ICharStream
{
    private readonly AntlrInputStream inner;

    public PreprocessingCharStream(string filePath)
    {
        string preprocessed = Preprocessor.Process(filePath);
        this.inner = new AntlrInputStream(preprocessed);
    }

    public int Index => this.inner.Index;
    public int Size => this.inner.Size;
    public string SourceName => this.inner.SourceName;
    public void Consume() => this.inner.Consume();
    public int LA(int i) => this.inner.LA(i);
    public int Mark() => this.inner.Mark();
    public void Release(int marker) => this.inner.Release(marker);
    public void Seek(int index) => this.inner.Seek(index);
    public string GetText(Interval interval) => this.inner.GetText(interval);
}