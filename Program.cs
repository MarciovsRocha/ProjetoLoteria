using System.Collections.Concurrent;

namespace ProjetoLoteria;

public class Program
{
    public static void Main()
    {
        const int tamanhoConjuntoUniverso = 25;
        const int limiteInferior = 11;
        const int limiteSuperior = 15;
        
        #region Gerando Conjuntos de combinações
        var conjuntoUniverso = new List<int>();
        for (int i = 0; i < tamanhoConjuntoUniverso; i++) conjuntoUniverso.Add(i);
        var results = new ConcurrentDictionary<int, ConcurrentBag<List<int>>>();
        Parallel.For(limiteInferior, limiteSuperior+1, i =>
        {
            var combinations = new ConcurrentBag<List<int>>();
            foreach (var combination in GetCombinations(conjuntoUniverso, i))
            {
                combinations.Add(combination.ToList());
            }
            results[i] = combinations;
        });
        #endregion
        // E.g. to get combinations for C25,15 you can use results[15]
        #region contém todas as combinações em formato de IList (otimização)

        List<List<int>> c2514Sequences = new List<List<int>>(); 
        List<List<int>> c2513Sequences = new List<List<int>>();
        List<List<int>> c2512Sequences = new List<List<int>>();
        List<List<int>> c2511Sequences = new List<List<int>>();
        if (results.ContainsKey(14))
        {
            results.Remove(14, out var auxOut);
            c2514Sequences = auxOut!.Select(list => list.ToList()).ToList();
        }
        if (results.ContainsKey(13))
        {
            results.Remove(13, out var auxOut);
            c2513Sequences = auxOut!.Select(list => list.ToList()).ToList();
        }
        if (results.ContainsKey(12))
        {
            results.Remove(12, out var auxOut);
            c2512Sequences = auxOut!.Select(list => list.ToList()).ToList();
        }
        if (results.ContainsKey(11))
        {
            results.Remove(11, out var auxOut);
            c2511Sequences = auxOut!.Select(list => list.ToList()).ToList();
        }
        #endregion 
        
        #region CheckSubSequence
        // Função que verifica se uma lista é um subconjunto de outra lista.
        Func<List<int>, List<int>, bool> CheckSubsequence = (subset, set) => 
        {
            var hashSet = new HashSet<int>(set);
            return subset.All(hashSet.Contains);
        };
        #endregion
        
        #region containers para armazenamento de sequencias com subsequencias
        // Prepare um contêiner para armazenar as sequências C25,15 que contêm todas as sequências C25,14.
        List<int> c25_15_SubsetsWithAllC25_14 = new List<int>();
        List<int> c25_15_SubsetsWithAllC25_13 = new List<int>();
        List<int> c25_15_SubsetsWithAllC25_12 = new List<int>();
        List<int> c25_15_SubsetsWithAllC25_11 = new List<int>();
        #endregion

        #region For loop variables
        int qtdC25_14 = 0, qtdC25_13 = 0, qtdC25_12 = 0, qtdC25_11 = 0, max14 = 0, max13 = 0, max12 = 0, max11 = 0;
        #endregion
        
        #region Para cada sequência C25,15 ...
        foreach (var c25_15_Sequence in results[15].Select(list => list.ToList()))
        {
            // Verifique se ela contém alguma sequência C25,14.
            #region count sub-sequences
            qtdC25_14 = c2514Sequences.Where(seq => CheckSubsequence(seq, c25_15_Sequence)).Count();
            qtdC25_13 = c2513Sequences.Where(seq => CheckSubsequence(seq, c25_15_Sequence)).Count();
            qtdC25_12 = c2512Sequences.Where(seq => CheckSubsequence(seq, c25_15_Sequence)).Count();
            qtdC25_11 = c2511Sequences.Where(seq => CheckSubsequence(seq, c25_15_Sequence)).Count();
            #endregion

            // Se c25_15_Sequence contém todas as sequências C25,14, adicione-a ao nosso contêiner.
            #region check C25,14
            if (max14 <= qtdC25_14)
            {
                c25_15_SubsetsWithAllC25_14 = c25_15_Sequence;
                max14 = qtdC25_14;
            }
            #endregion
            
            #region check C25,13
            if (max13 <= qtdC25_13)
            {
                c25_15_SubsetsWithAllC25_13 = c25_15_Sequence;
                max13 = qtdC25_13;
            }
            #endregion
            
            #region check C25,12
            if (max12 <= qtdC25_12)
            {
                c25_15_SubsetsWithAllC25_12 = c25_15_Sequence;
                max12 = qtdC25_12;
            }
            #endregion

            #region check C25,11
            if (max11 <= qtdC25_11)
            {
                c25_15_SubsetsWithAllC25_11 = c25_15_Sequence;
                max11 = qtdC25_11;
            }
            #endregion
            
        }
        #endregion
        
        
        Console.WriteLine("============================================================");
        Console.WriteLine("Resultados:");
        Console.WriteLine($"<<<<<<\nC25,15 que melhor agrega C25,14:\n[{ImprimirListaConjunto(c25_15_SubsetsWithAllC25_14)}]\nSub conjuntos identificados para esta sequencia: {max14}");
        Console.WriteLine($"<<<<<<\nC25,15 que melhor agrega C25,13:\n[{ImprimirListaConjunto(c25_15_SubsetsWithAllC25_13)}]\nSub conjuntos identificados para esta sequencia: {max13}");
        Console.WriteLine($"<<<<<<\nC25,15 que melhor agrega C25,12:\n[{ImprimirListaConjunto(c25_15_SubsetsWithAllC25_12)}]\nSub conjuntos identificados para esta sequencia: {max12}");
        Console.WriteLine($"<<<<<<\nC25,15 que melhor agrega C25,11:\n[{ImprimirListaConjunto(c25_15_SubsetsWithAllC25_11)}]\nSub conjuntos identificados para esta sequencia: {max11}");
        Console.WriteLine();
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

    public static string ImprimirListaConjunto(IList<int> lista)
    {
        return string.Join(",", lista.Select(i => i.ToString()));
    }
}