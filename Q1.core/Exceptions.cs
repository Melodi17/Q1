namespace Q1.core;

public class ChipException(string message) : Exception(message);

public class InvalidInstructionException(u8 opcode) : ChipException($"Invalid instruction: 0x{opcode:X2}");
public class InvalidAddressingModeException(u8 mode) : ChipException($"Invalid addressing mode: 0x{mode:X2}");
public class HaltException() : ChipException("Execution halted");