# ASP.NET Core MVC Controllers & Action Results Guide

## Table of Contents
1. [What are Controllers?](#what-are-controllers)
2. [Action Methods](#action-methods)
3. [Action Results Overview](#action-results-overview)
4. [Content-Based Results](#content-based-results)
5. [File Results](#file-results)
6. [Status Code Results](#status-code-results)
7. [Redirect Results](#redirect-results)
8. [Best Practices](#best-practices)

---

## What are Controllers?

### The Problem Controllers Solve

Without controllers, you would need to define individual middleware or endpoints for each URL:

```csharp
// Without controllers - repetitive and hard to maintain
app.MapGet("/products", GetProducts);
app.MapGet("/products/{id}", GetProduct);
app.MapPost("/products", CreateProduct);
app.MapPut("/products/{id}", UpdateProduct);
app.MapDelete("/products/{id}", DeleteProduct);
app.MapGet("/products/search", SearchProducts);
// ... hundreds more endpoints
```

**The Solution:** Controllers group related functionality together.

```csharp
// With controllers - organized and maintainable
public class ProductsController : Controller
{
    public IActionResult Index() => View();
    public IActionResult Details(int id) => View();
    public IActionResult Create() => View();
    public IActionResult Edit(int id) => View();
    public IActionResult Delete(int id) => View();
    public IActionResult Search(string query) => View();
}
```

### What is a Controller?

A controller is a class that:
- **Groups related actions** that work with the same resource or feature
- **Handles HTTP requests** and generates appropriate responses
- **Orchestrates application logic** by coordinating between models and views
- **Follows the MVC pattern** (Model-View-Controller)

### The MVC Pattern

```
┌─────────────┐
│   Browser   │
└──────┬──────┘
       │ HTTP Request
       ▼
┌─────────────────┐
│   Controller    │ ◄── Orchestrates the flow
│  (Brain/Logic)  │
└────┬────────┬───┘
     │        │
     │        └────────┐
     ▼                 ▼
┌─────────┐      ┌──────────┐
│  Model  │      │   View   │
│ (Data)  │      │  (UI)    │
└─────────┘      └──────────┘
```

**Flow:**
1. Browser sends HTTP request
2. Controller receives request
3. Controller interacts with Model (gets/updates data)
4. Controller selects View and passes Model data
5. View renders HTML
6. Controller returns response to browser

### Controller Responsibilities

| Responsibility | Description | Example |
|----------------|-------------|---------|
| **Request Handling** | Process incoming HTTP requests | Extract route parameters, query strings |
| **Input Validation** | Validate user input | Check if ID is valid, data is complete |
| **Business Logic** | Coordinate application logic | Call services, process data |
| **Model Interaction** | Work with data layer | CRUD operations, database queries |
| **View Selection** | Choose appropriate view | Return `View()`, `PartialView()` |
| **Response Generation** | Create HTTP responses | Return JSON, files, redirects |

---

## Controller Basics

### Creating a Controller

**Step 1: Create Controller Class**

```csharp
using Microsoft.AspNetCore.Mvc;

namespace MyApp.Controllers
{
    // Option 1: Inherit from Controller (for MVC with views)
    public class HomeController : Controller
    {
        // Action methods go here
    }
    
    // Option 2: Inherit from ControllerBase (for APIs, no view support)
    public class ApiController : ControllerBase
    {
        // API action methods go here
    }
}
```

**Step 2: Register MVC Services**

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Register MVC services
builder.Services.AddControllers();  // For API controllers
// OR
builder.Services.AddControllersWithViews();  // For MVC with views

var app = builder.Build();

app.UseRouting();
app.MapControllers();  // Map controller routes

app.Run();
```

### Controller Conventions

#### Naming Convention

Controllers **must** end with "Controller":

```csharp
// ✅ Correct
public class HomeController : Controller { }
public class ProductsController : Controller { }
public class UserAccountController : Controller { }

// ❌ Wrong - won't be discovered
public class Home : Controller { }
public class Products : Controller { }
```

#### Location Convention

Controllers typically reside in the `Controllers` folder:

```
MyApp/
├── Controllers/
│   ├── HomeController.cs
│   ├── ProductsController.cs
│   └── AccountController.cs
├── Views/
├── Models/
└── Program.cs
```

### Marking a Class as Controller

**Option 1: Inherit from Controller or ControllerBase (Automatic)**

```csharp
public class HomeController : Controller
{
    // Automatically recognized as controller
}
```

**Option 2: Use [Controller] Attribute**

```csharp
[Controller]
public class Home  // Note: No "Controller" suffix needed with attribute
{
    // Marked as controller
}
```

**Option 3: Use [ApiController] Attribute (for APIs)**

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    // Enhanced API features enabled
}
```

---

## Action Methods

Action methods are public methods in controllers that handle HTTP requests.

### Basic Action Method

```csharp
public class HomeController : Controller
{
    // Simple action method
    public string Index()
    {
        return "Hello from Index";
    }
    
    // Action returning IActionResult
    public IActionResult About()
    {
        return Content("About page");
    }
}
```

### Action Method Characteristics

| Characteristic | Details |
|----------------|---------|
| **Visibility** | Must be public |
| **Return Type** | `IActionResult`, `string`, `JsonResult`, specific types, etc. |
| **Parameters** | Can accept route parameters, query strings, form data |
| **Attributes** | Can have routing, HTTP method, authorization attributes |

### Attribute Routing

Define routes directly on controllers and actions:

```csharp
public class HomeController : Controller
{
    // Multiple routes for same action
    [Route("")]
    [Route("home")]
    [Route("home/index")]
    public string Index()
    {
        return "Hello from Index";
    }
    
    // Route with parameters
    [Route("home/details/{id}")]
    public string Details(int id)
    {
        return $"Details for ID: {id}";
    }
    
    // Route with constraints
    [Route("contact-us/{mobile:regex(^\\d{{10}}$)}")]
    public string Contact(string mobile)
    {
        return $"Contact: {mobile}";
    }
}
```

### HTTP Method Attributes

Specify which HTTP methods an action handles:

```csharp
public class ProductsController : Controller
{
    // GET /products
    [HttpGet]
    [Route("products")]
    public IActionResult Index()
    {
        return View();
    }
    
    // GET /products/create
    [HttpGet]
    [Route("products/create")]
    public IActionResult Create()
    {
        return View();
    }
    
    // POST /products/create
    [HttpPost]
    [Route("products/create")]
    public IActionResult Create(Product product)
    {
        // Save product
        return RedirectToAction("Index");
    }
    
    // PUT /products/{id}
    [HttpPut]
    [Route("products/{id}")]
    public IActionResult Update(int id, Product product)
    {
        // Update product
        return Ok();
    }
    
    // DELETE /products/{id}
    [HttpDelete]
    [Route("products/{id}")]
    public IActionResult Delete(int id)
    {
        // Delete product
        return NoContent();
    }
}
```

### Complete Controller Example

```csharp
using Microsoft.AspNetCore.Mvc;

namespace ControllersExample.Controllers
{
    [Controller]
    public class HomeController
    {
        [Route("home")]
        [Route("/")]
        public string Index()
        {
            return "Hello from Index";
        }

        [Route("about")]
        public string About()
        {
            return "Hello from About";
        }

        [Route("contact-us/{mobile:regex(^\\d{{10}}$)}")]
        public string Contact(string mobile)
        {
            return $"Contact us at: {mobile}";
        }
        
        [Route("products/{id:int}")]
        public string Product(int id)
        {
            return $"Product ID: {id}";
        }
    }
}
```

**Program.cs Configuration:**

```csharp
var builder = WebApplication.CreateBuilder(args);

// Register controller services
builder.Services.AddControllers();

var app = builder.Build();

// Enable routing
app.UseRouting();

// Map controller routes
app.MapControllers();

app.Run();
```

---

## Action Results Overview

### What is IActionResult?

`IActionResult` is an interface that represents the result of an action method. It provides flexibility to return different response types.

```csharp
public interface IActionResult
{
    Task ExecuteResultAsync(ActionContext context);
}
```

### Why Use IActionResult?

**Without IActionResult (Limited):**
```csharp
public string Index()
{
    return "Hello";  // Can only return strings
}
```

**With IActionResult (Flexible):**
```csharp
public IActionResult Index()
{
    if (userIsAuthenticated)
        return View();
    else
        return RedirectToAction("Login");
}
```

### Action Result Type Hierarchy

```
IActionResult
├── ContentResult (text, HTML, XML)
├── JsonResult (JSON data)
├── FileResult
│   ├── VirtualFileResult (from wwwroot)
│   ├── PhysicalFileResult (from file system)
│   └── FileContentResult (from byte array)
├── EmptyResult (no content)
├── RedirectResult (redirect to URL)
├── RedirectToActionResult (redirect to action)
├── LocalRedirectResult (local redirect)
├── ViewResult (render view)
├── PartialViewResult (render partial view)
└── StatusCodeResult (HTTP status codes)
    ├── OkResult (200)
    ├── BadRequestResult (400)
    ├── NotFoundResult (404)
    ├── UnauthorizedResult (401)
    └── ForbidResult (403)
```

### Action Result Categories

| Category | Purpose | Examples |
|----------|---------|----------|
| **Content** | Return raw content | `ContentResult`, `JsonResult` |
| **Files** | Serve files | `VirtualFileResult`, `PhysicalFileResult` |
| **Redirects** | Navigate to other URLs | `RedirectResult`, `RedirectToActionResult` |
| **Views** | Render HTML views | `ViewResult`, `PartialViewResult` |
| **Status Codes** | Return HTTP status codes | `OkResult`, `NotFoundResult`, `BadRequestResult` |
| **Empty** | No content | `EmptyResult` |

---

## Content-Based Results

### ContentResult

Returns raw text content with customizable content type.

**Creating ContentResult:**

```csharp
// Method 1: Direct instantiation
public IActionResult Index()
{
    return new ContentResult() 
    { 
        Content = "Hello from Index", 
        ContentType = "text/plain",
        StatusCode = 200
    };
}

// Method 2: Using Content() helper (recommended)
public IActionResult About()
{
    return Content("Hello from About", "text/plain");
}
```

**Common Content Types:**

| Content Type | Description | Example |
|--------------|-------------|---------|
| `text/plain` | Plain text | `Content("Hello", "text/plain")` |
| `text/html` | HTML content | `Content("<h1>Hello</h1>", "text/html")` |
| `application/xml` | XML data | `Content("<root>data</root>", "application/xml")` |
| `text/csv` | CSV data | `Content("Name,Age\nJohn,30", "text/csv")` |

**Examples:**

```csharp
public class HomeController : Controller
{
    // Plain text
    [Route("text")]
    public IActionResult PlainText()
    {
        return Content("This is plain text", "text/plain");
    }
    
    // HTML content
    [Route("html")]
    public IActionResult HtmlContent()
    {
        return Content("<h1>Welcome</h1><p>Hello from HTML</p>", "text/html");
    }
    
    // XML content
    [Route("xml")]
    public IActionResult XmlContent()
    {
        string xml = @"<?xml version=""1.0""?>
                      <person>
                          <name>John</name>
                          <age>30</age>
                      </person>";
        return Content(xml, "application/xml");
    }
    
    // CSV content
    [Route("csv")]
    public IActionResult CsvContent()
    {
        string csv = "Name,Age,City\nJohn,30,NYC\nJane,25,LA";
        return Content(csv, "text/csv");
    }
}
```

### JsonResult

Returns data serialized as JSON.

**Creating JsonResult:**

```csharp
// Method 1: Direct instantiation
public IActionResult GetPerson()
{
    var person = new Person { Name = "John", Age = 30 };
    return new JsonResult(person);
}

// Method 2: Using Json() helper (recommended)
public IActionResult GetProduct()
{
    var product = new Product { Id = 1, Name = "Laptop" };
    return Json(product);
}
```

**Examples:**

```csharp
// Models
public class Person
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
}

public class HomeController : Controller
{
    // Single object
    [Route("person")]
    public IActionResult Person()
    {
        Person person = new Person() 
        { 
            Id = Guid.NewGuid(), 
            FirstName = "James", 
            LastName = "Smith", 
            Age = 25 
        };
        
        return Json(person);
    }
    
    // Array/List
    [Route("people")]
    public IActionResult People()
    {
        var people = new List<Person>
        {
            new Person { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Age = 30 },
            new Person { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith", Age = 25 }
        };
        
        return Json(people);
    }
    
    // Anonymous object
    [Route("status")]
    public IActionResult Status()
    {
        return Json(new 
        { 
            status = "success", 
            message = "Operation completed",
            timestamp = DateTime.UtcNow
        });
    }
}
```

**Output Example:**

```json
{
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "firstName": "James",
    "lastName": "Smith",
    "age": 25
}
```

**Customizing JSON Serialization:**

```csharp
// Program.cs
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Use camelCase for property names
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        
        // Include indentation for readability
        options.JsonSerializerOptions.WriteIndented = true;
        
        // Ignore null values
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
```

---

## File Results

File results serve files to clients - PDFs, images, documents, or any binary content.

### Types of File Results

| Type | Source | Use Case | Security |
|------|--------|----------|----------|
| `VirtualFileResult` | Web root (wwwroot) | Public static files | Safe |
| `PhysicalFileResult` | File system path | Files outside web root | **Use with caution** |
| `FileContentResult` | Byte array (memory) | Generated/dynamic files | Safe |

### 1. VirtualFileResult

Serves files from the web root directory (`wwwroot` by default).

**Syntax:**
```csharp
// Method 1: Direct instantiation
return new VirtualFileResult("/path/to/file.pdf", "application/pdf");

// Method 2: Using File() helper (recommended)
return File("/path/to/file.pdf", "application/pdf");

// With download name
return File("/path/to/file.pdf", "application/pdf", "download-name.pdf");
```

**Example:**

```csharp
public class HomeController : Controller
{
    // Project structure:
    // wwwroot/
    //   ├── files/
    //   │   └── sample.pdf
    //   └── images/
    //       └── logo.png
    
    [Route("download-pdf")]
    public IActionResult DownloadPdf()
    {
        // Serves from wwwroot/files/sample.pdf
        return File("/files/sample.pdf", "application/pdf");
    }
    
    [Route("download-with-name")]
    public IActionResult DownloadWithName()
    {
        // Suggests filename for download
        return File("/files/sample.pdf", "application/pdf", "MyDocument.pdf");
    }
    
    [Route("show-image")]
    public IActionResult ShowImage()
    {
        // Displays image inline
        return File("/images/logo.png", "image/png");
    }
}
```

**Benefits:**
- ✅ Secure (limited to web root)
- ✅ Simple and clean
- ✅ Best for public static files

### 2. PhysicalFileResult

Serves files from an absolute file system path.

**Syntax:**
```csharp
// Method 1: Direct instantiation
return new PhysicalFileResult(@"C:\Files\document.pdf", "application/pdf");

// Method 2: Using PhysicalFile() helper (recommended)
return PhysicalFile(@"C:\Files\document.pdf", "application/pdf");

// With download name
return PhysicalFile(@"C:\Files\document.pdf", "application/pdf", "document.pdf");
```

**Example:**

```csharp
public class HomeController : Controller
{
    [Route("system-file")]
    public IActionResult SystemFile()
    {
        // Absolute path on server
        return PhysicalFile(@"C:\Reports\sales-report.pdf", "application/pdf");
    }
    
    [Route("user-file/{userId}")]
    public IActionResult UserFile(int userId)
    {
        // ⚠️ SECURITY RISK - validate userId carefully!
        string filePath = $@"C:\UserFiles\{userId}\document.pdf";
        
        // Validate file exists and user has permission
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound();
        }
        
        return PhysicalFile(filePath, "application/pdf");
    }
}
```

**Security Concerns:**

```csharp
// ❌ DANGEROUS - Path traversal vulnerability
[Route("download/{filename}")]
public IActionResult Download(string filename)
{
    string path = $@"C:\Files\{filename}";
    return PhysicalFile(path, "application/pdf");
}
// User could request: /download/../../Windows/System32/config/SAM

// ✅ SAFE - Validate and sanitize input
[Route("download/{filename}")]
public IActionResult DownloadSafe(string filename)
{
    // Remove dangerous characters
    filename = Path.GetFileName(filename);  // Removes path traversal
    
    // Whitelist allowed files
    string[] allowedFiles = { "report1.pdf", "report2.pdf" };
    if (!allowedFiles.Contains(filename))
    {
        return BadRequest("Invalid file");
    }
    
    string path = Path.Combine(@"C:\Files", filename);
    
    if (!System.IO.File.Exists(path))
    {
        return NotFound();
    }
    
    return PhysicalFile(path, "application/pdf");
}
```

**When to Use:**
- Files stored outside web root
- User-uploaded files in protected directories
- Generated reports in temporary locations
- **Always validate paths and check permissions!**

### 3. FileContentResult

Serves files from in-memory byte arrays.

**Syntax:**
```csharp
// Method 1: Direct instantiation
byte[] bytes = ...;
return new FileContentResult(bytes, "application/pdf");

// Method 2: Using File() helper (recommended)
return File(bytes, "application/pdf");

// With download name
return File(bytes, "application/pdf", "filename.pdf");
```

**Example:**

```csharp
public class HomeController : Controller
{
    [Route("file-from-memory")]
    public IActionResult FileFromMemory()
    {
        // Read file into memory
        byte[] bytes = System.IO.File.ReadAllBytes(@"C:\Reports\report.pdf");
        
        return File(bytes, "application/pdf", "report.pdf");
    }
    
    [Route("generate-text-file")]
    public IActionResult GenerateTextFile()
    {
        // Generate content dynamically
        string content = "This is a dynamically generated file\n";
        content += $"Generated at: {DateTime.Now}\n";
        
        byte[] bytes = Encoding.UTF8.GetBytes(content);
        
        return File(bytes, "text/plain", "generated.txt");
    }
    
    [Route("generate-csv")]
    public IActionResult GenerateCsv()
    {
        // Generate CSV content
        var csv = new StringBuilder();
        csv.AppendLine("Name,Age,City");
        csv.AppendLine("John,30,NYC");
        csv.AppendLine("Jane,25,LA");
        
        byte[] bytes = Encoding.UTF8.GetBytes(csv.ToString());
        
        return File(bytes, "text/csv", "data.csv");
    }
    
    [Route("generate-image")]
    public IActionResult GenerateImage()
    {
        // Generate image dynamically (requires System.Drawing or similar)
        // This is a simplified example
        byte[] imageBytes = CreateImage();  // Your image generation code
        
        return File(imageBytes, "image/png", "generated.png");
    }
}
```

**When to Use:**
- Dynamically generated files (reports, CSVs, images)
- Files retrieved from database BLOB fields
- Temporary files that don't need disk storage
- When you want to hide the file's actual path

### Common MIME Types

| File Type | MIME Type |
|-----------|-----------|
| PDF | `application/pdf` |
| Text | `text/plain` |
| HTML | `text/html` |
| JSON | `application/json` |
| XML | `application/xml` |
| CSV | `text/csv` |
| ZIP | `application/zip` |
| PNG | `image/png` |
| JPEG | `image/jpeg` |
| GIF | `image/gif` |
| MP4 | `video/mp4` |
| MP3 | `audio/mpeg` |
| Word | `application/vnd.openxmlformats-officedocument.wordprocessingml.document` |
| Excel | `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet` |

### Complete File Example

```csharp
public class FilesController : Controller
{
    [Route("files/virtual")]
    public IActionResult VirtualFile()
    {
        // From wwwroot
        return File("/documents/sample.pdf", "application/pdf", "sample.pdf");
    }
    
    [Route("files/physical")]
    public IActionResult PhysicalFile()
    {
        // From file system (use with caution!)
        return PhysicalFile(@"C:\Reports\report.pdf", "application/pdf", "report.pdf");
    }
    
    [Route("files/memory")]
    public IActionResult MemoryFile()
    {
        // From byte array
        byte[] bytes = System.IO.File.ReadAllBytes(@"C:\Temp\temp.pdf");
        return File(bytes, "application/pdf", "document.pdf");
    }
    
    [Route("files/generate")]
    public IActionResult GenerateFile()
    {
        // Generate content dynamically
        var content = $"Generated at: {DateTime.Now}\nUser: {User.Identity.Name}";
        byte[] bytes = Encoding.UTF8.GetBytes(content);
        return File(bytes, "text/plain", "info.txt");
    }
}
```

---

## Status Code Results

Status code results return HTTP status codes to inform clients about request outcomes.

### Common Status Codes

| Code | Name | Meaning | Use Case |
|------|------|---------|----------|
| 200 | OK | Success | Successful GET, PUT, PATCH |
| 201 | Created | Resource created | Successful POST |
| 204 | No Content | Success, no body | Successful DELETE |
| 400 | Bad Request | Client error | Invalid input |
| 401 | Unauthorized | Not authenticated | Login required |
| 403 | Forbidden | Not authorized | Insufficient permissions |
| 404 | Not Found | Resource doesn't exist | Invalid ID/URL |
| 500 | Internal Server Error | Server error | Unhandled exception |

### Creating Status Code Results

**Method 1: Using Helper Methods (Recommended)**

```csharp
public class ProductsController : Controller
{
    [Route("products/{id}")]
    public IActionResult GetProduct(int id)
    {
        if (id <= 0)
        {
            return BadRequest("Invalid product ID");  // 400
        }
        
        var product = GetProductById(id);
        
        if (product == null)
        {
            return NotFound($"Product {id} not found");  // 404
        }
        
        return Ok(product);  // 200
    }
}
```

**Method 2: Direct Instantiation**

```csharp
return new BadRequestResult();
return new NotFoundResult();
return new OkResult();
```

**Method 3: Generic StatusCode Method**

```csharp
return StatusCode(403);  // Forbidden
return StatusCode(500, "Internal server error");
```

### Common Status Code Helper Methods

#### Success Responses (2xx)

```csharp
// 200 OK
return Ok();  // No content
return Ok(data);  // With content

// 201 Created
return Created("/products/123", product);  // With location and content
return CreatedAtAction("GetProduct", new { id = 123 }, product);

// 202 Accepted
return Accepted();  // Request accepted for processing

// 204 No Content
return NoContent();  // Success but no content to return
```

#### Client Error Responses (4xx)

```csharp
// 400 Bad Request
return BadRequest();  // No message
return BadRequest("Invalid input");  // With message
return BadRequest(modelState);  // With validation errors

// 401 Unauthorized
return Unauthorized();  // Authentication required

// 403 Forbidden
return Forbid();  // Not authorized

// 404 Not Found
return NotFound();  // No message
return NotFound("Resource not found");  // With message

// 409 Conflict
return Conflict("Resource already exists");

// 422 Unprocessable Entity
return UnprocessableEntity(modelState);
```

#### Server Error Responses (5xx)

```csharp
// 500 Internal Server Error
return StatusCode(500);
return StatusCode(500, "An error occurred");

// 503 Service Unavailable
return StatusCode(503, "Service temporarily unavailable");
```

### Complete Example

```csharp
public class BooksController : Controller
{
    [Route("book")]
    public IActionResult GetBook()
    {
        // Validate query string exists
        if (!Request.Query.ContainsKey("bookid"))
        {
            return BadRequest("Book ID is not supplied");
        }
        
        // Parse book ID
        if (!int.TryParse(Request.Query["bookid"], out int bookId))
        {
            return BadRequest("Book ID must be a number");
        }
        
        // Validate book ID range
        if (bookId <= 0)
        {
            return BadRequest("Book ID cannot be less than or equal to zero");
        }
        
        if (bookId > 1000)
        {
            return NotFound("Book ID cannot be greater than 1000");
        }
        
        // Check authentication
        if (!Convert.ToBoolean(Request.Query["isloggedin"]))
        {
            return Unauthorized("User must be logged in");
        }
        
        // All validations passed - return the file
        return File("/files/sample.pdf", "application/pdf");
    }
}
```

### With Object Content

```csharp
public class ApiController : Controller Base
{
    [HttpGet("users/{id}")]
    public IActionResult GetUser(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new { error = "Invalid user ID", code = "INVALID_ID" });
        }
        
        var user = FindUser(id);
        
        if (user == null)
        {
            return NotFound(new { error = "User not found", userId = id });
        }
        
        return Ok(user);
    }
    
    [HttpPost("users")]
    public IActionResult CreateUser(User user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var createdUser = SaveUser(user);
        
        return CreatedAtAction(
            nameof(GetUser), 
            new { id = createdUser.Id }, 
            createdUser
        );
    }
    
    [HttpDelete("users/{id}")]
    public IActionResult DeleteUser(int id)
    {
        var user = FindUser(id);
        
        if (user == null)
        {
            return NotFound();
        }
        
        DeleteUser(id);
        
        return NoContent();  // 204 - successful deletion
    }
}
```

---

## Redirect Results

Redirect results instruct the browser to navigate to a different URL.

### Types of Redirects

| Type | HTTP Code | Description | Use Case |
|------|-----------|-------------|----------|
| **Temporary** | 302 Found | Temporary redirect | After form submission, login |
| **Permanent** | 301 Moved Permanently | Permanent redirect | URL has permanently changed |

### 1. RedirectResult

Redirects to any URL (relative or absolute).

**Syntax:**
```csharp
// Temporary redirect (302)
return Redirect("/home");
return Redirect("https://example.com");

// Permanent redirect (301)
return RedirectPermanent("/new-location");
return RedirectPermanent("https://newsite.com");
```

**Examples:**

```csharp
public class HomeController : Controller
{
    [Route("old-page")]
    public IActionResult OldPage()
    {
        // Permanent redirect - page moved permanently
        return RedirectPermanent("/new-page");
    }
    
    [Route("external")]
    public IActionResult External()
    {
        // Redirect to external site
        return Redirect("https://www.google.com");
    }
    
    [Route("process-form")]
    [HttpPost]
    public IActionResult ProcessForm()
    {
        // Process form data...
        
        // Temporary redirect after form submission (PRG pattern)
        return Redirect("/success");
    }
}
```

### 2. RedirectToActionResult

Redirects to a specific action method.

**Syntax:**
```csharp
// Same controller
return RedirectToAction("ActionName");

// Different controller
return RedirectToAction("ActionName", "ControllerName");

// With route values
return RedirectToAction("ActionName", "ControllerName", new { id = 123 });

// Permanent redirect
return RedirectToActionPermanent("ActionName");
return RedirectToActionPermanent("ActionName", "ControllerName");
```

**Examples:**

```csharp
public class ProductsController : Controller
{
    [Route("products")]
    public IActionResult Index()
    {
        return View();
    }
    
    [Route("products/{id}")]
    public IActionResult Details(int id)
    {
        return View();
    }
    
    [Route("products/create")]
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }
    
    [Route("products/create")]
    [HttpPost]
    public IActionResult Create(Product product)
    {
        if (!ModelState.IsValid)
        {
            return View(product);
        }
        
        // Save product
        int newProductId = SaveProduct(product);
        
        // Redirect to details page
        return RedirectToAction("Details", new { id = newProductId });
    }
    
    [Route("products/edit/{id}")]
    [HttpPost]
    public IActionResult Edit(int id, Product product)
    {
        // Update product
        UpdateProduct(id, product);
        
        // Redirect back to index
        return RedirectToAction("Index");
    }
    
    [Route("products/delete/{id}")]
    public IActionResult Delete(int id)
    {
        // Delete product
        DeleteProduct(id);
        
        // Redirect to index
        return RedirectToAction("Index");
    }
}

public class OrdersController : Controller
{
    [Route("orders/create")]
    [HttpPost]
    public IActionResult CreateOrder(Order order)
    {
        // Save order
        int orderId = SaveOrder(order);
        
        // Redirect to different controller
        return RedirectToAction("Details", "Products", new { id = order.ProductId });
    }
}
```

### 3. LocalRedirectResult

Redirects to a local URL within the application (security feature).

**Syntax:**
```csharp
// Temporary local redirect
return LocalRedirect("/products");
return LocalRedirect("~/admin/dashboard");

// Permanent local redirect
return LocalRedirectPermanent("/new-location");
```

**Why LocalRedirect?**

```csharp
// ❌ SECURITY RISK - Open redirect vulnerability
[Route("login")]
public IActionResult Login(string returnUrl)
{
    // Authenticate user...
    
    // Dangerous! returnUrl could be external: https://evil.com
    return Redirect(returnUrl);
}

// ✅ SAFE - Only allows local URLs
[Route("login")]
public IActionResult LoginSafe(string returnUrl)
{
    // Authenticate user...
    
    // Safe! Will throw exception if returnUrl is external
    return LocalRedirect(returnUrl ?? "/");
}

// ✅ EVEN SAFER - Validate URL
[Route("login")]
public IActionResult LoginSafer(string returnUrl)
{
    // Authenticate user...
    
    if (Url.IsLocalUrl(returnUrl))
    {
        return Redirect(returnUrl);
    }
    
    return RedirectToAction("Index", "Home");
}
```

**Examples:**

```csharp
public class AccountController : Controller
{
    [Route("login")]
    [HttpPost]
    public IActionResult Login(LoginModel model, string returnUrl)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        // Authenticate user
        bool authenticated = AuthenticateUser(model.Username, model.Password);
        
        if (!authenticated)
        {
            ModelState.AddModelError("", "Invalid credentials");
            return View(model);
        }
        
        // Safe redirect to local URL
        if (!string.IsNullOrEmpty(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }
        
        return RedirectToAction("Index", "Home");
    }
    
    [Route("logout")]
    public IActionResult Logout()
    {
        // Log out user
        SignOut();
        
        return LocalRedirect("/");
    }
}
```

### 4. RedirectToRouteResult

Redirects using route name.

**Syntax:**
```csharp
return RedirectToRoute("RouteName");
return RedirectToRoute("RouteName", new { id = 123 });
return RedirectToRoutePermanent("RouteName");
```

**Example:**

```csharp
public class ProductsController : Controller
{
    [Route("products/{id}", Name = "ProductDetails")]
    public IActionResult Details(int id)
    {
        return View();
    }
    
    [Route("products/create")]
    [HttpPost]
    public IActionResult Create(Product product)
    {
        int newId = SaveProduct(product);
        
        // Redirect using route name
        return RedirectToRoute("ProductDetails", new { id = newId });
    }
}
```

### Post-Redirect-Get (PRG) Pattern

The PRG pattern prevents duplicate form submissions.

```csharp
public class OrdersController : Controller
{
    // GET: Display form
    [HttpGet]
    [Route("orders/create")]
    public IActionResult Create()
    {
        return View();
    }
    
    // POST: Process form
    [HttpPost]
    [Route("orders/create")]
    public IActionResult Create(Order order)
    {
        if (!ModelState.IsValid)
        {
            // Return to form with errors
            return View(order);
        }
        
        // Save order
        int orderId = SaveOrder(order);
        
        // ✅ REDIRECT after POST (PRG pattern)
        // Prevents duplicate submission on refresh
        return RedirectToAction("Confirmation", new { id = orderId });
    }
    
    // GET: Show confirmation
    [Route("orders/confirmation/{id}")]
    public IActionResult Confirmation(int id)
    {
        var order = GetOrder(id);
        return View(order);
    }
}
```

**Why PRG?**

Without redirect:
```
User submits form → POST /orders/create → Show success page
User refreshes → Browser warns about resubmitting form
User clicks OK → Duplicate order created! ❌
```

With redirect:
```
User submits form → POST /orders/create → Redirect to GET /orders/confirmation
User refreshes → Just reloads confirmation page
No duplicate submission! ✅
```

### Complete Redirect Example

```csharp
public class StoreController : Controller
{
    [Route("bookstore")]
    public IActionResult Index()
    {
        // Validate query parameters
        if (!Request.Query.ContainsKey("bookid"))
        {
            return BadRequest("Book ID is required");
        }
        
        if (!int.TryParse(Request.Query["bookid"], out int bookId))
        {
            return BadRequest("Invalid book ID");
        }
        
        // Validate range
        if (bookId <= 0)
        {
            return BadRequest("Book ID must be positive");
        }
        
        if (bookId > 1000)
        {
            return NotFound("Book ID not found");
        }
        
        // Check authentication
        bool isLoggedIn = Convert.ToBoolean(Request.Query["isloggedin"]);
        
        if (!isLoggedIn)
        {
            // Redirect to login
            return RedirectToAction("Login", "Account", new { returnUrl = Request.Path });
        }
        
        // Check if book is special
        if (bookId == 999)
        {
            // Permanent redirect to new location
            return RedirectToActionPermanent("FeaturedBook", "Books");
        }
        
        // Normal redirect to book details
        return RedirectToAction("Details", "Books", new { id = bookId });
    }
}
```

### Redirect Comparison

| Redirect Type | Target | Security | Use Case |
|---------------|--------|----------|----------|
| `Redirect` | Any URL | ⚠️ Risk of open redirect | External URLs, full control needed |
| `RedirectToAction` | Controller action | ✅ Safe | Most common, type-safe |
| `LocalRedirect` | Local URL only | ✅ Safe | After login, security-sensitive |
| `RedirectToRoute` | Named route | ✅ Safe | Complex routing scenarios |

---

## Best Practices

### 1. Use IActionResult for Flexibility

```csharp
// ✅ Good - flexible return type
public IActionResult GetProduct(int id)
{
    if (id <= 0)
        return BadRequest();
    
    var product = FindProduct(id);
    
    if (product == null)
        return NotFound();
    
    return Ok(product);
}

// ❌ Limited - can only return one type
public Product GetProduct(int id)
{
    return FindProduct(id);  // What if product is null?
}
```

### 2. Follow Naming Conventions

```csharp
// ✅ Good naming
public class ProductsController : Controller { }
public class UserAccountController : Controller { }
public class OrderManagementController : Controller { }

// ❌ Bad naming
public class Products : Controller { }  // Missing "Controller"
public class ProductsCtrl : Controller { }  // Non-standard abbreviation
```

### 3. Use Appropriate HTTP Methods

```csharp
// ✅ Good - RESTful design
[HttpGet]
public IActionResult Index() { }

[HttpGet]
public IActionResult Create() { }  // Show form

[HttpPost]
public IActionResult Create(Product product) { }  // Process form

[HttpPut]
public IActionResult Update(int id, Product product) { }

[HttpDelete]
public IActionResult Delete(int id) { }

// ❌ Bad - all GET requests
[HttpGet]
public IActionResult CreateProduct() { }

[HttpGet]
public IActionResult DeleteProduct() { }  // Dangerous!
```

### 4. Validate Input Early

```csharp
// ✅ Good - validate early and return appropriate status
public IActionResult GetProduct(int id)
{
    if (id <= 0)
        return BadRequest("Invalid product ID");
    
    var product = _productService.GetById(id);
    
    if (product == null)
        return NotFound($"Product {id} not found");
    
    return Ok(product);
}

// ❌ Bad - no validation
public IActionResult GetProduct(int id)
{
    var product = _productService.GetById(id);
    return Ok(product);  // What if id is invalid or product is null?
}
```

### 5. Use Helper Methods

```csharp
// ✅ Good - use built-in helpers
return Content("Hello", "text/plain");
return Json(data);
return File(bytes, "application/pdf");
return RedirectToAction("Index");
return BadRequest("Invalid input");
return NotFound();

// ❌ Verbose - unnecessary instantiation
return new ContentResult { Content = "Hello", ContentType = "text/plain" };
return new JsonResult(data);
return new FileContentResult(bytes, "application/pdf");
return new RedirectToActionResult("Index", null, null);
```

### 6. Secure File Operations

```csharp
// ❌ DANGEROUS - path traversal vulnerability
[Route("download/{filename}")]
public IActionResult Download(string filename)
{
    return PhysicalFile($@"C:\Files\{filename}", "application/pdf");
}

// ✅ SAFE - validate and sanitize
[Route("download/{filename}")]
public IActionResult DownloadSafe(string filename)
{
    // Remove path components
    filename = Path.GetFileName(filename);
    
    // Whitelist validation
    string[] allowed = { "report1.pdf", "report2.pdf" };
    if (!allowed.Contains(filename))
        return BadRequest("Invalid file");
    
    string path = Path.Combine(@"C:\Files", filename);
    
    if (!System.IO.File.Exists(path))
        return NotFound();
    
    return PhysicalFile(path, "application/pdf");
}
```

### 7. Prevent Open Redirects

```csharp
// ❌ DANGEROUS - open redirect vulnerability
public IActionResult Login(string returnUrl)
{
    // Authenticate...
    return Redirect(returnUrl);  // Could redirect to external site!
}

// ✅ SAFE - use LocalRedirect
public IActionResult LoginSafe(string returnUrl)
{
    // Authenticate...
    
    if (string.IsNullOrEmpty(returnUrl))
        return RedirectToAction("Index", "Home");
    
    return LocalRedirect(returnUrl);
}

// ✅ ALSO SAFE - validate URL
public IActionResult LoginValidated(string returnUrl)
{
    // Authenticate...
    
    if (Url.IsLocalUrl(returnUrl))
        return Redirect(returnUrl);
    
    return RedirectToAction("Index", "Home");
}
```

### 8. Use PRG Pattern for Forms

```csharp
// ✅ Good - POST-Redirect-GET pattern
[HttpGet]
public IActionResult Create() => View();

[HttpPost]
public IActionResult Create(Product product)
{
    if (!ModelState.IsValid)
        return View(product);
    
    int id = SaveProduct(product);
    
    // Redirect after successful POST
    return RedirectToAction("Details", new { id });
}

// ❌ Bad - returning view directly after POST
[HttpPost]
public IActionResult CreateBad(Product product)
{
    SaveProduct(product);
    return View("Success");  // Refresh = duplicate submission!
}
```

### 9. Return Appropriate Status Codes

```csharp
// ✅ Good - meaningful status codes
[HttpPost]
public IActionResult Create(Product product)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);  // 400
    
    var created = SaveProduct(product);
    
    return CreatedAtAction(nameof(GetProduct), 
        new { id = created.Id }, created);  // 201
}

[HttpDelete]
public IActionResult Delete(int id)
{
    var product = FindProduct(id);
    
    if (product == null)
        return NotFound();  // 404
    
    DeleteProduct(id);
    
    return NoContent();  // 204
}

// ❌ Bad - always 200 OK
[HttpPost]
public IActionResult CreateBad(Product product)
{
    SaveProduct(product);
    return Ok();  // Should be 201 Created
}

[HttpDelete]
public IActionResult DeleteBad(int id)
{
    DeleteProduct(id);
    return Ok();  // Should be 204 No Content
}
```

### 10. Organize Controllers Logically

```csharp
// ✅ Good - organized by resource
public class ProductsController : Controller
{
    [HttpGet] public IActionResult Index() { }
    [HttpGet] public IActionResult Details(int id) { }
    [HttpGet] public IActionResult Create() { }
    [HttpPost] public IActionResult Create(Product p) { }
    [HttpGet] public IActionResult Edit(int id) { }
    [HttpPost] public IActionResult Edit(int id, Product p) { }
    [HttpPost] public IActionResult Delete(int id) { }
}

// ❌ Bad - mixed responsibilities
public class HomeController : Controller
{
    public IActionResult Index() { }
    public IActionResult GetProduct(int id) { }  // Should be in ProductsController
    public IActionResult CreateOrder() { }  // Should be in OrdersController
    public IActionResult UserProfile() { }  // Should be in UsersController
}
```

---

## Quick Reference

### Controller Setup

```csharp
// Program.cs
builder.Services.AddControllers();  // or AddControllersWithViews()
app.UseRouting();
app.MapControllers();

// Controller class
public class MyController : Controller
{
    // Actions here
}
```

### Action Result Quick Guide

```csharp
// Content
return Content("text", "text/plain");
return Json(object);

// Files
return File("/path/file.pdf", "application/pdf");
return PhysicalFile(@"C:\file.pdf", "application/pdf");
return File(bytes, "application/pdf");

// Status Codes
return Ok(data);                    // 200
return Created(uri, data);          // 201
return NoContent();                 // 204
return BadRequest(message);         // 400
return Unauthorized();              // 401
return Forbid();                    // 403
return NotFound(message);           // 404
return StatusCode(code, message);   // Any code

// Redirects
return Redirect("/path");
return RedirectToAction("Action", "Controller", new { id = 1 });
return LocalRedirect("/path");
return RedirectPermanent("/path");
```

### Common Patterns

```csharp
// CRUD operations
[HttpGet] public IActionResult Index() { }          // List all
[HttpGet] public IActionResult Details(int id) { }  // Get one
[HttpGet] public IActionResult Create() { }         // Show form
[HttpPost] public IActionResult Create(T model) { } // Process form
[HttpGet] public IActionResult Edit(int id) { }     // Show form
[HttpPost] public IActionResult Edit(T model) { }   // Process form
[HttpPost] public IActionResult Delete(int id) { }  // Delete

// Validation pattern
if (!ModelState.IsValid)
    return BadRequest(ModelState);

// Not found pattern
var item = FindItem(id);
if (item == null)
    return NotFound($"Item {id} not found");

// PRG pattern
[HttpPost]
public IActionResult Create(Model model)
{
    if (!ModelState.IsValid)
        return View(model);
    
    int id = Save(model);
    return RedirectToAction("Details", new { id });
}
```

---

## Common Pitfalls

### Pitfall 1: Not Using IActionResult

```csharp
// ❌ Wrong - inflexible
public string GetProduct(int id)
{
    return "Product data";
}

// ✅ Correct - flexible
public IActionResult GetProduct(int id)
{
    if (id <= 0)
        return BadRequest();
    return Ok("Product data");
}
```

### Pitfall 2: Returning View After POST

```csharp
// ❌ Wrong - causes duplicate submissions
[HttpPost]
public IActionResult Create(Product p)
{
    Save(p);
    return View("Success");  // Refresh = duplicate!
}

// ✅ Correct - use PRG pattern
[HttpPost]
public IActionResult Create(Product p)
{
    int id = Save(p);
    return RedirectToAction("Details", new { id });
}
```

### Pitfall 3: Insecure File Operations

```csharp
// ❌ DANGEROUS
public IActionResult Download(string file)
{
    return PhysicalFile($@"C:\Files\{file}", "application/pdf");
}

// ✅ SAFE
public IActionResult Download(string file)
{
    file = Path.GetFileName(file);  // Remove path traversal
    // Add whitelist validation
    return PhysicalFile(Path.Combine(@"C:\Files", file), "application/pdf");
}
```

### Pitfall 4: Open Redirect Vulnerability

```csharp
// ❌ DANGEROUS
public IActionResult Login(string returnUrl)
{
    return Redirect(returnUrl);  // Could be external!
}

// ✅ SAFE
public IActionResult Login(string returnUrl)
{
    return LocalRedirect(returnUrl ?? "/");
}
```

### Pitfall 5: Wrong Status Codes

```csharp
// ❌ Wrong
[HttpPost]
public IActionResult Create(Product p)
{
    Save(p);
    return Ok();  // Should be 201 Created
}

// ✅ Correct
[HttpPost]
public IActionResult Create(Product p)
{
    var created = Save(p);
    return CreatedAtAction(nameof(Details), new { id = created.Id }, created);
}
```

---

## Summary

### Key Concepts

| Concept | Purpose |
|---------|---------|
| **Controllers** | Group related actions, organize application logic |
| **Action Methods** | Handle HTTP requests, return responses |
| **IActionResult** | Flexible return type for various response types |
| **Content Results** | Return text, JSON, or other content |
| **File Results** | Serve files to clients |
| **Status Codes** | Communicate request outcomes |
| **Redirects** | Navigate to different URLs |

### Controller Flow

```
HTTP Request
    ↓
Routing (matches URL to controller/action)
    ↓
Controller Action Method
    ↓
├── Validate Input → Return BadRequest (400)
├── Check Authorization → Return Unauthorized (401) / Forbid (403)
├── Find Resource → Return NotFound (404)
├── Process Request → Business Logic
└── Return Result
    ├── View (HTML)
    ├── Json (API data)
    ├── File (download)
    ├── Redirect (navigation)
    └── Status Code (result indication)
    ↓
HTTP Response
```

### Remember

1. **Use IActionResult** for flexibility in return types
2. **Validate early** and return appropriate status codes
3. **Follow RESTful conventions** for HTTP methods
4. **Secure file operations** - validate paths
5. **Prevent open redirects** - use LocalRedirect
6. **Use PRG pattern** to prevent duplicate submissions
7. **Return meaningful status codes** (200, 201, 204, 400, 404, etc.)
8. **Organize controllers** by resource or feature
9. **Use helper methods** for cleaner code
10. **Test thoroughly** - especially security-sensitive operations