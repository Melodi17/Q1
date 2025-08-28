#load "font.bin"

int drawChar(int ch, int x, int y, int color)
{
    // adjust for jmp instruction
    char* fontStart = 0x1FFF + 2;
    int charStart = ch * 8;

    for (int yOff = 0; yOff < 8; yOff++)
    {
        int row = fontStart[charStart + yOff];

        for (int xOff = 0; xOff < 8; xOff++)
        {
            int bit = (row >> (7 - xOff)) & 1;
            int pix = bit ? 0 : color;
            setPixel(x + xOff, y + yOff, pix);
        }
    }
}

int setPixel(int x, int y, int c)
{
    char* screenStart = 0x402F;
    int i = (y * 128) + x;
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
    int color = 1;
    int* text = "HELLO WORLD!";
    int len = 12;

    clear(0);
    while (1)
    {
        for (int i = 0; i < len; i++)
        {
            int ch = text[i];
            drawChar(ch, i * 8, 0, color);
        }
        color++;
    }
}