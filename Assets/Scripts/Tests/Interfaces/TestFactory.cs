using UnityEngine;

namespace Deynai.Markov.Tests
{
    internal class TestFactory : INodeFactory
    {
        public INode Create(Vector2 position)
        {
            return new TestNode();
        }
    }
}
