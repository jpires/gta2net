﻿// GTA2.NET
// 
// File: MapCollision.cs
// Created: 09.03.2013
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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.Xna.Framework;

namespace Hiale.GTA2NET.Core.Collision
{
    public class MapCollision
    {
        private static Dictionary<Direction, int> _baseDirectionPriority;

        private readonly Map.Map _map;

        public MapCollision(Map.Map map)
        {
            _map = map;
        }

        public List<IObstacle> GetObstacles(int currentLayer)
        {
            var obstacles = new List<IObstacle>();
            var rawObstacles = GetBlockObstacles(currentLayer);
            var nodes = GetAllObstacleNodes(rawObstacles);

            while (nodes.Count > 0)
            {
                var currentFigure = new Figure(currentLayer, nodes);
                foreach (var line in currentFigure.Lines)
                {
                    nodes.Remove(line.Start);
                    nodes.Remove(line.End);
                }
                currentFigure.Optimize();

                obstacles.AddRange(currentFigure.Tokenize());
            }
            return obstacles;
        }

        private IEnumerable<ILineObstacle> GetBlockObstacles(int z)
        {
            var obstacles = new List<ILineObstacle>();
            for (var x = 0; x < _map.Width; x++)
            {
                for (var y = 0; y < _map.Length; y++)
                {
                    _map.CityBlocks[x, y, z].GetCollision(obstacles, false);
                }
            }
            return obstacles;
        }

        private static Dictionary<Vector2, List<LineSegment>> GetAllObstacleNodes(IEnumerable<ILineObstacle> obstacles)
        {
            var nodes = new Dictionary<Vector2, List<LineSegment>>();
            foreach (var lineObstacle in obstacles)
            {
                if (lineObstacle is SlopeLineObstacle)
                    continue;

                //start point
                var newLine = new LineSegment(lineObstacle.Start, lineObstacle.End);
                InsertLine(nodes, newLine);

                //end point
                newLine = new LineSegment(lineObstacle.End, lineObstacle.Start);
                InsertLine(nodes, newLine);
            }
            return nodes;
        }

        private static void InsertLine(IDictionary<Vector2, List<LineSegment>> nodes, LineSegment newLine)
        {
            List<LineSegment> vectorList;
            if (nodes.TryGetValue(newLine.Start, out vectorList))
                vectorList.Add(newLine);
            else
            {
                vectorList = new List<LineSegment> { newLine };
                nodes.Add(newLine.Start, vectorList);
            }
        }

        //Debug methods

        public static void SaveSegmentsPicture(List<LineSegment> segments, Stream outputStream, string name)
        {
            var fileName = name + ".png";
            Bitmap bmp;
            if (File.Exists(fileName))
            {
                var image = Image.FromFile(fileName);
                bmp = new Bitmap(image);
                image.Dispose();
            }
            else
                bmp = new Bitmap(2560, 2560);
            using (var g = Graphics.FromImage(bmp))
            {
                foreach (var segment in segments)
                {
                    g.DrawLine(new Pen(new SolidBrush(System.Drawing.Color.Red), 1), segment.Start.X * 10, segment.Start.Y * 10, segment.End.X * 10, segment.End.Y * 10);
                }
            }
            if (outputStream == null)
                bmp.Save("debug\\" + fileName, ImageFormat.Png);
            else
                bmp.Save(outputStream, ImageFormat.Png);
            bmp.Dispose();
            
        }

        public static void SaveSegmentsPicture(List<LineSegment> segments, string name)
        {
            SaveSegmentsPicture(segments, null, name);
        }
    }
}
