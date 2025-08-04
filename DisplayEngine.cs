using System;
using System.Collections.Generic;
using Q1Emu.Chip;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Q1Emu;

public class DisplayEngine : Game
{
    private readonly Action<Texture2D>     _onScreenTextureLoaded;
    private readonly Action                _requestUpdate;
    private          GraphicsDeviceManager _graphics;
    private          SpriteBatch           _spriteBatch;

    private Texture2D _texture;
    public int Scaling { get; }
    public int Width { get; }
    public int Height { get; }

    public DisplayEngine(int width, int height, int scaling, Action<Texture2D> onScreenTextureLoaded, Action requestUpdate)
    {
        this.Width                  = width;
        this.Height                 = height;
        this.Scaling                = scaling;
        this._onScreenTextureLoaded = onScreenTextureLoaded;
        this._requestUpdate         = requestUpdate;

        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        this._texture = new Texture2D(GraphicsDevice, Width, Height);
        this._onScreenTextureLoaded?.Invoke(this._texture);
        // set window size
        _graphics.PreferredBackBufferWidth = Width * Scaling;
        _graphics.PreferredBackBufferHeight = Height * Scaling;
        _graphics.ApplyChanges();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        var state = Keyboard.GetState();

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || state.IsKeyDown(Keys.Escape))
            Exit();
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        this._requestUpdate();

        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
        // Draw the texture to the screen
        _spriteBatch.Draw(_texture, new Rectangle(0, 0, Width * Scaling, Height * Scaling), Color.White);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}