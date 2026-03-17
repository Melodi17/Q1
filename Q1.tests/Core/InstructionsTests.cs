namespace Q1.tests.Core;

using core;
using core.Arch;

[TestFixture]
public class InstructionsTests
{

    [Test]
    public void LookupShouldHaveInstructions()
    {
        var populatedSets = InstructionSet.Lookup
            .Where(x => x != default)
            .ToArray();
        
        Assert.That(populatedSets, Has.Length.EqualTo(18), 
            "Expected 8 instruction groups to be defined");
    }
}