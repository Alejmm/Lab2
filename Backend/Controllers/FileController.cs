using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace TuProyecto.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly string rutaCompartida = @"\\192.168.1.125\Compartido\archivo.txt";
        private readonly string usuario = "smbuser";
        private readonly string contrasena = "1234";
        private readonly string dominio = "."; // usar "." si es usuario local en el servidor samba

        [HttpPost]
        public IActionResult GuardarTexto([FromBody] string texto)
        {
            try
            {
                // Autenticación temporal en el recurso compartido
                using (new NetworkConnection(@"\\192.168.1.125\Compartido", new NetworkCredential(usuario, contrasena, dominio)))
                {
                    System.IO.File.WriteAllText(rutaCompartida, texto);
                }

                return Ok("Texto guardado correctamente en Samba.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al guardar el archivo: {ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult ObtenerTexto()
        {
            try
            {
                using (new NetworkConnection(@"\\192.168.1.125\Compartido", new NetworkCredential(usuario, contrasena, dominio)))
                {
                    if (!System.IO.File.Exists(rutaCompartida))
                        return NotFound("El archivo no existe.");

                    var contenido = System.IO.File.ReadAllText(rutaCompartida);
                    return Ok(contenido);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al leer el archivo: {ex.Message}");
            }
        }
    }

    // Clase auxiliar para mapear conexión SMB temporalmente con credenciales
    public class NetworkConnection : IDisposable
    {
        string _networkName;

        public NetworkConnection(string networkName, NetworkCredential credentials)
        {
            _networkName = networkName;

            var netResource = new NETRESOURCE()
            {
                dwType = 1, // RESOURCETYPE_DISK
                lpRemoteName = networkName
            };

            var result = WNetAddConnection2(
                netResource,
                credentials.Password,
                string.IsNullOrEmpty(credentials.Domain) ? credentials.UserName : $@"{credentials.Domain}\{credentials.UserName}",
                0);

            if (result != 0)
                throw new IOException("Error al conectar al recurso de red.", result);
        }

        ~NetworkConnection()
        {
            Dispose();
        }

        public void Dispose()
        {
            WNetCancelConnection2(_networkName, 0, true);
            GC.SuppressFinalize(this);
        }

        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2(NETRESOURCE lpNetResource,
            string lpPassword, string lpUserName, int dwFlags);

        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2(string lpName, int dwFlags, bool fForce);

        [StructLayout(LayoutKind.Sequential)]
        public class NETRESOURCE
        {
            public int dwScope = 0;
            public int dwType = 0;
            public int dwDisplayType = 0;
            public int dwUsage = 0;
            public string lpLocalName = null;
            public string lpRemoteName = null;
            public string lpComment = null;
            public string lpProvider = null;
        }
    }
}
