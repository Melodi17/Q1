#include "std.c"
#include "screen.c"

int main()
{
    // Test values
    char a = 0b10101010;   // 170
    char b = 0b11001100;   // 204
    char c;

    // AND
    c = a & b;              // expect 0b10001000 = 136
    *((char*)0x7100) = c;

    // OR
    c = a | b;              // expect 0b11101110 = 238
    *((char*)0x7101) = c;

    // XOR
    c = a ^ b;              // expect 0b01100110 = 102
    *((char*)0x7102) = c;

    // NOT
    c = ~a;                 // expect 0b01010101 = 85
    *((char*)0x7103) = c;

    // SHL
    c = a << 2;             // expect 0b10101000 = 168
    *((char*)0x7104) = c;

    // SHR
    c = a >> 2;             // expect 0b00101010 = 42
    *((char*)0x7105) = c;

    // Program ends cleanly
    return 0;
}