/// <summary>
/// Multithreaded Asynchronous programming that runs code concurrently 
/// is a complicated topic that new developers often have difficulty grasp.
/// It's prone to error because the Task based programing hides the underlying
/// complexity of managing Thread, ThreadPool, and TaskScheduler.
/// Below are examples, both good and bad to demonstrate how to run multiple 
/// tasks/promises/futures at the same time.
/// </summary>
Console.WriteLine("Start Notification Server!");
Job job = new Job();
await job.ExecuteJob2();
Console.WriteLine("Done reading notifications");
Console.ReadLine();

public class Job {
    public List<int> notifications = new() { 1, 2, 3, 4, 5 };

    /// <summary>
    /// Bad example that I observed from code reviewing a junior developer.
    /// </summary>
    /// <returns></returns>
    public async Task ExecuteJob1() {
        List<Task> tasks = new();
        foreach (int n in notifications) {
            tasks.Add(new Task(() => {
                Console.WriteLine($"enter task {n}");
                Task.Delay(1000);
                Console.WriteLine($"enter task {n}");
            }));
        }
        await Task.WhenAll(tasks);
    }
    /// <summary>
    /// Good example that fixes the previous mistake.
    /// </summary>
    /// <returns></returns>
    public async Task ExecuteJob2() {
        List<Task> tasks = new();
        foreach (int n in notifications) {
            Task task = new Task(() => {
                Console.WriteLine($"enter task {n}");
                Task.Delay(1000);
                Console.WriteLine($"enter task {n}");
            });
            task.Start();
            tasks.Add(task);            
        }
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Good example if you need fine control over tasks (long running)
    /// </summary>
    /// <returns></returns>
    public async Task ExecuteJob3() {
        List<Task> tasks = new();
        foreach (int n in notifications) {
            Task task = Task.Factory.StartNew(() => {
                Console.WriteLine($"enter task {n}");
                Task.Delay(1000);
                Console.WriteLine($"enter task {n}");
            });
            tasks.Add(task);
        }
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Good example; recommended example from Microsoft.
    /// </summary>
    /// <returns></returns>
    public async Task ExecuteJob4() {
        List<Task> tasks = new();
        foreach (int n in notifications) {
            Task task = Task.Run(() => {
                Console.WriteLine($"enter task {n}");
                Task.Delay(1000);
                Console.WriteLine($"enter task {n}");
            });
            tasks.Add(task);
        }
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Bad example, change annoymous Action within Task constructor does not fix the issue that task doesn't start.
    /// </summary>
    /// <returns></returns>
    public async Task ExecuteJob5() {
        List<Task> tasks = new();
        foreach (int n in notifications) {
            tasks.Add(new Task(async() => {
                Console.WriteLine($"enter task {n}");
                await Task.Delay(1000);
                Console.WriteLine($"enter task {n}");
            }));
        }
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Questionable example, caller finished before tasks finishes
    /// because WhenAll only waits on the tasks but not the inner async anoymous action.
    /// </summary>
    /// <returns></returns>
    public async Task ExecuteJob6() {
        List<Task> tasks = new();
        foreach (int n in notifications) {
            Task task = new Task(async() => {
                Console.WriteLine($"enter task {n}");
                await Task.Delay(1000);
                Console.WriteLine($"enter task {n}");
            });
            task.Start();
            tasks.Add(task);
        }
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Questionable example, caller finished before tasks finishes
    /// because WhenAll only waits on the tasks but not the inner async anoymous action.
    /// </summary>
    /// <returns></returns>
    public async Task ExecuteJob7() {
        List<Task> tasks = new();
        foreach (int n in notifications) {
            Task task = Task.Factory.StartNew(async () => {
                Console.WriteLine($"enter task {n}");
                await Task.Delay(1000);
                Console.WriteLine($"enter task {n}");
            });
            tasks.Add(task);
        }
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Good example, tasks are queued to thread pool and return as proxy.
    /// </summary>
    /// <returns></returns>
    public async Task ExecuteJob8() {
        List<Task> tasks = new();
        foreach (int n in notifications) {
            Task task = Task.Run(async () => {
                Console.WriteLine($"enter task {n}");
                await Task.Delay(1000);
                Console.WriteLine($"enter task {n}");
            });
            tasks.Add(task);
        }
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Great example; personal favorite; clean code; shows seperation of concern.
    /// </summary>
    /// <returns></returns>
    public async Task ExecuteJob9() {
        List<Task> tasks = new();
        foreach (int n in notifications) {
            tasks.Add(ExecuteAction(n));
        }
        await Task.WhenAll(tasks);
    }

    public async Task ExecuteAction(int n) {
        Console.WriteLine($"enter task {n}");
        await Task.Delay(1000);
        Console.WriteLine($"enter task {n}");
    }
}