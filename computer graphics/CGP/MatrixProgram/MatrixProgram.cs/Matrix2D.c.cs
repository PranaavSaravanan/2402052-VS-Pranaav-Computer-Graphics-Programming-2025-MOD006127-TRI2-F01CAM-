using System;

public class Matrix2D
{
    public int[,] matrix;

    public Matrix2D()
    {
        matrix = new int[0, 0];
    }

    public Matrix2D(int x, int y)
    {
        matrix = new int[x, y];
    }

    public Matrix2D(int[,] toSet)
    {
        matrix = toSet;
    }

    public void SetMatrix(int[,] toSet)
    {
        if (toSet.GetLength(0) == matrix.GetLength(0) && toSet.GetLength(1) == matrix.GetLength(1))
            matrix = toSet;
    }

    public int NumberOfColumns()
    {
        return matrix.GetLength(0);
    }

    public int NumberOfRows()
    {
        return matrix.GetLength(1);
    }

    public string OutputMatrix()
    {
        string toOut = "";

        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                toOut += matrix[i, j].ToString();

                if (j < matrix.GetLength(1) - 1)
                    toOut += ",";
            }

            toOut += "\n";
        }

        return toOut;
    }

    // reads real dimensions each time rather than assuming 2x2, so this works for any compatible size
    public static Matrix2D Multiply(Matrix2D a, Matrix2D b)
    {
        int aRows = a.matrix.GetLength(0);
        int aCols = a.matrix.GetLength(1);
        int bRows = b.matrix.GetLength(0);
        int bCols = b.matrix.GetLength(1);

        if (aCols != bRows)
            return new Matrix2D(0, 0);

        int[,] result = new int[aRows, bCols];

        for (int i = 0; i < aRows; i++)
        {
            for (int j = 0; j < bCols; j++)
            {
                int sum = 0;
                for (int k = 0; k < aCols; k++)
                {
                    sum += a.matrix[i, k] * b.matrix[k, j];
                }
                result[i, j] = sum;
            }
        }

        return new Matrix2D(result);
    }
}