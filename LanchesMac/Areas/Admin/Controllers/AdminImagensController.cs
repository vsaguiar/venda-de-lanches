using LanchesMac.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LanchesMac.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminImagensController : Controller
    {
        private readonly ConfigurationImagens _myConfig; // através dessa classe podemos obter o nome da pasta onde os arquivos serão salvos
        private readonly IWebHostEnvironment _hostingEnvironment;

        public AdminImagensController(IWebHostEnvironment hostingEnvironment, IOptions<ConfigurationImagens> myConfiguration)
        {
            _hostingEnvironment = hostingEnvironment;
            _myConfig = myConfiguration.Value;
        }


        public IActionResult Index()
        {
            return View();
        }



        public async Task<IActionResult> UploadFiles(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                ViewData["Erro"] = "Error: Arquivo(s) não selecionado(s)";
                return View(ViewData);
            }

            if (files.Count > 10)
            {
                ViewData["Erro"] = "Error: Quantidade de arquivos excedeu o limite";
                return View(ViewData);
            }

            long size = files.Sum(f => f.Length);

            var filePathsName = new List<string>(); // armezando os nomes dos arquivos que foram enviados

            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, _myConfig.NomePastaImagensProdutos); // caminho completo do local onde os arquivos serão armazenados

            foreach (var formFile in files)
            {
                if (formFile.FileName.Contains(".jpg") || formFile.FileName.Contains(".gif") || formFile.FileName.Contains(".png"))
                {
                    var fileNameWithPath = string.Concat(filePath, "\\", formFile.FileName);

                    filePathsName.Add(fileNameWithPath);

                    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
                else
                {
                    ViewData["Erro"] = "Error: Arquivo deve ser do tipo .jpg .gif ou .png";
                }
            }
            if (files.Count > 1)
            {
                ViewData["Resultado"] = $"{files.Count} arquivos foram enviados ao servidor com tamanho de: {size} bytes";
            }
            else
            {
                ViewData["Resultado"] = $"{files.Count} arquivo foi enviado ao servidor com tamanho de: {size} bytes";
            }

            ViewBag.Arquivos = filePathsName;

            return View(ViewData);
        }
    }
}
