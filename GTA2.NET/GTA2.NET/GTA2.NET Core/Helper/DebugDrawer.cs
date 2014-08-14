using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hiale.GTA2NET.Core.Helper
{
    class DebugDrawer : Jitter.IDebugDrawer
    {
        private List<VertexPositionColor> lineList;
        private List<int> lineIndex;
        private List<VertexPositionColor> triangleList;
        private List<int> triangleIndex;

        private Frame drawInfo;
        public Frame DrawInfo { 
            get{
                if (drawInfo == null)
                    drawInfo = new Frame(lineList, triangleList);

                return drawInfo;
                }
            private set 
            {
                drawInfo = value;
            }
        }

        public DebugDrawer()
        {
            lineList = new List<VertexPositionColor>();
            lineIndex = new List<int>();

            triangleList = new List<VertexPositionColor>();
            triangleIndex = new List<int>();
            DrawInfo = null;
        }

        /// <summary>
        /// Creates the correct vertices used to draw a line.
        /// </summary>
        /// <param name="start">The start point of the line</param>
        /// <param name="end">The end point of the line.</param>
        public void DrawLine(JVector start, JVector end)
        {
            VertexPositionColor p1 = new VertexPositionColor();
            p1.Color = Color.Red;
            p1.Position = new Vector3(start.X, -start.Y, start.Z);
            lineList.Add(p1);

            VertexPositionColor p2 = new VertexPositionColor();
            p2.Color = Color.Red;
            p2.Position = new Vector3(end.X, -end.Y, end.Z);
            lineList.Add(p2);
        }

        public void DrawPoint(JVector pos)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Creates the correct vertices used to draw a triangle
        /// </summary>
        /// <remarks>This method used the DrawLine to draw the triangle.</remarks>
        /// <param name="pos1">The first point.</param>
        /// <param name="pos2">The second point.</param>
        /// <param name="pos3">The third point.</param>
        public void DrawTriangle(JVector pos1, JVector pos2, JVector pos3)
        {
            DrawLine(pos1, pos2);
            DrawLine(pos2, pos3);
            DrawLine(pos1, pos3);
        }
    }
}
