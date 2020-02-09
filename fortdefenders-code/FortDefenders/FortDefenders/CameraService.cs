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
using System.Diagnostics;


namespace FortDefenders
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class CameraService : Microsoft.Xna.Framework.GameComponent
    {
        private const Single timeLimit = 1.5f;
        private const Single DEFAULT_HEIGHT = 40.0f;
        private const Single DEFAULT_SCALE = 20.0f;

        private Matrix World;
        private Matrix Projection;
        private Matrix View;
        private Vector3 observationPoint;
        private Vector3 cameraPos;
        private InputManager inputMng;
        private Vector2 cursorDeltaPosition;
        private Single timeToMove;

        public CameraService(Game game)
            : base(game)
        {
            this.inputMng = game.GetService<InputManager>();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            this.observationPoint = Vector3.Zero;
            this.cameraPos = Vector3.Transform(new Vector3(0, DEFAULT_HEIGHT, 0), Matrix.CreateRotationX(MathHelper.ToRadians(30.0f)));
            this.World = Matrix.CreateTranslation(this.observationPoint);
            this.View = Matrix.CreateLookAt(this.cameraPos, this.observationPoint, Vector3.Up);
            this.Projection = Matrix.CreateOrthographic(this.Game.GraphicsDevice.Viewport.Width, this.Game.GraphicsDevice.Viewport.Height, 0.1f, 1000.0f) * Matrix.CreateScale(DEFAULT_SCALE);
            this.timeToMove = timeLimit;

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            this.MoveCamera(gameTime);

            base.Update(gameTime);
        }

        private void MoveCamera(GameTime gameTime)
        {
            this.cursorDeltaPosition = this.inputMng.GetDeltaPosition();

            if (this.cursorDeltaPosition != Vector2.Zero)
            {
                Vector2 Pos = this.inputMng.GetDeltaPosition();
                Vector3 result = new Vector3(this.cursorDeltaPosition.X, 0, this.cursorDeltaPosition.Y);

                Single amount = timeToMove / timeLimit;
                this.inputMng.ResetDelta(amount);
                timeToMove -= (gameTime.ElapsedGameTime.Milliseconds / 1000.0f);

                if (this.timeToMove <= 0)
                {
                    this.timeToMove = timeLimit;
                }

                this.cameraPos -= result * gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
                this.observationPoint -= result * gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
                this.View = Matrix.CreateLookAt(this.cameraPos, this.observationPoint, Vector3.Up);
            }
        }

        public void GetMatrices(out Matrix world, out Matrix view, out Matrix projection)
        {
            world = this.World;
            view = this.View;
            projection = this.Projection;
        }

        public void SetCurrentZoom(Single coef)
        {
            this.Projection = Matrix.CreateOrthographic(this.Game.GraphicsDevice.Viewport.Width, this.Game.GraphicsDevice.Viewport.Height, 0.1f, 1000.0f * coef) * Matrix.CreateScale(DEFAULT_SCALE * coef);
        }
    }
}
