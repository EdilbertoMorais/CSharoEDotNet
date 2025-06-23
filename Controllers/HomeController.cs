using System.Diagnostics;
using Fiap.Web.Aluno.Data;
using Microsoft.AspNetCore.Mvc;
using Fiap.Web.Aluno.Models;

namespace Fiap.Web.Aluno.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    // configuração inicial do controler para acessar a classe
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}