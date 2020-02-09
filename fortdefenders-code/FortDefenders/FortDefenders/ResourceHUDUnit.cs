using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FortDefenders
{
    public sealed class ResourceHUDUnit
    {
        public Texture2D texture { get; private set; }
        public Rectangle Rect { get; set; }
        public String Text { get; set; }

        public ResourceHUDUnit(Texture2D texture)
        {
            this.texture = texture;
        }
    }
}
