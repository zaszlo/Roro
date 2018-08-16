﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Serialization;

namespace Roro.Workflow
{
    public sealed partial class Page : NotifyPropertyHelper
    {
        public const string MAIN_PAGE_NAME = "Main";

        public const int GRID_SIZE = 20;

        [XmlAttribute]
        public Guid Id { get; set; }

        public string Name
        {
            get => this._name;
            set
            {
                if (this.Name != Page.MAIN_PAGE_NAME)
                {
                    this.OnPropertyChanged(ref this._name, value);
                }
            }
        }
        private string _name;

        public NotifyCollectionHelper<Node> Nodes { get; }

        public IEnumerable<Node> SelectedNodes => this.Nodes.Where(x => x.Selected);

        public Node StartNode => this.Nodes.First(x => x is StartNode);

        [XmlIgnore]
        public Flow ParentFlow { get; internal set; }

        private Page()
        {
            this.Id = Guid.NewGuid();
            this.Name = string.Empty;
            this.Nodes = new NotifyCollectionHelper<Node>();
            this.Nodes.CollectionChanging += Nodes_CollectionChanging;
            this.Nodes.CollectionChanged += Nodes_CollectionChanged;
        }

        public Page(string name) : this()
        {
            this.Name = name;
            var startNode = new StartNode();
            var endNode = new EndNode();

            startNode.Next.To = endNode.Id;

            startNode.Bounds = new NodeRect()
            {
                X = Page.GRID_SIZE * 1,
                Y = Page.GRID_SIZE * 2,
                Width = startNode.Bounds.Width,
                Height = startNode.Bounds.Height
            };
            endNode.Bounds = new NodeRect()
            {
                X = Page.GRID_SIZE * 1,
                Y = Page.GRID_SIZE * 8,
                Width = startNode.Bounds.Width,
                Height = startNode.Bounds.Height
            };

            this.Nodes.Add(startNode);
            this.Nodes.Add(endNode);

            //// test: add nodes
            var count = 3;// RandomHelper.Next(5, 10);
            ActionNode nextNode = null;
            ActionNode prevNode = null;
            for (var i = 0; i < count; i++)
            {
                prevNode = nextNode;
                nextNode = new ActionNode();
                if (prevNode != null)
                {
                    prevNode.Next.To = nextNode.Id;
                }
                this.Nodes.Add(nextNode);
            }
            //count = RandomHelper.Next(2, 5);
            //for (var i = 0; i < count; i++)
            //{
            //    this.Nodes.Add(new DecisionNode());
            //}
            //count = RandomHelper.Next(2, 3);
            //for (var i = 0; i < count; i++)
            //{
            //    this.Nodes.Add(new LoopStartNode());
            //}
            //count = RandomHelper.Next(2, 3);
            //for (var i = 0; i < count; i++)
            //{
            //    this.Nodes.Add(new VariableNode());
            //}
            //count = RandomHelper.Next(2, 3);
            //for (var i = 0; i < count; i++)
            //{
            //    this.Nodes.Add(new PageNode());
            //}
        }

        private void Nodes_CollectionChanging(object sender, NotifyCollectionChangingEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var nodeToAdd = e.NewItems[0] as Node;
                    if (nodeToAdd is LoopEndNode loopEndNode && loopEndNode.LoopStart.To == Guid.Empty)
                    {
                        e.Cancel = true;
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    var nodeToRemove = e.OldItems[0] as Node;
                    if (nodeToRemove is StartNode && this.Nodes.Count(x => x is StartNode) == 1)
                    {
                        e.Cancel = true;
                    }
                    break;
            }
        }

        private void Nodes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var addedNode = e.NewItems[0] as Node;
                    if (addedNode is StartNode newStartNode)
                    {
                        this.Nodes.RemoveAll(x => x is StartNode && x != newStartNode);
                    }
                    if (addedNode is LoopStartNode loopStartNode && loopStartNode.LoopEnd.To == Guid.Empty)
                    {
                        var loopEndNode = new LoopEndNode();
                        loopEndNode.LoopStart.To = loopStartNode.Id;
                        loopStartNode.LoopEnd.To = loopEndNode.Id;
                        loopEndNode.Bounds = new NodeRect()
                        {
                            X = loopStartNode.Bounds.X,
                            Y = loopStartNode.Bounds.Y + 12 * Page.GRID_SIZE,
                            Width = loopStartNode.Bounds.Width,
                            Height = loopStartNode.Bounds.Height
                        };
                        this.Nodes.Add(loopEndNode);
                    }
                    addedNode.ParentPage = this;
                    addedNode.Ports.ToList().ForEach(x => x.ParentNode = addedNode);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    var removedNode = e.OldItems[0] as Node;
                    removedNode.ParentPage = null;
                    break;
            }
        }

    }
}
