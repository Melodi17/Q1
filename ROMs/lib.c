#define SK_MAX 256
#define SK_NL 13
#define SK_BS 8
#define SK_SHIFT 160
#define SK_A 65
#define SK_B 66
#define SK_C 67
#define SK_D 68
#define SK_E 69

#define MS_LEFT 1
#define MS_RIGHT 2
#define MS_MIDDLE 4

#define SCREEN_WIDTH 128
#define SCREEN_HEIGHT 64
#define SCREEN_MAX 128 * 64

#define true 1
#define false 0

#define COLOR_BLACK 8
#define COLOR_RED 1
#define COLOR_GREEN 2
#define COLOR_BLUE 3
#define COLOR_TRANSPARENT 0

int setPixel(int x, int y, int c)
{
    char* screenStart = 0x402F;
    int i = (y * SCREEN_WIDTH) + x;
    screenStart[i] = c;
}

int setPixel(int i, int c)
{
    char* screenStart = 0x402F;
    screenStart[i] = c;
}

int clear(int c)
{
    char* screenStart = 0x402F;
    for (int i = 0; i < SCREEN_MAX; i++)
    {
        screenStart[i] = c;
    }
}

int getKeyState(int scanCode)
{
    char* hid = 0x6033;
    int index = scanCode / 8;
    int bit   = scanCode % 8;

    int val = hid[index - 1];
    return (val & (1 << bit)) != 0;
}

int getPressedKey()
{
    for (int sk = 0; sk < SK_MAX; sk++)
    {
        if (getKeyState(sk) && sk != SK_SHIFT)
            return sk;
    }
    return 0;
}

int readKey()
{
    while (true)
    {
        int sk = getPressedKey();
        if (sk != 0)
            return sk;
    }
}

int getMouseIndex()
{
    return *(0x602F);
}

int getMouseState()
{
    return *(0x602F + 2);
}