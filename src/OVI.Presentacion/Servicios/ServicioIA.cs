using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
namespace OVI.Presentacion.Servicios;
public class ServicioIA(HttpClient http,IConfiguration config,ILogger<ServicioIA> log)
{
 public bool Configurada=>!string.IsNullOrWhiteSpace(config["IA:ApiKey"]) && !string.IsNullOrWhiteSpace(config["IA:Endpoint"]) && !string.IsNullOrWhiteSpace(config["IA:Model"]);
 public async Task<string?> PreguntarAsync(string mensaje,string? contexto,CancellationToken ct)
 {
  if(!Configurada)return null;
  var endpoint=config["IA:Endpoint"]!;
  if(!Uri.TryCreate(endpoint,UriKind.Absolute,out var uri)||uri.Scheme!=Uri.UriSchemeHttps)throw new InvalidOperationException("La URL de IA debe usar HTTPS.");
  using var req=new HttpRequestMessage(HttpMethod.Post,uri);
  req.Headers.Authorization=new AuthenticationHeaderValue("Bearer",config["IA:ApiKey"]);
  req.Content=JsonContent.Create(new{model=config["IA:Model"],messages=new object[]{new{role="system",content="Eres un asistente de orientación vocacional para estudiantes de El Salvador. Da información general y recomendaciones orientativas, sin garantizar admisión, empleo ni diagnosticar. No solicites datos sensibles. Contexto de perfil: "+(contexto??"no disponible")},new{role="user",content=mensaje}},temperature=0.4,max_tokens=450});
  try
  {
   using var resp=await http.SendAsync(req,ct);resp.EnsureSuccessStatusCode();
   using var json=await JsonDocument.ParseAsync(await resp.Content.ReadAsStreamAsync(ct),cancellationToken:ct);
   return json.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
  }
  catch(Exception ex) when(ex is HttpRequestException or TaskCanceledException or JsonException or KeyNotFoundException or InvalidOperationException)
  {log.LogWarning(ex,"El proveedor de IA no respondió correctamente");return null;}
 }
}
