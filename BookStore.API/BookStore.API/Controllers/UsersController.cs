using BookStore.API.Contracts;
using BookStore.Core.Abstractions;
using DomainUser = BookStore.Core.Models.User;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUsersRepository _users;

    public UsersController(IUsersRepository users)
    {
        _users = users;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<UserResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<UserResponse>>> GetAll([FromQuery] string? search, CancellationToken cancellationToken)
    {
        var users = await _users.GetAsync(search, cancellationToken);
        return Ok(users.Select(ToResponse).ToList());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(id, cancellationToken);
        if (user is null)
            return NotFound();

        return Ok(ToResponse(user));
    }

    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponse>> Create([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var (user, error) = DomainUser.CreateNew(request.Email, request.FullName);
        if (user is null)
            return BadRequest(error);

        if (await _users.EmailExistsAsync(user.Email, cancellationToken: cancellationToken))
            return Conflict("A user with this email already exists.");

        var id = await _users.CreateAsync(user, cancellationToken);
        var created = await _users.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, ToResponse(created!));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var (_, error) = DomainUser.Create(id, request.Email, request.FullName);
        if (!string.IsNullOrEmpty(error))
            return BadRequest(error);

        var existing = await _users.GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound();

        var normalized = request.Email.Trim().ToLowerInvariant();
        if (normalized != existing.Email && await _users.EmailExistsAsync(normalized, exceptUserId: id, cancellationToken: cancellationToken))
            return Conflict("A user with this email already exists.");

        var updated = await _users.UpdateAsync(id, request.Email, request.FullName, cancellationToken);
        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _users.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return NotFound();

        return NoContent();
    }

    private static UserResponse ToResponse(DomainUser user) =>
        new(user.Id, user.Email, user.FullName);
}
