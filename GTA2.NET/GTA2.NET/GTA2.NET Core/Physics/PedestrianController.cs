// GTA2.NET
// 
// File: Pedestrian.cs
// Created: 22.04.2015
// Created by: João Pires
// 
// The MIT License (MIT)
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

using Jitter;
using Jitter.Dynamics;
using Jitter.Dynamics.Constraints;
using Jitter.LinearMath;

namespace Hiale.GTA2NET.Core.Physics
{
    public enum Walk {Forward, Backward, None};
    public enum Rotate {Clockwise, Anticlockwise, None}

    /// <summary>
    /// Constraint used to move pedestrians.
    /// </summary>
    public class PedestrianController : Constraint
    {
        /// <summary>
        /// The Direction that the pedestrian is moving.
        /// </summary>
        public Walk Direction { get; set; }
        
        /// <summary>
        /// The rotation of the pedestrians.
        /// </summary>
        public Rotate Rotate { get; set; }

        public PedestrianController(World world, RigidBody body)
            : base(body, null)
        {
        }

        public override void PrepareForIteration(float timestep)
        {}

        public override void Iterate()
        {
            if (Rotate == Rotate.Clockwise)
            {
                Body1.IsActive = true;
                JVector torque = new JVector(0, 0, 1);
                torque = JVector.Transform(torque, Body1.Orientation);
                torque = JVector.Transform(torque, Body1.Inertia);
                Body1.AddTorque(torque * Body1.Mass);
            }

            if (Rotate == Rotate.Anticlockwise)
            {
                Body1.IsActive = true;
                JVector torque = new JVector(0, 0, -1);
                torque = JVector.Transform(torque, Body1.Orientation);
                torque = JVector.Transform(torque, Body1.Inertia);
                Body1.AddTorque(torque * Body1.Mass);
            }

            if (Direction == Walk.Forward)
            {
                Body1.IsActive = true;
                JVector force = new JVector(1, 0, 0);
                force = JVector.Transform(force, Body1.Orientation);
                Body1.AddForce(force * Body1.Mass);
            }

            if (Direction == Walk.Backward)
            {
                Body1.IsActive = true;
                JVector force = new JVector(-1, 0, 0);
                force = JVector.Transform(force, Body1.Orientation);
                Body1.AddForce(force * Body1.Mass);
            }
        }
    }
}