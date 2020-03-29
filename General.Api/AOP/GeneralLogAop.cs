using Castle.DynamicProxy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace General.Api
{
    public class GeneralLogAop : IInterceptor
    {
        private readonly IHttpContextAccessor accessor;

        public GeneralLogAop(IHttpContextAccessor httpContext)
        {
            this.accessor = httpContext;
        }
        public void Intercept(IInvocation invocation)
        {

            //记录被拦截方法信息的日志信息
            var dataIntercept = $"{DateTime.Now.ToString("yyyyMMddHHmmss")} " +
                $"当前执行方法：{ invocation.Method.Name} " +
                $"参数是： {string.Join(", ", invocation.Arguments.Select(a => (a ?? "").ToString()).ToArray())} \r\n";

            try
            {
                //在被拦截的方法执行完毕后 继续执行当前方法
                invocation.Proceed();
                dataIntercept += ($"被拦截方法执行完毕，返回结果：{invocation.ReturnValue}");
            }
            catch (Exception ex)
            {
                dataIntercept += $"当前错误信息："+ex.Message;
            }

            #region 输出到当前项目日志
            var path = PlatformServices.Default.Application.ApplicationBasePath + @"Log";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string fileName = path + $@"\{DateTime.Now.ToString("yyyyMMddHH")}.log";

            StreamWriter sw = File.AppendText(fileName);
            sw.WriteLine(dataIntercept);
            sw.Close();
            #endregion

            #region  先注释
            //string UserName = accessor.HttpContext.User.Identity.Name;

            ////记录被拦截方法信息的日志信息
            //var dataIntercept = "" +
            //    $"【当前操作用户】：{ UserName} \r\n" +
            //    $"【当前执行方法】：{ invocation.Method.Name} \r\n" +
            //    $"【携带的参数有】： {string.Join(", ", invocation.Arguments.Select(a => (a ?? "").ToString()).ToArray())} \r\n";

            //try
            //{
            //    //MiniProfiler.Current.Step($"执行Service方法：{invocation.Method.Name}() -> ");
            //    //在被拦截的方法执行完毕后 继续执行当前方法，注意是被拦截的是异步的
            //    invocation.Proceed();

            //    // 异步获取异常，先执行
            //    if (IsAsyncMethod(invocation.Method))
            //    {

            //        //Wait task execution and modify return value
            //        if (invocation.Method.ReturnType == typeof(Task))
            //        {
            //            invocation.ReturnValue = InternalAsyncHelper.AwaitTaskWithPostActionAndFinally(
            //                (Task)invocation.ReturnValue,
            //                async () => await TestActionAsync(invocation),
            //                ex =>
            //                {
            //                    LogEx(ex, ref dataIntercept);
            //                });
            //        }
            //        else //Task<TResult>
            //        {
            //            invocation.ReturnValue = InternalAsyncHelper.CallAwaitTaskWithPostActionAndFinallyAndGetResult(
            //             invocation.Method.ReturnType.GenericTypeArguments[0],
            //             invocation.ReturnValue,
            //             async () => await TestActionAsync(invocation),
            //             ex =>
            //             {
            //                 LogEx(ex, ref dataIntercept);
            //             });
            //        }
            //    }
            //    else
            //    {// 同步1

            //    }
            //}
            //catch (Exception ex)// 同步2
            //{
            //    LogEx(ex, ref dataIntercept);

            //}

            //var type = invocation.Method.ReturnType;
            //if (typeof(Task).IsAssignableFrom(type))
            //{
            //    var resultProperty = type.GetProperty("Result");
            //    dataIntercept += ($"【执行完成结果】：{JsonConvert.SerializeObject(resultProperty.GetValue(invocation.ReturnValue))}");
            //}
            //else
            //{
            //    dataIntercept += ($"【执行完成结果】：{invocation.ReturnValue}");
            //}


            //Parallel.For(0, 1, e =>
            //{
            //    LogLock.OutSql2Log("AOPLog", new string[] { dataIntercept });
            //});

            ////_hubContext.Clients.All.SendAsync("ReceiveUpdate", LogLock.GetLogData()).Wait();

            #endregion
        }

        public static bool IsAsyncMethod(MethodInfo method)
        {
            return (
                method.ReturnType == typeof(Task) ||
                (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                );
        }

        private void LogEx(Exception ex, ref string dataIntercept)
        {
            if (ex != null)
            {
                //执行的 service 中，收录异常
                //MiniProfiler.Current.CustomTiming("Errors：", ex.Message);
                //执行的 service 中，捕获异常
                dataIntercept += ($"方法执行中出现异常：{ex.Message + ex.InnerException}\r\n");
            }
        }

        private async Task TestActionAsync(IInvocation invocation)
        {
            //Console.WriteLine("Waiting after method execution for " + invocation.MethodInvocationTarget.Name);
            await Task.Delay(20); // 仅作测试
            //Console.WriteLine("Waited after method execution for " + invocation.MethodInvocationTarget.Name);
        }
    }

    internal static class InternalAsyncHelper
    {
        public static async Task AwaitTaskWithPostActionAndFinally(Task actualReturnValue, Func<Task> postAction, Action<Exception> finalAction)
        {
            Exception exception = null;

            try
            {
                await actualReturnValue;
                await postAction();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                finalAction(exception);
            }
        }

        public static async Task<T> AwaitTaskWithPostActionAndFinallyAndGetResult<T>(Task<T> actualReturnValue, Func<Task> postAction, Action<Exception> finalAction)
        {
            Exception exception = null;

            try
            {
                var result = await actualReturnValue;
                await postAction();
                return result;
            }
            catch (Exception ex)
            {
                exception = ex;
                throw;
            }
            finally
            {
                finalAction(exception);
            }
        }

        public static object CallAwaitTaskWithPostActionAndFinallyAndGetResult(Type taskReturnType, object actualReturnValue, Func<Task> action, Action<Exception> finalAction)
        {
            return typeof(InternalAsyncHelper)
                .GetMethod("AwaitTaskWithPostActionAndFinallyAndGetResult", BindingFlags.Public | BindingFlags.Static)
                .MakeGenericMethod(taskReturnType)
                .Invoke(null, new object[] { actualReturnValue, action, finalAction });
        }
    }
}
