using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;

namespace MoogleEngine;
public class Obtein
{
    public string[] documents { get; }
    public List<Tuple<string, int[]>> vocabulary { get; }
    public Dictionary<string, string[]> words { get; }
    public int[] wordsCount { get; }



    public Obtein()
    {
        string carpeta = Path.Combine(Directory.GetParent(".")!.ToString(), "Content");
        documents = Directory.GetFiles(carpeta, "*.txt", SearchOption.AllDirectories);
        words = Obtein.Words(documents);
        vocabulary = Obtein.Vocabulary(words);
        wordsCount = Obtein.WordsCaunt(words);
    }


    public static List<Tuple<string, int[]>> Vocabulary(Dictionary<string, string[]> words)
    {
        List<Tuple<string, int[]>> vocabulary = new List<Tuple<string, int[]>>();
        int i = 0;

        foreach (var file in words)
        {
            string[] text = file.Value;

            //convertir a lista de palabras unicas asociadas con su frecuencia
            for (int j = 0; j < text.Length; j++)
            {
                string word = text[j];
                bool wordFund = false;

                foreach (Tuple<string, int[]> tupla in vocabulary)
                {
                    if (tupla.Item1 == word)
                    {
                        tupla.Item2[i] += 1;
                        wordFund = true;
                    }
                }
                if (!wordFund)
                {
                    int[] freq = new int[words.Count];
                    freq[i] = 1;

                    vocabulary.Add(Tuple.Create(word, freq));
                }
            }
            i++;
        }
        return vocabulary;
    }

    public static string[] NormalizeText(string content)
    {
        //Leer y normalizar texto
        content = content.ToLower();
        //el patron dentro de regex hace que se elimine todo lo que aparece el el texto con excepcion de las letras minúsculas, las letras mayúsculas y los numeros
        Regex patron = new Regex("[^a-zA-Z0-9 ]");
        content = patron.Replace(content.Normalize(System.Text.NormalizationForm.FormD), "");

        //convertir a string[] y ordenar
        string[] words = content.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        return words;
    }

    public static double CosineSimilarity(double[] a, double[] b)
    {
        double dotProduct = 0;
        double normA = 0;
        double normB = 0;
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] != 0 && b[i] != 0) dotProduct += a[i] * b[i];
            if (a[i] != 0) normA += a[i] * a[i];
            if (b[i] != 0) normB += b[i] * b[i];
        }
        return dotProduct / (Math.Sqrt(normA) * Math.Sqrt(normB));
    }

    public static Dictionary<string, string[]> Words(string[] documents)
    {
        Dictionary<string, string[]> words = new Dictionary<string, string[]>();
        string[] text;

        for (int i = 0; i < documents.Length; i++)
        {
            StreamReader reader = new StreamReader(documents[i]);
            string content = reader.ReadToEnd();
            text = Obtein.NormalizeText(content);
            Array.Sort(text);
            words.Add(documents[i], text);
        }

        return words;
    }

    public static int[] WordsCaunt(Dictionary<string, string[]> words)
    {
        int[] wordsCount = new int[words.Count];
        int i = 0;
        foreach (var item in words)
        {
            string[] text = item.Value;
            wordsCount[i] = text.Length;
            i++;
        }
        return wordsCount;
    }

    public static string[] ComunWords(string a)
    {
        List<string> palabrasComunes = new List<string> { "el", "la", "los", "las", "un", "una", "unos", "unas", "y", "o", "pero", "por", "para", "con", "contra", "sin", "de", "del", "al", "a", "ante", "cabe", "con", "desde", "en", "entre", "hacia", "hasta", "segun", "so", "sobre", "tras" };
        string query = a.ToLower();
        Regex patron = new Regex("[^a-zA-Z0-9 ]");
        query = patron.Replace(query.Normalize(System.Text.NormalizationForm.FormD), "");
        string[] temp = query.Split();

        List<string> palabras = temp.ToList();
        palabras.RemoveAll(p => palabrasComunes.Contains(p.ToLower()));
        
        string[] palabrasFinales = palabras.ToArray();

        return palabrasFinales;
    }

    public static int FindLineNumber(string title, string query)
    {
        //se lee linea a linea del texto para encontrar en cual se halla la frase o palabra q buscamos para q nos devuelva el numero de esa linea
        StreamReader reader = new StreamReader(title);
        int lineNumber = 0;
        while (!reader.EndOfStream)
        {
            lineNumber++;
            string line = reader.ReadLine()!;
            line = line.ToLower();
            if (line.Contains(query)) return lineNumber;
        }
        return -1;
    }

    public static string GetLine(string title, int lineNumber)
    {
        //una vez obtenido el numero de la linea en la que se encuentra la palabra que buscamos con este metodo accedemos a ella apara poder mostrarla
        StreamReader reader = new StreamReader(title);
        for (int i = 1; i < lineNumber; i++)
        {
            reader.ReadLine();
        }
        return reader.ReadLine()!;
    }

}