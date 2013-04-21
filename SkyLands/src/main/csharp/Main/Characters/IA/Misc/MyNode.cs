﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Characters.IA.Misc
{
    class AstarNode<T> {
        T data;
        AstarNode<T> nextNode = null;

        public T Data                { get { return this.data; } } 
        public AstarNode<T> NextNode { get { return this.nextNode; } }

        public void setNextNode(AstarNode<T> n) { this.nextNode = n; }

        public AstarNode(T data) { this.data = data; }
    }
}
