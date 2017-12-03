﻿using Roro.Activities;
using Roro.Activities.Excel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Serialization;

namespace Roro.Workflow
{
    [DataContract]
    public abstract class Node
    {
        [DataMember]
        public Guid Id { get; private set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Rectangle Bounds { get; private set; }

        public void SetBounds(Rectangle rect)
        {
            rect.X = (rect.X / PageRenderOptions.GridSize) * PageRenderOptions.GridSize;
            rect.Y = (rect.Y / PageRenderOptions.GridSize) * PageRenderOptions.GridSize;
            foreach (var port in this.Ports)
            {
                port.UpdateBounds(rect);
            }
            this.Bounds = rect;
        }

        [DataMember]
        public Activity Activity { get; set; }

        [DataMember]
        public List<Port> Ports { get; private set; }

        public Dictionary<Port, GraphicsPath> RenderedPorts { get; }

        public Port GetPortById(Guid id)
        {
            return this.Ports.FirstOrDefault(x => x.Id == id);
        }

        public Port GetPortFromPoint(Point pt)
        {
            if (this.RenderedPorts.FirstOrDefault(x => x.Value.IsVisible(pt.X, pt.Y)) is KeyValuePair<Port, GraphicsPath> item)
            {
                return item.Key;
            }
            return null;
        }

        public virtual Guid Execute()
        {
            return Guid.Empty;
        }

        public Node()
        {
            this.Id = Guid.NewGuid();
            this.Name = string.Format("{0} {1} {2}", this.GetType().Name, Helper.Between(0, 999), Helper.Between(0, 999));
            this.Bounds = new Rectangle(
                PageRenderOptions.GridSize * Helper.Between(4, 30),
                PageRenderOptions.GridSize * Helper.Between(4, 30),
                PageRenderOptions.GridSize * this.GetSize().Width,
                PageRenderOptions.GridSize * this.GetSize().Height);
            this.Ports = new List<Port>();
            this.RenderedPorts = new Dictionary<Port, GraphicsPath>();
        }

        public abstract Size GetSize();

        public abstract GraphicsPath Render(Graphics g, Rectangle r, NodeStyle o);

        public void RenderText(Graphics g, Rectangle r, NodeStyle o)
        {
            g.DrawString(this.Name, o.Font, o.FontBrush, r, o.StringFormat);
        }
    }
}