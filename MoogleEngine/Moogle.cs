namespace MoogleEngine;

public static class Moogle
{
    public static TextAnalize? textAnalize;
    
    public static SearchResult Query(string query)
    {
        //se llama a la función query para obtener los valores a devolver por la interfaz gráfica que serían el título y el snippet 
        //ordena los documentos de mayor a menor en dependencia de su valor de score
        SearchItem[] items = textAnalize!.Query(query);
        SearchItem[] best = items.OrderByDescending(x => x.Score).ToArray();

        return new SearchResult(best, query);
    }

}