using BookStore.API.Contracts;
using BookStore.Core.Abstractions;
using BookStore.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBooksRepository _books;

    public BooksController(IBooksRepository books)
    {
        _books = books;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<BookResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<BookResponse>>> GetAll([FromQuery] string? search, CancellationToken cancellationToken)
    {
        var books = await _books.GetAsync(search, cancellationToken);
        return Ok(books.Select(ToResponse).ToList());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BookResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var book = await _books.GetByIdAsync(id, cancellationToken);
        if (book is null)
            return NotFound();

        return Ok(ToResponse(book));
    }

    [HttpPost]
    [ProducesResponseType(typeof(BookResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookResponse>> Create([FromBody] CreateBookRequest request, CancellationToken cancellationToken)
    {
        var (book, error) = Book.CreateNew(request.Title, request.Description, request.Price);
        if (book is null)
            return BadRequest(error);

        var id = await _books.CreateAsync(book, cancellationToken);
        var created = await _books.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, ToResponse(created!));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBookRequest request, CancellationToken cancellationToken)
    {
        var (_, error) = Book.Create(id, request.Title, request.Description, request.Price);
        if (!string.IsNullOrEmpty(error))
            return BadRequest(error);

        var updated = await _books.UpdateAsync(id, request.Title, request.Description, request.Price, cancellationToken);
        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _books.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return NotFound();

        return NoContent();
    }

    private static BookResponse ToResponse(Book book) =>
        new(book.Id, book.Title, book.Description, book.Price);
}
