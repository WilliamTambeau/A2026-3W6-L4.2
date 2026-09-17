using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZombieParty.Models;
using ZombieParty.Models.Data;

namespace ZombieParty.Controllers
{
    public class ZombieController : Controller
    {
        private ZombiePartyDbContext _baseDonnees { get; set; }

        public ZombieController(ZombiePartyDbContext baseDonnees)
        {
            _baseDonnees = baseDonnees;
        }

        public IActionResult Index()
        {
            List<Zombie> zombiesList = _baseDonnees.Zombies.Include(z => z.ZombieType).OrderBy(z => z.Name.ToLower()).ToList();

            return View(zombiesList);
        }

        public IActionResult StrongestZombies(int? minPt)
        {
            if (minPt == null) minPt = 8;

            List<Zombie> zombiesList = _baseDonnees.Zombies
                .Include(z => z.ZombieType)
                .Where(z => z.Point + z.ZombieType.Point >= minPt)
                .OrderBy(z => z.Point + z.ZombieType.Point)
                .ThenBy(z => z.Name.ToLower())
                .ToList();

            return View(zombiesList);
        }

        public IActionResult Create()
        {
            ZombieVM zombieVM = new ZombieVM();
            zombieVM.ZombieTypeSelectList = _baseDonnees.ZombieTypes.Select(t => new SelectListItem
            {
                Text = t.TypeName,
                Value = t.Id.ToString()
            }).OrderBy(t => t.Text);

            return View(zombieVM);
        }

        public IActionResult Edit(int? id)
        {

            ZombieVM zombieVM = new ZombieVM();
            zombieVM.Zombie = _baseDonnees.Zombies.Find(id);

            if (zombieVM.Zombie == null) return NotFound();

            zombieVM.ZombieTypeSelectList = _baseDonnees.ZombieTypes.Select(t => new SelectListItem
            {
                Text = t.TypeName,
                Value = t.Id.ToString()
            }).OrderBy(t => t.Text);

            return View(zombieVM);
        }

        [HttpPost]
        public IActionResult Create(ZombieVM zombieVM)
        {
            //Si le modèle est valide le zombie est ajouté et nous sommes redirigé vers index.
            if (ModelState.IsValid)
            {
                _baseDonnees.Zombies.Add(zombieVM.Zombie);
                _baseDonnees.SaveChanges();
                TempData["Success"] = $"Zombie {zombieVM.Zombie.Name} added";
                return this.RedirectToAction("Index");
            }
            zombieVM.ZombieTypeSelectList = _baseDonnees.ZombieTypes.Select(t => new SelectListItem
            {
                Text = t.TypeName,
                Value = t.Id.ToString()
            }).OrderBy(t => t.Text);

            return View(zombieVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ZombieVM zombieVM)
        {
            //Si le modèle est valide, le zombie est modifié et nous sommes redirigé vers index.
            if (ModelState.IsValid)
            {
                _baseDonnees.Zombies.Update(zombieVM.Zombie);
                _baseDonnees.SaveChanges();
                TempData["Success"] = $"Zombie {zombieVM.Zombie.Name} has been modified";
                return this.RedirectToAction("Index");
            }
            zombieVM.ZombieTypeSelectList = _baseDonnees.ZombieTypes.Select(t => new SelectListItem
            {
                Text = t.TypeName,
                Value = t.Id.ToString()
            }).OrderBy(t => t.Text);

            return View(zombieVM);
        }
    }
}
