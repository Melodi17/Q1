using System;

namespace Q1Emu.Chip;

using System.Collections.Generic;
using Assembler;

public partial class Q1Cpu
{
    public const u16 StackStart  = 0x2FFF;
    public const u16 StackLength = 16 * 2; // 16 16-bit values

    public Random Random;

    public u16 AX = 0; // Accumulator register
    public u16 DX = 0; // Data register
    public u16 LX = 0; // Logical register

    public u16 SP { get; set; } // Stack pointer
    public u16 PC { get; set; } // Program counter

    public u16[] V      { get; set; } // Variable registers (64 total)
    public u16   I      { get; set; } // Index register
    public u16   Cycles { get; set; } // Number of cycles

    public Bus Bus { get; set; }

    public Q1Cpu()
    {
        this.LoadInstructionLookup();

        this.Reset();
    }

    public void Reset()
    {
        this.V = new u16[16];

        this.SP = StackStart;

        this.I = 0;
    }

    public void Clock()
    {
        var pc = this.PC;
        u16 instruction = this.FetchInstruction(out u8 op, out u8 m1, out u8 m2, out _);
        Console.WriteLine($"CPU.Clock: {pc:X4} |  {Disassembler.ParseInstruction(instruction)}");
        // Console.WriteLine($"{V[0]:X4} {V[1]:X4} {V[2]:X4} {V[3]:X4} {V[4]:X4} {V[5]:X4} {V[6]:X4} {V[7]:X4}");

        bool ex = m2 > 0;
        switch (m1, ex)
        {
            case (0x00, false):
                this.InstructionLookup[op].Item1(); // Implicit
                break;
            case (> 0x00, false):
                this.InstructionLookup[op].Item2(m1); // Simple addressing mode
                break;
            case (0x00, true):
                this.InstructionLookup[op].Item3(); // Extended implicit
                break;
            case (> 0x00, true):
                this.InstructionLookup[op].Item4(m1, m2); // Extended addressing mode
                break;
        }

        this.Cycles++;
    }
    public u16 FetchInstruction(out u8 op, out u8 m1, out u8 m2, out u8 length)
    {
        u16 instruction = this.Bus.ReadWord(this.PC);
        this.PC += 2;

        op = (u8) ((instruction >> 8) & 0xFF); // 256 opcodes
        m1 = (u8) ((instruction >> 4) & 0x0F); // 16 modes
        m2 = (u8) (instruction        & 0x0F); // 16 modes

        length = (u8) (this.AddressingModeLength(m1) + this.AddressingModeLength(m2));
        return instruction;
    }

    private void PushStack(u16 value)
    {
        if (this.SP + 2 >= StackStart + StackLength)
            throw new InvalidOperationException("Stack overflow");

        this.Bus.WriteWord(this.SP, value);
        this.SP += 2;
    }

    private u16 PopStack()
    {
        if (this.SP - 2 < StackStart)
            throw new InvalidOperationException("Stack underflow");

        this.SP -= 2;
        return this.Bus.ReadWord(this.SP);
    }

    private u16 FetchRegister(u8 regIndex)
    {
        return regIndex switch
        {
            <= 0x3F => this.V[regIndex],

            0x40 => this.AX,
            0x41 => (u16) (this.AX >> 8),   // High byte of AX
            0x42 => (u16) (this.AX & 0xFF), // Low byte of AX

            0x43 => this.DX,
            0x44 => (u16) (this.DX >> 8),   // High byte of DX
            0x45 => (u16) (this.DX & 0xFF), // Low byte of DX

            0x46 => this.LX,
            0x50 => this.PC,
            0x51 => this.SP,

            _ => throw new InvalidOperationException($"Unknown register index: {regIndex:X2}"),
        };
    }

    private void StoreRegister(u8 regIndex, u16 value)
    {
        switch (regIndex)
        {
            case <= 0x3F:
                this.V[regIndex] = value;
                break;

            case 0x40:
                this.AX = value; break;
            case 0x41:
                this.AX = (u16) ((this.AX & 0x00FF) | (value << 8)); break; // Set high byte
            case 0x42:
                this.AX = (u16) ((this.AX & 0xFF00) | (value & 0x00FF)); break; // Set low byte

            case 0x43:
                this.DX = value; break;
            case 0x44:
                this.DX = (u16) ((this.DX & 0x00FF) | (value << 8)); break; // Set high byte
            case 0x45:
                this.DX = (u16) ((this.DX & 0xFF00) | (value & 0x00FF)); break; // Set low byte

            case 0x46:
                this.LX = value; break;
            case 0x50:
                this.PC = value; break;
            case 0x51:
                this.SP = value; break;

            default:
                throw new InvalidOperationException($"Unknown register index: {regIndex:X2}");
        }
    }
}