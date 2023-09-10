using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;

namespace MoogleEngine;
public class Obtein
{
    public string[] documents;
    public Dictionary<string, int[]> vocabulary;
    public Dictionary<string, string[]> words;
    public int[] wordsCount;

    public Obtein()
    {
        string carpeta = Path.Combine(Directory.GetParent(".")!.ToString(), "Content");
        documents = Directory.GetFiles(carpeta, "*.txt", SearchOption.AllDirectories);
        words = Obtein.Words(documents);
        vocabulary = Obtein.Vocabulary(words);
        wordsCount = Obtein.WordsCaunt(words);
    }

    public static Dictionary<string, int[]> Vocabulary(Dictionary<string, string[]> words)
    {
        //el key es la palabra y el value, cuyo tamaño va a ser igual a la cantidad de documentos, es la cantidad de veces que se repite dicha palabra en cada doc
        Dictionary<string, int[]> vocabulary = new Dictionary<string, int[]>();
        int i = 0;

        //recorre cada uno de los documentos que están en el diccionario "words"
        foreach (var file in words)
        {
            //tomar el texto completo normalizado perteneciente a cada documento
            string[] text = file.Value;

            //convertir a lista de palabras únicas asociadas con su frecuencia
            for (int j = 0; j < text.Length; j++)
            {
                string word = text[j];
                bool wordFund = false;

                //se revisa si existe una key en el vocabulario que coincida con la word y se obtiene su value. 
                //en caso de encontrarla se añade 1 a su valor de frecuencia en la posición correspondiente del texto.
                //si no hay coincidencia, se añade la palabra al diccionario con valor de freq 1 en la posición correspondiente dentro del array.
                //el valor booleano wordFund es el indicador de si hay q añadir o no la palabra dado que indica su existencia.
                if (vocabulary.TryGetValue(word, out int[] value))
                {
                    value[i] += 1;
                    wordFund = true;
                }
                if (!wordFund)
                {
                    int[] freq = new int[words.Count];
                    freq[i] = 1;

                    vocabulary.Add(word, freq);
                }
            }
            i++;
        }
        return vocabulary;
    }

    public static string[] NormalizeText(string content)
    {
        //poner todas las palabras en minúscula
        content = content.ToLower();
        //el patron dentro de regex hace que se elimine todo lo que aparece el el texto con excepción de las letras minúsculas, las letras mayúsculas y los números
        Regex patron = new Regex("[^a-zA-Z0-9 ]");
        content = patron.Replace(content.Normalize(System.Text.NormalizationForm.FormD), "");

        //convertir a string[]
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
        //tiene como key las direcciones de los documentos analizador y como value su contenido normalizado
        Dictionary<string, string[]> words = new Dictionary<string, string[]>();
        string[] text;

        //pasando por caa documento, se extrae el texto, se normaliza y se coloca en un array para añadirlo al diccionario
        for (int i = 0; i < documents.Length; i++)
        {
            StreamReader reader = new StreamReader(documents[i]);
            string content = reader.ReadToEnd();
            text = Obtein.NormalizeText(content);
            words.Add(documents[i], text);
        }

        return words;
    }

    public static int[] WordsCaunt(Dictionary<string, string[]> words)
    {
        //contiene el tamaño de cada texto una vez normalizado para que sea más rápido acceder a este dato para hacer el idf
        int[] wordsCount = new int[words.Count];
        int i = 0;
        //pasa por cada value del diccionario "words" que es el texto normalizado para obtener el tamaño de este array
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
        string[] palabrasComunes = new string[] { "el", "la", "los", "las", "un", "una", "unos", "unas", "y", "o", "pero", "por", "para", "con", "contra", "sin", "de", "del", "al", "a", "ante", "cabe", "con", "desde", "en", "entre", "hacia", "hasta", "segun", "so", "sobre", "tras" };
        string query = a.ToLower();
        Regex patron = new Regex("[^a-zA-Z0-9 ]");
        query = patron.Replace(query.Normalize(System.Text.NormalizationForm.FormD), "");

        string palabras = string.Join(" ", query.Split(' ').Where(p => !palabrasComunes.Contains(p)));
        string[] palabrasFinales = palabras.Split(" ");

        return palabrasFinales;
    }

    
   public static string Oper(string sentence, char startChar)
    {
        int startIndex = sentence.IndexOf(startChar);
        if(startIndex == -1) return null!;

        int endIndex = sentence.IndexOf(' ', startIndex);
        if(endIndex == -1) endIndex = sentence.Length;

        return sentence.Substring(startIndex, endIndex - startIndex);
    }

}