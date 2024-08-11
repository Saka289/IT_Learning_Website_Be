using LW.Infrastructure.Hubs;

namespace LW.API.Extensions;

public static class ApplicationExtensions
{
    public static void UseInfrastructure(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        // // Configure the HTTP request pipeline.
        // if (env.IsDevelopment())
        // {
        //     app.UseSwagger();
        //     app.UseSwaggerUI();
        // }

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "LW-API");
            options.RoutePrefix = string.Empty;
        });
        app.UseCors();

        app.UseRouting();
        // for production only 
        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
            endpoints.MapHub<NotificationHub>("/notificationHub");
        });
    }
}