using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using FortDefenders.PathFinder;
using FortDefenders.PersonHelperClasses;
using FortDefenders.PersonsMenus;

namespace FortDefenders
{    
    public abstract class HumansBase
    {
        private const Int16 DIAGONAL = 14;
        private const Int16 HV = 10;
        private const Single STEP_DELTA = 1.0f;     //Equals the size of the cell in value

        #region Variables

        protected Game game;
        protected Texture2D personTex;          //Currently used texture
        protected Texture2D leftLegTex;         //Step forward with left leg
        protected Texture2D rightLegTex;        //Step forward with right leg
        protected Texture2D stopTex;            //Person stays
        protected Dictionary<Int32, Texture2D[]> textures;
        protected Point currentPos;
        private Point targetPos;
        private readonly Single movingSpeed;
        private readonly Single stepTime;       //Time before changing a texture

        protected VertexPositionNormalTexture[] vertices;
        protected Boolean canDraw;
        protected Matrix world;
        protected Matrix terrainWorld;
        protected Matrix view;
        protected Matrix projection;
        protected BasicEffect effect;
        private BoundingBox box;
        private Boolean isBoxConstructed;
        private BoundingFrustum frustum;
        private GameField gameField;

        public CellState Role { get; protected set;}
        public Int16 UnitsUnderControll { get; set; } 
        public Texture2D FaceTex{ get; protected set; }           //Texture for selector
        public Int16 Health { get; set; }

        private BHeap<PathNode> openHeap;
        private Dictionary<Int32, PathNode> closedList;
        private Stack<Point> pathToTarget;
        private Boolean isFound;
        private Single currentDistance;
        private Point posToMove;
        private Boolean CanPopNextPos;
        private Int32 dX;
        private Int32 dZ;
        private Matrix humanWorld;
        private Boolean recalculateLater;  
        //It's faster than float point operations and conversions. 
        //As alternative to the table of degrees, this formula could 
        //be used: arccos(dx/sqrt(dz^2 + dx^2)), which is much slower than fetching a value by its index.
        //The index is calculated with this formula: dX + 1 + (dZ + 1) * 3. dX = dZ = -1..1
        private readonly Int32[] degrees = { 225, 180, 135, 270, 0, 90, 315, 0, 45 };
        private Single currentTexTime;      //Texture's current time
        private Boolean isRightLegTex;

        protected HumansMenu menu;
        protected MenusCaller menusCaller;
        protected PickingService pickingService;

        #endregion

        public HumansBase(Game game, CellState role, Single movingSpeed, Point currentPos, 
                          Int16 UnitsUnderControll, Single stepTime)
        {
            this.game = game;
            this.effect = new BasicEffect(this.game.GraphicsDevice);
            this.currentPos = currentPos;
            this.Role = role;
            this.isBoxConstructed = false;
            this.canDraw = false;
            this.movingSpeed = movingSpeed;
            this.UnitsUnderControll = UnitsUnderControll;
            this.gameField = this.game.Components.OfType<GameField>().ElementAt(0);
            this.openHeap = new BHeap<PathNode>();
            this.closedList = new Dictionary<Int32, PathNode>();
            this.pathToTarget = new Stack<Point>();
            this.stepTime = stepTime;
            this.currentDistance = 0;
            this.humanWorld = Matrix.Identity;
            this.isFound = false;
            this.recalculateLater = false;
            this.isRightLegTex = false;
            this.Health = 100;
        }

        public virtual void LoadTextures()
        {
            this.menusCaller = this.game.Components.OfType<MenusCaller>().ElementAt(0);
            this.pickingService = this.game.GetService<PickingService>();
            this.menu.EnabledChanged += new EventHandler<EventArgs>(menu_EnabledChanged);
            this.personTex = this.stopTex;
            this.frustum = new BoundingFrustum(this.view * this.projection);
        }

        public virtual void Update(GameTime gameTime)
        {
            this.frustum.Matrix = this.view * this.projection;
            this.canDraw = this.frustum.Intersects(this.box);
            this.FollowPath();
            if (this.recalculateLater)
            {
                this.WaitRecalculatePath();
            }
        }

