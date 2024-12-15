namespace _10th_0712.Tasks;

public class TasksHT
{

    private string init = "0123456789";
    private string result = "";
    private static int i = 0;
    private object stringLocker = new();
    private object intLocker = new();


    public void Run()
    {
        Task.Run(_Run).Wait();
        Console.WriteLine("total: {0}", result);
    }

    private async Task _Run()
    {
        
        int n = init.Length;
        var tasks = new Task[n];
        for (int j = 0; j < n; j++)
        {
            tasks[j] = Task.Run(CombineRandomString);
        }
        
        await Task.WhenAll(tasks);
    }

    private async Task CombineRandomString()
    {
        int r =  GetI().Result;
        if (r != -1)
        {
            lock (stringLocker)
            {
                result += init[r];
            }

            Console.WriteLine("Task {0}: {1}", r + 1, result);
        }
    }

    private async Task<int> GetI()
    {
        lock (intLocker)
        {
            if (i < init.Length - 1)
                return i++;
            if (i == init.Length - 1)
                return i;
            return -1;
        }
    }
    
    /*
    private async Task First()
    {
        result += init[0];
        Console.WriteLine("Task {0}: {1}", init[0], result);
    }
    
    private async Task Second()
    {
        result += init[1];
        Console.WriteLine("Task {0}: {1}", init[1] , result);
    }
    
    private async Task Third()
    {
        result += init[2];
        Console.WriteLine("Task {0}: {1}", init[2], result);
    }
    
    private async Task Fourth()
    {
        result += init[3];
        Console.WriteLine("Task {0}: {1}", init[3], result);
    }
    
    private async Task Fifth()
    {
        result += init[4];
        Console.WriteLine("Task {0}: {1}", init[4], result);
    }
    
    private async Task Sixth()
    {
        result += init[5];
        Console.WriteLine("Task {0}: {1}", init[5], result);
    }
    
    private async Task Seventh()
    {
        result += init[6];
        Console.WriteLine("Task {0}: {1}", init[6], result);
    }
    
    private async Task Eighth()
    {
        result += init[7];
        Console.WriteLine("Task {0}: {1}", init[7], result);
    }
    
    private async Task Nineth()
    {
        result += init[8];
        Console.WriteLine("Task {0}: {1}", init[8], result);
    }
    
    private async Task Tenth()
    {
        result += init[9];
        Console.WriteLine("Task {0}: {1}", init[9], result);
    }
    */
}