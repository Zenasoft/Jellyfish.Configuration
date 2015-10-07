using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using System.IO;

namespace Jellyfish.Configuration.Sample
{
    public class PropertiesMiddleware
    {      
		private RequestDelegate _next;
		
		public PropertiesMiddleware(RequestDelegate next) {
			_next = next;
		}
		
		public async Task Invoke(HttpContext ctx) 
		{
			ctx.Response.StatusCode=200;
			ctx.Response.ContentType = "application/json";
			
			var json = File.ReadAllText("properties.json");
			var buffers = Encoding.UTF8.GetBytes(json);
			await ctx.Response.Body.WriteAsync(buffers, 0, buffers.Length);
		}
	}
}