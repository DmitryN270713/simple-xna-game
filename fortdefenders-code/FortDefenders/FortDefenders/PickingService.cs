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
    public class PickingService : Microsoft.Xna.Framework.GameComponent
    {
        public delegate void CellClicked(Object o, CellArgs e);
        public event CellClicked OnCellClicked;

        private readonly Single forbiddenY;

        private CameraService camera;
        private Matrix world;
        private Matrix view;
        private Matrix projection;
        private Vector2 tapPosition;
        private Vector2 oldTapPosition;
        private InputManager inputMng;
        private Matrix terrainWorld;

        public PickingService(Game game)
            : base(game)
        {
            this.camera = game.GetService<CameraService>();
            this.inputMng = game.GetService<InputManager>();
            this.EnabledChanged += new EventHandler<EventArgs>(PickingService_EnabledChanged);
            this.forbiddenY = game.GraphicsDevice.Viewport.Height - MenusCaller.HEIGHT;
        }

        public void SetTerrainWorldMatrix(ref Matrix terrainWorld)
        {
            this.terrainWorld = terrainWorld;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            this.camera.GetMatrices(out this.world, out this.view, out this.projection);
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            this.camera.GetMatrices(out this.world, out this.view, out this.projection);
            this.GetTapPosition();

            base.Update(gameTime);
        }

        private void GetTapPosition()
        {
            this.tapPosition = this.inputMng.GetTapPosition();
            if (this.tapPosition != this.oldTapPosition && this.tapPosition.Y < this.forbiddenY)
            {
                Vector3 result;
                this.Convert2DTo3D(out result, ref this.tapPosition);
                Single x = result.X / GameField.STEP;
                Single z = (result.Z * (-1)) / GameField.STEP;
                Int16 X = Convert.ToInt16(Math.Floor(x / GameField.STEP) * GameField.STEP);
                Int16 Z = Convert.ToInt16(Math.Floor(z / GameField.STEP) * GameField.STEP);
                //To prevent possible convertion mistakes
                X = ((X < (GameField.MAP_WIDTH - 1) ? X : Convert.ToInt16(GameField.MAP_WIDTH - 2)));
                Z = ((Z < (GameField.MAP_WIDTH - 1) ? Z : Convert.ToInt16(GameField.MAP_WIDTH - 2)));
                this.OnCellClickedEvent(X, Z);

                this.oldTapPosition = this.tapPosition;
            }
        }

        private void Convert2DTo3D(out Vector3 result, ref Vector2 position)
        {
            Ray ray = this.CalculateCursorRay(ref position);
            Plane plane = new Plane(Vector3.Up, 0);         //Since the terrain is a plain with its normal vector pointed to the Y -axis's positive direction

            Matrix inverseTransform = Matrix.Invert(this.terrainWorld);
            ray.Position = Vector3.Transform(ray.Position, inverseTransform);
            ray.Direction = Vector3.TransformNormal(ray.Direction, inverseTransform);

            //The ray equation is P = P0 + t*P1 and the plane equation is n.P = -d
            Single t = -(plane.D + Vector3.Dot(ray.Position, plane.Normal)) / (Vector3.Dot(ray.Direction, plane.Normal));
            Vector3 resultVector = ray.Position + ray.Direction * t;

            result = resultVector;
        }

        private Ray CalculateCursorRay(ref Vector2 position)
        {
            Vector3 nearSrc = new Vector3(position, 0f);
            Vector3 farSrc = new Vector3(position, 1.0f);

            Vector3 nearPoint = this.Game.GraphicsDevice.Viewport.Unproject(nearSrc, this.projection, this.view, Matrix.Identity);
            Vector3 farPoint = this.Game.GraphicsDevice.Viewport.Unproject(farSrc, this.projection, this.view, Matrix.Identity);

            Vector3 direction = Vector3.Normalize(farPoint - nearPoint);

            return new Ray(nearPoint, direction);
        }

        private void OnCellClickedEvent(Int16 X, Int16 Z)
        {
            if (this.OnCellClicked != null)
            {
                CellArgs e = new CellArgs(X, Z);
                this.OnCellClicked(this, e);
            }
        }

        private void PickingService_EnabledChanged(Object sender, EventArgs e)
        {
            if (this.Enabled)
            {
                this.inputMng.SetTapPosition(this.oldTapPosition);
            }
        }
    }
}
