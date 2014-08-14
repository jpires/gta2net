// GTA2.NET
// 
// File: Physics.cs
// Created: 27.07.2013
// 
// 
// Copyright (C) 2010-2013 Hiale
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software
// and associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software
// is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies
// or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Grand Theft Auto (GTA) is a registred trademark of Rockstar Games.

using Hiale.GTA2NET.Core.Logic;
using Hiale.GTA2NET.Core.Map;
using Jitter;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.DataStructures;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Hiale.GTA2NET.Core.Physics
{
    /// <summary>
    /// Used to abstract the interaction with the physics engine.
    /// </summary>
    public class WorldSimulation
    {
        /// <summary>
        /// The jitter representation of the world.
        /// </summary>
        private World world;

        /// <summary>
        /// Creates an instance of WorldSimulation class.
        /// </summary>
        /// <param name="map">The map to use as physic base.</param>
        public WorldSimulation(Map.Map map)
        {
            CollisionSystem collisionSystem = new CollisionSystemSAP();
            collisionSystem.UseTriangleMeshNormal = false;
            world = new World(collisionSystem);
            world.Gravity = new Jitter.LinearMath.JVector(0, 0, -10);
            Initialize(map);
        }
        /// <summary>
        /// Initialize the physic world with the geometric detail of map.
        /// </summary>
        /// <param name="map">The base to create the physic world.</param>
        private void Initialize(Map.Map map)
        {
            List<JVector> vertices = new List<JVector>();
            List<TriangleVertexIndices> indices = new List<TriangleVertexIndices>();
            
            for (uint i = 0; i < map.Width; i++)
                for (uint j = 0; j < map.Length; j++)
                    for (uint k = 0; k < map.Height; k++)
                    {
                        int pos = 0;
                        Block block = map.GetBlock(new Vector3(i, j, k));
                        block.CreateColisions();
                        foreach (JVector vertice in block.CollisionVertices)
                        {
                            vertices.Add(vertice);
                            pos++;
                        }

                        int newPos = vertices.Count - pos;
                        foreach (TriangleVertexIndices indice in block.CollisionTriangles)
                        {
                            TriangleVertexIndices t = new TriangleVertexIndices(indice.I0 + newPos, indice.I1 + newPos, indice.I2 + newPos);
                            indices.Add(t);
                        }
                    }

            //ToDo: The vertices list has duplicated vertices. In the worst case each vertices has 4 different instantiation.
            //Probably some performance can be expected by remove the duplicated ones.
            //However it is also necessary to update the indies List with the correct positions.

            Octree tree = new Octree(vertices, indices);
            TriangleMeshShape shape = new TriangleMeshShape(tree);
            RigidBody body = new RigidBody(shape);
            body.IsStatic = true;
            world.AddBody(body);
        }

        /// <summary>
        /// Updates the simulation.
        /// </summary>
        /// <param name="elapsedTime">The elapsed time since the last update.</param>
        public void Update(float elapsedTime)
        {
            world.Step(elapsedTime, false);
        }

        /// <summary>
        /// Add a new pedestrian to the simulation.
        /// </summary>
        /// <param name="ped">The pedestrian to add.</param>
        public void AddPed(Pedestrian ped)
        {
            RigidBody body = ped.Body;
            world.AddBody(body);

            PedestrianController controller = new PedestrianController(world, body);
            world.AddConstraint(controller);
            ped.Controller = controller;
        }

        public ReadOnlyHashset<RigidBody> RigidBodiesList
        {
            get
            {
                return world.RigidBodies;
            }
        }
    }
}
