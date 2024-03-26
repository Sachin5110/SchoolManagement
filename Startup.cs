using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using school_management_backend.Services;
using school_management_backend.Helpers;
using school_management_backend.Interfaces;
using school_management_backend.Chat;

namespace school_management_backend
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
            services.AddCors();

            services.AddDbContext<DBContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
                );

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ITeacherService, TeacherService>();
            services.AddTransient<IStudentService, StudentService>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<ITeacherRepository, TeacherRepository>();
            services.AddTransient<IStudentRepository, StudentRepository>();
            services.AddTransient<IDashboardRepository, DashboardRepository>();
            services.AddAutoMapper(typeof(AutoMapperProfile));
            services.AddControllers(configure =>
            {
                AuditConfiguration.AddAudit(configure);
                AuditConfiguration.ConfigureAudit(services, Configuration);
            });
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();
            // custom jwt auth middleware
            app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chathub");
                endpoints.MapControllers();
            });
        }

    }
}
