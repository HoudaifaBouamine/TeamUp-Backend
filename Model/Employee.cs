using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Models;

namespace Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }   
        public string Password { get; set; }
    }

    public class EmployeeDto(Employee model)
    {
        public int Id { get; set; } = model.Id;
        public string Name { get; set; } = model.Name;

    }
}

