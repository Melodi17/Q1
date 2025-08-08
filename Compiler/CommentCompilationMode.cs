namespace Q1.Compiler;

[Flags]
public enum CommentCompilationMode
{
    None        = 0,
    UserDefined = 1 << 0,
    Generated   = 1 << 1,
    All         = CommentCompilationMode.UserDefined | CommentCompilationMode.Generated
}