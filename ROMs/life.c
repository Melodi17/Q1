#include "std.c"
#include "hid.c"
#include "screen.c"

#define GRID_SIZE   64
#define GRID_BYTES  8
#define GRID_WIDTH  8
#define GRID_HEIGHT 8

char getState(char* grid, int index)
{
    int byteIndex = index / 8;
    int bitIndex  = index % 8;
    int byte      = grid[byteIndex];

    return ((byte >> bitIndex) & 1) != 0;
}

int setState(char* grid, int index, int state)
{
    int byteIndex = index / 8;
    int bitIndex  = index % 8;
    int byte      = grid[byteIndex];
    
    if (state)
        byte |= (1 << bitIndex);
    else
        byte &= ~(1 << bitIndex);
    
    grid[byteIndex] = byte;
}

int drawGrid(char* grid)
{
    screen_clear(COLOR_BLACK);

    for (int y = 0; y < GRID_HEIGHT; y++)
    {
        for (int x = 0; x < GRID_WIDTH; x++)
        {
            int index = (y * GRID_WIDTH) + x;
            if (getState(grid, index))
                screen_setPixel(x, y, COLOR_WHITE);
        }
    }
}

int main()
{
    char* grid = new char[GRID_BYTES];

    // clear memory
    for (int i = 0; i < GRID_BYTES; i++)
        grid[i] = 0;

    // turn on pixel at (0,0)
    setState(grid, 0, 1);

    drawGrid(grid);

    while (1) { }
}