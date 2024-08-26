using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskMgt.Models;

namespace TaskMgt.Services
{
    public class BookService
    {
        private readonly List<Book> _books;

        public BookService()
        {
            _books = 
            [
                new Book {Id = 1, Title = "Book one" },
            ];
        }

        public IEnumerable<Book> GetBooks()
        {
            return _books;
        }

    }
}