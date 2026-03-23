using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;

namespace MinioProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController(MinioClient minioClient) : ControllerBase
    {
        private const string BucketName = "minioproject";

        [HttpPost("[action]")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            await minioClient.PutObjectAsync(
                new PutObjectArgs()
                    .WithBucket(BucketName)
                    .WithObject(file.FileName)
                    .WithStreamData(file.OpenReadStream())
                    .WithObjectSize(file.Length));
            
            return Ok();
        }

        [HttpGet("[action]/{filename}")]
        public async Task<IActionResult> Download(string fileName)
        {
            var stream = new MemoryStream();
            await minioClient.GetObjectAsync(
                new GetObjectArgs()
                    .WithBucket(BucketName)
                    .WithObject(fileName)
                    .WithCallbackStream(s => s.CopyTo(stream)));

            stream.Position = 0;
            
            return File(stream, "application/octet-stream", fileName);
        }

        [HttpGet("[action]/{filename}")]
        public async Task<IActionResult> Show(string fileName)
        {
            var stream = new MemoryStream();

            await minioClient.GetObjectAsync(
                new GetObjectArgs()
                    .WithBucket(BucketName)
                    .WithObject(fileName)
                    .WithCallbackStream(s => s.CopyTo(stream)));

            stream.Position = 0;
            
            var contentType = GetContentType(fileName);

            return File(stream, contentType);
        }
        
        private string GetContentType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLower();

            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
    }
    
    
}
