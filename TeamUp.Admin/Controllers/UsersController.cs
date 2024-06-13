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
    public class UsersController : Controller
    {
        private AppDbContext _context;

        public UsersController(AppDbContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var aspnetusers = _context.AspNetUsers.Select(i => new {
                i.Id,
                i.Handler,
                i.DisplayName,
                i.Rate,
                i.ProfilePicture,
                i.IsMentor,
                i.UserName,
                i.Email,
                i.EmailConfirmed,
            });

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "Id" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(aspnetusers, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new AspNetUser();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.AspNetUsers.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.Id });
        }

        [HttpPut]
        public async Task<IActionResult> Put(Guid key, string values) {
            var model = await _context.AspNetUsers.FirstOrDefaultAsync(item => item.Id == key);
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
        public async Task Delete(Guid key) {
            var model = await _context.AspNetUsers.FirstOrDefaultAsync(item => item.Id == key);

            _context.AspNetUsers.Remove(model);
            await _context.SaveChangesAsync();
        }


        [HttpGet]
        public async Task<IActionResult> VerificationCodesLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.VerificationCodes
                         orderby i.Code
                         select new {
                             Value = i.Id,
                             Text = i.Code
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        private void PopulateModel(AspNetUser model, IDictionary values) {
            string ID = nameof(AspNetUser.Id);
            string FIRST_NAME = nameof(AspNetUser.FirstName);
            string LAST_NAME = nameof(AspNetUser.LastName);
            string HANDLER = nameof(AspNetUser.Handler);
            string DISPLAY_NAME = nameof(AspNetUser.DisplayName);
            string EMAIL_VERIFICATION_CODE_ID = nameof(AspNetUser.EmailVerificationCodeId);
            string PASSWORD_REST_CODE_ID = nameof(AspNetUser.PasswordRestCodeId);
            string PASSWORD_RESET_TOKEN = nameof(AspNetUser.PasswordResetToken);
            string RATE = nameof(AspNetUser.Rate);
            string PROFILE_PICTURE = nameof(AspNetUser.ProfilePicture);
            string FULL_ADDRESS = nameof(AspNetUser.FullAddress);
            string IS_MENTOR = nameof(AspNetUser.IsMentor);
            string USER_NAME = nameof(AspNetUser.UserName);
            string NORMALIZED_USER_NAME = nameof(AspNetUser.NormalizedUserName);
            string EMAIL = nameof(AspNetUser.Email);
            string NORMALIZED_EMAIL = nameof(AspNetUser.NormalizedEmail);
            string EMAIL_CONFIRMED = nameof(AspNetUser.EmailConfirmed);
            string PASSWORD_HASH = nameof(AspNetUser.PasswordHash);
            string SECURITY_STAMP = nameof(AspNetUser.SecurityStamp);
            string CONCURRENCY_STAMP = nameof(AspNetUser.ConcurrencyStamp);
            string PHONE_NUMBER = nameof(AspNetUser.PhoneNumber);
            string PHONE_NUMBER_CONFIRMED = nameof(AspNetUser.PhoneNumberConfirmed);
            string TWO_FACTOR_ENABLED = nameof(AspNetUser.TwoFactorEnabled);
            string LOCKOUT_END = nameof(AspNetUser.LockoutEnd);
            string LOCKOUT_ENABLED = nameof(AspNetUser.LockoutEnabled);
            string ACCESS_FAILED_COUNT = nameof(AspNetUser.AccessFailedCount);

            if(values.Contains(ID)) {
                model.Id = ConvertTo<System.Guid>(values[ID]);
            }

            if(values.Contains(FIRST_NAME)) {
                model.FirstName = Convert.ToString(values[FIRST_NAME]);
            }

            if(values.Contains(LAST_NAME)) {
                model.LastName = Convert.ToString(values[LAST_NAME]);
            }

            if(values.Contains(HANDLER)) {
                model.Handler = Convert.ToString(values[HANDLER]);
            }

            if(values.Contains(DISPLAY_NAME)) {
                model.DisplayName = Convert.ToString(values[DISPLAY_NAME]);
            }

            if(values.Contains(EMAIL_VERIFICATION_CODE_ID)) {
                model.EmailVerificationCodeId = values[EMAIL_VERIFICATION_CODE_ID] != null ? Convert.ToInt32(values[EMAIL_VERIFICATION_CODE_ID]) : (int?)null;
            }

            if(values.Contains(PASSWORD_REST_CODE_ID)) {
                model.PasswordRestCodeId = values[PASSWORD_REST_CODE_ID] != null ? Convert.ToInt32(values[PASSWORD_REST_CODE_ID]) : (int?)null;
            }

            if(values.Contains(PASSWORD_RESET_TOKEN)) {
                model.PasswordResetToken = Convert.ToString(values[PASSWORD_RESET_TOKEN]);
            }

            if(values.Contains(RATE)) {
                model.Rate = Convert.ToSingle(values[RATE], CultureInfo.InvariantCulture);
            }

            if(values.Contains(PROFILE_PICTURE)) {
                model.ProfilePicture = Convert.ToString(values[PROFILE_PICTURE]);
            }

            if(values.Contains(FULL_ADDRESS)) {
                model.FullAddress = Convert.ToString(values[FULL_ADDRESS]);
            }

            if(values.Contains(IS_MENTOR)) {
                model.IsMentor = Convert.ToBoolean(values[IS_MENTOR]);
            }

            if(values.Contains(USER_NAME)) {
                model.UserName = Convert.ToString(values[USER_NAME]);
            }

            if(values.Contains(NORMALIZED_USER_NAME)) {
                model.NormalizedUserName = Convert.ToString(values[NORMALIZED_USER_NAME]);
            }

            if(values.Contains(EMAIL)) {
                model.Email = Convert.ToString(values[EMAIL]);
            }

            if(values.Contains(NORMALIZED_EMAIL)) {
                model.NormalizedEmail = Convert.ToString(values[NORMALIZED_EMAIL]);
            }

            if(values.Contains(EMAIL_CONFIRMED)) {
                model.EmailConfirmed = Convert.ToBoolean(values[EMAIL_CONFIRMED]);
            }

            if(values.Contains(PASSWORD_HASH)) {
                model.PasswordHash = Convert.ToString(values[PASSWORD_HASH]);
            }

            if(values.Contains(SECURITY_STAMP)) {
                model.SecurityStamp = Convert.ToString(values[SECURITY_STAMP]);
            }

            if(values.Contains(CONCURRENCY_STAMP)) {
                model.ConcurrencyStamp = Convert.ToString(values[CONCURRENCY_STAMP]);
            }

            if(values.Contains(PHONE_NUMBER)) {
                model.PhoneNumber = Convert.ToString(values[PHONE_NUMBER]);
            }

            if(values.Contains(PHONE_NUMBER_CONFIRMED)) {
                model.PhoneNumberConfirmed = Convert.ToBoolean(values[PHONE_NUMBER_CONFIRMED]);
            }

            if(values.Contains(TWO_FACTOR_ENABLED)) {
                model.TwoFactorEnabled = Convert.ToBoolean(values[TWO_FACTOR_ENABLED]);
            }

            if(values.Contains(LOCKOUT_END)) {
                model.LockoutEnd = values[LOCKOUT_END] != null ? Convert.ToDateTime(values[LOCKOUT_END]) : (DateTime?)null;
            }

            if(values.Contains(LOCKOUT_ENABLED)) {
                model.LockoutEnabled = Convert.ToBoolean(values[LOCKOUT_ENABLED]);
            }

            if(values.Contains(ACCESS_FAILED_COUNT)) {
                model.AccessFailedCount = Convert.ToInt32(values[ACCESS_FAILED_COUNT]);
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