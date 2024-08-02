using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hello_world_api.Models;
using hello_world_api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace hello_world_api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddDbContext<HelloDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("MssqlConnectionString")));
            services.AddControllers().AddXmlDataContractSerializerFormatters();
            AddEncryptionManagers(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }

        /// <summary>
        /// Initialize, configure, and add singleton instances of the SymmetricEncryptionManager and AsymmetricEncryptionManager classes.
        /// </summary>
        private async void AddEncryptionManagers(IServiceCollection services)
        {
            // TODO remove old code
            // string key = "MPxBpDwpOpyS1k6kxdE++7KelLkvt99bSPZC2c0B/Mc=";
            // SymmetricEncryptionManager symmetricEncryptionManager = new SymmetricEncryptionManager(Encoding.ASCII.GetBytes(key));

            string symmetricKey = Configuration.GetValue<string>("Cryptography:AESkey");
            SymmetricEncryptionManager symmetricEncryptionManager = new SymmetricEncryptionManager(Encoding.ASCII.GetBytes(symmetricKey));
            services.AddSingleton(symmetricEncryptionManager);

            using(var publicSr = new StreamReader(Configuration.GetValue<string>("Cryptography:RSAPublicKey")))
            using (var privateSr = new StreamReader(Configuration.GetValue<string>("Cryptography:RSAPrivateKey")))
            {
                string asymmetricPublicKey = await publicSr.ReadToEndAsync();
                string asymmetricPrivateKey = await privateSr.ReadToEndAsync();
                AsymmetricEncryptionManager asymmetricEncryptionManager = new AsymmetricEncryptionManager(asymmetricPublicKey.ToCharArray(), asymmetricPrivateKey.ToCharArray());
                services.AddSingleton(asymmetricEncryptionManager);
            }
        }

    }
}
