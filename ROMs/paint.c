#include "std.c"
#include "hid.c"
#include "screen.c"

int main()
{
	int color = 0;
    while (true)
    {
      	int mouseIndex = hid_getMouseIndex();
		int mouseState = hid_getMouseState();

        if (mouseState & MS_LEFT)
            screen_setPixel(mouseIndex, (color % 3) + 1);

        if (hid_getKeyState(SK_C))
            screen_clear(COLOR_BLACK);

 		if (hid_getKeyState(SK_A))
		{
            color++;
			while (hid_getKeyState(SK_A)) { }
		}
    }
}