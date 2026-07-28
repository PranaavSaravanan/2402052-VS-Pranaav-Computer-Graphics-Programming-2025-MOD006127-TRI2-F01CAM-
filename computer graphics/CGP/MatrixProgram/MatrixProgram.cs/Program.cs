using System;

public class Program
{
    static int[,] temp;

    public static void Main()
    {
        temp = new int[,] { { 2, 1 }, { 3, 4 } };
        Matrix2D a = new Matrix2D(temp);

        temp = new int[,] { { 5, 6 }, { 7, 8 } };
        Matrix2D b = new Matrix2D(temp);

        Console.WriteLine("Matrix A: num of col: " + a.NumberOfColumns() + ", num of row: " + a.NumberOfRows());
        Console.WriteLine("Matrix B: num of col: " + b.NumberOfColumns() + ", num of row: " + b.NumberOfRows());

        Console.WriteLine();
        Console.WriteLine();

        Console.WriteLine("A x B =");
        Console.WriteLine(Matrix2D.Multiply(a, b).OutputMatrix());

        Console.ReadLine();
    }
}