        public virtual void Draw()
        {
            if (this.canDraw)
            {
                this.effect.World = this.humanWorld * this.terrainWorld * this.world;
                this.effect.View = this.view;
                this.effect.Projection = this.projection;
                this.effect.TextureEnabled = true;
                this.effect.Texture = this.personTex;

                foreach (EffectPass pass in this.effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    this.game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, this.vertices, 0, 2);
                }
            }
        }

        public virtual void SetMatrices(ref Matrix world, ref Matrix view, ref Matrix projection, ref Matrix terrainWorld)
        {
            this.world = world;
            this.view = view;
            this.projection = projection;
            this.terrainWorld = terrainWorld;

            if (!this.isBoxConstructed)
            {
                Vector3 transformedVert = Vector3.Zero;
                Vector3 min = Vector3.Transform(this.vertices[0].Position, this.humanWorld * this.terrainWorld * this.world);
                Vector3 max = Vector3.Transform(this.vertices[0].Position, this.humanWorld * this.terrainWorld * this.world);

                for (Int16 i = 0; i < this.vertices.Length; i++)
                {
                    transformedVert = Vector3.Transform(this.vertices[i].Position, this.terrainWorld * this.world);
                    min = Vector3.Min(min, transformedVert);
                    max = Vector3.Max(max, transformedVert);
                }
                this.box = new BoundingBox(min, max);
            }
        }

        public virtual void SetTargetPosition(Point targetPos)
        {
            this.targetPos = targetPos;
            if (this.currentDistance == 0)
            {
                this.CalculatePathToTarget();
            }
            else
            {
                this.recalculateLater = true;
            }
        }

        public virtual void SetVertices(VertexPositionNormalTexture[] vertices)
        {
            this.vertices = vertices;
            Int16 length = Convert.ToInt16(this.vertices.Length);
            //It appears that the manipulation with the DethStencilState class will have a negative effect on the drawing process. 
            //As result, the texture will be drawn below on the same level(Y -coordinate) as the terrain. This will provoke so-called Z -fighting.
            //Thus, the plane with a texture will be drawn a bit above the terrain - Y = 0.01f. Please remember it when overriding this method
            for (Int16 i = 0; i < length; i++)
            {
                this.vertices[i].Position.Y += 0.01f;
            }
        }

        #region Person moves

        private void FollowPath()
        {
            if (this.isFound)
            {
                if (this.currentPos != this.targetPos || this.currentPos != this.posToMove)
                {
                    this.ChangeCurrentPosition();
                }
                else
                {
                    this.isFound = false;
                    this.gameField.ChangeCellStateTillTargetReached(CellState.Path, this.currentPos.X, this.currentPos.Y);
                    this.gameField.TargetReached(this.Role, this.currentPos.X, this.currentPos.Y);
                    this.personTex = this.textures[this.degrees[this.dX + 1 + (-this.dZ + 1) * 3]][(Int32)PersonMovement.stop];
                }
            }
        }

        private void ChangeCurrentPosition()
        {
            if (this.currentDistance <= STEP_DELTA)
            {
                this.PopNextPositionIfPossible();
                if (!this.isFound)
                {
                    this.gameField.ChangeCellStateTillTargetReached(CellState.Path, this.currentPos.X, this.currentPos.Y);
                    this.gameField.TargetReached(this.Role, this.currentPos.X, this.currentPos.Y); 
                    return;
                }
                //The distance is equal to the time multiplied by the speed: S = v * t
                Single stepToMove = (this.game.TargetElapsedTime.Milliseconds / 1000.0f) * this.movingSpeed;
                this.currentDistance += stepToMove;
                this.humanWorld *= Matrix.CreateTranslation(this.dX * stepToMove, 0, -this.dZ * stepToMove);
                this.SwitchTextures();
            }
            else
            {
                this.gameField.ChangeCellStateTillTargetReached(CellState.Path, this.currentPos.X, this.currentPos.Y);
                this.currentPos = this.posToMove;
                this.currentDistance = 0;
                this.CanPopNextPos = true;
            }
        }

