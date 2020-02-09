using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FortDefenders
{
    public abstract class HUD : DrawableGameComponent
    {
        public delegate void HUDHidden(Object o);
        public event HUDHidden OnHUDHidden;

        public HUD(Game game)
            : base(game)
        {
            
        }

        public virtual void OnHUDHiddenEvent()
        {
            if (this.OnHUDHidden != null)
            {
                this.OnHUDHidden(this);
            }
        }
    }
}
