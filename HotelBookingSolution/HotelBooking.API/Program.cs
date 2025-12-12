

using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using HotelBooking.API.Filters;
using HotelBooking.API.Middleware;
using HotelBooking.API.StartupExtensions;
using HotelBooking.Core.Services.Jobs;
using HotelBooking.Infrastructure.DBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Initialize Serilog from appsettings.json
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services));


builder.Services.ConfigureServicesExtension(builder.Configuration);
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
var app = builder.Build();
app.UseSwagger();
if (app.Environment.IsDevelopment())
{

    app.UseSwaggerUI();
}
else
{
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty;
    });
}
// Configure the HTTP request pipeline.
app.UseExceptionHandlingMiddleware();
app.UseHttpsRedirection();

app.UseCors(o => o.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().WithExposedHeaders("*"));

app.UseAuthentication();
app.UseAuthorization();
app.UseMiniProfiler();

// ? Hangfire Dashboard
//app.UseHangfireDashboard("/hangfire", new DashboardOptions
//{
//    Authorization = new[] { app.Services.GetRequiredService<HangfireDashboardAuthFilter>() }
//});
if (app.Environment.IsDevelopment())
{
    app.UseHangfireDashboard("/hangfire");
}
else
{
    var hangUserName = builder.Configuration["HangFireSettings:UserName"];
    var hangPass = builder.Configuration["HangFireSettings:Password"];
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[] {
        new BasicAuthAuthorizationFilter( new BasicAuthAuthorizationFilterOptions {
    RequireSsl = true,
    Users = new[] {
        new BasicAuthAuthorizationUser { Login = hangUserName, PasswordClear = hangPass }
    }
})
    }
    });
}
    

// ? Schedule Hangfire recurring job
RecurringJob.AddOrUpdate<ReservationJobService>(
    "update-expired-reservations",
    job => job.UpdateExpiredReservationsAsync(),
    Cron.Daily(0, 0) // Run daily at midnight
);

// ? Schedule the job for every day at 11:30 AM
//RecurringJob.AddOrUpdate<ReservationJobService>(
//    "auto-cancel-expired-reservations",
//    service => service.UpdateExpiredReservationsAsync(),
//    "42 14 * * *",   // CRON expression for 2:45 PM
//    TimeZoneInfo.FindSystemTimeZoneById("Nepal Standard Time") // UTC+5:45
//);
app.MapControllers();

// Run migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<HotelDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}


app.Run();
