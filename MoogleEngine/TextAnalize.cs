using System.IO.MemoryMappedFiles;
using System.Linq;

namespace MoogleEngine;

public class TextAnalize
{
    static double[]? idf;
    static double[,]? tf_idf;
    static Obtein? a;

    public TextAnalize()
    {
        a = new Obtein();
        double[,] tf = TF();
        idf = IDF();
        tf_idf = TF_IDF(tf);
    }

    public static double[,] TF()
    {
        Dictionary<string, int[]> vocabulary = a!.vocabulary;
        int[] wordsCount = a!.wordsCount;

        //se crea una matriz de array doble tal que las filas representen cada documento y las columnas el valor de las palabras únicas en cada documento
        double[,] tf = new double[wordsCount.Length, vocabulary.Count];
        int j = 0;
        //se recorre el vocabulario 
        foreach (var item in vocabulary)
        {
            //se extrae el value que es un array del tamaño de la cantidad de documentos 
            int[] freqVal = item.Value;
            //se recorre cada posición de la columna que representa la palabra única para colocar los valores correspondientes
            for (int i = 0; i < tf.GetLength(0); i++)
            {
                int x = wordsCount[i];
                int freq = freqVal[i];
                tf[i, j] = (double)freq / x;
            }
            j++;
        }

        return tf;
    }

    public static double[] IDF()
    {
        Dictionary<string, int[]> vocabulary = a!.vocabulary;
        double[] idf = new double[vocabulary.Count];
        int i = 0;

        foreach (var item in vocabulary)
        {
            //se extrae el value para obtener las frecuencias por documento, y su tamaño nos da la cantidad de documentos
            int[] freq = item.Value;
            int x = freq.Length;
            int caunt = 0;

            //se recorre cada valor del array de frecuencias y se lleva la cuenta de en cuantos es distinta de 0 para saber en cuantos documentos aparece la palabra
            for (int j = 0; j < freq.Length; j++)
            {
                if (freq[j] != 0) caunt++;
            }

            idf[i] = Math.Log((double)(x + 1) / (double)caunt);
            i++;
        }

        return idf;
    }

    public static double[,] TF_IDF(double[,] tf)
    {
        //se crea una matriz de array doble tal que las filas representen cada documento y las columnas el valor relacionado a las palabras únicas en cada documento
        double[,] tf_idf = new double[tf.GetLength(0), tf.GetLength(1)];
        //se recorre cada columna de la matriz de tf y con el mismo indice se obtiene el valor de idf que le corresponde a la palabra de la columna
        for (int j = 0; j < tf.GetLength(1); j++)
        {
            double idfValues = idf![j];

            //se recorre cada valor de la columna y se multiplica el valor de tf correspondiente con el idf ya definido
            for (int i = 0; i < tf_idf.GetLength(0); i++)
            {
                double tfValues = tf[i, j];
                if (tfValues != 0) tf_idf[i, j] = tfValues * idfValues;
            }
        }
        return tf_idf;
    }

    public double[] QueryVec(string query)
    {
        Dictionary<string, int[]> vocabulary = a!.vocabulary;
        
        //solo se recibe una palabra como la que presenta el operador
        //se busca la palabra que tenga el operador '*' y la cant de veces que se repite el mismo para aumentarle el valor de tf y asi su relevancia en la búsqueda
        //se elimina con el normalizador los símbolos y se vuelve a convertir en string
        string temp = Obtein.Oper(query, '*');
        int count = 0;
        if (temp != null)
        {
            foreach (char y in temp)
            {
                if (y == '*') count++;
                else break;
            }

            string[] x = Obtein.NormalizeText(temp!);
            temp = String.Join(" ", x);
        }

        string[] queryWords = Obtein.NormalizeText(query);

        //el vector del query tiene que tener el mismo tamaño que las filas de las matrices con las que se trabaja 
        //que en este caso sería un tamaño igual a la cantidad de palabras únicas.
        double[] queryVector = new double[vocabulary.Count];
        int i = 0;

        foreach (var item in vocabulary)
        {
            double tf = 0;
            int c = 0;
            string word = item.Key;

            //se busca cuantas veces se repite la misma palabra 
            for (int j = 0; j < queryWords.Length; j++)
            {
                if (queryWords[j] == word) c++;
            }
            if (temp != null)
            {
                if (word == temp) c += count * 2;
            }
            tf = (double)c / queryWords.Length;
            
            //para el valor de idf se emplea un indice que coincide en la posición de cada palabra
            queryVector[i] = tf * idf![i];
            i++;
        }

        return queryVector;
    }

