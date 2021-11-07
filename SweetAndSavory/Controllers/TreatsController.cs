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
  public class TreatsController : Controller
  {
    private readonly SweetAndSavoryContext _db;

    public TreatsController(SweetAndSavoryContext db)
    {
      _db = db;
    }
    public ActionResult Index()
    {
      return View(_db.Treat.ToList());
    }
    public ActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public ActionResult Create(Treat treat)
    {
      bool isUnique = true;
      List<Treat> treatList = _db.Treat.ToList();
      foreach(Treat iteration in treatList)
      {
        if (treat.TreatName == iteration.TreatName)
        {
        isUnique = false;
        ModelState.AddModelError("DuplicateName", treat.TreatName + " Is already taken");
        return View();
        }
      }
      if (isUnique)
      {
      _db.Treat.Add(treat);
      _db.SaveChanges();
      }
      return RedirectToAction("Index");
    }

    public ActionResult Details(int id)
    {
      Treat thisTreat = _db.Treat
          .Include(treat => treat.JoinEntities)
          .ThenInclude(join => join.Flavor)
          .FirstOrDefault(treat => treat.TreatId == id);
      return View(thisTreat);
    }
    
    [AllowAnonymous]
    public ActionResult Edit(int id)
    {
      Treat thisTreat = _db.Treat.FirstOrDefault(treat => treat.TreatId == id);
      return View(thisTreat);
    }

    [HttpPost]
    public ActionResult Edit(Treat treat)
    {
      _db.Entry(treat).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult AddTreat(int id)
    {
      Treat thisTreat = _db.Treat.FirstOrDefault(treat => treat.TreatId == id);
      return View(thisTreat);
    }
    
    // public ActionResult AddFlavor(int id)
    // {
    //   Treat thisTreat = _db.Treat.FirstOrDefault(treat => treat.TreatId == id);
    //   ViewBag.RecipeId = new SelectList(_db.Flavor, "FlavorId", "FlavorName");
    //   return View(thisTreat);
    // }
    
    [HttpPost]
    public ActionResult AddTreat(Flavor flavor, int TreatId)
      {
        if (TreatId != 0)
        {
        _db.TreatFlavor.Add(new TreatFlavor() { TreatId = TreatId, FlavorId = flavor.FlavorId });
        }

        _db.SaveChanges();
        return RedirectToAction("Index");
      }

    public ActionResult Delete(int id)
    {
      Treat thisTreat = _db.Treat.FirstOrDefault(treat => treat.TreatId == id);
      return View(thisTreat);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      Treat thisTreat = _db.Treat.FirstOrDefault(treat => treat.TreatId == id);
      _db.Treat.Remove(thisTreat);
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