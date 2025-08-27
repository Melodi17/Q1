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

int main() 
{
    while (1)
    {
        int* hid = 0x602F;
        int mouseIndex = hid[0];
        int mouseState = hid[1];
        if (mouseState & 1 == 1)
        setPixelIndex(mouseIndex, 1);
    }
}