#include "lib.c"

int main()
{
	int color = 0;
    while (true)
    {
      	int mouseIndex = getMouseIndex();
		int mouseState = getMouseState();

        if (mouseState & MS_LEFT)
            setPixel(mouseIndex, (color % 3) + 1);

        if (getKeyState(SK_C))
            clear(COLOR_BLACK);

 		if (getKeyState(SK_A))
		{
            color++;
			while (getKeyState(SK_A)) { }
		}
    }
}