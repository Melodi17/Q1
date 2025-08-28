int setPixel(int x, int y, int c)
{
    char* screenStart = 0x402F;
    int i = (y * 128) + x;
    screenStart[i] = c;
}

int setPixelIndex(int i, int c)
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

int main()
{
	int color = 0;
    while (1)
    {
        int* hid = 0x602F;
        int mouseIndex = hid[0];
        int mouseState = hid[1];

        if (mouseState & 1)
            setPixelIndex(mouseIndex, (color % 3) + 1);

        if (getKeyState(67))
            clear(8);

 		if (getKeyState(65))
		{
            color++;
			while (getKeyState(65)) { }
		}
    }
}