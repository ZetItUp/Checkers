using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.GameApp.Helpers
{
    public class MouseHelper : GameComponent
    {
        public enum MouseButton
        {
            Left,
            Middle,
            Right
        }

        static MouseState mouseState;
        static MouseState lastMouseState;

        static Rectangle mouseRect;

        public MouseHelper(Game game)
            : base(game)
        {
            mouseState = Mouse.GetState();
        }

        public override void Update(GameTime gameTime)
        {
            lastMouseState = mouseState;
            mouseState = Mouse.GetState();

            mouseRect = new Rectangle(mouseState.X, mouseState.Y, 1, 1);

            base.Update(gameTime);
        }

        public static float MouseScrollWheelValue()
        {
            return mouseState.ScrollWheelValue;
        }

        public static float LastMouseScrollWheelValue()
        {
            return lastMouseState.ScrollWheelValue;
        }

        public static Rectangle MouseRectangle()
        {
            return mouseRect;
        }

        public static Vector2 MousePosition()
        {
            return new Vector2(mouseRect.X, mouseRect.Y);
        }

        public static bool MouseReleased(MouseButton mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButton.Left:
                    {
                        if (mouseState.LeftButton == ButtonState.Released && lastMouseState.LeftButton == ButtonState.Pressed)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                case MouseButton.Middle:
                    {
                        if (mouseState.MiddleButton == ButtonState.Released && lastMouseState.MiddleButton == ButtonState.Pressed)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                case MouseButton.Right:
                    {
                        if (mouseState.RightButton == ButtonState.Released && lastMouseState.RightButton == ButtonState.Pressed)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        public static bool MousePressed(MouseButton mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButton.Left:
                    {
                        if (mouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                case MouseButton.Middle:
                    {
                        if (mouseState.MiddleButton == ButtonState.Pressed && lastMouseState.MiddleButton == ButtonState.Released)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                case MouseButton.Right:
                    {
                        if (mouseState.RightButton == ButtonState.Pressed && lastMouseState.RightButton == ButtonState.Released)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        public static bool MouseDown(MouseButton mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButton.Left:
                    {
                        if (mouseState.LeftButton == ButtonState.Pressed)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                case MouseButton.Middle:
                    if (mouseState.MiddleButton == ButtonState.Pressed)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case MouseButton.Right:
                    if (mouseState.RightButton == ButtonState.Pressed)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                default:
                    {
                        return false;
                    }
            }
        }
    }
}
