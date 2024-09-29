using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Configurar serviços
builder.Services.AddControllers();

// Configuração para o HttpClient com certificado
builder.Services.AddHttpClient<ItauService>()
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        var handler = new HttpClientHandler
        {
            ClientCertificateOptions = ClientCertificateOption.Manual
        };

        // Carregar o certificado
        var settings = builder.Configuration.GetSection("ConfiguracoesItau").Get<ConfiguracoesItau>();
        var cert = new X509Certificate2(settings.CertPath, settings.CertPassword);
        handler.ClientCertificates.Add(cert);

        return handler;
    });

// Carregar configurações personalizadas
builder.Services.Configure<ConfiguracoesItau>(builder.Configuration.GetSection("ConfiguracoesItau"));

// Adicionar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar o pipeline de requisições
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger(); // Habilitar Swagger
    app.UseSwaggerUI(); // Habilitar UI do Swagger
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();
