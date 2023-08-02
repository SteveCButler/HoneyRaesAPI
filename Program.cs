using HoneyRaesAPI.Models;
using Microsoft.OpenApi.Models;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Reflection;



var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "ToDo API",
        Description = "An ASP.NET Core Web API for managing ToDo items",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Example Contact",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });

    
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


List<Customer> customers = new List<Customer>

{
    new Customer
    {
        Id = 1,
        Name = "Customer1",
        Address = "123 South Street"
    },
    new Customer
    {
        Id = 2,
        Name = "Customer2",
        Address = "123 North Street"
    },
    new Customer
    {
        Id = 3,
        Name = "Customer3",
        Address = "123 East Street"
    }
};

List<Employee> employees = new List<Employee> 
{
    new Employee
    {
        Id = 1,
        Name = "Sally",
        Specialty = "Handheld devices"
    },
    new Employee
    {
        Id = 2,
        Name = "Johnny",
        Specialty = "Computer repair"
    },
    new Employee
    {
        Id = 3,
        Name = "Aidan",
        Specialty = "Blacksmith"
    }
};
List<ServiceTicket> serviceTickets = new List<ServiceTicket>
{
    new ServiceTicket
    {
        Id = 1,
        CustomerId = 1,
        EmployeeId = 1,
        Description = "Broken phone screen",
        Emergency = false,
        DateCompleted = new DateTime(2023, 7, 22),
    },  
    new ServiceTicket
    {
        Id = 2,
        CustomerId = 2,
        EmployeeId = 2,
        Description = "Laptop keyboard not working",
        Emergency = false,
        DateCompleted = new DateTime(2023, 7, 15),
    },  
    new ServiceTicket
    {
        Id = 3,
        CustomerId = 2,
        EmployeeId = 2,
        Description = "Laptop screen flickering",
        Emergency = false,
        DateCompleted = new DateTime(2023, 7 , 22)
    },  
    new ServiceTicket
    {
        Id = 4,
        CustomerId = 3,
        EmployeeId = 1,
        Description = "System won't power on",
        Emergency = false,
        DateCompleted = new DateTime(2021, 3 , 12)
    },  
    new ServiceTicket
    {
        Id = 5,
        CustomerId = 1,
        EmployeeId = 2,
        Description = "Tablet won't charge",
        Emergency = true,
        DateCompleted = new DateTime(2020, 1, 20),
    }, new ServiceTicket
    {
        Id = 6,
        CustomerId = 1,
        EmployeeId = 2,
        Description = "Tablet sound not working",
        Emergency = false,
        DateCompleted = new DateTime(2023, 8, 1),
    }, new ServiceTicket
    {
        Id = 7,
        CustomerId = 1,
        EmployeeId = 2,
        Description = "Tablet sound won't turn off",
        Emergency = true,
        DateCompleted = new DateTime(2023, 8, 1),
       
    }, new ServiceTicket
    {
        Id = 8,
        CustomerId = 1,
        EmployeeId = 1,
        Description = "Tablet sound won't turn off",
        Emergency = true,
        DateCompleted = new DateTime(2023, 8, 1),

    }, new ServiceTicket
    {
        Id = 9,
        CustomerId = 1,
        EmployeeId = 2,
        Description = "Tablet sound won't turn off",
        Emergency = true,
        DateCompleted = null,
       
    },
};


//Get All Service Tickets
app.MapGet("/servicetickets", () =>
{
    return serviceTickets;
});


//Get Service ticket by ID, include Employee and Customer info related to ticket
app.MapGet("/servicetickets/{id}", (int id) =>
{
    ServiceTicket serviceticket = serviceTickets.FirstOrDefault(x => x.Id == id);
    if (serviceticket == null)
    {
        return Results.NotFound();
    }
    serviceticket.Employee = employees.FirstOrDefault(e => e.Id == serviceticket.EmployeeId);

    serviceticket.Customer = customers.FirstOrDefault(c => c.Id == serviceticket.CustomerId);
    return Results.Ok(serviceticket);
});

//Get All Customers
app.MapGet("/customers", () =>
{
    return customers;
});

//GET Customer by Id
app.MapGet("/customers/{id}", (int id) =>
{
    Customer customer = customers.FirstOrDefault(x => x.Id == id);
    if (customer == null)
    {
        return Results.NotFound();
    }
customer.ServiceTicket = serviceTickets.FirstOrDefault(s => s.CustomerId == customer.Id);
    return Results.Ok(customer);
});

//GET All employees
app.MapGet("/employees", () =>
{
    return employees;
});

