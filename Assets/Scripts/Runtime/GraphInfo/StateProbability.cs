namespace Deynai.Markov
{
    public struct StateProbability
    {
        public int Index { get; }
        public ulong NodeID { get; }
        public string StateName { get; }
        public double Probability { get; }

        public StateProbability(string name, double p, int index, ulong id)
        {
            StateName = name;
            Probability = p;
            Index = index;
            NodeID = id;
        }
    }
}
