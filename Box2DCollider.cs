using System;
using System.Collections.Generic;
using System.Text;

using FarseerPhysics.Common;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;

namespace NoZ.Platform.Box2D
{
    public class Box2DCollider : Physics.ICollider
    {
        internal Shape _shape;
        internal Fixture _fixture;

        public Physics.PhysicsLayer Layers {
            get => (Physics.PhysicsLayer)(uint)_fixture.CollisionCategories;
            set {
                _fixture.CollisionCategories = (Category)value;
            }
        }

        public Physics.PhysicsLayer CollidesWithLayers {
            get => (Physics.PhysicsLayer)(uint)_fixture.CollidesWith;
            set {
                _fixture.CollidesWith = (Category)value;
            }
        }

        public Box2DCollider(Box2DBody body, in Vector2 position, in Vector2 size)
        {
            var verts = new Vertices(4);
            verts.Add(new Microsoft.Xna.Framework.Vector2(-size.x * 0.5f + position.x, -size.y * 0.5f + position.y));
            verts.Add(new Microsoft.Xna.Framework.Vector2( size.x * 0.5f + position.x, -size.y * 0.5f + position.y));
            verts.Add(new Microsoft.Xna.Framework.Vector2( size.x * 0.5f + position.x,  size.y * 0.5f + position.y));
            verts.Add(new Microsoft.Xna.Framework.Vector2(-size.x * 0.5f + position.x,  size.y * 0.5f + position.y));

            _shape = new PolygonShape(verts, 0.0f);

            _fixture = body._body.CreateFixture(_shape);
            _fixture.UserData = this;
        }

        public Box2DCollider(Box2DBody body, in Vector2 position, float radius)
        {
            _shape = new CircleShape(radius, 0.0f);

            _fixture = body._body.CreateFixture(_shape);
            _fixture.UserData = this;
        }

        public void DrawDebug(GraphicsContext gc)
        {
            if (_shape is PolygonShape)
            {
                var polygon = _shape as PolygonShape;

                var verts = new Graphics.Vertex[polygon.Vertices.Count];
                var indexBuffer = new short[verts.Length * 2];
                for (int i = 0; i < verts.Length; i++)
                {
                    verts[i] = new Graphics.Vertex(
                        _fixture.Body.Position.X - polygon.Vertices[i].X,
                        _fixture.Body.Position.Y - polygon.Vertices[i].Y,
                        Color.Green);
                    indexBuffer[i * 2] = (short)i;
                    indexBuffer[i * 2 + 1] = (short)(i + 1);
                }

                indexBuffer[indexBuffer.Length - 1] = 0;

                gc.Draw(PrimitiveType.LineList, verts, verts.Length, indexBuffer, indexBuffer.Length);
            }
            else if (_shape is CircleShape)
            {
                var circle = _shape as CircleShape;
                var verts = new Graphics.Vertex[16];
                var indexBuffer = new short[verts.Length * 2];
                for (int i=0; i<verts.Length;i++)
                {
                    var angle = i / (float)verts.Length * MathEx.PI * 2.0f;
                    verts[i] = new Graphics.Vertex(
                        _fixture.Body.Position.X + MathEx.Sin(angle) * circle.Radius,
                        _fixture.Body.Position.Y + MathEx.Cos(angle) * circle.Radius,
                        Color.Green);
                    indexBuffer[i * 2] = (short)i;
                    indexBuffer[i * 2 + 1] = (short)(i + 1);
                }

                indexBuffer[indexBuffer.Length - 1] = 0;
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
