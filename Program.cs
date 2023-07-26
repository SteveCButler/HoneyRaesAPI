using HoneyRaesAPI.Models;

var builder = WebApplication.CreateBuilder(args);


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
        DateCompleted = null,
    },  new ServiceTicket
    {
        Id = 2,
        CustomerId = 2,
        EmployeeId = 2,
        Description = "Laptop keyboard not working",
        Emergency = false,
        DateCompleted = null,
    },  new ServiceTicket
    {
        Id = 3,
        CustomerId = 2,
        EmployeeId = 2,
        Description = "Laptop screen flickering",
        Emergency = false,
        DateCompleted = new DateTime(2023, 7 , 22)
    },  new ServiceTicket
    {
        Id = 4,
        CustomerId = 3,
        EmployeeId = 1,
        Description = "System won't power on",
        Emergency = false,
        DateCompleted = new DateTime(2023, 3 , 12)
    },  new ServiceTicket
    {
        Id = 5,
        CustomerId = 1,
        EmployeeId = 2,
        Description = "Tablet won't charge",
        Emergency = false,
        DateCompleted = new DateTime(2023, 6 , 22)
    },
};

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/servicetickets", () =>
{
    return serviceTickets;
});



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
app.MapGet("/cutomers", () =>
{
    return customers;
});

//GET Customer by Id
app.MapGet("/cutomers/{id}", (int id) =>
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

//Complete ticket by adding DateTime
app.MapPost("/servicetickets/{id}/complete", (int id) =>
{
    ServiceTicket ticketToComplete = serviceTickets.FirstOrDefault(st => st.Id == id);
    ticketToComplete.DateCompleted = DateTime.Today;

});


app.Run();


