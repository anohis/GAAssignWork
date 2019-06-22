using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAAssignWork.GA
{
    public class GeneticAlgorithm 
    {
        public event Action<int[]> OnCreateChromosomeEvent;
        public event Func<int[], float> OnCalculateFitnessEvent;
        public event Action<int, int[], int[]> OnCrossOverEvent;
        public event Action<int, int[]> OnMutationEvent;

        private List<Chromosome> _chromosomes = new List<Chromosome>();
        private readonly int _maxBit;
        private readonly int _chromosomeCount;
        private readonly int _selectCount;
        private readonly float _terminateValue;
        private Random _random = new Random();

        public GeneticAlgorithm(int maxBit, int chromosomeCount, int selectCount, float terminateValue)
        {
            if (chromosomeCount <= selectCount)
            {
                throw new Exception("[GeneticAlgorithm] chromosomeCount <= selectCount");
            }

            _maxBit = maxBit;
            _chromosomeCount = chromosomeCount;
            _selectCount = selectCount;
            _terminateValue = terminateValue;
        }

        public int[] Execute(int loopCount)
        {
            if (loopCount <= 0)
            {
                throw new Exception("[GeneticAlgorithm.Execute] loopCount <= 0");
            }

            while (loopCount > 0)
            {
                CheckChromosomeCount();
                CalculateFitness();
                if (CheckTerminate())
                {
                    break;
                }
                Selection(_selectCount);
                CrossOver();
                Mutation();

                Console.WriteLine(string.Format("Loop = {0}, OptimalFitness = {1}"
                    , loopCount, _chromosomes[0].Fitness));

                loopCount--;
            }

            Selection(1);
            Console.WriteLine("");
            Console.WriteLine(string.Format("Loop = {0}, OptimalFitness = {1}"
                    , loopCount, _chromosomes[0].Fitness));
            Console.WriteLine("");
            return _chromosomes[0].Value;
        }
        private void Mutation()
        {
            for (int i = _selectCount; i < _chromosomes.Count; i++)
            {
                Chromosome chr = _chromosomes[i];
                int index = _random.Next(0, _maxBit);
                chr.Value[index] = (chr.Value[index] + 1) % 2;
                OnMutationEvent?.Invoke(index, chr.Value);
            }
        }
        private void CrossOver()
        {
            List<Chromosome> childs = new List<Chromosome>();
            while (childs.Count + _chromosomes.Count < _chromosomeCount)
            {
                List<Chromosome> pool = new List<Chromosome>(_chromosomes);
                int index = _random.Next(0, pool.Count);
                Chromosome a = pool[index].Clone();
                pool.RemoveAt(index);
                index = _random.Next(0, pool.Count);
                Chromosome b = pool[index].Clone();

                index = _random.Next(0, _maxBit);
                var temp = a.Value[index];
                a.Value[index] = b.Value[index];
                b.Value[index] = temp;

                OnCrossOverEvent?.Invoke(index, a.Value, b.Value);

                childs.Add(a);
                childs.Add(b);
            }

            _chromosomes.AddRange(childs);
        }
        private void Selection(int selectCount)
        {
            _chromosomes.Sort((a, b) => 
            {
                return Math.Sign(a.Fitness - b.Fitness);
            });
            _chromosomes.Reverse();
            _chromosomes.RemoveRange(selectCount, _chromosomes.Count - selectCount);
        }
        private bool CheckTerminate()
        {
            foreach (var chr in _chromosomes)
            {
                if (chr.Fitness >= _terminateValue)
                {
                    return true;
                }
            }
            return false;
        }
        private void CalculateFitness()
        {
            if (OnCalculateFitnessEvent == null)
            {
                return;
            }

            foreach (var chr in _chromosomes)
            {
                chr.Fitness = OnCalculateFitnessEvent.Invoke(chr.Value);
            }
        }
        private void CheckChromosomeCount()
        {
            while (_chromosomes.Count < _chromosomeCount)
            {
                Create();
            }
        }
        private void Create()
        {
            Chromosome chr = new Chromosome(_maxBit);
            _chromosomes.Add(chr);
            OnCreateChromosomeEvent?.Invoke(chr.Value);
        }
    }
}
