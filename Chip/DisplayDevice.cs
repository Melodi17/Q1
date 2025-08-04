namespace Q1Emu.Chip;

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class DisplayDevice : RamDevice
{
    public           Texture2D? OutputTexture;
    private readonly u16        _paletteSize;
    private readonly u16        _displayWidth;
    private readonly u16        _displayHeight;
    private          Color[]    textureData;

    public DisplayDevice(u16 paletteSize, u16 displayWidth, u16 displayHeight, u16 addressableStart)
        : base(addressableStart, DisplayDevice.DetermineSize(paletteSize, displayWidth, displayHeight))
    {
        this._paletteSize   = paletteSize;
        this._displayWidth  = displayWidth;
        this._displayHeight = displayHeight;
        this.textureData    = new Color[this._displayWidth * this._displayHeight];
    }

    private static u16 DetermineSize(u16 paletteSize, u16 displayWidth, u16 displayHeight)
    {
        return (u16) ((paletteSize * 3) + (displayWidth * displayHeight));
    }

    public override void Clock()
    {
        Color[] palette = this.ReadPalette();
        ref8 pixelData = this.ReadSeq((u16) (this.AddressableStart + this._paletteSize * 3), (u16) (this._displayWidth * this._displayHeight));

        for (u16 i = 0; i < textureData.Length; i++)
        {
            u8 pixelValue = pixelData[i];
            textureData[i] = palette[pixelValue];
        }

        if (this.OutputTexture != null)
            this.OutputTexture.SetData(textureData);
    }

    private Color[] ReadPalette()
    {
        ref8 paletteData = this.ReadSeq(this.AddressableStart, (u16) (this._paletteSize * 3));
        Color[] colors = new Color[this._paletteSize + 1];
        colors[0] = Color.Transparent;

        for (u16 i = 0; i < this._paletteSize; i++)
        {
            u16 offset = (u16) (i * 3);
            byte r = paletteData[offset];
            byte g = paletteData[offset + 1];
            byte b = paletteData[offset + 2];
            colors[(u16) (i             + 1)] = new Color(r, g, b);
        }

        return colors;
    }

    public void SetPalette(Color[] palette)
    {
        if (palette.Length != this._paletteSize)
            throw new ArgumentException($"Palette must have {this._paletteSize} colors.");

        ref8 paletteData = this.ReadSeq(this.AddressableStart, (u16) (this._paletteSize * 3));
        for (u16 i = 0; i < this._paletteSize; i++)
        {
            u16 offset = (u16) (i * 3);
            paletteData[offset]     = palette[i].R;
            paletteData[offset + 1] = palette[i].G;
            paletteData[offset + 2] = palette[i].B;
        }
    }

    public override void Write(u16 address, u8 value)
    {
        Console.WriteLine($"DisplayDevice.Write: {address:X4} = {value:X2}");
        base.Write(address, value);
    }
    public override void WriteSeq(u16 address, u16 length, ref8 value)
    {
        Console.WriteLine($"DisplayDevice.WriteSeq: {address:X4} length={length} value={Make.u16(value)}");
        base.WriteSeq(address, length, value);
    }
}