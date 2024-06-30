using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TeamUp.Admin.Models.EF;

namespace TeamUp.Admin.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ProjectPostsController : Controller
    {
        private AppDbContext _context;

        public ProjectPostsController(AppDbContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var projectposts = _context.ProjectPosts.Select(i => new {
                i.Id,
                i.PostingTime,
                i.Title,
                i.Summary,
                i.Scenario,
                i.LearningGoals,
                i.TeamAndRols,
                i.ExpextedDuration,
                i.ExpectedTeamSize,
                i.CreatorId,
                i.IsStarted
            });

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "Id" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(projectposts, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new ProjectPost();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.ProjectPosts.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.Id });
        }

        [HttpPut]
        public async Task<IActionResult> Put(int key, string values) {
            var model = await _context.ProjectPosts.FirstOrDefaultAsync(item => item.Id == key);
            if(model == null)
                return StatusCode(409, "Object not found");

            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        public async Task Delete(int key) {
            var model = await _context.ProjectPosts.FirstOrDefaultAsync(item => item.Id == key);

            _context.ProjectPosts.Remove(model);
            await _context.SaveChangesAsync();
        }


        [HttpGet]
        public async Task<IActionResult> AspNetUsersLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.AspNetUsers
                         orderby i.FirstName
                         select new {
                             Value = i.Id,
                             Text = i.FirstName
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        [HttpGet]
        public async Task<IActionResult> ProjectsLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.Projects
                         orderby i.Name
                         select new {
                             Value = i.Id,
                             Text = i.Name
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        private void PopulateModel(ProjectPost model, IDictionary values) {
            string ID = nameof(ProjectPost.Id);
            string POSTING_TIME = nameof(ProjectPost.PostingTime);
            string TITLE = nameof(ProjectPost.Title);
            string SUMMARY = nameof(ProjectPost.Summary);
            string SCENARIO = nameof(ProjectPost.Scenario);
            string LEARNING_GOALS = nameof(ProjectPost.LearningGoals);
            string TEAM_AND_ROLS = nameof(ProjectPost.TeamAndRols);
            string EXPEXTED_DURATION = nameof(ProjectPost.ExpextedDuration);
            string EXPECTED_TEAM_SIZE = nameof(ProjectPost.ExpectedTeamSize);
            string CREATOR_ID = nameof(ProjectPost.CreatorId);
            string IS_STARTED = nameof(ProjectPost.IsStarted);

            if(values.Contains(ID)) {
                model.Id = Convert.ToInt32(values[ID]);
            }

            if(values.Contains(POSTING_TIME)) {
                model.PostingTime = Convert.ToDateTime(values[POSTING_TIME]);
            }

            if(values.Contains(TITLE)) {
                model.Title = Convert.ToString(values[TITLE]);
            }

            if(values.Contains(SUMMARY)) {
                model.Summary = Convert.ToString(values[SUMMARY]);
            }

            if(values.Contains(SCENARIO)) {
                model.Scenario = Convert.ToString(values[SCENARIO]);
            }

            if(values.Contains(LEARNING_GOALS)) {
                model.LearningGoals = Convert.ToString(values[LEARNING_GOALS]);
            }

            if(values.Contains(TEAM_AND_ROLS)) {
                model.TeamAndRols = Convert.ToString(values[TEAM_AND_ROLS]);
            }

            if(values.Contains(EXPEXTED_DURATION)) {
                model.ExpextedDuration = Convert.ToString(values[EXPEXTED_DURATION]);
            }

            if(values.Contains(EXPECTED_TEAM_SIZE)) {
                model.ExpectedTeamSize = Convert.ToInt32(values[EXPECTED_TEAM_SIZE]);
            }

            if(values.Contains(CREATOR_ID)) {
                model.CreatorId = ConvertTo<System.Guid>(values[CREATOR_ID]);
            }

            if(values.Contains(IS_STARTED)) {
                model.IsStarted = Convert.ToBoolean(values[IS_STARTED]);
            }
        }

        private T ConvertTo<T>(object value) {
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));
            if(converter != null) {
                return (T)converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);
            } else {
                // If necessary, implement a type conversion here
                throw new NotImplementedException();
            }
        }

        private string GetFullErrorMessage(ModelStateDictionary modelState) {
            var messages = new List<string>();

            foreach(var entry in modelState) {
                foreach(var error in entry.Value.Errors)
                    messages.Add(error.ErrorMessage);
            }

            return String.Join(" ", messages);
        }
    }
}