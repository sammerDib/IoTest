using System;
using System.Linq;
using System.Linq.Expressions;
using CommunityToolkit.Mvvm.Messaging;
using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Shared.Test.Tools
{
    public class FakeDbRecipeService : ServiceInvoker<IDbRecipeService>
    {
        public FakeDbRecipeService(string endPoint, ILogger<IDbRecipeService> logger, IMessenger messenger = null, ServiceAddress customAddress = null)
            : base(endPoint, logger, messenger, customAddress)
        {
        }
        public override TResult Invoke<TResult>(Expression<Func<IDbRecipeService, Response<TResult>>> serviceMethodExpr, bool silentLogger = true)
        {
            var completeResponse = InternalInvokeAndGetMessages<TResult>(serviceMethodExpr, silentLogger);
            return completeResponse.Result;
        }
        public Response<TResult> InternalInvokeAndGetMessages<TResult>(Expression<Func<IDbRecipeService, Response<TResult>>> serviceMethodExpr, bool silentLogger)
        {
            // Here you can simulate the behaviour of the
            // depending on the method called. For example:

            if (serviceMethodExpr.Body is MethodCallExpression methodCall)
            {
                string methodName = methodCall.Method.Name;
                // You can simulate different scenarios based on the method name.
                switch (methodName)
                {
                    case "GetChamberFromKeys":
                        // Simulate the behaviour of the MyMethod method
                        // You can return a dummy response for your unit tests
                        var simulatedChamber = new Chamber
                        {
                            Id = 1,
                            ToolId = 1
                        };
                        return new Response<TResult>
                        {
                            Result = (TResult)(object)simulatedChamber,
                        };
                    case "GetLastRecipeWithProductAndStep":
                        var simulatedEmeRecipe = new Recipe
                        {
                            KeyForAllVersion = Guid.Parse("01111111-1c1a-1f1e-a111-1d11111b1111"),
                            Name = "emeRecipe",
                            StepId = 1,
                            Type = ActorType.EMERA,
                            ActorType = 225,
                            XmlContent = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<EMERecipe xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" FileVersion=\"1.0.0\" />",
                        };
                        return new Response<TResult>
                        {
                            Result = (TResult)(object)simulatedEmeRecipe,
                        };
                    case "SetRecipe":
                        return new Response<TResult>
                        {
                            Result = (TResult)(object)1007,
                        };
                    case "GetRecipe":

                        return new Response<TResult>
                        {
                            Result = (TResult)(object)null,
                        };
                    ////var dd = methodCall.Arguments.OfType<Expression>().FirstOrDefault(arg => arg. is string value && value == "");
                    //var emeRecipe = new Recipe
                    //{
                    //    KeyForAllVersion = Guid.Parse("01111111-1c1a-1f1e-a111-1d11111b1111"),
                    //    Name = "emeRecipe",
                    //    StepId = 1,
                    //    Type = ActorType.EMERA,
                    //    ActorType = 225,
                    //    XmlContent = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<EMERecipe xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" FileVersion=\"1.0.0\" />",
                    //};
                    //return new Response<TResult>
                    //{
                    //    Result = (TResult)(object)emeRecipe,
                    //};
                    default:
                        throw new NotSupportedException($"Method '{methodName}' not supported by FakeDbRecipeService.");
                }
            }
            else
            {
                // If the expression is not a call method, you can raise an exception.
                throw new ArgumentException("Expression must be a method call.", nameof(serviceMethodExpr));
            }
        }
    }
}
