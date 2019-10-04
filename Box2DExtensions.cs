using System;
using System.Collections.Generic;
using System.Text;

namespace NoZ.Platform.Box2D
{
    static internal class Box2DExtensions
    {
        public static Microsoft.Xna.Framework.Vector2 ToXna(this Vector2 v) =>
            new Microsoft.Xna.Framework.Vector2(v.x, v.y);
    }
}
