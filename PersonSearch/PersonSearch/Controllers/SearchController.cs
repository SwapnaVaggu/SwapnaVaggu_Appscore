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

        [HttpPost]
        public ActionResult AdvancedSearch(SortingPagingInfo info)
        {

            List<People> model = new List<People>();
            if (info.direction == "Ancestors")
            {
                Int64[] Ancestors = new Int64[300];
                model = getAncestorsOrDescendanta(info);
            }
            else if (info.direction == "Descendants")
            {
                Int64[] descendants = new Int64[300];
                model = getAncestorsOrDescendanta(info);
            }          
           
            
            return View(model);
            
        }

        private List<People> getAncestorsOrDescendanta(SortingPagingInfo info)
        {
            string type = info.direction;
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
                                      BIRTHPLACE = d["name"].Value<string>(),
                                      FATHER_ID = s["father_id"].Value<string>() == null ? 0 : s["father_id"],
                                      LEVEL = s["level"].Value<string>()
                                  });



            var join1 = join.Where(s => s.NAME.ToLower().TrimEnd().TrimStart() == info.name.ToLower().TrimEnd().TrimStart() && s.GENDER == info.gender);
            string JSONFinalresult = JsonConvert.SerializeObject(join1);
            List<People> myDeserializedObjList = (List<People>)Newtonsoft.Json.JsonConvert.DeserializeObject(JSONFinalresult, typeof(List<People>));
            People cust1 = new People();
            cust1 = myDeserializedObjList.First();

            Int64[] AncestorsOrDescendants = new Int64[300];
            Int64[] fatheridorChildID = new Int64[100];
            if ( type=="Ancestors" && cust1.FATHER_ID!=0)
            {
                fatheridorChildID[0] =  cust1.FATHER_ID ;
            }
            else if (type == "Descendants")
            {
                fatheridorChildID[0] = cust1.ID;
            }
            else
            {
                info.SortField = "ID";
                info.SortDirection = "ascending";
                info.PageSize = 10;
                info.PageCount = 0;
                info.CurrentPageIndex = 0;

                ViewBag.SortingPagingInfo = info;
                return model;
            }

            int k = 0;
            for (int i = 0; i < 10; i++)
            {
                var join_1 = join;
                var filteringQuery= join_1;

                if (type == "Ancestors")
                {
                    filteringQuery = join_1.Where(s => fatheridorChildID.Contains(Convert.ToInt32(s.ID))).
                        Select(s => new
                        {
                            ID = s.ID,
                            NAME = s.NAME,
                            GENDER = s.GENDER,
                            BIRTHPLACE = s.BIRTHPLACE,
                            FATHER_ID = s.FATHER_ID,
                            LEVEL = s.LEVEL
                        });
                }
                else
                {
                    filteringQuery = join_1.Where(s => fatheridorChildID.Contains(Convert.ToInt32(s.FATHER_ID)) && Convert.ToInt32(s.FATHER_ID) != 0).
                        Select(s => new
                        {
                            ID = s.ID,
                            NAME = s.NAME,
                            GENDER = s.GENDER,
                            BIRTHPLACE = s.BIRTHPLACE,
                            FATHER_ID = s.FATHER_ID,
                            LEVEL = s.LEVEL
                        });
                }
                string JSONFinalresult_ans = JsonConvert.SerializeObject(filteringQuery);
                List<People> myDeserializedObjList_ans = (List<People>)Newtonsoft.Json.JsonConvert.DeserializeObject(JSONFinalresult_ans, typeof(List<People>));
                int j = 0;
                foreach (var item in myDeserializedObjList_ans)
                {
                    if (type == "Ancestors")
                    {
                        fatheridorChildID[j] = item.FATHER_ID;
                    }
                    else
                    {
                        fatheridorChildID[j] = item.ID;
                    }

                    AncestorsOrDescendants[k] = item.ID;
                    j = j + 1;
                    k = k + 1;
                }


            }


            var join2 = join.Where(s => AncestorsOrDescendants.Contains(Convert.ToInt32(s.ID))).
                        Select(s => new
                        {
                            ID = s.ID,
                            NAME = s.NAME,
                            GENDER = s.GENDER,
                            BIRTHPLACE = s.BIRTHPLACE,
                            FATHER_ID = s.FATHER_ID,
                            LEVEL = s.LEVEL
                        });



            string JSONFinalresult_ans1 = JsonConvert.SerializeObject(join2);
            List<People> myDeserializedObjList_ans1 = (List<People>)Newtonsoft.Json.JsonConvert.DeserializeObject(JSONFinalresult_ans1, typeof(List<People>));
            //  myDeserializedObjList[0];

            info.PageSize = 10;
            int rem = myDeserializedObjList_ans1.Count() % info.PageSize;
            if (rem == 0)
            {
                info.PageCount = Convert.ToInt32(Math.Ceiling((double)(myDeserializedObjList_ans1.Count() / info.PageSize)));
            }
            else
            {
                info.PageCount = Convert.ToInt32(Math.Ceiling((double)(myDeserializedObjList_ans1.Count() / info.PageSize))) + 1;
            }
            // info.CurrentPageIndex = 0;

            // query = myDeserializedObjList.AsQueryable().Take(info.PageSize);
            query = myDeserializedObjList_ans1.AsQueryable();
            query = query.Skip(info.CurrentPageIndex * info.PageSize).Take(info.PageSize);

            ViewBag.SortingPagingInfo = info;
            model = query.ToList();

            return model;
        }
    }
}