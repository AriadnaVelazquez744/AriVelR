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

        //se crea una matriz de array doble tal que las filas representen cada documento y las columas el valor de las palabras unicas en cada documento
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
            //se extrae el value para obtener las frecuencias por documneto, y su tamaño nos da la cantidad de documentos
            int[] freq = item.Value;
            int x = freq.Length;
            int caunt = 0;

            //se recorre cada valor del array de frecuencias y se lleva la cuenta de en cuantos es distinta de 0 para saber encuantos documentos aparece la palabra
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
        //se crea una matriz de array doble tal que las filas representen cada documento y las columas el valor relacionado a las palabras unicas en cada documento
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
        //se elimina con el normalizador los simbolos y se vuelve a convertir en string
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
            
            //para el valor de idf se emplea un indice que coincide en la posicion de cada palabra
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
        //luego se añaden al diccionario cuya key es el documento y su value la similitu de cosenos
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
        //normalizar el query para poder utilizarlo en caso de que no aparezca la frase completa que buscamos
        //al mimo tiempo que se le eliminan las preposiciones, conjunciones y articulos
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

        //si no se encontra ninguna de las palabras del query dentro del texto se devuelve la primera linea del .txt
        //esto también ocurre si la única palabra de la busqueda que aparece en el doc es una de las eliminadas por ser extremadamente comunes
        StreamReader reader = new StreamReader(title);
        line = reader.ReadLine()!;
        return line;
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
            //de no ser así se añade a la lista el titulo del libro

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