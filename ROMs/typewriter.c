#load "font.bin"
#include "lib.c"

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
    while (true)
    {
        if (x >= SCREEN_WIDTH)
        {
            x = 0;
            y += 8;
        }

        if (y >= SCREEN_HEIGHT)
        {
            x = 0;
            y = 0;
            clear(8);
        }

        int sk = readKey();
        while (getKeyState(sk)) { }

        if (sk == SK_NL)
        {
            y += 8;
            x = 0;
            continue;
        }

        if (sk == SK_BS)
        {
            x -= 8;
            if (x < 0)
                x = 0;
            drawChar(' ', x, y, 1);
            continue;
        }
        
        drawChar(sk, x, y, 1);
        x += 8;
    }
}