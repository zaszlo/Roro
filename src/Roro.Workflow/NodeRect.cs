﻿using System;
using System.Xml.Serialization;

namespace Roro.Workflow
{
    public struct NodeRect
    {
        [XmlAttribute]
        public int X
        {
            get => this._x;
            set => this._x = Math.Max(0, value / Page.GRID_SIZE * Page.GRID_SIZE);
        }
        private int _x;

        [XmlAttribute]
        public int Y
        {
            get => this._y;
            set => this._y = Math.Max(0, value / Page.GRID_SIZE * Page.GRID_SIZE);
        }
        private int _y;

        [XmlAttribute]
        public int Width
        {
            get => this._width;
            set => this._width = Math.Max(0, value / Page.GRID_SIZE * Page.GRID_SIZE);
        }
        private int _width;

        [XmlAttribute]
        public int Height
        {
            get => this._height;
            set => this._height = Math.Max(0, value / Page.GRID_SIZE * Page.GRID_SIZE);
        }
        private int _height;

        public override string ToString()
        {
            return string.Format("X={0}, Y={1}, Width={2}, Height={3}", this.X, this.Y, this.Width, this.Height);
        }
    }
}