//GET employee by Id
app.MapGet("/employees/{id}", (int id) =>
{
    Employee employee = employees.FirstOrDefault(x => x.Id == id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    employee.ServiceTickets = serviceTickets.Where(st => st.EmployeeId == id).ToList();
    return Results.Ok(employee);
});

//Create new service ticket
app.MapPost("/servicetickets", (ServiceTicket serviceTicket) =>
{
    serviceTicket.Id = serviceTickets.Max(st => st.Id) + 1;
    serviceTickets.Add(serviceTicket);
    return serviceTicket;
});

//Delete ticket by Id
app.MapDelete("/servicetickets", (int id) =>
{
    serviceTickets.RemoveAll(s => s.Id == id);
});

//Update Service Ticket
app.MapPut("/servicetickets/{id}", (int id, ServiceTicket serviceTicket) =>
{
    ServiceTicket ticketToUpdate = serviceTickets.FirstOrDefault(st => st.Id == id);
    int ticketIndex = serviceTickets.IndexOf(ticketToUpdate);
    if (ticketToUpdate == null)
    {
        return Results.NotFound();
    }
   // the id in the request route doesn't match the id from the ticket in the request body. That's a bad request!
    if (id != serviceTicket.Id)
    {
        return Results.BadRequest();
    }
    serviceTickets[ticketIndex] = serviceTicket;
    return Results.Ok();
});

//GET all open tickets
app.MapGet("/opentickets", () =>
{
    var openTickets = serviceTickets.Where(st => st.DateCompleted == null).ToList();
    return openTickets;
});



//GET all available employees
app.MapGet("/availableEmployee", () =>
{
    List<int> employeeIds = employees.Select(p => p.Id).ToList();
    //var availableEmployees = serviceTickets.Where()
    return employeeIds;
});



//Complete ticket by adding DateTime
app.MapPost("/servicetickets/{id}/complete", (int id) =>
{
    ServiceTicket ticketToComplete = serviceTickets.FirstOrDefault(st => st.Id == id);
    ticketToComplete.DateCompleted = DateTime.Today;

});

//1. Emergencies - GET all open Emergency tickets - 
app.MapGet("/emergencytickets", () =>
{
    var openTickets = serviceTickets.Where(st => st.DateCompleted == null).ToList();
    var emergencyTickets = serviceTickets.Where(st => st.Emergency == true).ToList();
    return emergencyTickets;
});

//2. Unassigned - GET all unassigned tickets - 
app.MapGet("/unassigned", () =>
{
    var unassignedTickets = serviceTickets.Where(st => st.EmployeeId == null).ToList();
    return unassignedTickets;
});

//3. Inactive Customers - GET all tickets open more than 1 year
app.MapGet("/customers/inactive", () =>
{
    DateTime now = DateTime.Now.Date;


    var closedTickets = serviceTickets.Where(st => st.DateCompleted != null).ToList();
    List<ServiceTicket> inactiveCustomerTickets = new List<ServiceTicket>();
    List<Customer> inactiveCustomers = new List<Customer>();

    foreach (var ticket in closedTickets)
    {
        var time = ticket.DateCompleted;
        TimeSpan timeSpan = now - Convert.ToDateTime(time);
        if (timeSpan.TotalDays > 365)
        {
           
            inactiveCustomerTickets.Add(ticket);
            
        }
    }

    foreach (var ticket in inactiveCustomerTickets)
    {
        var inactiveCustomer = customers.FirstOrDefault(x => x.Id == ticket.CustomerId);
        inactiveCustomers.Add(inactiveCustomer);
    }
    return  inactiveCustomers;
});



//4. Available employees - GET employees not currently assigned to an incomplete service ticket
app.MapGet("/employees/available", () =>
{
    List<ServiceTicket> incompleteTickets = serviceTickets.Where(t => t.DateCompleted == null).ToList();
    List<Employee> availableEmployees = employees.Where(e => !incompleteTickets.Exists(t => t.EmployeeId == e.Id)).ToList();

    return availableEmployees;

});

//5. Employee's customers - return all of the customers for whom a given employee
//   has been assigned to a service ticket (whether completed or not)
app.MapGet("/employee/{id}/customers", (int id) =>
{
    var employeeTickets = serviceTickets.Where(t => t.EmployeeId == id).ToList();
    var employeeCustomerIds = employeeTickets.Select(t => t.CustomerId).ToList();
    return customers.Where(c => employeeCustomerIds.Contains(c.Id));
});

//6. Employee of the month - return the employee who has completed the most service tickets last month
app.MapGet("employee/employee-of-month", () =>
{
    var currentMonth = DateTime.Now.Month;

    var monthlyTicketsByEmployee = serviceTickets.Where(t => Convert.ToDateTime(t.DateCompleted).Month == currentMonth).GroupBy(t => t.EmployeeId).ToList();
    var employeesGroups = monthlyTicketsByEmployee.Select( s =>  s.Key);

    var employee = employees.FirstOrDefault(x => employeesGroups.FirstOrDefault() == x.Id);
    return employee;

});

//7. Past Ticket review - return completed tickets in order of the completion date
app.MapGet("pastTickets", () =>
{
    // var pastTicketsByDate = serviceTickets.Where(st => st.DateCompleted != null).OrderBy().ToList();

});

// 8.Prioritized Tickets(challenge) - return all tickets that are incomplete,
//   in order first by whether they are emergencies, then by whether they are assigned or not (unassigned first).


app.Run();


