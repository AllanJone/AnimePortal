using AnimePortal.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace AnimePortal.Controllers
{
    public class AnimesController : Controller
    {
        // GET: Animes
        public ActionResult List()
        {
            string animeList = GetAnimesFromAPI();
            var animeObj = MapAnimesFromJSON(animeList);
            ViewData["AnimeModel"] = animeObj;
            return View();
        }

        public ActionResult Index(int id)
        {
            string anime = GetAnimeFromAPI(id);
            var animeObj = MapAnimeFromJSON(anime);
            ViewData["AnimeModel"] = animeObj;
            return View();
        }

        public string GetAnimeFromAPI(int id)
        {
            try
            {
                string animeData = "";
                var client = new HttpClient();
                var getDataTask = client.GetAsync("https://kitsu.io/api/edge/anime/"+id)
                    .ContinueWith(response =>
                    {
                        var result = response.Result;
                        if (result.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var responseData = result.Content.ReadAsStringAsync();
                            responseData.Wait();
                            animeData = responseData.Result;
                        }
                    });

                getDataTask.Wait();
                return animeData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetAnimesFromAPI()
        {
            try
            {
                string animeData = "";
                var client = new HttpClient();
                var getDataTask = client.GetAsync("https://kitsu.io/api/edge/anime?page[limit]=20&page[offset]=0")
                    .ContinueWith(response =>
               {
                   var result = response.Result;
                   if (result.StatusCode == System.Net.HttpStatusCode.OK)
                   {
                       var responseData = result.Content.ReadAsStringAsync();
                       responseData.Wait();
                       animeData = responseData.Result;
                   }
               });

                getDataTask.Wait();
                return animeData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Anime MapAnimeFromJSON(string jsonObj)
        {
            int id = 0;
            string name = "";
            string imageUrl = "";
            Anime anime = new Anime();
            JObject jObject = JObject.Parse(jsonObj);
            id = (int)jObject["data"]["id"];
            name = (string)jObject["data"]["attributes"]["titles"]["en_jp"];
            imageUrl = (string)jObject["data"]["attributes"]["posterImage"]["medium"];
            return new Anime() { id = id, name = name, imageUrl = imageUrl };
        }

        public List<Anime> MapAnimesFromJSON(string jsonObj)
        {
            int id = 0;
            string name = "";
            string imageUrl = "";
            List<Anime> animeList = new List<Anime>();
            JObject jObject = JObject.Parse(jsonObj);
            for (int i = 0; i < 20; i++)
            {
                id = (int)jObject["data"][i]["id"];
                name = (string)jObject["data"][i]["attributes"]["titles"]["en_jp"];
                imageUrl = (string)jObject["data"][i]["attributes"]["posterImage"]["medium"];
                animeList.Add(new Anime() { id = id, name = name, imageUrl = imageUrl });
            }
            return animeList;
        }
    }
}