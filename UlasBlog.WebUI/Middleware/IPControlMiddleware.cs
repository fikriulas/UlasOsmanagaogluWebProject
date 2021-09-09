using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UlasBlog.Data.Abstract;
using UlasBlog.Data.Concrete.EntityFramework;
using UlasBlog.Entity;

namespace UlasBlog.WebUI.Middleware
{
    public class IPControlMiddleware
    {
        readonly RequestDelegate _next;
        IConfiguration _configuration;
        public IPControlMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _configuration = configuration;
            _next = next;
        }
        public async Task Invoke(HttpContext context, AppDbContext db)
        {

            //Client'ın IP adresini alıyoruz.
            string remoteIp = context.Connection.RemoteIpAddress.ToString();
            //Whitelist'te ki tüm IP'leri çekiyoruz.
            //var ips = _configuration.GetSection("WhiteList").AsEnumerable().Where(ip => !string.IsNullOrEmpty(ip.Value)).Select(ip => ip.Value).ToList();

            var ips = db.Iplists.ToList()
                              .Where(i=> i.Block == true)
                              .Select(i => new Iplist()
                              {
                                  Ip = i.Ip,
                              }).ToList();

            //Client IP, whitelist'te var mı kontrol ediyoruz.
            foreach (var item in ips)
            {
                if (item.Ip == remoteIp)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    await context.Response.WriteAsync("Erisim Engellendi. Bilgi icin; \n=>info@ulasosmanagaoglu.com");
                    return;
                }
            }

            await _next.Invoke(context);

        }
    }
}
