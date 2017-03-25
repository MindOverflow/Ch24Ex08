using System;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

namespace Ch24Ex08
{
    internal static class Program
    {
        // Метод, исполняемы как задача.
        private static void MyTask(object ct)
        {
            var cancelToken = (CancellationToken) ct;

            // Проверь, отменена ли задача, прежде чем запускать её. 
            cancelToken.ThrowIfCancellationRequested();

            WriteLine("Выполнение метода-задачи MyTask() запущено.");

            for (var count = 0; count < 10; count++)
            {
                // В данном примере для отслеживания отмены задачи применяется опрос.
                if (cancelToken.IsCancellationRequested)
                {
                    WriteLine("Получен запрос на отмену задачи.");
                    cancelToken.ThrowIfCancellationRequested();
                }

                Thread.Sleep(500);
                WriteLine($"В методе MyTask() подсчёт равен {count}");
            }

            WriteLine("Выполнение метода-задачи MyTask() завершено.");
        }

        private static void Main()
        {
            WriteLine("Основной поток выполнения запущен.");

            // Источник признаков отмены выполнения задачи. 
            var cancellationTokenSource = new CancellationTokenSource();

            // Запустить задачу на выполнние, передав признак отмены ей самой и делегату. 
            var cancellationToken = cancellationTokenSource.Token;
            var task = Task.Factory.StartNew(MyTask, cancellationToken, cancellationToken);

            // Дать задаче исполняться вплоть до её отмены.
            Thread.Sleep(2000);

            try
            {
                cancellationTokenSource.Cancel();

                task.Wait();
            }
            catch (AggregateException e)
            {
                if (task.IsCanceled)
                {
                    WriteLine("\nЗадача task отменена\n");
                }

                WriteLine(e);
            }
            finally
            {
                task.Dispose();
                cancellationTokenSource.Dispose();
            }
        }
    }
}
