// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860
namespace Microsoft.eShopOnContainers.Services.Catalog.API.Controllers;

[ApiController]
public class PicController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly CatalogDbContext _catalogContext;

    public PicController(IWebHostEnvironment env,
        CatalogDbContext catalogContext)
    {
        _env = env;
        _catalogContext = catalogContext;
    }

    [HttpGet]
    [Route("api/v1/catalog/items/{catalogItemId:int}/pic")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    // GET: /<controller>/
    public async Task<ActionResult> GetImageAsync(int catalogItemId)
    {
        if (catalogItemId <= 0)
        {
            return BadRequest();
        }

        var item = await _catalogContext.CatalogItems
            .SingleOrDefaultAsync(ci => ci.Id == catalogItemId);

        if (item != null)
        {
            var webRoot = _env.ContentRootPath;
            Console.WriteLine("The root path is{0}", webRoot);
            //get the image file path from environment variables if available or use "Pics" folder
            var picFolder = Environment.GetEnvironmentVariable("PICS_FOLDER") ?? "Pics";
            var path = Path.Combine(_env.ContentRootPath, picFolder, item.PictureFileName);
            Console.WriteLine("The root path is{0}", path); 
            string imageFileExtension = Path.GetExtension(item.PictureFileName);
            string mimetype = GetImageMimeTypeFromImageFileExtension(imageFileExtension);

            var buffer = await System.IO.File.ReadAllBytesAsync(path);

            return File(buffer, mimetype);
        }

        return NotFound();
    }

    private string GetImageMimeTypeFromImageFileExtension(string extension)
    {
        string mimetype = extension switch
        {
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".bmp" => "image/bmp",
            ".tiff" => "image/tiff",
            ".wmf" => "image/wmf",
            ".jp2" => "image/jp2",
            ".svg" => "image/svg+xml",
            _ => "application/octet-stream",
        };
        return mimetype;
    }
}
