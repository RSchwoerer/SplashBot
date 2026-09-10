using SplashBot_2.Service;
using Unsplasharp.Models;

namespace SplashBot_2
{
    internal class PhotoHistoryViewModel
    {
        private readonly UnsplashService unsplashService;

        public PhotoHistoryViewModel(UnsplashService us)
        {
            this.unsplashService = us;
        }

        public List<Photo> Photos { get; set; }

        public async Task<bool> Rehydrate()
        {
            for (int p = 0; p < Photos.Count; p++)
            {
                Photo? photo = Photos[p];
                var newP = await unsplashService.GetPhoto(photo.Id);
                Photos[p] = newP;
            }

            return true;
        }
    }
}