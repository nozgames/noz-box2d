/*
  NoZ Game Engine

  Copyright(c) 2019 NoZ Games, LLC

  Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files(the "Software"), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions :

  The above copyright notice and this permission notice shall be included in all
  copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
  SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Text;

using FarseerPhysics.Common;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;

namespace NoZ.Platform.Box2D
{
    public class Box2DCollider : ICollider
    {
        internal Fixture _fixture;

        public Collider Node { get; set; }

        public uint CollisionMask {
            get => (uint)_fixture.CollisionCategories;
            set {
                _fixture.CollisionCategories = (Category)value;
            }
        }

        public uint CollidesWithMask {
            get => (uint)_fixture.CollidesWith;
            set {
                _fixture.CollidesWith = (Category)value;
            }
        }

        protected Box2DCollider(Box2DBody body, Shape shape)
        {
            _fixture = body._body.CreateFixture(shape);
            _fixture.UserData = this;
        }

        public static Box2DCollider CreateBox (Box2DBody body, in Vector2 position, in Vector2 size) {
            var verts = new Vertices(4);
            var hsize = size * 0.5f;
            verts.Add((position-hsize).ToXna());
            verts.Add(new Microsoft.Xna.Framework.Vector2( hsize.x + position.x, -hsize.y + position.y));
            verts.Add((position+hsize).ToXna());
            verts.Add(new Microsoft.Xna.Framework.Vector2(-hsize.x + position.x,  hsize.y + position.y));
            return new Box2DCollider(body, new PolygonShape(verts, 0.0f));
        }

        public static Box2DCollider CreateCircle (Box2DBody body, in Vector2 position, float radius)
        {
            var circleShape = new CircleShape(radius, 0.0f);
            circleShape.Position = position.ToXna();
            return new Box2DCollider(body, circleShape);
        }

        public static Box2DCollider CreatePolygon (Box2DBody body, in Vector2 position, Vector2[] points)
        {
            var verts = new Vertices(points.Length);
            foreach(var point in points)
                verts.Add((position+point).ToXna());

            return new Box2DCollider(body, new PolygonShape(verts, 0.0f));
        }

        public static Box2DCollider CreateEdge (Box2DBody body, in Vector2 start, in Vector2 end)
        {
            return new Box2DCollider(body, new EdgeShape(start.ToXna(), end.ToXna()));
        }

        public static Box2DCollider CreateChain (Box2DBody body, in Vector2 position, in Vector2[] points, bool loop=false)
        {
            return null;
        }

        public void DrawDebug(GraphicsContext gc)
        {
            if (_fixture.Shape is PolygonShape)
            {
                var polygon = _fixture.Shape as PolygonShape;

                var verts = new Vertex[polygon.Vertices.Count];
                var indexBuffer = new short[verts.Length * 2];
                for (int i = 0; i < verts.Length; i++)
                {
                    verts[i] = new Vertex(
                        NoZ.Physics.MetersToPixels(_fixture.Body.Position.X + polygon.Vertices[i].X),
                        NoZ.Physics.MetersToPixels(_fixture.Body.Position.Y + polygon.Vertices[i].Y),
                        Color.Green);
                    indexBuffer[i * 2] = (short)i;
                    indexBuffer[i * 2 + 1] = (short)(i + 1);
                }

                indexBuffer[indexBuffer.Length - 1] = 0;

                gc.Draw(PrimitiveType.LineList, verts, verts.Length, indexBuffer, indexBuffer.Length);
            }
            else if (_fixture.Shape is CircleShape)
            {
                var circle = _fixture.Shape as CircleShape;
                var verts = new Vertex[16];
                var indexBuffer = new short[verts.Length * 2];
                for (int i=0; i<verts.Length;i++)
                {
                    var angle = i / (float)verts.Length * MathEx.PI * 2.0f;
                    verts[i] = new Vertex(
                        NoZ.Physics.MetersToPixels(_fixture.Body.Position.X + circle.Position.X + MathEx.Sin(angle) * circle.Radius),
                        NoZ.Physics.MetersToPixels(_fixture.Body.Position.Y + circle.Position.Y + MathEx.Cos(angle) * circle.Radius),
                        Color.Green);
                    indexBuffer[i * 2] = (short)i;
                    indexBuffer[i * 2 + 1] = (short)(i + 1);
                }

                indexBuffer[indexBuffer.Length - 1] = 0;
                gc.Draw(PrimitiveType.LineList, verts, verts.Length, indexBuffer, indexBuffer.Length);
            }
            else if (_fixture.Shape is EdgeShape)
            {
                var edge = _fixture.Shape as EdgeShape;
                var verts = new Vertex[2];
                var indexBuffer = new short[2];
                verts[0] = new Vertex(
                        NoZ.Physics.MetersToPixels(_fixture.Body.Position.X + edge.Vertex1.X),
                        NoZ.Physics.MetersToPixels(_fixture.Body.Position.Y + edge.Vertex1.Y),
                        Color.Green);
                verts[1] = new Vertex(
                        NoZ.Physics.MetersToPixels(_fixture.Body.Position.X + edge.Vertex2.X),
                        NoZ.Physics.MetersToPixels(_fixture.Body.Position.Y + edge.Vertex2.Y),
                        Color.Green);

                indexBuffer[0] = 0;
                indexBuffer[1] = 1;
                gc.Draw(PrimitiveType.LineList, verts, verts.Length, indexBuffer, indexBuffer.Length);
            }
        }

        public void Dispose()
        {
            _fixture?.Dispose();
            _fixture = null;
        }
    }
}