        private void PopNextPositionIfPossible()
        {
            if (this.CanPopNextPos)
            {
                this.CanPopNextPos = false;
                this.posToMove = this.pathToTarget.Pop();
                if (this.gameField.IsNextCellFreeForbidden(this.posToMove.X, this.posToMove.Y))
                {
                    this.dX = this.posToMove.X - this.currentPos.X;
                    this.dZ = this.posToMove.Y - this.currentPos.Y;
                    this.gameField.TargetReached(CellState.Path, this.posToMove.X, this.posToMove.Y);
                }
                else
                {
                    this.RecalculateNow();
                }
            }
        }

        private void RecalculateNow()
        {
            this.isFound = false;
            this.CalculatePathToTarget();
        }

        private void WaitRecalculatePath()
        {
            if (this.currentPos == this.posToMove)
            {
                this.isFound = false;
                this.recalculateLater = false;
                this.CalculatePathToTarget();
            }
        }

        #endregion

        #region Switching Textures while person is moving

        private void SwitchTextures()
        {
            this.currentTexTime += (this.game.TargetElapsedTime.Milliseconds / 1000.0f);
            if (this.currentTexTime >= this.stepTime)
            {
                Int32 key = this.degrees[this.dX + 1 + (-this.dZ + 1) * 3];
                this.currentTexTime = 0;
                this.personTex = (this.isRightLegTex ?
                                  this.personTex = this.textures[key][(Int32)PersonMovement.right]
                                  : this.personTex = this.textures[key][(Int32)PersonMovement.left]);
                this.isRightLegTex = (this.isRightLegTex ? false : true);
            }
        }

        #endregion

        #region Path calculation

        /// <summary>
        /// Path calculation using the A* algorithm
        /// </summary>
        private void CalculatePathToTarget()
        {
            if (currentPos != targetPos)
            {
                this.openHeap.Clear();
                this.closedList.Clear();
                PathNode startNode = new PathNode()
                {
                    CellCoordinates = this.currentPos,
                    F = 0,
                    G = 0,
                    H = 0,
                    Parent = null
                };
                this.openHeap.Push(startNode);

                while (this.openHeap.GetCount() > 0)
                {
                    PathNode node = this.openHeap.Pop();
                    Int32 parentX = node.CellCoordinates.X;
                    Int32 parentZ = node.CellCoordinates.Y;
                    this.closedList.Add(parentZ + parentX * (GameField.MAP_WIDTH - 1), node);

                    for (Int32 i = -1; i <= 1; i++)
                    {
                        for (Int32 j = -1; j <= 1; j++)
                        {
                            Int32 X = parentX + i;
                            Int32 Z = parentZ + j;
                            this.CheckAdjacentNodes(X, Z, node);
                        }
                    }

                    if (node.CellCoordinates == this.targetPos)
                    {
                        Int32 key = this.degrees[this.dX + 1 + (-this.dZ + 1) * 3];
                        this.BuildPath();
                        this.isFound = true;
                        this.CanPopNextPos = true;
                        this.gameField.ChangeCellStateTillTargetReached(this.Role, this.currentPos.X, this.currentPos.Y);
                        this.gameField.TargetReached(CellState.Path, this.currentPos.X, this.currentPos.Y);
                        this.personTex = (this.isRightLegTex ?
                                          this.personTex = this.textures[key][(Int32)PersonMovement.right]
                                          : this.personTex = this.textures[key][(Int32)PersonMovement.left]);
                        break;
                    }
                }

                if (!this.isFound)
                {
                    this.targetPos = this.currentPos;
                    this.gameField.TargetReached(this.Role, this.currentPos.X, this.currentPos.Y);
                    this.personTex = this.textures[this.degrees[this.dX + 1 + (-this.dZ + 1) * 3]][(Int32)PersonMovement.stop];
                }
            }
        }

        /// <summary>
        /// Checking adjacent nodes
        /// </summary>
        /// <param name="X">node's X coordinate</param>
        /// <param name="Z">node's Z coordinate</param>
        /// <param name="parent">Node's parent</param>
        private void CheckAdjacentNodes(Int32 X, Int32 Z, PathNode parent)
        {
            if (X > -1 && Z > -1 && X < GameField.MAP_HEIGHT && Z < GameField.MAP_WIDTH)
            {
                if (!this.IsInClosedList(X, Z))
                {
                    this.AddNewChildIfPossible(X, Z, parent);
                }
            }
        }

