﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvancedC_Final.Models;
using Microsoft.AspNetCore.Identity;

namespace AdvancedC_Final.Areas.Identity.Data;

// Add profile data for application users by adding properties to the TaskManagerUser class
public class TaskManagerUser : IdentityUser
{
    public HashSet<DeveloperTicket> Tickets { get; set; } = new HashSet<DeveloperTicket>();
    public HashSet<DeveloperProject> DeveloperProjects { get; set; } = new HashSet<DeveloperProject>();
    public HashSet<Project> ManagerProjects { get; set; } = new HashSet<Project>();
}

