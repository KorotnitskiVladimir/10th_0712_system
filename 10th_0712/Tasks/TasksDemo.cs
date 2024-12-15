namespace _10th_0712.Tasks;

public class TasksDemo
{
    public void Run()
    {
        int w, c;
        ThreadPool.GetMinThreads(out w, out c);
        Console.WriteLine("Tasks Demo {0} {1}", w, c);
        _Run().Wait();
        
        //Task.Run(_Run).Wait(); // переход к async методу
        //Console.WriteLine(Task4("Name 5", 5).Result); // В синхронном режиме Result ожидает
        Console.WriteLine("TaskDemo Finished");
    }
    private async Task _Run()
    {
        //Task1(); // Синхронный запуск (хотя метод объявлен как async)
        var task1 = Task.Run(Task1); // Асинхронный запуск (при этом метод может и не быть объявлен async)
        var work1 = Task.Run(Work1); // Асинхронный запуск, метод не async
        var task2 = new Task(Task2); // запуск не происходит
        var work2 = new Task(Work2); // запуск не происходит
        task2.Start();
        work2.Start();
        var task3 = Task3("Task3",10);              // ~ Task.Run(Task3); В отличие от потоков в задачи аргументы можно передавать
        // в обычном стиле - произвольное количество, тип. Но вызов остатется асинхронным, без ожидания может быть
        // отменен завершением программы (хотя синтаксис вызова отвечает синхронному)
        var ret = Task4("Task4", 4); // ret - не string, а объект Task<string>
        Console.WriteLine(ret.Result); // В асинхронном контексте Result нужно ожидать
        //var res = await ret; // await и дождаться, и получить результат
        //Console.WriteLine(res);
        //await Task.Delay(10);

        await Task4("First task", 10)// Task <"Hello from 'First task' 10">
            .ContinueWith(task => Task5(task.Result))
            .Unwrap()
            .ContinueWith(task => Console.WriteLine(task.Result));

        Console.WriteLine(await Task.Run(LoadConfig) // Вызов синхронных методов не осуществляет дополнительную
            .ContinueWith(ConnectDB)    // обертку Task<...> и, соответственно, нет потрбености в .Unwrap()
            .ContinueWith(SyncDB));
        
        await Task.WhenAll(task1, task2, work1, work2, task3, ret);
    }

    private string LoadConfig()
    {
        return "Config string ";
    }
    
    private string ConnectDB(Task<String> configTask)
    {
        return "Connected with " + configTask.Result;
    }

    private string SyncDB(Task<String> connectTask)
    {
        return "Sync by " + connectTask.Result;
    }
    private async void Task1()
    {
        Console.WriteLine("Hello from task1");
    }

    private void Work1()
    {
        Console.WriteLine("Hello from work1");
    }
    
    private async void Task2()
    {
        Console.WriteLine("Hello from task2");
    }

    private void Work2()
    {
        Console.WriteLine("Hello from work2");
    }

    private async Task Task3(string name, int num)
    {
        Console.WriteLine($"Hello from '{name}' {num}");
    }
    
    /* Возвращение результата - или void, или Task/Task<TResult>
     * !!! void не ожидается даже если запускается через Task.Run()
     * Task<TResult> - обертка результата, которая позволяет его ожидать. Возвращается некоторый объект, который
     * со временем перейдет в состояние определенности. В этом состоянии будет доступен результат соответствующего типа.
     */
    
    private async Task<string> Task4(string name, int num)
    {
        return $"Hello from '{name}' {num}";
    }

    private async Task<string> Task5(string input)
    {
        return input + " Task 5 addon";
    }
}

/* Асинхронность. ч. 2: Многозадачность
* Многозадачность - использование объектов уровня языка программирования (не уровня ОС) для создания асинхронности.
 * Рекоменоваднный подход для большинства задач.
 *
 * Задачу можно запустить несколькоми методами:
 * Task.Run(Action)
 * new Task(Action).Start()
 * При завершении основной программы завершаются все задачи. Те, которые не успели завершиться - отменяются.
 *
 * Реальное (скрытое) выполнение задач выполняется специальным "исполнителем" (ThreadPool): запуск задачи =
 * установка в очередь к исполнителю. Емкость управляется ОС и варьируется на разных ПК. ThreadPool отменяется
 * после остановки программы и все задачи в нем останавливаются
 * !!! отличие в запуске методов заключается в их сигнатуре:
 * обычные заключаются в главном потоке
 * async - в ThreadPool
 *
 * ---- Main -- Demo -- cw(Main Finished) --- |
 * ---- Main -- cw(Main Finished)- |
 *          \                      |
 *           \    await            |
 *           Demo ---- task1       |
 *                           \   \   \  
 *                            \   \    work2
 *                             \   task2
 *                               work1
 *
 * Главные отличия многозадачности от многопоточности:
 * - приоритеты выполнения: потоки имеют одинаковый приоритет с главным потоком, задачи имеют фоновый приоритет. Это
 *      ведет к тому, окончанием главного потока задачи отменяются, даже если не были завершены, а потоки продолжают
 *      работу, перенимая на себя метку "главный"
 * - в задачи можно передавать произвольное колечество аргументов произвольных типов. в поток можно передать только
 *      один обобщенный (object?) аргумент
 * - задачи могут возвращать значение (в виде объектов-задач), потоки - только менять глобальные ресурсы
 *
 * Нитевое программирование / Continuations
 * Используется для параллельных задач, каждая из которых требует последовательных действий
 *
 * LoadConfig() -- return config ->|
 *              v-<----------------|
 *  .then(connectBD) -- retrun connection ->|
 *          v--<----------------------------|
 *  .tnen(SyncBD)
*/
