namespace _10th_0712.Threading;

public class HT
{
    private int threadsCount;
    private string init = "0123456789";
    private string res = "";
    private object stringLocker = new();
    
    public void Run()
    {
        int n = init.Length;
        threadsCount = n;
        Thread[] threads = new Thread[n];
        for (int i = 0; i < n; i++)
        {
            threads[i] = new Thread(CombineRandomString);
            threads[i].Start(i);
        }
    }

    private void CombineRandomString(object? index)
    {
        int m = (int)(index ?? -1);
        string resLocal;
        lock (stringLocker)
        {
            resLocal = res += init[m];
        }
        Console.WriteLine("thread {0}: '{1}'", m+1, resLocal);

        lock (this)
        {
            threadsCount--;
            if (threadsCount == 0)
                Console.WriteLine("total: '{0}'", res);
        }
    }
}