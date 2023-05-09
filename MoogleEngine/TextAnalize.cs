using System.IO.MemoryMappedFiles;
using System.Linq;

namespace MoogleEngine;

public class TextAnalize
{
    static double[]? idf;
    static List<double[]>? tf_idf;

    static Obtein? a;

    public TextAnalize()
    {
        a = new Obtein();
        List<double[]> tf = TF();
        idf = IDF();
        tf_idf = TF_IDF(tf);
    }

    public static List<double[]> TF()
    {
        List<Tuple<string, int[]>> vocabulary = a!.vocabulary;
        int[] wordsCount = a!.wordsCount;

        List<double[]> tf = new List<double[]>();

        foreach (Tuple<string, int[]> tupla in vocabulary)
        {
            int[] freqVal = tupla.Item2;
            double[] tfValues = new double[freqVal.Length];
            for (int i = 0; i < tfValues.Length; i++)
            {
                int x = wordsCount[i];
                int freq = freqVal[i];
                tfValues[i] = (double)freq / x;
            }
            tf.Add(tfValues);
        }

        return tf;
    }

    public static double[] IDF()
    {
        List<Tuple<string, int[]>> vocabulary = a!.vocabulary;
        double[] idf = new double[vocabulary.Count];
        int i = 0;

        foreach (Tuple<string, int[]> tupla in vocabulary)
        {
            int[] freq = tupla.Item2;
            int x = freq.Length;
            int caunt = 0;

            for (int j = 0; j < freq.Length; j++)
            {
                if (freq[j] != 0) caunt++;
            }

            idf[i] = Math.Log((double)(x + 1) / (double)caunt);
            i++;
        }

        return idf;
    }

    public static List<double[]> TF_IDF(List<double[]> tf)
    {
        List<double[]> tf_idf = new List<double[]>();
        int j = 0;
        foreach (var list in tf)
        {
            double[] tf_idfValues = new double[list.Length];
            double idfValues = idf![j];

            for (int i = 0; i < tf_idfValues.Length; i++)
            {
                double tfValues = list[i];
                if (tfValues != 0) tf_idfValues[i] = tfValues * idfValues;
            }

            tf_idf.Add(tf_idfValues);
            j++;
        }

        return tf_idf;
    }

    public double[] QueryVec(string query)
    {
        List<Tuple<string, int[]>> vocabulary = a!.vocabulary;

        string[] queryWords = Obtein.NormalizeText(query);
        double[] queryVector = new double[vocabulary.Count];
        int i = 0;

        foreach (Tuple<string, int[]> tupla in vocabulary)
        {
            double tf = 0;
            int c = 0;
            string word = tupla.Item1;

            for (int j = 0; j < queryWords.Length; j++)
            {
                if (queryWords[j] == word) c++;
            }
            tf = (double)c / queryWords.Length;

            queryVector[i] = tf * idf![i];
            i++;
        }

        return queryVector;
    }

    public Dictionary<string, double> Score(double[] queryVector)
    {
        string[] documents = a!.documents;

        Dictionary<string, double> score = new Dictionary<string, double>();

        for (int i = 0; i < documents.Length; i++)
        {
            double[] docVec = new double[tf_idf!.Count];
            double temp = 0;
            int j = 0;

            foreach (double[] item in tf_idf)
            {
                docVec[j] = item[i];
                j++;
            }

            temp = Obtein.CosineSimilarity(queryVector, docVec);
            score.Add(documents[i], temp);
        }

        return score;
    }

    static string Snippet(string title, string query)
    {
        //normalizar el query para poder utilizarlo en caso de q no aparezca la frase completa que buscamos
        string[] querySplit = Obtein.ComunWords(query);
        int lineNumber = Obtein.FindLineNumber(title, query);
        string line;

        if (lineNumber != -1)
        {
            // obtener la linea en la que se encuentra la expresion que buscamos para devolverla
            line = Obtein.GetLine(title, lineNumber);
            return line;
        }
        else
        {
            //como no se encontro la frase completa se empieza a buscar palabta por palabra dentro del query hasta que alguna coincida para devolver esa linea de texto
            for (int i = 0; i < querySplit.Length; i++)
            {
                lineNumber = Obtein.FindLineNumber(title, querySplit[i]);
                if (lineNumber != -1)
                {
                    line = Obtein.GetLine(title, lineNumber);
                    return line;
                }
            }
        }

        // como no se encontro ninguna de las palabras del query dentro del texto se devuelve la primera linea del .txt
        StreamReader reader = new StreamReader(title);
        line = reader.ReadLine()!;
        return line;
    }

    public SearchItem[] Query(string query)
    {
        double[] vector = QueryVec(query);
        Dictionary<string, double> score = Score(vector);
        SearchItem[] search = new SearchItem[score.Count];

        int index = 0;

        foreach (var item in score)
        {
            string direction = item.Key;
            string[] text = direction.Split("/");
            string title = text[text.Length - 1];
            string snippet = TextAnalize.Snippet(direction, query);
            search[index] = new SearchItem(title, snippet, (float)item.Value);
            index++;
        }

        return search;
    }
}