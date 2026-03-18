namespace Q1.tests.Core;

using core;
using core.Arch;
using core.Arch.Lookups;

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
        
        Assert.That(AddressingModes.Lookup.Length, Is.EqualTo(8),
            "Expected addressing mode lookup to have 8 entries");
    }
}