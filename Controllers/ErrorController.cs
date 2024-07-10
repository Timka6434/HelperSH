using Microsoft.AspNetCore.Mvc;

namespace test_indentity.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Извините, запрашиваемая вами страница не найдена.";
                    break;
                case 403:
                    ViewBag.ErrorMessage = "Извините, у вас нет доступа к этой странице.";
                    break;
                default:
                    ViewBag.ErrorMessage = "Произошла ошибка.";
                    break;
            }
            return View("Error");
        }
    }
}
