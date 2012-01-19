using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MnistConverter
{
    public class CounterList<T>
    {
        Dictionary<T, int> counterListDictionary = new Dictionary<T, int>();

        public void Set(T iObject)
        {
            if (counterListDictionary.ContainsKey(iObject))
            {
                int value = counterListDictionary[iObject];
                value++;
                counterListDictionary[iObject] = value;
            }
            else
            {
                counterListDictionary.Add(iObject, 1);
            }
        }

        public int Get(T iObject)
        {
            if (counterListDictionary.ContainsKey(iObject))
            {
                return counterListDictionary[iObject];
            }
            return 0;
        }

    }
}
