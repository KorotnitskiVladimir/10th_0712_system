namespace _10th_0712.Threading;

public class ThreadingDemo
{
    private int nThreads;
    public void Run()
    {
        Thread thread = new(ThreadMethod);
        thread.Start();
        thread.Join(); // ожидание завршения потока - блокируется поток, который вызывает операцию (родительский поток)
        Console.WriteLine("Threading Demo");
        sum = 100.0;
        // если необходимо ждать несколько потоков, то ждать нужно каждый
        int n = 12;
        Thread[] threads = new Thread[n];
        nThreads = n;
        for (int i = 0; i < n; i++)
        {
            threads[i] = new Thread(CalcOneMonth); // создание объекта не запускает поток
            threads[i].Start(i + 1); // это делает Start(), также данные для потока (аргумент метода) передается через Start()
            // threads[i].Join(); // в этом месте неправильно.
        }
        //for (int i = 0; i < n; i++) // ожидание всех потоков - это ожидание каждого, НО после того как запеущены все потоки
        //{
            //threads[i].Join(); // если поток завершен, то вызов Join() игнорируется без проблем
        //}
        // Ожидание позволяет организовать "точку", в которой есть суммирующий результат
    }

    private void ThreadMethod()
    {
        Console.WriteLine("Hello from thread 1");
    }

    private double sum;
    private object sumLocker = new(); // объект, созданнный ради критической секции
    
    private void CalcOneMonth(object? month)
    {
        int m = (int)(month ?? -1); // null-coalescence - форма сокращенного тернарного выражения
        // y = (x != null) ? x : -1 --> y = x ?? -1; y = a ?? b ?? c ?? -1;
        // null-propagation/null-forgiving x?.prop --> (x == null) ? null : x.prop;
        // a?.b?.c; a?.b?.c ?? -1;
        // null-checking a! --> (a != null) ? a : throw new NullReference();
        // x ??= -1; x = (x != null) ? x : -1; XOr
        Thread.Sleep(300); // имитация запроса API
        double percent = 10.0; // с возвратом результата
        double sumLocal;
        lock (sumLocker)
        {
           sumLocal = sum *= 1.0 + percent / 100;
        }
        Console.WriteLine("Month {2}: + {0:F1}% -> {1:F1} UAH", percent, sumLocal, m);

        lock (this) // для синхронизации можно использовать и другие объекты
        {
            nThreads--;
            if (nThreads == 0)
                Console.WriteLine("Total sum = {0:F2}", sum);
        }
    }
}