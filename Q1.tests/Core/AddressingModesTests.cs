namespace Q1.tests.Core;

using core;
using core.Arch;

[TestFixture]
public class AddressingModesTests
{

    [Test]
    public void LookupShouldHaveAddressingModes()
    {
        var populatedModes = AddressingModes.Lookup
            .Where(x => x != default)
            .ToArray();
        
        Assert.That(populatedModes, Has.Length.EqualTo(8), 
            "Expected 8 addressing modes to be defined");
    }
}