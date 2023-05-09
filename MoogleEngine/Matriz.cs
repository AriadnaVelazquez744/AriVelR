
public class Matrix
{
    int[,] matrix;
    public Matrix(int[,] matrix)
    {
        this.matrix = matrix; 
    }

    //para restar matrices se multiplica la matriz por un escalar (MatrixEsc) que es -1 y luegp realizarc la suma (MatrixAdd)
    public static int[,] MatrixAdd(int[,] a, int[,] b)
    {
        int[,] c = new int[a.GetLength(0), a.GetLength(1)];

        if (a.GetLength(0) == b.GetLength(0) && a.GetLength(1) == b.GetLength(1))
        {
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    c[i, j] = a[i, j] + b[i, j];
                }
            }
        }
        return c;
    }

    public static int[,] MatrixMult(int[,]a, int[,]b)
    {
        int[,] c = new int[a.GetLength(0), b.GetLength(1)];
        int[,] error = new int[0,0];

        if(a.GetLength(1)!=b.GetLength(0)) return error;

        for (int i = 0; i < a.GetLength(0); i++)
        {
            int mult = 0;

            for (int k = 0; k < b.GetLength(1); k++)
            {
                int result = 0;
                for (int j = 0; j < b.GetLength(0); j++)
                {
                    mult = a[i,j] * b[j,k];
                    result += mult; 
                }
                c[i,k] = result;
            }
        }

        return c;
    }

    public static int[] MatrixVec(int[,]a, int[]b)
    {
        int[]c = new int[a.GetLength(0)];
        int mult = 0;

        for (int i = 0; i < c.Length; i++)
        {
            int result = 0;
            for (int j = 0; j < b.Length; j++)
            {
                mult = a[i,j] * b[j];
                result += mult;
            }
            c[i] = result;
        }

        return c;
    }

    public static int[,] MatrixEsc(int[,]a, int x)
    {
        int[,] c = new int[a.GetLength(0), a.GetLength(1)];

        for (int i = 0; i < a.GetLength(0); i++)
        {
            for (int j = 0; j < a.GetLength(1); j++)
            {
                c[i,j] = a[i,j] * x;
            }
        }
        return c;
    }
}