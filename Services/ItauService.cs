using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

public class ItauService
{
    private readonly HttpClient _httpClient;
    private readonly ConfiguracoesItau _settings;

    public ItauService(IHttpClientFactory httpClientFactory, IOptions<ConfiguracoesItau> options)
    {
        _settings = options.Value;

        // Cria uma instância de HttpClientHandler
        var handler = new HttpClientHandler
        {
            ClientCertificateOptions = ClientCertificateOption.Manual // Define a opção para usar o certificado manualmente
        };

        // Carrega o certificado
        var cert = new X509Certificate2(_settings.CertPath, _settings.CertPassword);
        handler.ClientCertificates.Add(cert); // Adiciona o certificado ao HttpClientHandler

        // Cria uma instância do HttpClient com o handler
        _httpClient = httpClientFactory.CreateClient(); // Você pode usar o handler para criar o HttpClient se precisar
    }

    public async Task<ResponseItauModel> CallItauApi(RequestItauController request)
    {
        var json = JsonConvert.SerializeObject(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_settings.BaseUrl}/endpoint", content); // Ajuste o endpoint

        if (response.IsSuccessStatusCode)
        {
            var responseData = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ResponseItauModel>(responseData);
        }
        
        throw new HttpRequestException($"Erro ao chamar a API do Itaú: {response.StatusCode}");
    }
}