    public Dictionary<string, double> Score(double[] queryVector, string query)
    {
        string[] documents = a!.documents;

        Dictionary<string, double> score = new Dictionary<string, double>();

        //se extraen los valores de cada fila de la matriz de tf*idf que representan los valores de cada documento para calcular su similitud con el query
        //luego se añaden al diccionario cuya key es el documento y su value la similitud de cosenos
        for (int i = 0; i < documents.Length; i++)
        {
            double[] docVec = new double[tf_idf!.GetLength(1)];
            double temp = 0;

            for (int j = 0; j < tf_idf!.GetLength(1); j++)
            {
                docVec[j] = tf_idf[i, j];
            }

            temp = Obtein.CosineSimilarity(queryVector, docVec);
            if (temp > 0) score.Add(documents[i], temp);
        }

        List<string> remove = UnimportantText(query);
        if (remove != null)
        {
            foreach (string key in remove)
            {
                if (score.ContainsKey(key)) score.Remove(key);
            }
        }

        return score;
    }

    static string Snippet(string title, string query)
    {
        string[] words = Obtein.ComunWords(query);

        StreamReader reader = new StreamReader(title);
        string content = reader.ReadToEnd();
        string[] back = content.Split(" ");
        string[] text = Obtein.NormalizeText(content);

        int x = 50;
        if (text.Length <= 80) x = text.Length / 2;

        int maxCount = 0;
        int startIndex = 0;
        int endIndex = x - 1;

        int count = 0;
        //cuenta la cantidad de repeticiones de las palabras en la búsqueda
        string[] fragment = text[0..(x - 1)];
        for (int i = 0; i < words.Length; i++)
        {
            count += fragment.Count(w => w == words[i]);
        }
        maxCount = count;

        for (int i = 1, j = x; i < (text.Length - x) || (j < text.Length); i++, j++)
        {
            for (int k = 0; k < words.Length; k++)
            {
                if (words[k] == text[i - 1]) count--;
                if (words[k] == text[j]) count++;
            }

            if (count > maxCount)
            {
                maxCount = count;
                startIndex = i;
                endIndex = j;
            }
        }

        if ((endIndex != back.Length) && ((endIndex + 30) <= back.Length))
        {
            endIndex += 30;
        }
        else    endIndex = back.Length;

        if ((startIndex != 0) && ((startIndex - 15) >= 0))
        {
            startIndex -= 15;
        }
        else    startIndex = 0;

        string snippet = String.Join(" ", back.Skip(startIndex).Take(endIndex - startIndex - 1));

        return snippet;
    }

    public SearchItem[] Query(string query)
    {
        double[] vector = QueryVec(query);
        Dictionary<string, double> score = Score(vector, query);
        SearchItem[] search = new SearchItem[score.Count];

        int index = 0;

        //utilizando el diccionario del score se itera a través de cada key que es empleada para escoger la última sección separada por "/" que es el título exacto del documento y para encontrar el snippet correspondiente
        //esto se va añadiendo al SearchItem que será llamado desde la clase MOogle de donde será ejecutada la respuesta final del programa
        //solo se realizan los procesos si el valor de score que se encuentra dentro del cada value del diccionario es distinto de 0
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

    public static List<string> UnimportantText(string query)
    {
        Dictionary<string, string[]> words = a!.words;
        //Dictionary<string, int[]> vocabulary = a.vocabulary;

        List<string> remove = new List<string>();

        string not = Obtein.Oper(query, '^');
        string dont = Obtein.Oper(query, '!');

        if (not != null || dont != null)
        {
            //se busca en cada texto analizado si la palabra con el operador se encuentra
            //de no ser así se añade a la lista el título del libro

            foreach (var item in words)
            {
                string[] text = item.Value;

                if (not != null)
                {
                    string[] temp = Obtein.NormalizeText(not!);
                    not = String.Join(" ", temp);

                    int index = Array.IndexOf(text, not);
                    if (index < 0) remove.Add(item.Key);
                }

                if (dont != null)
                {
                    string[] temp = Obtein.NormalizeText(dont);
                    dont = String.Join(" ", temp);

                    int index = Array.IndexOf(text, dont);
                    if (index >= 0 && !remove.Contains(item.Key)) remove.Add(item.Key);
                }
            }
        }

        return remove;
    }
}