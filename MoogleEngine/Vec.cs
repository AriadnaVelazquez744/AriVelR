namespace MoogleServer;

public class Vec
{
    int[] vector;
    public Vec(int[] vec)
    {
        vector = vec;
    }
    public static int VecMult(int[]a, int[]b)
    {
        int c = 0;

        for (int i = 0; i < a.Length; i++)
        {
            int mult = a[i] * b[i];
            c += mult;
        }
        return c;
    }

    public static int[] VecEsc(int[]a, int x)
    {
        int[] c = new int[a.Length];

        for (int i = 0; i < a.Length; i++)
        {
            c[i] = a[i] * x;
        }

        return c;
    }
}