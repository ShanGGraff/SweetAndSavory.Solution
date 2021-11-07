using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using SweetAndSavory.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

namespace SweetAndSavory.Controllers
{
  public class FlavorsController : Controller
  {
    private readonly SweetAndSavoryContext _db;

    public FlavorsController(SweetAndSavoryContext db)
    {
      _db = db;
    }
    public ActionResult Index()
    {
      return View(_db.Flavor.ToList());
    }
    public ActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public ActionResult Create(Flavor flavor)
    {
      bool isUnique = true;
      List<Flavor> flavorList = _db.Flavor.ToList();
      foreach(Flavor iteration in flavorList)
      {
        if (flavor.FlavorName == iteration.FlavorName)
        {
        isUnique = false;
        ModelState.AddModelError("DuplicateName", flavor.FlavorName + " Is already taken");
        return View();
        }
      }
      if (isUnique)
      {
      _db.Flavor.Add(flavor);
      _db.SaveChanges();
      }
      return RedirectToAction("Index");
    }

    public ActionResult Details(int id)
    {
      Flavor thisFlavor = _db.Flavor
          .Include(flavor => flavor.JoinEntities)
          .ThenInclude(join => join.Treat)
          .FirstOrDefault(flavor => flavor.FlavorId == id);
      return View(thisFlavor);
    }
    [AllowAnonymous]
    public ActionResult Edit(int id)
    {
      Flavor thisFlavor = _db.Flavor.FirstOrDefault(flavor => flavor.FlavorId == id);
      return View(thisFlavor);
    }

    [HttpPost]
    public ActionResult Edit(Flavor flavor)
    {
      _db.Entry(flavor).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult AddFlavor(int id)
    {
      Flavor thisFlavor = _db.Flavor.FirstOrDefault(flavor => flavor.FlavorId == id);
      return View(thisFlavor);
    }
    
    [HttpPost]
    public ActionResult AddFlavor(Treat treat, int FlavorId)
      {
        if (FlavorId != 0)
        {
        _db.TreatFlavor.Add(new TreatFlavor() { FlavorId = FlavorId, TreatId = treat.TreatId });
        }

        _db.SaveChanges();
        return RedirectToAction("Index");
      }

    public ActionResult Delete(int id)
    {
      Flavor thisFlavor = _db.Flavor.FirstOrDefault(flavor => flavor.FlavorId == id);
      return View(thisFlavor);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      Flavor thisFlavor = _db.Flavor.FirstOrDefault(flavor => flavor.FlavorId == id);
      _db.Flavor.Remove(thisFlavor);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult DeleteFlavor(int joinId)
    {
      TreatFlavor joinEntry = _db.TreatFlavor.FirstOrDefault(entry => entry.TreatFlavorId == joinId);
      _db.TreatFlavor.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}