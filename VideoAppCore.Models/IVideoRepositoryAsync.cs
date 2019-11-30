using System.Collections.Generic;
using System.Threading.Tasks;

namespace VideoAppCore.Models
{
    /// <summary>
    /// [3][2] 인터페이스(비동기 방식): Videos 테이블에 대한 CRUD API 명세서 작성
    /// </summary>
    public interface IVideoRepositoryAsync
    {
        Task<Video> AddVideoAsync(Video model);        // 입력: T Add(T model);
        Task<List<Video>> GetVideosAsync();            // 출력: List<T> GetAll();
        Task<Video> GetVideoByIdAsync(int id);         // 상세: GetById(int id);
        Task<Video> UpdateVideoAsync(Video model);     // 수정: T Edit(T model);
        Task RemoveVideoAsync(int id);           // 삭제: void Delete(int id);
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
