// Install-Package System.Data.SqlClient
// Install-Package Dapper
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Dapper;
using System.Data.SqlClient;
using System.Data;

namespace VideoAppCore.Models
{
    public class VideoRepositoryDapper : IVideoRepositoryAsync
    {
        private readonly IDbConnection db; 

        public VideoRepositoryDapper(string connectionString)
        {
            db = new SqlConnection(connectionString);
        }

        public async Task<Video> AddVideoAsync(Video model)
        {
            const string query =
                "Insert Into Videos(Title, Url, Name, Company, CreatedBy) Values(@Title, @Url, @Name, @Company, @CreatedBy);" +
                "Select Cast(SCOPE_IDENTITY() As Int);";

            int id = await db.ExecuteScalarAsync<int>(query, model);

            model.Id = id;

            return model; 
        }

        public async Task<Video> GetVideoByIdAsync(int id)
        {
            const string query = "Select * From Videos Where Id = @Id";

            var video = await db.QueryFirstOrDefaultAsync<Video>(query, new { id }, commandType: CommandType.Text);

            return video; 
        }

        public async Task<List<Video>> GetVideosAsync()
        {
            const string query = "Select * From Videos;";

            var videos = await db.QueryAsync<Video>(query);

            return videos.ToList(); 
        }

        public async Task RemoveVideoAsync(int id)
        {
            const string query = "Delete Videos Where Id = @Id";

            await db.ExecuteAsync(query, new { id }, commandType: CommandType.Text);
        }

        public async Task<Video> UpdateVideoAsync(Video model)
        {
            const string query = @"
                    Update Videos 
                    Set 
                        Title = @Title, 
                        Url = @Url, 
                        Name = @Name, 
                        Company = @Company, 
                        ModifiedBy = @ModifiedBy 
                    Where Id = @Id";

            await db.ExecuteAsync(query, model);

            return model;
        }
    }
}

//ASP.NET & Core를 다루는 기술 13장에서 발췌
//CRUD와 연관된 메서드 이름

//CRUD 관련 메서드 이름을 지을 때에는 Add, Get, Update, Remove 등의 단어를 많이 사용한다.이러한 단어를 접두사 또는 접미사로 사용하는 것은 권장 사항이지 필수 사항은 아니다.
//Add()
//	AddHero()
//Get()
//	GetAll(): 최근에 GetAll() 메서드 이름을 많이 사용하는 경향이 있다.
//	GetHeroes()
//GetById()
//	GetHeroById()
//Update()
//	UpdateHero()
//Remove()
//	RemoveHero()
