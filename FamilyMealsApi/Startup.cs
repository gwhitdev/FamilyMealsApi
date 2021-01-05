using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using FamilyMealsApi.Models;
using FamilyMealsApi.Services;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.Swagger;

namespace FamilyMealsApi
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
            services.Configure<IngredientsDatabaseSettings>(
                Configuration.GetSection(nameof(IngredientsDatabaseSettings)));

            services.AddSingleton<IIngredientsDatabaseSettings>(sp =>
            sp.GetRequiredService<IOptions<IngredientsDatabaseSettings>>().Value);

            services.AddSingleton<IngredientsService>();

            services.AddControllers();
            
            services.AddSwaggerGen(options =>
            {   
                options.SwaggerDoc(name: "v1", info: new OpenApiInfo
                {
                    Title = "Ingredients Service API",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "Gareth Whitley",
                        Email = "me@garethwhitley.online",
                        Url = new System.Uri("https://garethwhitley.online")
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
           {
               c.SwaggerEndpoint("/swagger/v1/swagger.json",
                   "Ingredients Service API Version 1");
               c.SupportedSubmitMethods(new[]
               {
                    SubmitMethod.Get, SubmitMethod.Post,
                    SubmitMethod.Put, SubmitMethod.Delete
               });
           });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
