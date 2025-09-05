namespace Compiler.Testing;

using Q1;

[TestFixture]
public class SanityTests
{
    [Test]
    public void BlankProgram()
    {
        string program = """
                         int main() { }
                         """;

        string result = TestingHelpers.CompileString(program).Trim();
        Assert.That(
            result, Is.EqualTo(
                """
                jmp _main
                _main:
                    mov 0, V0
                    ret
                """));
    }

    [Test]
    public void ReturnConstant()
    {
        string program = """
                         int main() { return 42; }
                         """;

        string result = TestingHelpers.CompileString(program).Trim();
        Assert.That(
            result, Is.EqualTo(
                """
                jmp _main
                _main:
                    mov 42, V0
                    ret
                """));
    }
}