        /// <summary>
        /// If node to add doesn't exist in the open list(binary heap) then it will be added.
        /// Otherwise, G-cost to go to the node will be checked from the new parent. If G-cost will be smaller 
        /// than the existing one, the parent of the existing node will be substitude with a new parent. Also the
        /// open list will be reordered
        /// </summary>
        /// <param name="X">node's X coordinate</param>
        /// <param name="Z">node's Z coordinte</param>
        /// <param name="parent">node's parent</param>
        private void AddNewChildIfPossible(Int32 X, Int32 Z, PathNode parent)
        {
            if (this.gameField.IsNextCellFreeForbidden(X, Z)
                && this.IsCornerWalkable(X, Z, parent))
            {
                Int32 gCost = parent.G + ((Math.Abs(parent.CellCoordinates.X - X) == 1 && Math.Abs(parent.CellCoordinates.Y - Z) == 1) ? DIAGONAL : HV);
                Int32 hCost = 10 * (Math.Abs(parent.CellCoordinates.X - X) + Math.Abs(parent.CellCoordinates.Y - Z));
                Int32 fCost = gCost + hCost;

                if(!this.openHeap.IsOnHeap(new Point(X, Z)))
                {
                    PathNode child = new PathNode()
                    {
                        CellCoordinates = new Point(X, Z),
                        F = fCost,
                        H = hCost,
                        G = gCost,
                        Parent = parent
                    };
                    this.openHeap.Push(child);
                    return;
                }

                Int32 index = -1;
                PathNode existed = this.openHeap.GetItemByPosition(new Point(X, Z), out index);

                if (gCost < existed.G)
                {
                    existed.Parent = parent;
                    existed.G = gCost;
                    existed.F = existed.H + existed.G;
                    this.openHeap.ChangeExistingItem(existed, index);
                }
            }
        }

        /// <summary>
        /// The path will be built backwards from target position to start position
        /// </summary>
        private void BuildPath()
        {
            Point destination = this.targetPos;
            this.pathToTarget.Clear();

            do
            {
                this.pathToTarget.Push(destination);
                destination = this.closedList[destination.Y + destination.X * (GameField.MAP_WIDTH - 1)].Parent.CellCoordinates;
            }
            while (destination != this.currentPos);
        }

        /// <summary>
        /// Does the closed list contain this node?
        /// </summary>
        /// <param name="X">node's X coordinate</param>
        /// <param name="Z">node's Z coordinate</param>
        /// <returns>if the list contains node - returns true. Otherwise false</returns>
        private Boolean IsInClosedList(Int32 X, Int32 Z)
        {
            return this.closedList.ContainsKey(Z + X * (GameField.MAP_WIDTH - 1));
        }

        /// <summary>
        /// Checking the route from the parent to the best child for corners cutting
        /// </summary>
        /// <param name="node">child to check</param>
        /// <returns>false if corners are cut</returns>
        private Boolean IsCornerWalkable(Int32 X, Int32 Z, PathNode node)
        {
            Int32 dX = X - node.CellCoordinates.X;
            Int32 dZ = Z - node.CellCoordinates.Y;

            if (Math.Abs(dX) == 0 || Math.Abs(dZ) == 0) return true;

            if (!gameField.IsNextCellFreeForbidden(X, node.CellCoordinates.Y))
                return false;
            if (!gameField.IsNextCellFreeForbidden(node.CellCoordinates.X, Z))
                return false;

            return true;
        }

        #endregion

        #region Menu part

        private void menu_EnabledChanged(Object sender, EventArgs e)
        {
            if (this.menu.Enabled)
            {
                this.pickingService.Enabled = false;
                this.menusCaller.OnHUDHiddenEvent();
                this.menusCaller.Enabled = false;
                this.menusCaller.Visible = false;
            }
            else
            {
                this.pickingService.Enabled = true;
                this.menusCaller.Enabled = true;
                this.menusCaller.Visible = true;
            }
        }
        
        public virtual void ShowHumansMenu()
        {
            this.menu.Enabled = true;
            this.menu.Visible = true;
        }

        #endregion
    }
}
