using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace VideoAppCore.Models.Tests
{
    [TestClass]
    public class VideoAppCoreModelsTest
    {
        private readonly VideoRepositoryDapper _repository;

        public VideoAppCoreModelsTest()
        {
            string connectionString = "server=(localdb)\\mssqllocaldb;database=VideoAppCore;Integrated Security=true;";
            _repository = new VideoRepositoryDapper(connectionString);
        }

        [TestMethod]
        public async Task AddVideoAsyncTest()
        {
            //Video video = new Video { Title = "ADO.NET", Url = "URL", Name = "Park", Company = "VisualAcademy", CreatedBy = "Park" };
            Video video = new Video { Title = "Dapper", Url = "URL", Name = "Park", Company = "VisualAcademy", CreatedBy = "Park" };

            Video newVideo = await _repository.AddVideoAsync(video);

            Assert.AreEqual(4, newVideo.Id);
        }

        [TestMethod]
        public async Task GetVideosAsyncTest()
        {
            var videos = await _repository.GetVideosAsync();

            foreach (var video in videos)
            {
                Console.WriteLine($"{video.Id} - {video.Title}");
            }
        }

        [TestMethod]
        public async Task GetVideoByIdAsyncTest()
        {
            var video = await _repository.GetVideoByIdAsync(4);
            Console.WriteLine($"{video.Id} - {video.Title}");
        }

        [TestMethod]
        public async Task UpdateVideoAsyncTest()
        {
            Video video = new Video { Id = 4, Title = "EF Core", Url = "URL", Name = "Park", Company = "VisualAcademy", ModifiedBy = "Park" };
            await _repository.UpdateVideoAsync(video);
        }

        [TestMethod]
        public async Task RemoveVideoAsyncTest()
        {
            await _repository.RemoveVideoAsync(3);
        }
    }
}
