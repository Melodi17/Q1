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
            int pix = bit ? 8 : color;
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

int getKeyState(int scanCode)
{
    char* hid = 0x6033;
    int index = scanCode / 8;
    int bit   = scanCode % 8;

    int val = hid[index - 1];
    return (val & (1 << bit)) != 0;
}

int print(int* text, int len, int x, int y, int color)
{
    for (int i = 0; i < len; i++)
    {
        int ch = text[i];
        drawChar(ch, x + (i * 8), y, color);
    }
}

int main() 
{
    int x = 0;
    int y = 0;
    while (1)
    {
        for (int sk = 0; sk < 256; sk++)
        {
            if (getKeyState(sk))
            {
                while (getKeyState(sk)) { }
                // Newline pressed
                if (sk == 13)
                {
                    y += 8;
                    x = 0;
                    continue;
                }
                // Backspace pressed
                if (sk == 8)
                {
                    x -= 8;
                    if (x < 0)
                        x = 0;
                    int* text = " ";
                    print(text, 1, x, y, 1);
                    continue;
                }
                
                int* text = new int[] { sk };
                print(text, 1, x, y, 1);
                x += 8;
            }
        }

        if (x >= 128)
        {
            x = 0;
            y += 8;
        }

        if (y >= 64)
        {
            x = 0;
            y = 0;
            clear(8);
        }
        
    }
}