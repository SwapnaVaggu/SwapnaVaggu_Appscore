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
            string file = Server.MapPath("~/App_Data/data_small.json");
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


            return View(myDeserializedObjList);
        }

      
        public ActionResult AdvancedSearch()
        {
            return View();
        }

    }
}