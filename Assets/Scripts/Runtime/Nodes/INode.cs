using System.Collections.Generic;
using UnityEngine;

namespace Deynai.Markov
{
    public interface INode
    {
        ulong ID { get; }
        string Name { get; set; }

        void SetID(ulong id);

        IReadOnlyList<ISocket> Sockets { get; }

        RectTransform RectTransform { get; }

        void SelectEntered();
        void SelectExited();

        void HoverEntered();
        void HoverExited();

        void Remove();
    }
}