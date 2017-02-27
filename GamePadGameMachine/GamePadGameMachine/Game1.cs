using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace GamePadGameMachine
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Dictionary<string, bool> ActiveKeys = new Dictionary<string, bool>();
        private Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;
            Window.AllowUserResizing = false;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            var controller = Content.Load<Texture2D>("Images/Controller");
            var lb = Content.Load<Texture2D>("Images/LB");
            var lt = Content.Load<Texture2D>("Images/LT");
            var rb = Content.Load<Texture2D>("Images/RB");
            var rt = Content.Load<Texture2D>("Images/RT");

            var a = Content.Load<Texture2D>("Images/AColor");
            var b = Content.Load<Texture2D>("Images/BColor");
            var y = Content.Load<Texture2D>("Images/YColor");
            var x = Content.Load<Texture2D>("Images/XColor");

            var start = Content.Load<Texture2D>("Images/StartColor");
            var back = Content.Load<Texture2D>("Images/BackColor");

            var dpadleft = Content.Load<Texture2D>("Images/DPadLeftColor");
            var dpadright = Content.Load<Texture2D>("Images/DPadRightColor");
            var dpadup = Content.Load<Texture2D>("Images/DPadUpColor");
            var dpaddown = Content.Load<Texture2D>("Images/DPadDownColor");

            var stickdirection = Content.Load<Texture2D>("Images/StickDirectionIndicator");
            var leftStickClick = Content.Load<Texture2D>("Images/LeftStickClick");
            var rightStickClick = Content.Load<Texture2D>("Images/RightStickClick");

            Textures.Add("Controller", controller);
            Textures.Add("LB", lb);
            Textures.Add("LT", lt);
            Textures.Add("RB", rb);
            Textures.Add("RT", rt);

            Textures.Add("A", a);
            Textures.Add("B", b);
            Textures.Add("Y", y);
            Textures.Add("X", x);

            Textures.Add("Start", start);
            Textures.Add("Back", back);

            Textures.Add("DPadLeft", dpadleft);
            Textures.Add("DPadRight", dpadright);
            Textures.Add("DPadUp", dpadup);
            Textures.Add("DPadDown", dpaddown);

            Textures.Add("StickIndicator", stickdirection);
            Textures.Add("LeftStickClick", leftStickClick);
            Textures.Add("RightStickClick", rightStickClick);
        }

        protected override void UnloadContent()
        {
            Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
#if DEBUG
            if(Keyboard.GetState().IsKeyDown(Keys.LeftControl))
            {
                Debug.Print(GetMousePosition().ToString());
            }
#endif
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(Textures["Controller"], Vector2.Zero);

            var gamepadState = GamePad.GetState(PlayerIndex.One);
            SynchronousSocketClient.wait.Set();
            DrawState(ref gamepadState);

            spriteBatch.End();

            base.Draw(gameTime);

            base.Draw(gameTime);
        }

        private void DrawState(ref GamePadState gamepadState)
        {
            var sendState = false;
            // Left & Right Bumpers - Triggers
            if(gamepadState.Buttons.LeftShoulder == ButtonState.Pressed)
            {
                sendState = true;
                spriteBatch.Draw(Textures["LB"], new Vector2(100, 97));
            }
            if(gamepadState.Triggers.Left == 1)
            {
                sendState = true;
                spriteBatch.Draw(Textures["LT"], new Vector2(160, 41));
            }
            if(gamepadState.Buttons.RightShoulder == ButtonState.Pressed)
            {
                sendState = true;
                spriteBatch.Draw(Textures["RB"], new Vector2(514, 97));
            }
            if(gamepadState.Triggers.Right == 1)
            {
                sendState = true;
                spriteBatch.Draw(Textures["RT"], new Vector2(535, 41));
            }

            // Key Buttons
            if(gamepadState.Buttons.A == ButtonState.Pressed)
            {
                sendState = true;
                spriteBatch.Draw(Textures["A"], new Vector2(547, 269));
            }

            if(gamepadState.Buttons.B == ButtonState.Pressed)
            {
                sendState = true;
                spriteBatch.Draw(Textures["B"], new Vector2(606, 221));
            }

            if(gamepadState.Buttons.Y == ButtonState.Pressed)
            {
                sendState = true;
                spriteBatch.Draw(Textures["Y"], new Vector2(553, 177));
            }

            if(gamepadState.Buttons.X == ButtonState.Pressed)
            {
                sendState = true;
                spriteBatch.Draw(Textures["X"], new Vector2(497, 226));
            }

            // Sticks
            if(gamepadState.Buttons.LeftStick == ButtonState.Pressed)
            {
                sendState = true;
                spriteBatch.Draw(Textures["LeftStickClick"], new Vector2(119, 233));
            }

            if(gamepadState.Buttons.RightStick == ButtonState.Pressed)
            {
                sendState = true;
                spriteBatch.Draw(Textures["LeftStickClick"], new Vector2(423, 335));
            }

            if(gamepadState.ThumbSticks.Left.X != 0 || gamepadState.ThumbSticks.Left.Y != 0)
            {
                sendState = true;
                var xStart = 162;
                var yStart = 270;
                var point1 = new Vector2(xStart, yStart);
                var point2 = GetStickDirection(ref gamepadState, xStart, yStart, true);
                DrawLine(point1, point2);
            }

            if(gamepadState.ThumbSticks.Right.X != 0 || gamepadState.ThumbSticks.Right.Y != 0)
            {
                sendState = true;
                var xStart = 467;
                var yStart = 373;
                var point1 = new Vector2(xStart, yStart);
                var point2 = GetStickDirection(ref gamepadState, xStart, yStart, false);
                DrawLine(point1, point2);
            }

            // D-Pad
            if(gamepadState.DPad.Left == ButtonState.Pressed)
            {
                sendState = true;
                spriteBatch.Draw(Textures["DPadLeft"], new Vector2(215, 336));
            }

            if(gamepadState.DPad.Down == ButtonState.Pressed)
            {
                sendState = true;
                spriteBatch.Draw(Textures["DPadDown"], new Vector2(241, 369));
            }

            if(gamepadState.DPad.Right == ButtonState.Pressed)
            {
                sendState = true;
                spriteBatch.Draw(Textures["DPadRight"], new Vector2(287, 340));
            }

            if(gamepadState.DPad.Up == ButtonState.Pressed)
            {
                sendState = true;
                spriteBatch.Draw(Textures["DPadUp"], new Vector2(243, 315));
            }

            // Menus
            if(gamepadState.Buttons.Start == ButtonState.Pressed)
            {
                sendState = true;
                spriteBatch.Draw(Textures["Start"], new Vector2(428, 237));
            }

            if(gamepadState.Buttons.Back == ButtonState.Pressed)
            {
                sendState = true;
                spriteBatch.Draw(Textures["Back"], new Vector2(279, 236));
            }
            if(sendState && SynchronousSocketClient.delayState.ElapsedMilliseconds > Program.config.ClientUpdateRate)
            {
                sendState = true;
                SynchronousSocketClient.SendQueue.Enqueue(JsonConvert.SerializeObject(new
                {
                    Buttons = gamepadState.Buttons,
                    Triggers = gamepadState.Triggers,
                    DPad = gamepadState.DPad,
                    leftstickX = gamepadState.ThumbSticks.Left.X,
                    leftstickY = gamepadState.ThumbSticks.Left.Y,
                    rightstickX = gamepadState.ThumbSticks.Right.X,
                    rightstickY = gamepadState.ThumbSticks.Right.Y
                }));
                SynchronousSocketClient.delayState.Restart();
            }
        }

        private Vector2 GetStickDirection(ref GamePadState gamepadState, int xStart, int yStart, bool isLeftStick)
        {
            float x = 0;
            float y = 0;

            if(isLeftStick)
            {
                x = ((gamepadState.ThumbSticks.Left.X * 20f));
                y = ((gamepadState.ThumbSticks.Left.Y * 20f));
            }
            else
            {
                x = ((gamepadState.ThumbSticks.Right.X * 20f));
                y = ((gamepadState.ThumbSticks.Right.Y * 20f));
            }

            var point2 = new Vector2(xStart + x, yStart - y);
            return point2;
        }

        private void DrawLine(Vector2 start, Vector2 end)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle = (float)Math.Atan2(edge.Y, edge.X);


            spriteBatch.Draw(Textures["StickIndicator"],
                new Rectangle(// rectangle defines shape of line and position of start of line
                    (int)start.X,
                    (int)start.Y,
                    (int)edge.Length(), //sb will strech the texture to fill this rectangle
                    3), //width of line, change this to make thicker line
                null,
                Color.WhiteSmoke, //colour of line
                angle,     //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                0);

        }

        private Point GetMousePosition()
        {
            var mouseState = Mouse.GetState(this.Window);
            return new Point(mouseState.X, mouseState.Y);
        }
    }
}