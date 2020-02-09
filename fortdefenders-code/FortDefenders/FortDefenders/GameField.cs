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
    public struct VertexPositionNormalTexture : IVertexType
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 Texture;

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(Single) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(sizeof(Single) * 6, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)

        );

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }
    }

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class GameField : DrawableGameComponent
    {
        #region Variables
        public delegate void NotEmptyClicked(Object o, NotEmptyArgs e);
        public event NotEmptyClicked OnNotEmptyClicked;
        public delegate void ResourceClicked(Object o, NotEmptyArgs e);
        public event ResourceClicked OnResourceClicked;
        public delegate void PersonClecked(Object o, CellState role);
        public event PersonClecked OnPersonClicked;
        public delegate void TargetForPersonClicked(Object o, Point targetCell);
        public event TargetForPersonClicked OnTargetForPersonClicked;

        public const Int16 MAP_WIDTH = 70;
        public const Int16 MAP_HEIGHT = 70;
        public const Single STEP = 1.0f;
        private const Single LIMIT_TO_DRAW_TAP = 0.5f;
        private readonly Color FADE_COLOR = Color.White;
        private readonly Color INITIAL_COLOR = Color.Green;

        private CameraService camera;
        private Matrix world;
        private Matrix view;
        private Matrix projection;
        private Matrix terrainWorld;
        private IndexBuffer indexBuffer;
        private VertexBuffer vertexBuffer;
        private VertexPositionNormalTexture[] vertices;
        private Texture2D grassTexture;
        private BasicEffect basicEffect;
        private BasicEffect lineEffect;
        private PickingService pickingService;
        private Cell[] cells;
        private Boolean canDrawTapCell;
        private Single cellDrawTapTimer;
        private VertexPositionColor[] cellVertices;
        private Color tappedCellColor;
        private Cell currentCell;
        private CellState typeOfBuildingSelected;
        private Dictionary<CellState, NeededResources> listOfTypes;
        private Texture2D msgBoxIcon;

        private BuildingsParent buildingsParent;
        private ResourcesParent resourcesParent;
        private HumansParent humansParent;
        private ResourcesHUD resourcesHUD;
        private BuildingsSelector buildingsSelector;
        private MessageBox msgBox;
        #endregion


        public GameField(Game game)
            : base(game)
        {
            this.camera = game.GetService<CameraService>();
            game.RegisterGameService<PickingService>(out this.pickingService, true);
            if (MAP_WIDTH.CompareTo(MAP_HEIGHT) != 0)
            {
                throw new Exception("The width and the height of the field must be equal");
            }
            this.cellVertices = new VertexPositionColor[8];

            this.Game.RegisterGameComponent<ResourcesHUD>(out this.resourcesHUD, true, true);
            this.Game.RegisterGameComponent<BuildingsParent>(out this.buildingsParent, true, true);
            this.buildingsParent.SetParent(this, ref this.resourcesHUD);
            this.Game.RegisterGameComponent<ResourcesParent>(out this.resourcesParent, true, true);
            this.resourcesParent.SetParent(this, ref this.resourcesHUD);
            this.listOfTypes = GameFieldHelper.GetResourcesNeeded(this.CheckResources, this.DecreaseResources);
            this.typeOfBuildingSelected = CellState.Forbidden;
            this.Game.RegisterGameComponent<HumansParent>(out this.humansParent, true, true);
            this.humansParent.SetParent(this);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {            
            this.cells = new Cell[(MAP_WIDTH - 1) * (MAP_HEIGHT - 1)];  //there are 625 vertices, but only 576 cells

            this.pickingService.OnCellClicked += new PickingService.CellClicked(pickingService_OnCellClicked);

            base.Initialize();
        }

        private void pickingService_OnCellClicked(Object o, CellArgs e)
        {
            this.canDrawTapCell = true;
            Int16 index = Convert.ToInt16(e.Z + e.X * (MAP_WIDTH - 1));     //There are only 69x69 cells. Don't mess them with the number of vertices
            this.currentCell = this.cells[index];
            this.cellDrawTapTimer = LIMIT_TO_DRAW_TAP;
            this.tappedCellColor = INITIAL_COLOR;

            this.FillSelectionCellVertices();
            this.SelectActionOnClicked(e.X, e.Z); Debug.WriteLine(e.X + " " + e.Z + " " + this.currentCell.state);

        }

        private void FillSelectionCellVertices()
        {
            this.cellVertices[0] = new VertexPositionColor(this.vertices[currentCell.ind3].Position, this.tappedCellColor);
            this.cellVertices[1] = new VertexPositionColor(this.vertices[currentCell.ind1].Position, this.tappedCellColor);
            this.cellVertices[2] = new VertexPositionColor(this.vertices[currentCell.ind1].Position, this.tappedCellColor);
            this.cellVertices[3] = new VertexPositionColor(this.vertices[currentCell.ind2].Position, this.tappedCellColor);
            this.cellVertices[4] = new VertexPositionColor(this.vertices[currentCell.ind2].Position, this.tappedCellColor);
            this.cellVertices[5] = new VertexPositionColor(this.vertices[currentCell.ind4].Position, this.tappedCellColor);
            this.cellVertices[6] = new VertexPositionColor(this.vertices[currentCell.ind4].Position, this.tappedCellColor);
            this.cellVertices[7] = new VertexPositionColor(this.vertices[currentCell.ind3].Position, this.tappedCellColor);
        }

        private void SelectActionOnClicked(Int16 X, Int16 Z)
        {
            if (currentCell.state == CellState.Free)
            {
                if (!this.humansParent.PersonSelected)
                {
                    this.TryConstructSelectedBuilding(this.typeOfBuildingSelected, X, Z);     //X is height and Z is width: arr[X][Z]
                }
                else
                {
                    this.OnTargetForPersonClickedEvent(new Point(X, Z));
                }
            }
            else if (currentCell.state >= CellState.House
                     && currentCell.state <= CellState.Gate)
            {
                this.OnNotEmptyCellClickedEvent(new Point(this.currentCell.startX, this.currentCell.startZ), this.currentCell.state);
            }
            else if (currentCell.state == CellState.Stone || currentCell.state == CellState.Tree)
            {
                this.OnResourceClickedEvent(new Point(this.currentCell.startX, this.currentCell.startZ), this.currentCell.state);
            }
            else if (currentCell.state >= CellState.Mayor
                    && currentCell.state <= CellState.BatteringRam)
            {
                this.typeOfBuildingSelected = CellState.Forbidden;

                if (this.humansParent.PersonSelected &&
                    this.humansParent.SelectedRole == currentCell.state)
                {
                    this.humansParent.PersonSelected = false;
                }
                else
                {
                    this.humansParent.PersonSelected = true;
                }

                this.OnPersonClickedEvent();
            }
        }

        public void SetBuildingsSelector(ref BuildingsSelector buildingsSelector)
        {
            this.buildingsSelector = buildingsSelector;
            this.buildingsSelector.OnBuildingSelected += new BuildingsSelector.BuildingSelected(buildingsSelector_OnBuildingSelected); 
        }

        private void buildingsSelector_OnBuildingSelected(Object o, CellState type)
        {
            this.buildingsSelector.Enabled = false;
            this.buildingsSelector.Visible = false;

            if (this.listOfTypes.ContainsKey(type))
            {
                Boolean result = this.listOfTypes[type].StartInvokation();
                if (result)
                {
                    this.typeOfBuildingSelected = type;
                }
                else
                {
                    MessageBox.Show(this.Game, "Building cannot be constructed.\nMore resources needed.",
                                    "Not enough resources", MessageBoxButton.OK, this.msgBoxIcon, out this.msgBox);
                }
            }
        }

        private Boolean CheckResources(Int16 wood, Int16 stone, Int16 gold)
        {
            Boolean result = this.resourcesHUD.CheckResources(stone, wood, gold, 0);
            return result;
        }

        private void DecreaseResources(Int16 wood, Int16 stone, Int16 gold)
        {
            this.resourcesHUD.SetCurrentResources(stone, wood, gold, 0);
        }

        private void TryConstructSelectedBuilding(CellState type, Int16 X, Int16 Z)
        {
            if (type != CellState.Forbidden)
            {
                this.buildingsParent.SetBuildingToBuild(type, X, Z);
            }
            else
            {
                this.buildingsParent.ResetAddWorkersRecall();
            }
            this.typeOfBuildingSelected = CellState.Forbidden;
        }

        /// <summary>
        /// This event will inform subscribers the cell with a construction on it was clicked
        /// </summary>
        /// <param name="pos">start position</param>
        private void OnNotEmptyCellClickedEvent(Point pos, CellState type)
        {
            if (this.OnNotEmptyClicked != null)
            {
                NotEmptyArgs e = new NotEmptyArgs(pos, type);
                this.OnNotEmptyClicked(this, e);
            }
        }

        private void OnResourceClickedEvent(Point pos, CellState type)
        {
            if (this.OnResourceClicked != null)
            {
                NotEmptyArgs e = new NotEmptyArgs(pos, type);
                this.OnResourceClicked(this, e);
            }
        }

        /// <summary>
        /// Checking coordinates for possibility to construct a new building and returning an array with vertices to draw
        /// </summary>
        /// <param name="X">first X</param>
        /// <param name="Z">first Z</param>
        /// <param name="type">type of the construction</param>
        /// <param name="lastX">last X</param>
        /// <param name="lastZ">last Z</param>
        /// <param name="vertToDraw">array of vertices to draw</param>
        /// <returns>returns true if building could be constructed. Otherwise, false</returns>
        public Boolean CheckCoordinates(Int16 X, Int16 Z, CellState type, Int16 lastX, Int16 lastZ, out VertexPositionNormalTexture[] vertToDraw)
        {
            vertToDraw = new VertexPositionNormalTexture[6];     

            if (lastX >= (MAP_HEIGHT - 1) || lastZ >= (MAP_WIDTH - 1))    //Be sure the X and Z are not out of range
            {
                this.typeOfBuildingSelected = CellState.Forbidden;
                return false;
            }

            Int16 index = 0;
            Cell selectedCell = null;
            List<Cell> tmp = new List<Cell>();          //For changing the state of selected cells

            for (Int16 i = Z; i <= lastZ; i++)
            {
                for (Int16 j = X; j <= lastX; j++)
                {
                    index = Convert.ToInt16(i + j * (MAP_WIDTH - 1));
                    selectedCell = this.cells[index];
                    tmp.Add(selectedCell);
                    if (selectedCell.state != CellState.Free)
                    {
                        tmp.Clear();    //Since the map checker fails to find needed amount of free cells there is no need to use prevoiusly selected cells
                        this.typeOfBuildingSelected = CellState.Forbidden;
                        return false;
                    }
                }
            }

            Int16 ind1 = this.cells[Convert.ToInt16(Z + X * (MAP_WIDTH - 1))].ind1;
            Int16 ind2 = this.cells[Convert.ToInt16(lastZ + X * (MAP_WIDTH - 1))].ind2;
            Int16 ind3 = this.cells[Convert.ToInt16(Z + lastX * (MAP_WIDTH - 1))].ind3;
            Int16 ind4 = this.cells[Convert.ToInt16(lastZ + lastX * (MAP_WIDTH - 1))].ind4;

            //1  3  2  2  3  4
            //01 11 00 00 11 10: the texture's normal state
            //00 01 10 10 01 11: the texture will be rotated clockwise by 90 degrees
            //11 10 01 01 10 00: the texture will be rotated by 90 degrees counterclockwise
            vertToDraw[0] = this.vertices[ind1];
            vertToDraw[0].Texture = new Vector2(1, 1); 
            vertToDraw[1] = this.vertices[ind3];
            vertToDraw[1].Texture = new Vector2(1, 0); 
            vertToDraw[2] = this.vertices[ind2];
            vertToDraw[2].Texture = new Vector2(0, 1); 

            vertToDraw[3] = this.vertices[ind2];
            vertToDraw[3].Texture = new Vector2(0, 1); 
            vertToDraw[4] = this.vertices[ind3];
            vertToDraw[4].Texture = new Vector2(1, 0); 
            vertToDraw[5] = this.vertices[ind4];
            vertToDraw[5].Texture = new Vector2(0, 0); 

            foreach (Cell cell in tmp)
            {
                cell.state = type;
                cell.ind1 = ind1; 
                cell.ind2 = ind3; 
                cell.ind3 = ind2; 
                cell.ind4 = ind4; 
                cell.startX = X;    //Every cell has to know about the construction's start cell. It's cheaper than ray casting.
                cell.startZ = Z; 
            }

            tmp.Clear();

            if (this.listOfTypes.ContainsKey(type))
            {
                this.listOfTypes[type].StartSetterInvokation();
            }

            return true;
        }

        /// <summary>
        /// Fast cells marking and resources creating
        /// </summary>
        /// <param name="X">first X</param>
        /// <param name="Z">first Z</param>
        /// <param name="type">type of the construction</param>
        /// <param name="lastX">last X</param>
        /// <param name="lastZ">last Z</param>
        /// <param name="vertToDraw">array of vertices to draw</param>
        public void MarkAddResources(Int16 X, Int16 Z, CellState type, Int16 lastX, Int16 lastZ, out VertexPositionNormalTexture[] vertToDraw)
        {
            vertToDraw = new VertexPositionNormalTexture[6];
            Int16 index = 0;

            Cell selectedCell = null;
            List<Cell> tmp = new List<Cell>();          //For changing the state of selected cells

            for (Int16 i = Z; i <= lastZ; i++)
            {
                for (Int16 j = X; j <= lastX; j++)
                {
                    index = Convert.ToInt16(i + j * (MAP_WIDTH - 1));
                    selectedCell = this.cells[index];
                    tmp.Add(selectedCell);
                }
            }

            Int16 ind1 = this.cells[Convert.ToInt16(Z + X * (MAP_WIDTH - 1))].ind1;
            Int16 ind2 = this.cells[Convert.ToInt16(lastZ + X * (MAP_WIDTH - 1))].ind2;
            Int16 ind3 = this.cells[Convert.ToInt16(Z + lastX * (MAP_WIDTH - 1))].ind3;
            Int16 ind4 = this.cells[Convert.ToInt16(lastZ + lastX * (MAP_WIDTH - 1))].ind4;

            //1  3  2  2  3  4
            //01 11 00 00 11 10: the texture's normal state
            //00 01 10 10 01 11: the texture will be rotated clockwise by 90 degrees
            //11 10 01 01 10 00: the texture will be rotated by 90 degrees counterclockwise
            vertToDraw[0] = this.vertices[ind1];
            vertToDraw[0].Texture = new Vector2(1, 1); 
            vertToDraw[1] = this.vertices[ind3];
            vertToDraw[1].Texture = new Vector2(1, 0); 
            vertToDraw[2] = this.vertices[ind2];
            vertToDraw[2].Texture = new Vector2(0, 1); 

            vertToDraw[3] = this.vertices[ind2];
            vertToDraw[3].Texture = new Vector2(0, 1); 
            vertToDraw[4] = this.vertices[ind3];
            vertToDraw[4].Texture = new Vector2(1, 0); 
            vertToDraw[5] = this.vertices[ind4];
            vertToDraw[5].Texture = new Vector2(0, 0); 
            
            foreach (Cell cell in tmp)
            {
                cell.state = type;
                cell.ind1 = ind1;
                cell.ind2 = ind3;
                cell.ind3 = ind2;
                cell.ind4 = ind4;
                cell.startX = X;    //Every cell has to know about the construction's start cell. It's cheaper than ray casting.
                cell.startZ = Z;
            }

            tmp.Clear();
        }

        /// <summary>
        /// Initialize main persons
        /// </summary>
        /// <param name="X">X</param>
        /// <param name="Z">Z</param>
        /// <param name="vertToDraw">vertices to draw</param>
        public void SetPersonVertices(Int16 X, Int16 Z, CellState role, out VertexPositionNormalTexture[] vertToDraw)
        {
            vertToDraw = new VertexPositionNormalTexture[6];
            Int16 index = Convert.ToInt16(Z + X * (MAP_WIDTH - 1));
            Cell selectedCell = this.cells[index];
            selectedCell.state = selectedCell.state | role;

            Int16 ind1 = selectedCell.ind1;
            Int16 ind2 = selectedCell.ind2;
            Int16 ind3 = selectedCell.ind3;
            Int16 ind4 = selectedCell.ind4;

            //1  3  2  2  3  4
            //01 11 00 00 11 10: the texture's normal state
            //00 01 10 10 01 11: the texture will be rotated clockwise by 90 degrees
            //11 10 01 01 10 00: the texture will be rotated by 90 degrees counterclockwise
            vertToDraw[0] = this.vertices[ind1];
            vertToDraw[0].Texture = new Vector2(1, 1);
            vertToDraw[1] = this.vertices[ind3];
            vertToDraw[1].Texture = new Vector2(1, 0);
            vertToDraw[2] = this.vertices[ind2];
            vertToDraw[2].Texture = new Vector2(0, 1);

            vertToDraw[3] = this.vertices[ind2];
            vertToDraw[3].Texture = new Vector2(0, 1);
            vertToDraw[4] = this.vertices[ind3];
            vertToDraw[4].Texture = new Vector2(1, 0);
            vertToDraw[5] = this.vertices[ind4];
            vertToDraw[5].Texture = new Vector2(0, 0);
        }

        private void OnPersonClickedEvent()
        {
            if (this.OnPersonClicked != null)
            {
                this.OnPersonClicked(this, currentCell.state);
            }
        }

        private void OnTargetForPersonClickedEvent(Point targetCell)
        {
            if (this.OnTargetForPersonClicked != null)
            {
                this.buildingsParent.ResetAddWorkersRecall();
                this.OnTargetForPersonClicked(this, targetCell);
            }
        }

        /// <summary>
        /// To avoid possible overhead a separate method used for changing cells' status to forbidden.
        /// Nothing can be build in such cells.
        /// </summary>
        /// <param name="X">X coordinate</param>
        /// <param name="Z">Z coordinate</param>
        public void AddForbidden(Int16 X, Int16 Z)
        {
            Int16 index = Convert.ToInt16(Z + X * (MAP_WIDTH - 1));
            Cell cell = this.cells[index];
            if (cell.state == CellState.Free)
            {
                cell.state = CellState.Forbidden;
            }
        }

        protected override void LoadContent()
        {
            this.camera.GetMatrices(out this.world, out this.view, out this.projection);
            this.grassTexture = this.Game.Content.Load<Texture2D>("Textures/grass");
            this.msgBoxIcon = this.Game.Content.Load<Texture2D>("Textures/MessageBox/warning");

            Int16[] indices = new Int16[MAP_WIDTH * MAP_HEIGHT * 6];
            this.GenerateVertices(MAP_WIDTH, MAP_HEIGHT, STEP, ref indices);
            this.vertexBuffer = new VertexBuffer(this.Game.GraphicsDevice, typeof(VertexPositionNormalTexture), this.vertices.Count(), BufferUsage.WriteOnly);
            this.vertexBuffer.SetData(this.vertices);
            this.indexBuffer = new IndexBuffer(this.Game.GraphicsDevice, typeof(Int16), indices.Count(), BufferUsage.WriteOnly);
            this.indexBuffer.SetData(indices);

            this.terrainWorld = Matrix.CreateTranslation(new Vector3(-MAP_WIDTH * STEP / 2.0f, 0, MAP_HEIGHT * STEP / 2.0f)) * Matrix.CreateRotationY(MathHelper.ToRadians(60.0f));

            this.basicEffect = new BasicEffect(this.Game.GraphicsDevice);
            this.pickingService.SetTerrainWorldMatrix(ref this.terrainWorld);
            this.buildingsParent.SetTerrainWorld(ref this.terrainWorld);
            this.resourcesParent.SetTerrainWorld(ref this.terrainWorld);
            this.humansParent.SetTerrainWorld(ref this.terrainWorld);
            this.humansParent.SetBuildingsResourcesParent(ref this.buildingsParent, ref this.resourcesParent);

            this.lineEffect = new BasicEffect(this.Game.GraphicsDevice);
            this.lineEffect.VertexColorEnabled = true;

            this.buildingsParent.InitializeWalls();
            this.resourcesParent.AddResources();
            this.humansParent.InitializeCommanders();

            base.LoadContent();
        }

        private void GenerateVertices(Int16 width, Int16 height, Single step, ref Int16[] indices)
        {
            this.vertices = new VertexPositionNormalTexture[width * height];
            for (Int16 i = 0; i < width; i++)
            {
                for (Int16 j = 0; j < height; j++)
                {
                    this.vertices[j + i * width].Position = new Vector3(i * step, 0.0f, -j * step);
                    this.vertices[j + i * width].Texture = new Vector2((Single)i / (Single)this.grassTexture.Width * 8.0f,
                                                                       (Single)j / (Single)this.grassTexture.Height * 8.0f);
                }
            }

            this.FillUpIndices(ref indices, width, height);
        }

        private void FillUpIndices(ref Int16[] indices, Int16 width, Int16 height)
        {
            Int16 counter = 0;
            Int16 squaresCounter = 0;
            for (Int16 i = 0; i < width - 1; i++)
            {
                for (Int16 j = 0; j < height - 1; j++)
                {
                    Int16 topLeft = Convert.ToInt16(j + i * width);
                    Int16 topRight = Convert.ToInt16(j + 1 + i * width);
                    Int16 bottomLeft = Convert.ToInt16(j + (i + 1) * width);
                    Int16 bottomRight = Convert.ToInt16(j + 1 + (i + 1) * width);

                    this.cells[squaresCounter++] = new Cell(topLeft, topRight, bottomLeft, bottomRight);

                    indices[counter++] = topLeft;
                    indices[counter++] = bottomRight;
                    indices[counter++] = bottomLeft;
                    this.CalculateNormalVectors(topLeft, bottomRight, bottomLeft);

                    indices[counter++] = topLeft;
                    indices[counter++] = topRight;
                    indices[counter++] = bottomRight;
                }
            } 
        }

        private void CalculateNormalVectors(Int16 b, Int16 d1, Int16 d2)
        {
            Vector3 direction1 = Vector3.Normalize(this.vertices[b].Position - this.vertices[d1].Position);
            Vector3 direction2 = Vector3.Normalize(this.vertices[b].Position - this.vertices[d2].Position);
            Vector3 normal = Vector3.Normalize(Vector3.Cross(direction2, direction1));
            this.vertices[b].Normal = normal;
            this.vertices[d1].Normal = normal;
            this.vertices[d2].Normal = normal;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            this.camera.GetMatrices(out this.world, out this.view, out this.projection);
            this.buildingsParent.SetMatrices(ref this.world, ref this.view, ref this.projection);
            this.resourcesParent.SetMatrices(ref this.world, ref this.view, ref this.projection);
            this.humansParent.SetMatrices(ref this.world, ref this.view, ref this.projection);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            this.DrawField();
            this.DrawCellTapSelection(gameTime);

            base.Draw(gameTime);
        }

        private void DrawField()
        {
            this.Game.GraphicsDevice.RasterizerState = new RasterizerState()
            {
                CullMode = CullMode.CullCounterClockwiseFace,
                FillMode = FillMode.Solid
            };

            this.Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            this.Game.GraphicsDevice.SetVertexBuffer(this.vertexBuffer);
            this.Game.GraphicsDevice.Indices = this.indexBuffer;

            this.basicEffect.World = this.terrainWorld * this.world;
            this.basicEffect.View = this.view;
            this.basicEffect.Projection = this.projection;
            this.basicEffect.TextureEnabled = true;
            this.basicEffect.Texture = grassTexture;


            foreach (EffectPass pass in this.basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                this.Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.vertices.Length, 0, this.vertices.Length * 2);
            }
        }

        private void DrawCellTapSelection(GameTime gameTime)
        {
            if (this.canDrawTapCell)
            {
                this.Game.GraphicsDevice.RasterizerState = new RasterizerState()
                {
                    CullMode = CullMode.CullCounterClockwiseFace,
                    FillMode = FillMode.WireFrame
                };

                this.Game.GraphicsDevice.DepthStencilState = DepthStencilState.None;

                this.lineEffect.World = this.terrainWorld * this.world;
                this.lineEffect.View = this.view;
                this.lineEffect.Projection = this.projection;

                Single amount = (1 - this.cellDrawTapTimer / LIMIT_TO_DRAW_TAP);

                this.tappedCellColor = Color.Lerp(this.tappedCellColor, FADE_COLOR, amount);
                this.cellDrawTapTimer -= (gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
                if (this.tappedCellColor == FADE_COLOR)
                {
                    this.canDrawTapCell = false;
                    this.tappedCellColor = INITIAL_COLOR;
                    Array.Clear(this.cellVertices, 0, this.cellVertices.Length);
                }

                for (Int16 i = 0; i < this.cellVertices.Length; i++)
                {
                    this.cellVertices[i].Color = this.tappedCellColor;
                }

                foreach (EffectPass pass in this.lineEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    this.Game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, this.cellVertices, 0, this.cellVertices.Length / 2);
                }
            }
        }

        #region Path calculating

        public Boolean IsNextCellFreeForbidden(Int32 X, Int32 Z)
        {
            Int32 index = Z + X * (MAP_WIDTH - 1);
            Cell selectedCell = this.cells[index];

            return ((selectedCell.state == CellState.Free || selectedCell.state == CellState.Forbidden) ? true : false);
        }

        public void ChangeCellStateTillTargetReached(CellState role, Int32 X, Int32 Z)
        {
            Cell cell = this.cells[Z + X * (MAP_WIDTH - 1)];
            cell.state = ((cell.state == CellState.Free || cell.state == CellState.Forbidden) ? cell.state : cell.state ^ role);
        }

        public void TargetReached(CellState role, Int32 X, Int32 Z)
        {
            Cell cell = this.cells[Z + X * (MAP_WIDTH - 1)];
            cell.state = cell.state | role;
        }

        #endregion
    }
}
