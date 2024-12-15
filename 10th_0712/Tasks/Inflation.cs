namespace _10th_0712.Tasks;

public class Inflation
{
    private object pLocker = new();
    private object caclLocerk = new();
    private double[] percent = new Double[12] { 0.1, 0.11, 0.12, 0.13, 0.14, 0.15, 0.16, 0.17, 0.18, 0.19, 0.2, 0.21 };
    private int p = 0;
    private double summ = 10000.0;

    public void Run()
    {
        Task.Run(_Run).Wait();
        Console.WriteLine("Total: {0:F2}", summ);
    }

    private async Task _Run()
    {
        int m = percent.Length;
        var calcTasks = new Task[m];
        for (int k = 0; k < m; k++)
        {
            calcTasks[k] = Task.Run(Calc);
        }
        
        await Task.WhenAll(calcTasks);
    }
    
    private async Task<int> GetPInd()
    {
        lock (pLocker)
        {
            if (p < percent.Length - 1)
                return p++;
            if (p == percent.Length - 1)
                return p;
            return -1;
        }
    }

    private async Task Calc()
    {
        int r = GetPInd().Result;
        if (r != -1)
        {
            lock (caclLocerk)
            {
                summ *= 1 + percent[r];
            }
            Console.WriteLine("Step {0}: {1:F2}", r+1, summ);
        }
    }
}