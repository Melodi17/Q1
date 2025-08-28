namespace Q1.Emulator;

using System;
using System.Linq;
using Chip;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class DisplayEngine : Game
{
    private readonly HIDDevice _hid;
    private readonly Action<Texture2D> _onScreenTextureLoaded;
    private readonly Action _requestUpdate;
    private readonly Func<bool> _checkAbort;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Texture2D _texture;
    private Keys[]? _oldKeys = null;
    public int Scaling { get; }
    public int Width { get; }
    public int Height { get; }

    public DisplayEngine(
        int width,
        int height,
        int scaling,
        HIDDevice hid,
        Action<Texture2D> onScreenTextureLoaded,
        Action requestUpdate,
        Func<bool> checkAbort)
    {
        this.Width = width;
        this.Height = height;
        this.Scaling = scaling;
        this._hid = hid;
        this._onScreenTextureLoaded = onScreenTextureLoaded;
        this._requestUpdate = requestUpdate;
        this._checkAbort = checkAbort;

        this._graphics = new GraphicsDeviceManager(this);
        this.Content.RootDirectory = "Content";
        this.IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        this._texture = new Texture2D(this.GraphicsDevice, this.Width, this.Height);
        this._onScreenTextureLoaded?.Invoke(this._texture);
        // set window size
        this._graphics.PreferredBackBufferWidth = this.Width   * this.Scaling;
        this._graphics.PreferredBackBufferHeight = this.Height * this.Scaling;
        this._graphics.ApplyChanges();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        this._spriteBatch = new SpriteBatch(this.GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();
        var mouse = Mouse.GetState();

        u16 mousePos = (u16) (((mouse.Y / this.Scaling) * this.Width) + (mouse.X / this.Scaling));
        this._hid.SetMouse(
            mousePos,
            mouse.LeftButton   == ButtonState.Pressed,
            mouse.RightButton  == ButtonState.Pressed,
            mouse.MiddleButton == ButtonState.Pressed);

        Keys[] keys = keyboard.GetPressedKeys();
        foreach (Keys newKey in keys.Except(this._oldKeys ?? []))
        {
            this._hid.SetKeyboard((u8) newKey, true);
            Console.WriteLine((u8) newKey);
        }
        
        foreach (Keys oldKey in this._oldKeys?.Except(keys) ?? [])
        {
            this._hid.SetKeyboard((u8) oldKey, false);
        }

        this._oldKeys = keys;

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
            || keyboard.IsKeyDown(Keys.Escape)
            || this._checkAbort())
            this.Exit();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // GraphicsDevice.Clear(Color.CornflowerBlue);

        this._requestUpdate();

        this._spriteBatch.Begin(
            SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp,
            DepthStencilState.None, RasterizerState.CullNone);
        // Draw the texture to the screen
        this._spriteBatch.Draw(
            this._texture,
            new Rectangle(0, 0, this.Width * this.Scaling, this.Height * this.Scaling),
            Color.White);

        this._spriteBatch.End();

        base.Draw(gameTime);
    }
}