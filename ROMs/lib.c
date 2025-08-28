int setPixel(int x, int y, int c)
{
    char* screenStart = 0x402F;
    int i = (y * 128) + x;
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
    char screenSize = 64 * 128;
    for (int i = 0; i < screenSize; i++)
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

int getMouseIndex()
{
    return *(0x602F);
}

int getMouseState()
{
    return *(0x602F + 2);
}