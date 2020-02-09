using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input.Touch;
using System.Diagnostics;


namespace FortDefenders
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class InputManager : Microsoft.Xna.Framework.GameComponent
    {
        private GestureSample gestureSample;
        private Vector2 deltaPos;
        private Vector2 tapPosition;



        public InputManager(Game game)
            : base(game)
        {
           
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            TouchPanel.EnabledGestures = GestureType.FreeDrag | GestureType.Tap;

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            while (TouchPanel.IsGestureAvailable)
            {
                this.gestureSample = TouchPanel.ReadGesture();

                switch (this.gestureSample.GestureType)
                {
                    case GestureType.FreeDrag:
                        this.deltaPos = this.gestureSample.Delta;
                        break;
                    case GestureType.Tap:
                        this.tapPosition = this.gestureSample.Position;
                        break;
                    default:
                        break;
                }
            }

            base.Update(gameTime);
        }

        public Vector2 GetDeltaPosition()
        {
            return this.deltaPos;
        }

        public Vector2 GetTapPosition()
        {
            return this.tapPosition;
        }

        public void ResetDelta(Single amount)
        {
            this.deltaPos = Vector2.Lerp(this.deltaPos, Vector2.Zero, amount);
        }

        public void SetTapPosition(Vector2 tapPosition)
        {
            this.tapPosition = tapPosition;
        }
    }
}
