using System.Collections.Generic;

namespace Deynai.Markov
{
    public class FinalStateProbability
    {
        public int AbsorbingCount => _absorbingMatrix.Columns;
        public int TransientCount => _absorbingMatrix.Rows;
        public bool Valid => _parser.Valid;

        public List<ulong> TransientIDs => _transientIDs;
        public List<ulong> AbsorbingIDs => _absorbingIDs;

        private Matrix _absorbingMatrix;
        private List<StateProbability>[] _stateLists;
        private double[] _expectedSteps;
        private List<ulong> _transientIDs = new List<ulong>();
        private List<ulong> _absorbingIDs = new List<ulong>();

        private GraphParser _parser;
        private NodeManager _nodeManager;

        public FinalStateProbability(GraphParser parser)
        {
            _parser = parser;
            _nodeManager = ObjectContainer.NodeManager;
            if (!_parser.Valid) return;

            _absorbingMatrix = GenerateAbsorbingMatrix();

            _transientIDs = InitialiseTransientIDList();
            _absorbingIDs = InitialiseAbsorbingIDList();

            _stateLists = InitialiseStateProbabilityLists();
            _expectedSteps = InitialiseStateExpectedSteps();
        }

        public string GetNameForIndex(int index)
        {
            ulong id = _parser.NodeIndexToIDMap[index];
            return _nodeManager.GetNodeForID(id).Name;
        }

        public List<StateProbability> GetStateProbabilities(int index)
        {
            if (_stateLists.Length == 0) return null;
            return _stateLists[index];
        }

        public double GetStateExpectedSteps(int index)
        {
            if (_expectedSteps.Length == 0) return 0;
            return _expectedSteps[index];
        }

        private List<ulong> InitialiseTransientIDList()
        {
            List<ulong> tResult = new List<ulong>();

            for (int i = 0; i < TransientCount; i++)
            {
                tResult.Add(_parser.NodeIndexToIDMap[i]);
            }

            return tResult;
        }
        
        private List<ulong> InitialiseAbsorbingIDList()
        {
            List<ulong> aResult = new List<ulong>();

            for (int i = 0; i < AbsorbingCount; i++)
            {
                aResult.Add(_parser.NodeIndexToIDMap[TransientCount + i]);
            }

            return aResult;
        }

        private List<StateProbability>[] InitialiseStateProbabilityLists()
        {
            // absorbing matrix is an A x T matrix where
            // [T][A] corresponds to probability of being absorbed by state A while starting in state T

            List<StateProbability>[] stateProbabilityLists = new List<StateProbability>[TransientCount];

            for (int tIndex = 0; tIndex < TransientCount; tIndex++)
            {
                ulong startingID = _parser.NodeIndexToIDMap[tIndex];
                List<StateProbability> statesResult = new List<StateProbability>();

                int indexOfStartingID = _parser.NodeIDToIndexMap[startingID];
                for (int i = 0; i < AbsorbingCount; i++)
                {
                    ulong absorbingNodeID = _parser.NodeIndexToIDMap[i + TransientCount];
                    double p = _absorbingMatrix.Value[i + indexOfStartingID * _absorbingMatrix.Columns];

                    StateProbability newStateProbability = new StateProbability(_nodeManager.GetNodeForID(absorbingNodeID).Name, p, i+TransientCount, absorbingNodeID);
                    statesResult.Add(newStateProbability);
                }

                stateProbabilityLists[tIndex] = statesResult;
            }

            return stateProbabilityLists;
        }

        private double[] InitialiseStateExpectedSteps()
        {
            double[] ones = new double[TransientCount];
            for (int i = 0; i < TransientCount; i++)
            {
                ones[i] = 1;
            }

            Matrix onesVector = new Matrix(ones, 1, TransientCount);
            Matrix result = Matrix.Multiply(_parser.Fundamental, onesVector);

            return result.Value;
        }

        private Matrix GenerateAbsorbingMatrix()
        {
            return Matrix.Multiply(_parser.Fundamental, _parser.R);
        }
    }
}
