using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ProjetoLoteria;

public class Program
{
    
    
    public static void Main()
    {
        const int CONJUNTO_MUNDO = 25;
        const int LIMITE_INFERIOR = 11;
        const int LIMITE_SUPERIOR = 16;
        var list = new List<int>();
        for (int i = 0; i < CONJUNTO_MUNDO; i++) list.Add(i);

        var results = new ConcurrentDictionary<int, ConcurrentBag<List<int>>>();

        Parallel.For(LIMITE_INFERIOR, LIMITE_SUPERIOR, i =>
        {
            var combinations = new ConcurrentBag<List<int>>();
            foreach (var combination in GetCombinations(list, i))
            {
                combinations.Add(combination.ToList());
            }
            results[i] = combinations;
        });
        // Now you have a dictionary containing the combinations
        // E.g. to get combinations for size 15, you can use results[15]
        Console.WriteLine(results);
        
        var filteredResults = results[14].Where(
            combination14 => results[15].Any(combination15 => new HashSet<int>(combination15).IsSupersetOf(combination14))
        ).ToList();
        
        Console.WriteLine(results);
    }
    
    public static IEnumerable<IEnumerable<T>> GetCombinations<T>(List<T> list, int length)
    {
        if (length == 0)
        {
            yield return new T[0];
        }
        else
        {
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                var rest = list.GetRange(i + 1, list.Count - i - 1);
                foreach (var result in GetCombinations(rest, length - 1))
                {
                    yield return new T[] { item }.Concat(result);
                }
            }
        }
    }
}