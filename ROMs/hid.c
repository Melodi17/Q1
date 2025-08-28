#include "std.c"

#define BUS_HID_SK_START 0x6033
#define BUS_HID_MS_INDEX 0x602F
#define BUS_HID_MS_STATE 0x602F + 2

#define SK_MAX 256
#define SK_NEWLINE 13
#define SK_BACKSPACE 8
#define SK_SHIFT 160
#define SK_A 65
#define SK_B 66
#define SK_C 67
#define SK_D 68
#define SK_E 69
#define SK_F 70
#define SK_G 71
#define SK_H 72
#define SK_I 73
#define SK_J 74
#define SK_K 75
#define SK_L 76
#define SK_M 77
#define SK_N 78
#define SK_O 79
#define SK_P 80
#define SK_Q 81
#define SK_R 82
#define SK_S 83
#define SK_T 84
#define SK_U 85
#define SK_V 86
#define SK_W 87
#define SK_X 88
#define SK_Y 89
#define SK_Z 90

#define MS_LEFT 1
#define MS_RIGHT 2
#define MS_MIDDLE 4

int hid_getKeyState(int scanCode)
{
    char* sk_table = BUS_HID_SK_START;
    int index = scanCode / 8;
    int bit   = scanCode % 8;

    int val = sk_table[index - 1];
    return (val & (1 << bit)) != 0;
}

int hid_getPressedKey()
{
    for (int sk = 0; sk < SK_MAX; sk++)
    {
        if (hid_getKeyState(sk) && sk != SK_SHIFT)
            return sk;
    }
    return 0;
}

int hid_waitForPressedKey()
{
    while (true)
    {
        int sk = hid_getPressedKey();
        if (sk != 0)
            return sk;
    }
}

int hid_getMouseIndex()
{
    return *(BUS_HID_MS_INDEX);
}

int hid_getMouseState()
{
    return *(BUS_HID_MS_STATE);
}