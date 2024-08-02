using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using hello_world_api.Models;
using hello_world_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace hello_world_api.Controllers
{
    [Route("api/[controller]")]
    public class HelloController : Controller
    {
        private readonly SymmetricEncryptionManager _symmetricEncryptionManager;
        private readonly AsymmetricEncryptionManager _asymmetricEncryptionManager;
        private readonly byte[] _constantIv = new byte[16];

        public HelloController(SymmetricEncryptionManager symmetricEncryptionManager, AsymmetricEncryptionManager asymmetricEncryptionManager)
        {
            this._symmetricEncryptionManager = symmetricEncryptionManager;
            this._asymmetricEncryptionManager = asymmetricEncryptionManager;
        }

        // GET api/helllo
        [HttpGet]
        public string Get()
        {
            return "Hello world!!!!!!";
        }

        [HttpPut("encrypt/{symmetric}")]
        public string encrypt(bool symmetric, [FromBody] string plainText)
        {
            if (symmetric)
            {
                // TODO use SymmetricEncryptionManager.encrypt once I figure out how to do that!
                Aes aes = Aes.Create();
                aes.Key = new byte[] { 0x61, 0x63, 0x39, 0x71, 0x56, 0x44, 0x6E, 0x71, 0x76, 0x51, 0x73, 0x57, 0x78, 0x75, 0x6D, 0x39, 0x66, 0x69, 0x4E, 0x61, 0x4B, 0x79, 0x4D, 0x42, 0x6F, 0x68, 0x46, 0x77, 0x57, 0x36, 0x4D, 0x50, 0x52, 0x51, 0x4F, 0x2B, 0x74, 0x4D, 0x53, 0x4D, 0x34, 0x70, 0x55, 0x3D };
                ICryptoTransform encryptor = aes.CreateEncryptor();
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        return msEncrypt.ToString();
                    }
                }
            } else
            {
                return this._asymmetricEncryptionManager.encrypt(Encoding.ASCII.GetBytes(plainText)).ToString();
            }
        }

        [HttpPut("decrypt/{symmetric}")]
        public string decrypt(bool symmetric, [FromBody] string cipherText)
        {
            if (symmetric)
            {
                return this._symmetricEncryptionManager.decrypt(Encoding.ASCII.GetBytes(cipherText), this._constantIv).ToString();
            } else
            {
                return this._asymmetricEncryptionManager.decrypt(Encoding.ASCII.GetBytes(cipherText)).ToString();
            }
        }

        [HttpGet]
        public string GetMilestonesTest()
        {
            ServicePointManager.ServerCertificateValidationCallback = (obj, certificate, chain, errors) => (true);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var req = WebRequest.CreateHttp(new Uri("https://gitlab.us.nelnet.biz/nds-it-delivery/common/tools/appsec/sandbox/dotnet-hello-world/milestones"));
            req.Headers.Add("Private-Token", "glpat-aTgZMQqiZ35vF4R6RziY");
			req.Headers.Add("Private-Token", "glpat-aTgZabviZ35vF4R6Rabc");
            req.Accept = "application/json";
            req.Method = "GET";
            var res = req.GetResponse();
            return res.GetResponseStream().ToString();
        }


        // <HelloDto Version="1"><apiKey>bf2bd32e-bbc3-4f8b-a2ab-78cb7414ca20</apiKey><tableName>Products</tableName><fieldName>PriceUSD</fieldName><value>2.00</value></HelloDto>
        [HttpPost]
        [Consumes("application/xml")]
        public IActionResult HelloService(HelloDto dto)
        {
            if (IsValidRequest(dto.apiKey))
            {
                Type type = Type.GetType($"hello_world_api.Models.{dto.tableName}, Models, Version=1.0.0.0, Culture=neutral");

                typeof(LegacyUpdateService).GetMethod("Update").MakeGenericMethod(type).Invoke(null, new object[] { dto.fieldName, dto.value });

                return Ok();
            }

            return Unauthorized();
        }

        #region Legacy
        public Boolean IsValidRequest(string apiKey)
        {
            if (apiKey == "j18xak9x-9ans-1ks0-8xks-1jx8ap0ngl8q") { return true; }

            return false;
        }
        #endregion
    }

    #region LegacyService
    public static class LegacyUpdateService
    {

        public static void Update<T>(string field, string value)
        {
            // blindly set the field and value
        }
    }
    #endregion
}
