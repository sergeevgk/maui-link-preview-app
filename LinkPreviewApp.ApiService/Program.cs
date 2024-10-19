using LinkPreviewApp.ApiService;
using LinkPreviewApp.ApiService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddTransient<IUrlDataService, ExternalUrlDataService>();

var linkPreviewServiceApiKey = Environment.GetEnvironmentVariable("LinkPreviewService__ApiKey");
builder.Services.Configure<LinkPreviewServiceSettings>(options =>
	{
		builder.Configuration.GetSection("LinkPreviewService").Bind(options);
		options.ApiKey ??= linkPreviewServiceApiKey;
	});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
