namespace CourseProject1.ServiceContracts
{
    public interface IUploadImage
    {
        public Task<string> UploadImageToCloudStorage(IFormFile imageFile);
    }
}
