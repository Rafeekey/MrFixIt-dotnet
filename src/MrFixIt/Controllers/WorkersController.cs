﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrFixIt.Models;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MrFixIt.Controllers
{
    public class WorkersController : Controller
    {
        private MrFixItContext db = new MrFixItContext();
        // GET: /<controller>/
        public IActionResult Index()
        {
            // Grab the users worker profile and his current jobs
            var thisWorker = db.Workers.Include(i =>i.Jobs).FirstOrDefault(i => i.UserName == User.Identity.Name);
            if (thisWorker != null)
            {
                return View(thisWorker);
            }
            else
            {
                return RedirectToAction("Create");
            }
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(Worker worker)
        {
            // Create a worker profile for the user
            worker.UserName = User.Identity.Name;
            db.Workers.Add(worker); 
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult SetAvaliableToFalse(int JobId, int WorkerId)
        {
            Worker worker = db.Workers.FirstOrDefault(model => model.WorkerId == WorkerId);
            worker.Avaliable = false;
            Job job = db.Jobs.FirstOrDefault(model => model.JobId == JobId);
            job.Pending = true;
            db.Entry(worker).State = EntityState.Modified;
            db.Entry(job).State = EntityState.Modified;
            db.SaveChanges();
            return Content(JobId.ToString(), "text/plain");
        }
        public IActionResult SetComplete(int JobId, int WorkerId)
        {
            Worker worker = db.Workers.FirstOrDefault(model => model.WorkerId == WorkerId);
            worker.Avaliable = true;
            Job job = db.Jobs.FirstOrDefault(model => model.JobId == JobId);
            job.Pending = false;
            job.Completed = true;
            db.Entry(worker).State = EntityState.Modified;
            db.Entry(job).State = EntityState.Modified;
            db.SaveChanges();
            return Content(JobId.ToString(), "text/plain");
        }
    }
}
