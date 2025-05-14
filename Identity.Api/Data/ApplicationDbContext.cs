using Identity.Api.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Identity.Api.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser> { }
