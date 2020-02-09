using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace FortDefenders.PersonHelperClasses
{
    public static class PersonTexturesLoader
    {

        public static void LoadTextures(String parentFolder, ContentManager loader, out Dictionary<Int32, Texture2D[]> texturesCollection)
        {
            texturesCollection = new Dictionary<Int32, Texture2D[]>();

            for (Int32 i = 0; i < 360; i += 45)
            {
                Texture2D[] tex = new Texture2D[3];
                tex[0] = loader.Load<Texture2D>(parentFolder + "/left/leftleg_tex_" + i);
                tex[1] = loader.Load<Texture2D>(parentFolder + "/right/rightleg_tex_" + i);
                tex[2] = loader.Load<Texture2D>(parentFolder + "/stop/stop_tex_" + i);
                texturesCollection.Add(i, tex);
            }
        }
    }
}
