﻿using Fur;
using Fur.DependencyInjection;
using Fur.FriendlyException;
using Fur.UnifyResult;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc.Filters
{
    /// <summary>
    /// 友好异常拦截器
    /// </summary>
    [SkipScan]
    public sealed class FriendlyExceptionFilter : IAsyncExceptionFilter
    {
        /// <summary>
        /// 服务提供器
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serviceProvider">服务提供器</param>
        public FriendlyExceptionFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 异常拦截
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task OnExceptionAsync(ExceptionContext context)
        {
            // 解析异常处理服务，实现自定义异常额外操作，如记录日志等
            var globalExceptionHandler = _serviceProvider.GetService<IGlobalExceptionHandler>();
            if (globalExceptionHandler != null)
            {
                await globalExceptionHandler.OnExceptionAsync(context);
            }

            // 排除 Mvc 视图
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (actionDescriptor.ControllerTypeInfo.BaseType == typeof(Controller)) return;

            // 标识异常已经被处理
            context.ExceptionHandled = true;

            // 设置异常结果
            var exception = context.Exception;

            // 解析验证异常
            var validationFlag = "[Validation]";
            var errorMessage = exception.Message.StartsWith(validationFlag) ? exception.Message[validationFlag.Length..] : exception.Message;

            // 判断是否跳过规范化结果
            if (UnifyResultContext.IsSkipUnifyHandler(actionDescriptor.MethodInfo, out var unifyResult))
            {
                context.Result = new ContentResult
                {
                    Content = errorMessage,
                    StatusCode = exception.Message.StartsWith(validationFlag) ? StatusCodes.Status400BadRequest : (int)(UnifyResultContext.Get(UnifyResultContext.UnifyResultStatusCodeKey) ?? StatusCodes.Status500InternalServerError)
                };
            }
            else context.Result = unifyResult.OnException(context);

            // 处理验证异常，打印验证失败信息
            if (exception.Message.StartsWith(validationFlag))
            {
                App.PrintToMiniProfiler("validation", "Failed", $"Exception Validation Failed:\r\n{errorMessage}", true);
            }
            // 打印错误到 MiniProfiler 中
            else Oops.PrintToMiniProfiler(context.Exception);
        }
    }
}