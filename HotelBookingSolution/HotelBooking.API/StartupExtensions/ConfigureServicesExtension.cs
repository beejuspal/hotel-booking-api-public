using CloudinaryDotNet;
using Hangfire;
using Hangfire.SqlServer;
using HotelBooking.API.Filters;
using HotelBooking.API.Middleware;
using HotelBooking.Core.Domain.RepositoryContracts;
using HotelBooking.Core.ServiceContracts;
using HotelBooking.Core.ServiceContracts.IAmenityService;
using HotelBooking.Core.ServiceContracts.IDashboard;
using HotelBooking.Core.ServiceContracts.IHotelService;
using HotelBooking.Core.ServiceContracts.IPayment;
using HotelBooking.Core.ServiceContracts.IReservation;
using HotelBooking.Core.ServiceContracts.IRoomAmenities;
using HotelBooking.Core.ServiceContracts.IRoomService;
using HotelBooking.Core.ServiceContracts.IRoomTypeService;

using HotelBooking.Core.Services;
using HotelBooking.Core.Services.AmenityService;
using HotelBooking.Core.Services.Dashbaord;
using HotelBooking.Core.Services.HotelService;
using HotelBooking.Core.Services.PaymentService;
using HotelBooking.Core.Services.ReservationService;
using HotelBooking.Core.Services.RoomAmenitiesService;
using HotelBooking.Core.Services.RoomService;
using HotelBooking.Core.Services.RoomTypeService;
using HotelBooking.Core.Settings;
using HotelBooking.Infrastructure.DBContext;
using HotelBooking.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Profiling;
using System.Text;

namespace HotelBooking.API.StartupExtensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureServicesExtension(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<KhaltiSettings>(
    configuration.GetSection("KhaltiSettings"));
            services.Configure<CloudinarySettings>(
    configuration.GetSection("CloudinarySettings"));
            services.AddSingleton(sp =>
            {
                var config = sp.GetRequiredService<IOptions<CloudinarySettings>>().Value;
                return new Cloudinary(new Account(config.CloudName, config.ApiKey, config.ApiSecret));
            });

            services.Configure<JwtSettings>(configuration.GetSection("ApiSettings"));
           services.AddSingleton(sp => sp.GetRequiredService<IOptions<JwtSettings>>().Value);
            var key = configuration.GetValue<string>("ApiSettings:Secret");
            services.AddAuthentication(u =>
            {
                u.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                u.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
.AddJwtBearer(u =>
{
    u.RequireHttpsMetadata = false;
    u.SaveToken = true;
    u.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false,

        // ✅ Enforce expiry
        ValidateLifetime = true,

        // ✅ No extra grace period
        ClockSkew = TimeSpan.Zero
    };
});
            services.AddCors();
            // Swagger

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description =
                    "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
                    "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                    "Example: \"Bearer 123455rtdfgt\"",
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });
                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference=new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                },
                Scheme="oauth2",
                Name="Bearer",
                In=ParameterLocation.Header
            },
            new List<string>()
        }
    });
            });

            // Add services to the container.

            services.AddControllers();
            // Add MiniProfiler
          services.AddMiniProfiler(options =>
            {
                options.RouteBasePath = "/mini-profiler";
                options.PopupRenderPosition = RenderPosition.BottomLeft;
                options.PopupShowTimeWithChildren = true;
            }).AddEntityFramework();
            //add services into IoC container
            //services.AddSingleton<IAuthorizationMiddlewareResultHandler, CustomAuthorizationResultHandler>();
            //services.AddScoped<RoleAuthorizationFilter>();
          
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IRoomTypeRepository, RoomTypeRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddScoped<IHotelRepository, HotelRepository>();
            services.AddScoped<IHotelImageRepository, HotelImageRepository>();
            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IReservationRepository, ReservationRepository>();
            services.AddScoped<IAmenityRepository, AmenityRepository>();
            services.AddScoped<IRoomAmenitiesRepository, RoomAmenityRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserAdderService, UserAdderService>();
            services.AddScoped<IUserGetterService, UserGetterService>();
            services.AddScoped<IRoomTypeGetterService, RoomTypeGetterService>();
            services.AddScoped<IRoomTypeAdderService, RoomTypeAdderService>();
            services.AddScoped<IRoomTypeUpdaterService, RoomTypeUpdaterService>();
            services.AddScoped<IRoomTypeDeleterService, RoomTypeDeleterService>();
            services.AddScoped<IPasswordHasherService, PasswordHasherService>();
            services.AddScoped<IUserValidatorService, UserValidatorService>();
            services.AddScoped<IHotelAdderService, HotelAdderService>();
            services.AddScoped<IHotelGetterService, HotelGetterService>();
            services.AddScoped<IHotelAdderService, HotelAdderService>();
            services.AddScoped<IHotelUpdaterService, HotelUpdaterService>();
            services.AddScoped<IHotelDeleterService, HotelDeleterService>();
            services.AddScoped<IImageStorageService, CloudinaryImageStorageService>();
            services.AddScoped<IRoomGetterService, RoomGetterService>();
            services.AddScoped<IRoomAdderService, RoomAdderService>();
            services.AddScoped<IRoomUpdaterService, RoomUpdaterService>();
            services.AddScoped<IRoomDeleterService, RoomDeleterService>();
            services.AddScoped<IAmenityAdderService, AmenityAdderService>();
            services.AddScoped<IAmenityGetterService, AmenityGetterService>();
            services.AddScoped<IAmenityDeleterService, AmenityDeleterService>();
            services.AddScoped<IAmenityUpdaterService, AmenityUpdaterService>();
            services.AddScoped<IRoomAmenityService, RoomAmenityService>();
            services.AddScoped<IReservationService, ReservationService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IPaymentService, PaymentService>();

            services.AddScoped<IEmailService, EmailService>();

            //services.AddScoped<ICountriesGetterService, CountriesGetterService>();
            //services.AddScoped<ICountriesAdderService, CountriesAdderService>();
            //services.AddScoped<ICountriesUploaderService, CountriesUploaderService>();

            //services.AddScoped<IPersonsGetterService, PersonsGetterServiceWithFewExcelFields>();
            //services.AddScoped<PersonsGetterService, PersonsGetterService>();

            //services.AddScoped<IPersonsAdderService, PersonsAdderService>();
            //services.AddScoped<IPersonsDeleterService, PersonsDeleterService>();
            //services.AddScoped<IPersonsUpdaterService, PersonsUpdaterService>();
            //services.AddScoped<IPersonsSorterService, PersonsSorterService>();

            //services.AddDbContext<HotelDbContext>(options =>
            //{
            //    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            //});
            // ✅ Hangfire configuration
            services.AddHangfire(config =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                      .UseSimpleAssemblyNameTypeSerializer()
                      .UseRecommendedSerializerSettings()
                      .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
                      {
                          CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                          SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                          QueuePollInterval = TimeSpan.FromSeconds(15),
                          UseRecommendedIsolationLevel = true,
                          DisableGlobalLocks = true
                      });
            });

            services.AddHangfireServer();
            services.AddDbContext<HotelDbContext>(options =>
    options.UseSqlServer(
        configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("HotelBooking.Infrastructure")
    ));


            return services;
        }
    }
}

