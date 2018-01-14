using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using PersonSearch.Models;

namespace PersonSearch.Controllers
{
    public class SearchController : Controller
    {
        public ActionResult SimpleSearch()
        {
            string file = Server.MapPath("~/App_Data/data_large.json");
            //deserialize JSON from file  
            string JSONresult = System.IO.File.ReadAllText(file);



            JObject JObject = JObject.Parse(JSONresult);
            JArray people = (JArray)JObject["people"];
            JArray places = (JArray)JObject["places"];


            var join = people.Join(places, s => s["place_id"].Value<string>(),
                                    d => d["id"].Value<string>(),
                                    (s, d) => new {
                                        ID = s["id"].Value<string>(),
                                        NAME = s["name"].Value<string>(),
                                        GENDER = s["gender"].Value<string>() == "M" ? "Male" : "Female",
                                        BIRTHPLACE = d["name"].Value<string>()
                                    });




            string JSONFinalresult = JsonConvert.SerializeObject(join);
            List<People> myDeserializedObjList = (List<People>)Newtonsoft.Json.JsonConvert.DeserializeObject(JSONFinalresult, typeof(List<People>));

            SortingPagingInfo info = new SortingPagingInfo();
            info.SortField = "ID";
            info.SortDirection = "ascending";
            info.PageSize = 10;
            int rem = myDeserializedObjList.Count() % info.PageSize;
            if (rem == 0)
            {
                info.PageCount = Convert.ToInt32(Math.Ceiling((double)(myDeserializedObjList.Count() / info.PageSize)));
            }
            else
            {
                info.PageCount = Convert.ToInt32(Math.Ceiling((double)(myDeserializedObjList.Count() / info.PageSize))) + 1;
            }
            info.CurrentPageIndex = 0;

            var query = myDeserializedObjList.AsQueryable().Take(info.PageSize);

            ViewBag.SortingPagingInfo = info;
            List<People> model = query.ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult SimpleSearch(SortingPagingInfo info)
        {
            string file = Server.MapPath("~/App_Data/data_large.json");
            //deserialize JSON from file  
            string JSONresult = System.IO.File.ReadAllText(file);
            IQueryable<People> query = null;
            List<People> model = new List<People>();

            JObject JObject = JObject.Parse(JSONresult);
            JArray people = (JArray)JObject["people"];
            JArray places = (JArray)JObject["places"];


            var join = people.Join(places, s => s["place_id"].Value<string>(),
                                  d => d["id"].Value<string>(),
                                  (s, d) => new {
                                      ID = s["id"].Value<string>(),
                                      NAME = s["name"].Value<string>(),
                                      GENDER = s["gender"].Value<string>() == "M" ? "Male" : "Female",
                                      BIRTHPLACE = d["name"].Value<string>()
                                  });


            if (!string.IsNullOrEmpty(info.name) || !string.IsNullOrEmpty(info.gender))
            {
                if (!string.IsNullOrEmpty(info.name) && string.IsNullOrEmpty(info.gender))
                {
                    join = join.Where(s => s.NAME.ToLower().Contains(info.name.ToLower()));
                }
                else if (string.IsNullOrEmpty(info.name) && !string.IsNullOrEmpty(info.gender))
                {
                    join = join.Where(s => s.GENDER == info.gender);
                }
                else if (!string.IsNullOrEmpty(info.name) && !string.IsNullOrEmpty(info.gender))
                {
                    join = join.Where(s => s.NAME.ToLower().Contains(info.name.ToLower()) && s.GENDER == info.gender);
                }

                string JSONFinalresult = JsonConvert.SerializeObject(join);
                List<People> myDeserializedObjList = (List<People>)Newtonsoft.Json.JsonConvert.DeserializeObject(JSONFinalresult, typeof(List<People>));



                info.PageSize = 10;
                int rem = myDeserializedObjList.Count() % info.PageSize;
                if (rem == 0)
                {
                    info.PageCount = Convert.ToInt32(Math.Ceiling((double)(myDeserializedObjList.Count() / info.PageSize)));
                }
                else
                {
                    info.PageCount = Convert.ToInt32(Math.Ceiling((double)(myDeserializedObjList.Count() / info.PageSize))) + 1;
                }
                // info.CurrentPageIndex = 0;

                // query = myDeserializedObjList.AsQueryable().Take(info.PageSize);
                query = myDeserializedObjList.AsQueryable();
                query = query.Skip(info.CurrentPageIndex * info.PageSize).Take(info.PageSize);

                ViewBag.SortingPagingInfo = info;
                model = query.ToList();


            }
            else
            {
                string JSONFinalresult = JsonConvert.SerializeObject(join);
                List<People> myDeserializedObjList = (List<People>)Newtonsoft.Json.JsonConvert.DeserializeObject(JSONFinalresult, typeof(List<People>));


                query = myDeserializedObjList.AsQueryable();
                query = query.Skip(info.CurrentPageIndex * info.PageSize).Take(info.PageSize);

                ViewBag.SortingPagingInfo = info;
                model = query.ToList();

            }
            
            return View(model);
            //}
        }

        public ActionResult AdvancedSearch()
        {
            SortingPagingInfo info = new SortingPagingInfo();
            info.SortField = "ID";
            info.SortDirection = "ascending";
            info.PageSize = 10;
            info.PageCount = 0;
            info.CurrentPageIndex = 0;

            ViewBag.SortingPagingInfo = info;
            List<People> model = new List<People>();

            return View(model);
        }

    }
}