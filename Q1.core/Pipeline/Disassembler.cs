namespace Q1.core.Pipeline;

using Arch;
using Arch.Lookups;

public class Disassembler
{
    public StringWriter Disassemble(MemoryStream stream)
    {
            var reader = new BinaryReader(stream);
            var writer = new StringWriter();
            
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                u16 instruction = reader.ReadUInt16();
                InstructionEncoding.Decode(instruction, out u8 opcode, out u8 m1, out u8 m2, out bool word);
                
                var group = InstructionSet.Lookup[opcode];
                (string name, int param) = (m1, m2) switch
                {
                    (0x00, 0x00) => (group.Implicit.Name, 0),
                    (_, 0x00) => (group.SimpleAddressing.Name, 1),
                    (0x00, 0x01) => (group.ExtendedImplicit.Name, 0),
                    (_, _) => (group.ExtendedAddressing.Name, 2)
                };
                
                writer.Write(name);
                if (param == 1)
                {
                    
                }
            }
    
            return writer;
    }
}