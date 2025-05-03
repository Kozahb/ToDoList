using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Todo.Models;
using Todo.Models.ViewModels;

namespace Todo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var todoListViewModel = GetAllTodos();
            return View(todoListViewModel);
        }

        

[HttpPost]
public JsonResult UpdateTaskStatus(int id, bool isCompleted)
{
    using (SqliteConnection con = new SqliteConnection("Data Source=db.sqlite"))
    {
        con.Open();

        using (var updateCmd = con.CreateCommand())
        {
            updateCmd.CommandText = "UPDATE todo SET IsCompleted = @isCompleted WHERE Id = @id";
            updateCmd.Parameters.AddWithValue("@id", id);
            updateCmd.Parameters.AddWithValue("@isCompleted", isCompleted ? 1 : 0); 

            try
            {
                updateCmd.ExecuteNonQuery();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}

        internal TodoViewModel GetAllTodos()
{
    List<TodoItem> todoList = new();

    using (SqliteConnection con = new SqliteConnection("Data Source=db.sqlite"))
    {
        using (var tableCmd = con.CreateCommand())
        {
            con.Open();
            tableCmd.CommandText = "SELECT * FROM todo";

            using (var reader = tableCmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        todoList.Add(
                            new TodoItem
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                IsCompleted = reader.GetInt32(2) == 1 
                            });
                    }
                }
            }
        }
    }

    return new TodoViewModel
    {
        TodoList = todoList
    };
}

        internal TodoItem GetById(int id)
        {
            TodoItem todo = new();

            using (var connection =
                   new SqliteConnection("Data Source=db.sqlite"))
            {
                using (var tableCmd = connection.CreateCommand())
                {
                    connection.Open();
                    tableCmd.CommandText = $"SELECT * FROM todo Where Id = '{id}'";

                    using (var reader = tableCmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            todo.Id = reader.GetInt32(0);
                            todo.Name = reader.GetString(1);
                        }
                        else
                        {
                            return todo;
                        }
                    };
                }
            }

            return todo;
        }

       public RedirectToActionResult Insert(TodoItem todo)
{
    if (string.IsNullOrWhiteSpace(todo.Name))
    {
        return RedirectToAction("Index");
    }

    using (var con = new SqliteConnection("Data Source=db.sqlite"))
    {
        con.Open();

        using (var checkCmd = con.CreateCommand())
        {
            checkCmd.CommandText = "SELECT COUNT(*) FROM todo;";
            long count = (long)checkCmd.ExecuteScalar();

            if (count == 0)
            {
                using (var resetCmd = con.CreateCommand())
                {
                    resetCmd.CommandText = "DELETE FROM sqlite_sequence WHERE name = 'todo';";
                    resetCmd.ExecuteNonQuery();
                }
            }
        }

        using (var insertCmd = con.CreateCommand())
        {
            insertCmd.CommandText = "INSERT INTO todo (Name) VALUES (@name);";
            insertCmd.Parameters.AddWithValue("@name", todo.Name);

            try
            {
                insertCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inserir tarefa: {ex.Message}");
            }
        }
    }

    return RedirectToAction("Index");
}

        [HttpPost]
        public JsonResult Delete(int id)
        {
            using (SqliteConnection con =
                   new SqliteConnection("Data Source=db.sqlite"))
            {
                using (var tableCmd = con.CreateCommand())
                {
                    con.Open();
                    tableCmd.CommandText = $"DELETE from todo WHERE Id = '{id}'";
                    tableCmd.ExecuteNonQuery();
                }
            }

            return Json(new {});
        }

[HttpPost]
public IActionResult Update(TodoItem todo)
{
    
    if (!ModelState.IsValid)
    {
        return View(todo);  
    }

    using (SqliteConnection con = new SqliteConnection("Data Source=db.sqlite"))
    {
        con.Open();

        using (var updateCmd = con.CreateCommand())
        {
            updateCmd.CommandText = "UPDATE todo SET Name = @name WHERE Id = @id";
            updateCmd.Parameters.AddWithValue("@id", todo.Id);
            updateCmd.Parameters.AddWithValue("@name", todo.Name);

            try
            {
                updateCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View("Error");  
            }
        }
    }

         return RedirectToAction("Index");  
      }

    }
} 