using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Checkers.GameApp.Helpers
{
    ///<summary>
    /// MouseHelper komponent för att hantera musen i MonoGame
    /// </summary>
    public class MouseHelper : GameComponent
    {
        // Statiska variabler som ska vara samma för alla instancer
        private static MouseState mouseState;
        private static MouseState lastMouseState;

        // Rectangle för musens position och hur många pixlar den ska räkna som interaktion
        private static Rectangle mouseRect;

        public MouseHelper(Game game)
            : base(game)
        {
            mouseState = Mouse.GetState();
        }

        public override void Update(GameTime gameTime)
        {
            // Uppdatera musens current och last state
            lastMouseState = mouseState;
            mouseState = Mouse.GetState();

            // Uppdatera musens rectangle
            mouseRect = new Rectangle(mouseState.X, mouseState.Y, 1, 1);

            base.Update(gameTime);
        }

        /// <summary>
        /// Wrapper för ScrollWheelValue
        /// </summary>
        /// <returns>Current ScrollWheelValue</returns>
        public static float MouseScrollWheelValue()
        {
            return mouseState.ScrollWheelValue;
        }

        /// <summary>
        /// Wrapper för LastScrollWheelValue
        /// </summary>
        /// <returns>Last ScrollWheelValue</returns>
        public static float LastMouseScrollWheelValue()
        {
            return lastMouseState.ScrollWheelValue;
        }

        /// <summary>
        /// Hämta MouseRectangle
        /// </summary>
        /// <returns>Rectangle Mouse</returns>
        public static Rectangle MouseRectangle()
        {
            return mouseRect;
        }

        /// <summary>
        /// Hämta Musens Position
        /// </summary>
        /// <returns>Vector2 Mouse Position</returns>
        public static Vector2 MousePosition()
        {
            return new Vector2(mouseRect.X, mouseRect.Y);
        }

        /// <summary>
        /// Kolla om musknappen släpptes
        /// </summary>
        /// <param name="mouseButton">MouseButton</param>
        /// <returns>True if button was released</returns>
        public static bool MouseReleased(MouseButton mouseButton)
        {
            // Kolla vilken knapp som ska hanteras
            switch (mouseButton)
            {
                // I alla cases så jämförs current state mot last state, har det ändrats korrekt, returnera true, annars false
                case MouseButton.Left:
                {
                    if (
                        mouseState.LeftButton == ButtonState.Released
                        && lastMouseState.LeftButton == ButtonState.Pressed
                    )
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
                    if (
                        mouseState.MiddleButton == ButtonState.Released
                        && lastMouseState.MiddleButton == ButtonState.Pressed
                    )
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
                    if (
                        mouseState.RightButton == ButtonState.Released
                        && lastMouseState.RightButton == ButtonState.Pressed
                    )
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

        /// <summary>
        /// Kolla om en musknapp trycktes ner
        /// </summary>
        /// <param name="mouseButton">MouseButton</param>
        /// <returns>True if button is down</returns>
        public static bool MousePressed(MouseButton mouseButton)
        {
            // Kolla vilken knapp som ska hanteras
            switch (mouseButton)
            {
                // I alla cases så jämförs current state mot last state, har det ändrats korrekt, returnera true, annars false
                case MouseButton.Left:
                {
                    if (
                        mouseState.LeftButton == ButtonState.Pressed
                        && lastMouseState.LeftButton == ButtonState.Released
                    )
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
                    if (
                        mouseState.MiddleButton == ButtonState.Pressed
                        && lastMouseState.MiddleButton == ButtonState.Released
                    )
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
                    if (
                        mouseState.RightButton == ButtonState.Pressed
                        && lastMouseState.RightButton == ButtonState.Released
                    )
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

        /// <summary>
        /// Hålls en musknapp nedtryckt?
        /// </summary>
        /// <param name="mouseButton">MouseButton</param>
        /// <returns>True if button is down</returns>
        public static bool MouseDown(MouseButton mouseButton)
        {
            // Kolla vilken knapp som ska hanteras
            switch (mouseButton)
            {
                // I alla cases så kollas det om knappen hålls nedtryckt
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
