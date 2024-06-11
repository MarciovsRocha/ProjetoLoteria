using System.Collections.Concurrent;
using System.Text;

namespace ProjetoLoteria;

public class Program
{
    public static void Main()
    {
        const int tamanhoConjuntoUniverso = 25;
        #region Get user input for combinacao menor
        Console.Write("Informar a combinação menor (Integer input): ");
        string userInput = Console.ReadLine()!;
        int number;
        bool success = Int32.TryParse(userInput, out number);
        if (!success)
        {
            Console.WriteLine("You entered an invalid integer number: " + userInput);
            Environment.Exit(0);
        } 
        int limiteInferior = number;
        #endregion
        #region Get user input for Heuristica
        Console.Write("Informar o ajuste da heuristica (Integer input): ");
        userInput = Console.ReadLine()!;
        success = Int32.TryParse(userInput, out number);
        if (!success)
        {
            Console.WriteLine("You entered an invalid integer number: " + userInput);
            Environment.Exit(0);
        }

        int percHeuristica = number;
        #endregion
        
        const int limiteSuperior = 15;
        
        #region Gerando Conjuntos de combinações
        Console.WriteLine($"Gerando os conjuntos das combinações [C25,15 C25,{limiteInferior}]");
        var conjuntoUniverso = new List<int>();
        for (int i = 0; i < tamanhoConjuntoUniverso; i++) conjuntoUniverso.Add(i);
        var results = new ConcurrentDictionary<int, ConcurrentBag<List<int>>>();
        Parallel.ForEach([limiteInferior, limiteSuperior], i =>
        {
            var combinations = new ConcurrentBag<List<int>>();
            foreach (var combination in GetCombinations(conjuntoUniverso, i))
            {
                combinations.Add(combination.ToList());
            }
            results[i] = combinations;
        });
        #endregion
        
        // to get combinations for C25,15 you can use results[15]
      
        
        #region CheckSubSequence
        // Função que verifica se uma lista é um subconjunto de outra lista.
        Func<List<int>, List<int>, bool> CheckSubsequence = (subset, set) => 
        {
            var hashSet = new HashSet<int>(set);
            return subset.All(hashSet.Contains);
        };
        #endregion

        #region For loop variables
        int qtdC25_n = 0, maxN = 0;
        List<int> c25_15_SubsetsWithAllC25_N = new List<int>();
        #endregion
        Console.WriteLine("Verificando combinações...");
        #region Para cada sequência C25,15 ...
        foreach (var c25_15_Sequence in results[15].ToArray())
        {
            // Heuristica com base na % do grupo preenchido
            if (((double)percHeuristica / 100000) <= ((double)maxN / results[limiteInferior].Count))
                break;
            // Verifique se ela contém alguma sequência C25,14.
            #region count sub-sequences
            // Console.WriteLine($"Verificando combinações agregadas para [{ImprimirListaConjunto(c25_15_Sequence)}]...");
            // heurística para pular a diferença entre os conjuntos, para aumentar a variedade entre as comparações
            // funciona como uma espécie de shuffle
            if (0 != results[15].ToList().IndexOf(c25_15_Sequence) % limiteInferior)
            {
                continue;
            }

            qtdC25_n = results[limiteInferior].Where(seq => CheckSubsequence(seq, c25_15_Sequence)).Count();
            #region check C25,limiteInferior
            if (maxN >= qtdC25_n)
            {
                continue;
            }
            Console.WriteLine($"Nova combinação com maior agregação para C25,{limiteInferior}");
            c25_15_SubsetsWithAllC25_N = c25_15_Sequence;
            maxN = qtdC25_n;
            #endregion
            
            #endregion
            // Se c25_15_Sequence contém todas as sequências C25,14, adicione-a ao nosso contêiner.
        }
        #endregion


        StringBuilder sb = new StringBuilder();
        sb.AppendLine("============================================================")
            .AppendLine("Resultados:")
            .AppendLine($"<<<<<<\nC25,15 que melhor agrega C25,{limiteInferior}:\n[{ImprimirListaConjunto(c25_15_SubsetsWithAllC25_N)}]\nSub conjuntos identificados para esta sequencia: {maxN}");
        Console.WriteLine(sb.ToString());
        string fileOutput = Path.Combine(Directory.GetCurrentDirectory(), $"ExportResult_C25-{limiteSuperior}_C25-{limiteInferior}.txt");
        using (StreamWriter streamWriter = new StreamWriter(fileOutput))
        {
            streamWriter.Write(sb.ToString());
        }
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