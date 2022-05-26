using challenge.Data;
using challenge.Repositories;
using challenge.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace code_challenge
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
			services.AddSwaggerGen();
			services.AddDbContext<EmployeeContext>(options =>
			{
				options.UseInMemoryDatabase("EmployeeDB");
			});
			services.AddScoped<IEmployeeRepository, EmployeeRespository>();
			services.AddTransient<EmployeeDataSeeder>();
			services.AddScoped<IEmployeeService, EmployeeService>();
			services.AddMvc();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, EmployeeDataSeeder seeder)
		{
			if (env.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
				app.UseDeveloperExceptionPage();
				seeder.Seed().Wait();
			}

			app.UseMvc();
		}
	}
}
