#include "std.c"

#define SCREEN_WIDTH 128
#define SCREEN_HEIGHT 64
#define SCREEN_MAX 128 * 64

#define COLOR_TRANSPARENT 0
#define COLOR_RED 1
#define COLOR_GREEN 2
#define COLOR_BLUE 3
#define COLOR_YELLOW 4
#define COLOR_CYAN 5
#define COLOR_MAGENTA 6
#define COLOR_WHITE 7
#define COLOR_BLACK 8
#define COLOR_GRAY 9
#define COLOR_ORANGE 10
#define COLOR_PURPLE 11
#define COLOR_BROWN 12
#define COLOR_LIGHTGRAY 13
#define COLOR_DARKGRAY 14
#define COLOR_LIGHTBLUE 15
#define COLOR_LIGHTGREEN 16

#define BUS_SCREEN_PALETTE_START 0x3FFF
#define BUS_SCREEN_PIXELS_START 0x402F

int screen_setPixel(int x, int y, int c)
{
    char* screenStart = BUS_SCREEN_PIXELS_START;
    int i = (y * SCREEN_WIDTH) + x;
    screenStart[i] = c;
}

int screen_setPixel(int i, int c)
{
    char* screenStart = BUS_SCREEN_PIXELS_START;
    screenStart[i] = c;
}

int screen_clear(int c)
{
    char* screenStart = BUS_SCREEN_PIXELS_START;
    for (int i = 0; i < SCREEN_MAX; i++)
    {
        screenStart[i] = c;
    }
}