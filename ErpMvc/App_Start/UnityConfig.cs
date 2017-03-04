using System.Data.Entity;
using System.Web.Mvc;
using ErpMvc.Controllers;
using ErpMvc.Models;
using Microsoft.Practices.Unity;
using Unity.Mvc5;

namespace ErpMvc
{
	public static class UnityConfig
	{
		public static void RegisterComponents()
		{
			var container = new UnityContainer();

			// register all your components with the container here
			// it is NOT necessary to register your controllers

			// e.g. container.RegisterType<ITestService, TestService>();
			container.RegisterType<DbContext, ErpContext>();

			DependencyResolver.SetResolver(new UnityDependencyResolver(container));

		}
	}
}