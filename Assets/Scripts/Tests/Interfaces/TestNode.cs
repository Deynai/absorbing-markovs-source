using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Deynai.Markov.Tests
{
    internal class TestNode : INode
    {
        public ulong ID { get; private set; }
        public string Name { get; set; }

        public RectTransform RectTransform => null;

        public IReadOnlyList<ISocket> Sockets => throw new NotImplementedException();

        public TestNode()
        {
            ID = ulong.MinValue;
        }

        public void SetID(ulong id)
        {
            ID = id;
        }

        public void Remove()
        {
            return;
        }

        public void SelectEntered()
        {
            throw new NotImplementedException();
        }

        public void SelectExited()
        {
            throw new NotImplementedException();
        }

        public void HoverEntered()
        {
            throw new NotImplementedException();
        }

        public void HoverExited()
        {
            throw new NotImplementedException();
        }
    }
}
