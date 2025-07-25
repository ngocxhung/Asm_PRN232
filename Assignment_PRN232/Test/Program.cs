using BussinessObjects.Models;
using Repositories;

namespace Test
{
    internal class Program
    {
        private readonly LibraryDbContext _context;
        private readonly IBookRepository bookRepository = new BookRepository();
        public Program(LibraryDbContext context)
        {
            _context = context;
        }
        static void Main(string[] args)
        {
            try
            {
                var books = new Program(new LibraryDbContext()).bookRepository.GetAllAsync().Result;
                foreach (var book in books)
                {
                    Console.WriteLine($"ID: {book.BookId}, Title: {book.Title}, Author: {book.Author?.AuthorName}, Category: {book.Category?.CategoryName}");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");

            }

        }
    }
}
