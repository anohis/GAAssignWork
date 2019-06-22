using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAAssignWork.GA
{
    public class Chromosome
    {
        public int[] Value;
        public float Fitness;
        public Chromosome(int maxBit)
        {
            Value = new int[maxBit];
        }

        public Chromosome Clone()
        {
            Chromosome clone = new Chromosome(Value.Length);
            for (int i = 0; i < Value.Length; i++)
            {
                clone.Value[i] = Value[i];
            }
            return clone;
        }
    }
}
