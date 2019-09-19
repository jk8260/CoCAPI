using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoCAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace CoCAPI.Controllers
{
    //[Route("api/[controller]")]
    [EnableCors("CorsPolicy")]
    [ApiController]
    public class CoCController : ControllerBase
    {
        // GET: api/CoC
        [HttpGet]
        [Route("api/coc")]
        public IEnumerable<Question> Get()
        {
            return GetQuestionsList();
        }

        [HttpGet]
        [Route("api/coc/{ProgramStartDate}/{EntryDate}/{DateOfService}/{MNFlag}")]
        public IEnumerable<Question> Get([FromQuery]string ProgramStartDate, [FromQuery]string EntryDate, [FromQuery]string DateOfService, [FromQuery]bool MNFlag)
        {
            string echo = "N/A";
            string exclude = "";
            bool abend = true;
            List<Question> lQuestions = new List<Question>();
            try
            {
                DateTime dEntryDate = DateTime.Parse(EntryDate);
                DateTime dDateOfService = DateTime.Parse(DateOfService);
                DateTime dProgramStartDate = DateTime.Parse(ProgramStartDate);
                DateTime dProgramStartDatePlus180 = dProgramStartDate.AddDays(180);
                DateTime dProgramStartDateMinus180 = dProgramStartDate.AddDays(-180);
                if (dEntryDate > dProgramStartDatePlus180)
                {
                    if (dDateOfService < dProgramStartDate) // hard stop
                        echo = "Hard Stop" + Environment.NewLine;
                    else if (dDateOfService >= dProgramStartDate)
                    {
                        echo = "A12345" + Environment.NewLine;
                        exclude = "B";
                        abend = false;
                    }
                    else
                        echo = "N/A" + Environment.NewLine;
                }
                if (!abend) // if we still need questions
                {
                    // get our full list
                    lQuestions = GetQuestionsList().ToList();
                    // remove our excludes
                    foreach (Char c in exclude)
                    { lQuestions.Remove(lQuestions.First(x => x.QuexId == c.ToString())); }
                    // do this just for logging the sequence
                    foreach (Question q in lQuestions)
                    { echo += q.QuexId; }
                }
            }
            catch (Exception ex) { echo += ex.Message; }
            return lQuestions;
        }

        // POST: api/CoC
        [HttpPost]
        [Route("api/coc")]
        public CoCResult Post([FromBody] CocRequest value)
        {
            string json = JsonConvert.SerializeObject(value.AnswerList);
            Debug.WriteLine(json);
            string jsonformatted = JValue.Parse(json).ToString(Formatting.Indented);
            Debug.WriteLine(jsonformatted);
            //Debug.WriteLine(value.Num);
            //Debug.WriteLine(value.Str);
            var alist = JsonConvert.DeserializeObject<List<Question>>(json);
            int one = 0;
            int two = 0;
            int three = 0;
            CoCResult result = new CoCResult();
            result.LevelOne = "Not Met";
            result.LevelTwo = "Not Met";
            result.LevelThree = "Not Met";
            result.CoCDetermination = "Not CoC";
            result.BadgeFlag = false;

            foreach (Question a in alist)
            {
                if (a.Level == "1")
                {
                    one++;
                }
                if (a.Level == "2")
                {
                    two++;
                }
                if (a.Level == "3")
                {
                    three++;
                }
            }
            if (one > 0) result.LevelOne = "Met";
            if (two > 1) result.LevelTwo = "Met";
            if (three > 0) result.LevelThree = "Met";
            if (result.LevelOne=="Met" && result.LevelTwo== "Met" && result.LevelThree == "Met")
                result.CoCDetermination = "Admin";
            else if (result.LevelTwo == "Met" && result.LevelThree == "Met")
                result.CoCDetermination = "Clinical";
            if (!result.CoCDetermination.StartsWith("N"))
                result.BadgeFlag = true;

            result.Extra = "";
            return result;

        }

        // PUT: api/CoC/5
        [HttpPut]
        [Route("api/coc/{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete]
        [Route("api/coc/{id}")]
        public void Delete(int id)
        {
        }

        private IEnumerable<Question> GetQuestionsList()
        {
            string json = System.IO.File.ReadAllText(@"content\attestations.json");
            var qlist = JsonConvert.DeserializeObject<List<Question>>(json);

            return qlist;
        }


    }
}
