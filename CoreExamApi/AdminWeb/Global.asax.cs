using Autofac;
using Autofac.Integration.Mvc;
using AdminWeb.Extensions;
using PMSoft.Common.Security;
using PMSoft.Fakes;
using PMSoft.Logging;
using PMSoft.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AdminWeb.Services;
using System.Configuration;

namespace AdminWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        protected void Application_Start()
        {
            var builder = new ContainerBuilder();
            SetupResolveRules(builder);
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            //autofac 注册依赖
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        private void SetupResolveRules(ContainerBuilder builder)
        {

            builder.Register(c =>
               //register FakeHttpContext when HttpContext is not available
               HttpContext.Current != null ?
               (new HttpContextWrapper(HttpContext.Current) as HttpContextBase) :
               (new FakeHttpContext("~/") as HttpContextBase))
               .As<HttpContextBase>()
               .InstancePerRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Request)
                .As<HttpRequestBase>()
                .InstancePerRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Response)
                .As<HttpResponseBase>()
                .InstancePerRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Server)
                .As<HttpServerUtilityBase>()
                .InstancePerRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Session)
                .As<HttpSessionStateBase>()
                .InstancePerRequest();

            builder.Register<ILogger>(c => LoggerFactory.GetLog()).SingleInstance();
            builder.RegisterType<FormsAuthenticationService>().As<IAuthenticationService>();
            //work context
            builder.RegisterType<WebWorkContext>().As<IWorkContext>().InstancePerRequest();
            builder.Register(c=>new UserService(ConnectionString)).As<IUserService>().InstancePerLifetimeScope();
            builder.Register(c => new ExamService(ConnectionString)).As<IExamService>().InstancePerLifetimeScope();
        }
    }
}
