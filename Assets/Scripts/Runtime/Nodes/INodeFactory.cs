using Deynai.Markov;
using UnityEngine;

public interface INodeFactory
{
    INode Create(Vector2 position);
}