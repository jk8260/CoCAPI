using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoCAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;

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

        //// GET: api/CoC/5
        //[HttpGet("{id}", Name = "Get")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // GET: api/CoC/5
        //[HttpGet]
        //[Route("api/coc/{EffectiveDate}/{DOS}/{ProgramDate}/{MNFlag}")]
        //public IEnumerable<Question> Get(string DOS, string EffectiveDate, string ProgramDate, bool MNFlag)
        //{
        //    List<Question> lQuestions = GetQuestionsList().ToList();

        //    return lQuestions;
        //}

        [HttpGet]
        [Route("api/coc/{ProgramStartDate}/{EntryDate}/{DateOfService}/{MNFlag}")]
        public IEnumerable<Question> Get(string ProgramStartDate, string EntryDate, string DateOfService, bool MNFlag)
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
        public void Post([FromBody] string value)
        {
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
