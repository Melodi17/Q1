namespace Q1.Emulator.Chip;

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class DisplayDevice : RamDevice
{
    public Texture2D? OutputTexture;
    
    private readonly u16 _paletteSize;
    private readonly u16 _displayWidth;
    private readonly u16 _displayHeight;
    
    private Color[] textureData;
    private Color[] palette;
    private bool _paletteDirty;

    public DisplayDevice(u16 paletteSize, u16 displayWidth, u16 displayHeight, u16 addressableStart)
        : base(
            addressableStart, DisplayDevice.DetermineSize(paletteSize, displayWidth, displayHeight))
    {
        this._paletteSize = paletteSize;
        this._displayWidth = displayWidth;
        this._displayHeight = displayHeight;
        this.textureData = new Color[this._displayWidth * this._displayHeight];
    }

    public static u16 DetermineSize(u16 paletteSize, u16 displayWidth, u16 displayHeight)
    {
        return (u16) ((paletteSize * 3) + (displayWidth * displayHeight));
    }

    public override void Clock()
    {
        if (this._paletteDirty)
            this.ReadPalette();
        
        ref8 pixelData = this.Memory.AsSpan(
            (u16) (this._paletteSize * 3), (u16) (this._displayWidth * this._displayHeight));
        
        for (u16 i = 0; i < this.textureData.Length; i++)
        {
            u8 pixelValue = pixelData[i];
            this.textureData[i] = palette[pixelValue % this._paletteSize];
        }

        if (this.OutputTexture != null)
            this.OutputTexture.SetData(this.textureData);
    }

    private void ReadPalette()
    {
        ref8 paletteData = this.Memory.AsSpan(0, (u16) (this._paletteSize * 3));
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

        this._paletteDirty = false;
        this.palette = colors;
    }

    public void SetPalette(Color[] palette)
    {
        if (palette.Length != this._paletteSize)
            throw new ArgumentException($"Palette must have {this._paletteSize} colors.");

        ref8 paletteData = this.Memory.AsSpan(0, (u16) (this._paletteSize * 3));
        for (u16 i = 0; i < this._paletteSize; i++)
        {
            u16 offset = (u16) (i * 3);
            paletteData[offset] = palette[i].R;
            paletteData[offset + 1] = palette[i].G;
            paletteData[offset + 2] = palette[i].B;
        }

        this._paletteDirty = true;
    }

    public override u8 Read(u16 address) => base.Read(address);
    public override void Write(u16 address, u8 value)
    {
        this._paletteDirty = true;
        base.Write(address, value);
    }
}