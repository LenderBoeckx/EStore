using System;
using Core.Interfaces;

namespace API.RequestHelpers;

public class UploadHandler(IProductRepository productService)
{

private readonly IProductRepository _productService = productService;

    public async Task<string> Upload(IFormFile image){
        List<string> extentions = new List<string>() {".jpg", ".jpeg", ".png"};
        string extention = Path.GetExtension(image.FileName);
        IReadOnlyList<string> filenames = await _productService.GetAfbeeldingenAsync();

        if(filenames.Contains(image.FileName))
        {
            return $"Er is al een afbeelding met dezelfde naam.";
        }

        if(!extentions.Contains(extention)){
            return $"Het bestand heeft geen geldige bestandsextentie. (toegelaten: {string.Join(',', extentions)})";
        }

        long size = image.Length;

        if(size > (5 * 1024 * 1024)){
            return "De afbeelding is te groot. Maximale bestandsgrootte is 5MB.";
        }

        string filename = image.FileName;
        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot","images","products");
        using FileStream stream = new FileStream(Path.Combine(path, filename), FileMode.Create);
        image.CopyTo(stream);

        return filename;
    }
}
