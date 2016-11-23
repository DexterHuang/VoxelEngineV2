using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
public class DelayedTask {
    public ThreadStart threadStart;
    public int delay;
    public int currentDelayCount;
    public bool isAsync;
    public bool repeat;
    public DelayedTask(ThreadStart threadStart, int delay, bool isAsync, bool repeat) {
        this.threadStart = threadStart;
        this.delay = delay;
        currentDelayCount = delay;
        this.isAsync = isAsync;
        this.repeat = repeat;
    }
}
public class Scheduler {
    static Queue<ThreadStart> syncTasks = new Queue<ThreadStart>();
    static Queue<ThreadStart> asyncTasks = new Queue<ThreadStart>();
    static LinkedList<DelayedTask> delayedTasks = new LinkedList<DelayedTask>();

    public static void onSyncTick() {
        while (syncTasks.Count > 0) {
            ThreadStart ts = syncTasks.Dequeue();
            if (ts != null) {
                ts.Invoke();
            }
        }
        for (LinkedListNode<DelayedTask> node = delayedTasks.First; node != null && node != delayedTasks.Last; node = node.Next) {
            DelayedTask task = node.Value;
            task.currentDelayCount--;
            if (task.currentDelayCount <= 0) {
                if (task.isAsync) {
                    runTaskAsynchronously(task.threadStart);
                } else {
                    runTaskSynchronously(task.threadStart);
                }
                if (task.repeat) {
                    task.currentDelayCount = task.delay;
                } else {
                    delayedTasks.Remove(node);
                }
            }
        }
    }
    public static void onAsycTick() {
        while (asyncTasks.Count > 0) {
            new Thread(asyncTasks.Dequeue()).Start();
        }
    }
    public static void runTaskSynchronously(ThreadStart task) {
        if (Thread.CurrentThread.Equals(GameManager.mainThread)) {
            task.Invoke();
        } else {
            syncTasks.Enqueue(task);
        }
    }
    public static void runTaskAsynchronously(ThreadStart task, bool sequential = false) {
        if (sequential == false) {
            Thread thread = new Thread(task);
            thread.Start();
        } else {
            asyncTasks.Enqueue(task);
        }
    }
    public static void scheduleSyncDelayedTask(ThreadStart task, int delay) {
        delayedTasks.AddLast(new DelayedTask(task, delay, false, false));
    }
    public static void scheduleAsyncDelayedTask(ThreadStart task, int delay) {
        delayedTasks.AddLast(new DelayedTask(task, delay, true, false));
    }
}
